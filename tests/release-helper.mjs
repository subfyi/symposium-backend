import {execFileSync, spawnSync} from 'node:child_process';
import {mkdtempSync, mkdirSync, copyFileSync, writeFileSync, rmSync} from 'node:fs';
import {tmpdir} from 'node:os';
import {join} from 'node:path';
import {fileURLToPath} from 'node:url';
import assert from 'node:assert/strict';

const fixture = mkdtempSync(join(tmpdir(), 'symposium-release-test-'));
const work = join(fixture, 'work');
const remote = join(fixture, 'remote.git');
const git = (args, cwd = work) => execFileSync('git', ['-c', 'user.name=Release Test', '-c', 'user.email=release@example.test', ...args], {cwd, encoding: 'utf8', stdio: ['ignore', 'pipe', 'pipe']}).trim();
const run = (...args) => spawnSync('bash', ['scripts/release.sh', ...args], {cwd: work, encoding: 'utf8'});
try {
  mkdirSync(work);
  git(['init', '--bare', remote], fixture);
  git(['init', '-b', 'main']);
  mkdirSync(join(work, 'scripts'));
  copyFileSync(fileURLToPath(new URL('../scripts/release.sh', import.meta.url)), join(work, 'scripts/release.sh'));
  git(['add', '.']); git(['commit', '-m', 'Fixture']);
  git(['remote', 'add', 'origin', remote]); git(['push', '-u', 'origin', 'main']);
  const preview = run();
  assert.equal(preview.status, 0, preview.stderr);
  assert(preview.stdout.includes('v0.0.1')); assert.equal(git(['tag']), '');
  assert.notEqual(run('--version', '01.2.3').status, 0);
  const published = run('--push');
  assert.equal(published.status, 0, published.stderr);
  assert.equal(git(['tag']), 'v0.0.1');
  assert.equal(git(['cat-file', '-t', 'v0.0.1']), 'tag');
  assert(git(['ls-remote', '--tags', 'origin']).includes('refs/tags/v0.0.1'));
  assert(run('--minor').stdout.includes('v0.1.0'));
  assert(run('--major').stdout.includes('v1.0.0'));
  assert.notEqual(run('--version', '0.0.1').status, 0);
  writeFileSync(join(work, 'dirty-fixture'), 'test');
  assert.notEqual(run().status, 0); rmSync(join(work, 'dirty-fixture'));
  git(['commit', '--allow-empty', '-m', 'Unpushed change']);
  assert.notEqual(run('--push').status, 0);
  assert.equal(git(['tag']), 'v0.0.1');
  console.log('Release helper checks passed: dry run, stable versions, exact annotated tag push, minor/major bumps, duplicate/dirty/unpushed rejection. Only temporary local repositories were used.');
} finally { rmSync(fixture, {recursive: true, force: true}); }
