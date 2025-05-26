import { test } from '@playwright/test';

for (let i = 0; i < 10000; i++) {
  const n = String(i + 1);
  const id = n.padStart(3, ' ');
  test(`load homepage - instance ${id}`, async ({ page }) => {
    await page.goto('http://localhost:5085');
    await page.close();
  });
};

