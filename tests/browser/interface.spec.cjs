const { test, expect } = require('playwright/test');

async function fillReport(page, animal = 'Búho de prueba') {
  await page.getByLabel('Descripción del animal').fill(animal);
  await page.getByLabel('Municipio o sector').fill('Sector norte');
  await page.getByLabel('Descripción de la situación').fill('No puede volar');
  await page.getByLabel('Condición del animal').selectOption('Injured');
}

test('carga en móvil y escritorio sin desbordamiento horizontal', async ({ page }) => {
  const errors = [];
  page.on('pageerror', error => errors.push(error.message));
  for (const width of [390, 1280]) {
    await page.setViewportSize({ width, height: 900 });
    await page.goto('/');
    await expect(page.getByRole('heading', { name: 'Registrar rescate' })).toBeVisible();
    await expect(page.getByRole('heading', { name: 'Cola de atención' })).toBeVisible();
    await expect(page.locator('#health-status')).toHaveText('Servicio disponible');
    await expect(page.locator('#queue-status')).not.toHaveText('Cargando reportes…');
    expect(await page.evaluate(() => document.documentElement.scrollWidth <= innerWidth)).toBe(true);
  }
  expect(errors).toEqual([]);
});

test('registra reportes y respeta la prioridad devuelta por la API', async ({ page }) => {
  await page.goto('/');
  await fillReport(page, 'Animal estable');
  await page.getByLabel('Condición del animal').selectOption('Stable');
  await page.getByRole('button', { name: 'Registrar rescate' }).click();
  await expect(page.locator('#form-status')).toContainText('Reporte registrado');
  await fillReport(page, 'Búho en peligro');
  await page.getByLabel('Existe peligro inmediato').check();
  await page.getByRole('button', { name: 'Registrar rescate' }).click();
  await expect(page.locator('#form-status')).toContainText('Reporte registrado');
  const cards = page.locator('#queue article');
  await expect(cards.first()).toContainText('Búho en peligro');
  await expect(cards.first()).toContainText('Crítica');
  await expect(cards.last()).toContainText('Animal estable');
  await expect(page.getByLabel('Descripción del animal')).toHaveValue('');
});

test('rechaza campos vacíos, espacios y cantidades no positivas sin enviar', async ({ page }) => {
  let posts = 0;
  page.on('request', request => {
    if (request.method() === 'POST') posts++;
  });
  await page.goto('/');
  await page.getByRole('button', { name: 'Registrar rescate' }).click();
  await expect(page.getByLabel('Descripción del animal')).toBeFocused();
  await fillReport(page, '   ');
  await page.getByRole('button', { name: 'Registrar rescate' }).click();
  await expect(page.getByLabel('Descripción del animal')).toBeFocused();
  await fillReport(page);
  await page.getByLabel('Cantidad de animales').fill('0');
  await page.getByRole('button', { name: 'Registrar rescate' }).click();
  await expect(page.getByLabel('Cantidad de animales')).toBeFocused();
  expect(posts).toBe(0);
});

test('muestra errores de cola y permite recuperarse con Actualizar', async ({ page }) => {
  await page.route('**/api/rescue-reports/queue', route => route.fulfill({ status: 503 }));
  await page.goto('/');
  await expect(page.locator('#queue-status')).toContainText('No se pudo cargar');
  await page.unroute('**/api/rescue-reports/queue');
  await page.getByRole('button', { name: 'Actualizar' }).click();
  await expect(page.locator('#queue-status')).not.toContainText('No se pudo cargar');
  await expect(page.getByRole('button', { name: 'Actualizar' })).toBeEnabled();
});

test('explica la cola vacía sin inventar reportes', async ({ page }) => {
  await page.route('**/api/rescue-reports/queue', route => route.fulfill({ json: [] }));
  await page.goto('/');
  await expect(page.locator('#queue-status')).toContainText('Todavía no hay reportes');
  await expect(page.locator('#queue-count')).toHaveText('Reportes: 0');
  await expect(page.locator('#queue article')).toHaveCount(0);
});

test('no muestra servicio disponible cuando falla health', async ({ page }) => {
  await page.route('**/health', route => route.abort());
  await page.goto('/');
  await expect(page.locator('#health-status')).toHaveText('Servicio no disponible');
});

test('un fallo al registrar conserva el formulario para revisarlo', async ({ page }) => {
  await page.route('**/api/rescue-reports', route => route.fulfill({ status: 500 }));
  await page.goto('/');
  await fillReport(page);
  await page.getByRole('button', { name: 'Registrar rescate' }).click();
  await expect(page.locator('#form-status')).toContainText('No se pudo confirmar');
  await expect(page.getByLabel('Descripción del animal')).toHaveValue('Búho de prueba');
  await expect(page.getByRole('button', { name: 'Registrar rescate' })).toBeEnabled();
});

test('evita doble envío y presenta las entradas como texto, no HTML', async ({ page }) => {
  let posts = 0;
  let releasePost;
  const gate = new Promise(resolve => { releasePost = resolve; });
  await page.route('**/api/rescue-reports', async route => {
    posts++;
    await gate;
    await route.continue();
  });
  await page.goto('/');
  const unsafeText = '<img src=x onerror="window.injected=true">';
  await fillReport(page, unsafeText);
  const submit = page.getByRole('button', { name: 'Registrar rescate' });
  await submit.click();
  await expect(submit).toBeDisabled();
  await page.locator('form').evaluate(form => form.requestSubmit());
  releasePost();
  await expect(page.locator('#form-status')).toContainText('Reporte registrado');
  await expect(page.locator('#queue')).toContainText(unsafeText);
  expect(posts).toBe(1);
  expect(await page.evaluate(() => window.injected)).toBeUndefined();
  await expect(page.locator('#queue img')).toHaveCount(0);
});
