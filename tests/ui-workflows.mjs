import assert from 'node:assert/strict';
import { mkdirSync } from 'node:fs';

export async function verifyUi({url, base, email, password, memberEmail, memberPassword, articleId, memberToken}) {
  const {chromium} = await import(process.env.PLAYWRIGHT_MODULE || 'playwright');
  const browser = await chromium.launch({channel: 'chrome', headless: true});
  const screenshots = process.env.SYMPOSIUM_SCREENSHOTS || '/tmp/symposium-functional-ui';
  mkdirSync(screenshots, {recursive: true});
  try {
    for (const width of [1440, 390]) {
      const context = await browser.newContext({viewport: {width, height: 960}});
      const page = await context.newPage();
      const errors = [];
      page.on('pageerror', e => errors.push(e.message));
      await page.goto(url + '/login');
      await page.locator('input[type=text]').first().fill(memberEmail);
      await page.locator('input[type=password]').fill(memberPassword);
      await page.locator('button[type=submit]').click();
      await page.locator('.rab-tabler-shell').waitFor();
      await page.goto(url + '/profile');
      await page.getByLabel('Institution', {exact: true}).fill('UI University ' + width);
      await page.locator('form').first().getByRole('button', {name: /^Save/}).click();
      await page.getByText('Saved', {exact: false}).first().waitFor();
      const profile = await (await fetch(base + '/user/me', {headers: {Authorization: `Bearer ${memberToken}`}})).json();
      assert.equal(profile.institution, 'UI University ' + width);
      await page.screenshot({path: `${screenshots}/${width}-profile.png`, fullPage: true});
      await page.goto(url + `/submission/${articleId}/edit`);
      const title = page.getByText('Article Title (ENG)', {exact: false}).first().locator('..').locator('..').locator('input');
      await title.fill('UI edited paper ' + width);
      await page.locator('form').first().getByRole('button', {name: /^Save/}).click();
      await page.getByText('Saved', {exact: false}).first().waitFor();
      const article = await (await fetch(base + `/submission/${articleId}`, {headers: {Authorization: `Bearer ${memberToken}`}})).json();
      assert.equal(article.en_title, 'UI edited paper ' + width);
      await page.getByLabel('Comment', {exact: true}).fill('UI comment ' + width);
      await page.getByRole('button', {name: 'Send Comment', exact: true}).click();
      await page.getByText('UI comment ' + width, {exact: true}).waitFor();
      for (const theme of ['light', 'dark']) {
        if (await page.locator('html').getAttribute('data-bs-theme') !== theme)
          await page.getByRole('button', {name: `Use ${theme} theme`}).click();
        await page.evaluate(() => window.scrollTo(0, 0));
        await page.screenshot({path: `${screenshots}/${width}-submission-${theme}.png`, fullPage: true});
        assert(await page.evaluate(() => document.documentElement.scrollWidth <= innerWidth + 1));
      }
      await page.goto(url + '/notifications');
      await page.getByRole('heading', {name: 'Notifications'}).waitFor();
      await page.screenshot({path: `${screenshots}/${width}-notifications.png`, fullPage: true});
      assert.deepEqual(errors, []);
      await context.close();
    }
    const context = await browser.newContext({viewport: {width: 1440, height: 960}});
    const page = await context.newPage();
    await page.goto(url + '/login');
    await page.locator('input[type=text]').first().fill(email);
    await page.locator('input[type=password]').fill(password);
    await page.locator('button[type=submit]').click();
    await page.locator('.rab-tabler-shell').waitFor();
    for (const route of ['/language', '/typepresentation', '/typepublish', '/typetopic']) {
      await page.goto(url + route);
      await page.getByRole('button', {name: 'Add', exact: true}).waitFor();
      await page.screenshot({path: `${screenshots}/admin-${route.slice(1)}.png`, fullPage: true});
    }
    await context.close();
    console.log('UI workflows passed: real member login, profile save, submission save, comment save, notifications and admin catalogs; desktop/mobile and light/dark.');
  } finally { await browser.close(); }
}
