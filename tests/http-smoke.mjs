import assert from 'node:assert/strict';
import { spawn, spawnSync } from 'node:child_process';
import { randomBytes, randomUUID } from 'node:crypto';
import { fileURLToPath } from 'node:url';
import { mkdtempSync, writeFileSync, rmSync } from 'node:fs';
import { tmpdir } from 'node:os';
import { join } from 'node:path';
import { createServer as createSmtpServer } from 'node:net';
import { createServer as createStorageServer } from 'node:http';

const root = fileURLToPath(new URL('../', import.meta.url));
const database = process.env.SYMPOSIUM_TEST_DATABASE;
assert(database && /Database=[^;]*_test(?:;|$)/i.test(database), 'Use a disposable database ending in _test.');
assert(/Host=(127\.0\.0\.1|localhost)(?:;|$)/i.test(database), 'Only a local test database is allowed.');
const port = process.env.SYMPOSIUM_TEST_PORT || '15088';
const base = `http://127.0.0.1:${port}/api`;
const email = `admin-${randomUUID()}@example.test`;
const password = randomBytes(24).toString('base64url');
const documentsRoot = mkdtempSync(join(tmpdir(), 'symposium-doc-test-'));
writeFileSync(join(documentsRoot, 'sample.pdf'), '%PDF-1.4\nTest document\n');
const env = {
  ...process.env,
  ASPNETCORE_ENVIRONMENT: 'Development',
  ASPNETCORE_URLS: `http://127.0.0.1:${port}`,
  database,
  databaseOptions__migrateOnStartup: 'true',
  jwt: randomBytes(48).toString('base64url'),
  hash: randomBytes(48).toString('base64url'),
  authentication__clients__0__id: '2799581f-f7af-4b23-97f6-6eb60c314358',
  authentication__clients__0__client_id: 'integration',
  authentication__clients__0__password: '$2y$12$GdIh5IJH4B/yVWRkthCXDepS15l4cAVmhHAEDwPdkzV2nATuN5S7i',
  registration__enabled: 'true',
  cors__origins__0: process.env.SYMPOSIUM_UI_URL || 'http://127.0.0.1:4180',
  authentication__rateLimit: '200',
  smtp__server: '127.0.0.1', smtp__port: '15025', smtp__from: 'test@example.test', smtp__enableSsl: 'false',
  notifications__intervalSeconds: '1', notifications__retryBaseSeconds: '1',
  legacyFiles__documentsRoot: documentsRoot,
  storage__server: 'http://127.0.0.1:15999/',
  storage__username: 'test',
  storage__password: 'test',
  storage__cdn__url: 'http://127.0.0.1:15999',
  storage__cdn__token: randomBytes(48).toString('base64url'),
  bootstrap__email: email,
  bootstrap__password: password,
  OTEL_SDK_DISABLED: 'true'
};
const project = 'src/Symposium.Api.Server';
const delivered = [];
let smtpConnections = 0;
const smtp = createSmtpServer(socket => {
  socket.on('error', () => {});
  smtpConnections++;
  if (smtpConnections === 1) { socket.end('421 Temporary test failure\r\n'); return; }
  socket.write('220 localhost test SMTP\r\n');
  let buffer = '', body = null;
  socket.on('data', chunk => {
    buffer += chunk;
    while (buffer.includes('\r\n')) {
      const index = buffer.indexOf('\r\n'), line = buffer.slice(0, index); buffer = buffer.slice(index + 2);
      if (body !== null) {
        if (line === '.') { delivered.push(body); body = null; socket.write('250 queued\r\n'); }
        else body += line + '\n';
      } else if (/^EHLO|^HELO/i.test(line)) socket.write('250-localhost\r\n250 OK\r\n');
      else if (/^DATA/i.test(line)) { body = ''; socket.write('354 Continue\r\n'); }
      else if (/^QUIT/i.test(line)) socket.end('221 Bye\r\n');
      else socket.write('250 OK\r\n');
    }
  });
});
await new Promise(resolve => smtp.listen(15025, '127.0.0.1', resolve));
const files = new Map();
const storage = createStorageServer(async (req, res) => {
  const path = new URL(req.url, 'http://localhost').pathname;
  if (req.method === 'MKCOL') { res.writeHead(201); res.end(); return; }
  if (req.method === 'PUT') {
    const chunks = []; for await (const chunk of req) chunks.push(chunk);
    files.set(path, {data: Buffer.concat(chunks), type: req.headers['content-type']}); res.writeHead(201); res.end(); return;
  }
  if (req.method === 'DELETE') { files.delete(path); res.writeHead(204); res.end(); return; }
  const file = files.get(path);
  if (!file) { res.writeHead(404); res.end(); return; }
  res.writeHead(200, {'Content-Type': file.type || 'application/octet-stream', 'Content-Length': file.data.length});
  res.end(req.method === 'HEAD' ? undefined : file.data);
});
await new Promise(resolve => storage.listen(15999, '127.0.0.1', resolve));
const bootstrap = spawnSync('dotnet', ['run', '--no-build', '--project', project, '--', '--bootstrap-admin=true'],
  { cwd: root, env, encoding: 'utf8', timeout: 60000 });
assert.equal(bootstrap.status, 0, bootstrap.stderr + bootstrap.stdout);
const server = spawn('dotnet', ['run', '--no-build', '--project', project], { cwd: root, env });
let logs = '';
server.stdout.on('data', data => { logs += data; });
server.stderr.on('data', data => { logs += data; });
let checks = 0;
async function request(path, { token, method = 'GET', body, status = 200 } = {}) {
  const response = await fetch(base + path, {
    method,
    headers: { ...(body ? { 'Content-Type': 'application/json' } : {}), ...(token ? { Authorization: `Bearer ${token}` } : {}) },
    body: body ? JSON.stringify(body) : undefined
  });
  const text = await response.text();
  assert.equal(response.status, status, `${method} ${path}: ${text}`);
  checks++;
  return text ? JSON.parse(text) : null;
}
async function login(username, secret) {
  const response = await fetch(base + '/token', {
    method: 'POST', headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
    body: new URLSearchParams({ grant_type: 'password', username, password: secret, client_id: 'integration', client_secret: 'integration-client' })
  });
  const body = await response.json();
  assert.equal(response.status, 200, JSON.stringify(body));
  assert(body.access_token);
  checks++;
  return body.access_token;
}
try {
  let ready = false;
  for (let attempt = 0; attempt < 100; attempt++) {
    if (server.exitCode !== null) throw new Error(logs);
    try {
      if ((await fetch(base + '/healthz')).ok) { ready = true; break; }
    } catch {}
    await new Promise(resolve => setTimeout(resolve, 100));
  }
  assert(ready, logs);
  const document = await fetch(base + '/doc/sample.pdf');
  assert.equal(document.status, 200);
  assert.equal(document.headers.get('x-content-type-options'), 'nosniff');
  assert((await document.text()).startsWith('%PDF-1.4'));
  assert.equal((await fetch(base + '/doc/missing.pdf')).status, 404);
  checks += 2;
  const admin = await login(email, password);
  await request('/user/me', { token: admin });
  const memberEmail = `member-${randomUUID()}@example.test`;
  await request('/register', { method: 'POST', status: 201, body: { name: 'Test', surname: 'Member', email: memberEmail, password } });
  const member = await login(memberEmail, password);
  await request('/user', { token: member, status: 403 });
  const me = await request('/user/me', { token: member });
  await request('/user/me', { token: member, method: 'PATCH', body: { email: memberEmail, first_name: 'Test', type: 1024 } });
  assert.equal((await request('/user/me', { token: member })).type, 1);
  await request(`/user/${me.id + 1000}`, { token: member, status: 401 });
  await request('/articles', { token: member, method: 'PUT', status: 400, body: { en_title: 'Incomplete' } });
  const suffix = randomUUID();
  const draft = await request('/articles', { token: admin, method: 'PUT', body: { en_title: `Draft-${suffix}`, title_year: '2023', paper_approved: false } });
  const published = await request('/articles', { token: admin, method: 'PUT', body: { en_title: `Public-${suffix}`, title_year: '2023', paper_approved: true, pre_type: '97', mes_congress: 'private-marker' } });
  await request(`/articles/${draft.id}`, { status: 404 });
  const article = await request(`/articles/${published.id}`);
  assert(!JSON.stringify(article).includes('private-marker'));
  const list = await request('/articles?year=2023&orals=1&itemPerPage=-1');
  assert(list.data.some(a => a.id === published.id));
  assert(!list.data.some(a => a.id === draft.id));
  await request('/articles?itemPerPage=0', { status: 400 });
  await request('/articles/years');
  const ownPayload = {en_title: 'Member paper', en_abstract: 'Abstract', pap_keyword: 'Ecology|Climate', paper_approved: true, video_approved: true, pap_creater: '999',
    authors: [{first_name: 'Test', last_name: 'Author', email: memberEmail, correspond: true, presenter: true}]};
  let own = await request('/submission', {token: member, method: 'PUT', body: ownPayload});
  assert.equal(own.pap_creater, String(me.id)); assert.equal(own.paper_approved, false);
  await request(`/submission/${own.id}`, {status: 404});
  assert((await request('/submission?itemPerPage=-1', {token: member})).data.every(a => a.pap_creater === String(me.id)));
  const otherEmail = `other-${suffix}@example.test`;
  await request('/register', {method: 'POST', status: 201, body: {name: 'Other', email: otherEmail, password}});
  const other = await login(otherEmail, password);
  await request(`/submission/${own.id}`, {token: other, method: 'PATCH', body: ownPayload, status: 403});
  await request(`/submission/${own.id}/comments`, {token: other, status: 404});
  await request(`/submission/${own.id}/comments`, {token: other, method: 'POST', body: {isicerik: 'Intrusion'}, status: 403});
  const ticketResponse = await fetch(base + '/file/paper.doc/start?hash=' + encodeURIComponent('XUFAKrxLKna5cZ2REBfFkg=='), {method: 'PUT', headers: {Authorization: `Bearer ${member}`}});
  assert.equal(ticketResponse.status, 200);
  const ticket = await ticketResponse.json();
  assert.equal((await fetch(ticket.upload_url, {method: 'PUT', body: 'hello', headers: {'Content-Type': 'application/msword'}})).status, 201);
  const complete = await request(ticket.complete_url, {token: member, method: 'DELETE'});
  own = await request(`/submission/${own.id}`, {token: member, method: 'PATCH', body: {...own, abstract_dosya: {storage_file_id: complete.id}}});
  assert.equal(await (await fetch(own.abstract_dosya.access_url)).text(), 'hello');
  await request('/submission', {token: other, method: 'PUT', body: {...ownPayload, abstract_dosya: {storage_file_id: complete.id}}, status: 403});
  await request(`/submission/${own.id}/comments`, {token: member, method: 'POST', body: {isicerik: 'A question'}});
  await request(`/submission/${own.id}/comments`, {token: admin, method: 'POST', body: {isicerik: 'A reply'}});
  assert.equal((await request(`/submission/${own.id}/comments`, {token: member})).length, 2);
  own = await request(`/submission/${own.id}`, {token: admin, method: 'PATCH', body: {...own, paper_approved: true}});
  await request(`/submission/${own.id}`, {token: member, method: 'PATCH', body: own, status: 409});
  const notices = await request('/notification', {token: member});
  assert(notices.data.some(a => a.kind === 'approved'));
  await request(`/notification/${notices.data[0].id}/read`, {token: other, method: 'PATCH', status: 404});
  await request(`/notification/${notices.data[0].id}/read`, {token: member, method: 'PATCH', status: 204});
  await request('/user/me', {token: member, method: 'PATCH', body: {name: 'Updated', surname: 'Member', email: memberEmail, institution: 'Test University', faculty: 'Science', department: 'Ecology', address: 'Test address', iletisim: '123'}});
  const profile = await request('/user/me', {token: member});
  assert.equal(profile.name, 'Updated'); assert.equal(profile.institution, 'Test University');
  await request('/user/me/password', {token: member, method: 'POST', status: 400, body: {current_password: 'wrong', new_password: password + 'new'}});
  await request('/user/me/password', {token: member, method: 'POST', body: {current_password: password, new_password: password + 'new'}});
  await login(memberEmail, password + 'new');
  const language = await request('/typelanguage', {token: admin, method: 'PUT', body: {name: 'English', code: 'en-' + suffix.slice(0, 8), active: true}});
  const presentationType = await request('/typepresentation', {token: admin, method: 'PUT', body: {name: 'Oral', active: true, lang: {id: language.id}}});
  assert.equal(presentationType.lang.name, 'English');
  const commentRows = await request(`/submission/${own.id}/comments`, {token: member});
  await request(`/submission/${own.id}/comments/${commentRows[1].id}`, {token: member, method: 'PATCH', body: {isicerik: 'Forbidden edit'}, status: 403});
  await request(`/submission/${own.id}/comments/${commentRows[0].id}`, {token: member, method: 'PATCH', body: {isicerik: 'Edited question'}, status: 204});
  await request(`/typelanguage/${language.id}`, {token: admin, method: 'DELETE', status: 409});
  for (let i = 0; i < 100 && !delivered.some(a => a.includes('Paper approved') && a.includes(memberEmail)); i++) await new Promise(r => setTimeout(r, 200));
  assert(delivered.some(a => a.includes('Paper approved') && a.includes(memberEmail)), 'Approval email was not delivered.');
  assert(delivered.some(a => a.includes('New paper comment') && a.includes(memberEmail)), 'Comment email was not delivered.');
  let retried = false;
  for (let i = 0; i < 100; i++) {
    const outbox = await request('/notification/outbox?itemPerPage=1000', {token: admin});
    if (outbox.data.some(a => a.attempts >= 2 && a.sent_at)) { retried = true; break; }
    await new Promise(r => setTimeout(r, 200));
  }
  assert(retried, 'Transient SMTP failure was not retried to completion.');
  await request('/notification/outbox', {token: member, status: 403});
  if (process.env.SYMPOSIUM_UI_URL) {
    const parameterIds = [];
    for (const type of ['keywordcon', 'uygulamacon', 'birimcon']) {
      parameterIds.push((await request(`/parameters/${type}`, {token: admin, method: 'PUT', body: {value: 'Test ' + type}})).id);
    }
    const editable = await request('/submission', {token: member, method: 'PUT', body: {...ownPayload,
      topic: {id: parameterIds[0]}, parampre: {id: parameterIds[1]}, parampap: {id: parameterIds[2]},
      abstract_dosya: {storage_file_id: complete.id},
      authors: ownPayload.authors.map(a => ({...a, adress: 'Test University'}))}});
    const {verifyUi} = await import('./ui-workflows.mjs');
    await verifyUi({url: process.env.SYMPOSIUM_UI_URL, base, email, password, memberEmail, memberPassword: password + 'new',
      articleId: editable.id, memberToken: member});
  }
  console.log(`HTTP smoke passed: ${checks} requests, PostgreSQL migrations, member ownership, comments, SMTP retry, uploads, profile/password and language catalogs.`);
} finally {
  server.kill('SIGTERM');
  await new Promise(resolve => server.exitCode !== null ? resolve() : server.once('exit', resolve));
  rmSync(documentsRoot, { recursive: true, force: true });
  await new Promise(resolve => smtp.close(resolve));
  await new Promise(resolve => storage.close(resolve));
}
