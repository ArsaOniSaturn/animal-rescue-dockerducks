'use strict';

const form = document.querySelector('#report-form');
const submit = document.querySelector('#submit-report');
const refresh = document.querySelector('#refresh');
const queue = document.querySelector('#queue');
const formStatus = document.querySelector('#form-status');
const queueStatus = document.querySelector('#queue-status');
const priorities = { Critical: 'Crítica', High: 'Alta', Medium: 'Media' };
const conditions = { Stable: 'Estable', Injured: 'Herido', Critical: 'Crítico' };
const dateFormat = new Intl.DateTimeFormat('es-CO', { dateStyle: 'short', timeStyle: 'short' });
let saving = false;
let queueVersion = 0;

function message(target, text, state = '') {
  target.textContent = text;
  target.dataset.state = state;
}

async function request(path, options = {}) {
  const response = await fetch(path, { ...options, signal: AbortSignal.timeout(10000) });
  if (!response.ok) throw new Error(`HTTP ${response.status}`);
  return response;
}

function textElement(tag, text, className = '') {
  const element = document.createElement(tag);
  element.textContent = text;
  element.className = className;
  return element;
}

function reportCard(report) {
  const card = document.createElement('article');
  card.className = 'report';
  // Treat all report content as text: reports are untrusted user input.
  card.append(
    textElement('span', `Prioridad ${priorities[report.priority] ?? report.priority}`, `priority priority-${report.priority.toLowerCase()}`),
    textElement('h3', report.animalDescription),
    textElement('p', report.situationDescription),
    textElement('p', `${report.municipalitySector} · Cantidad: ${report.quantity} · ${conditions[report.condition] ?? report.condition}`),
    textElement('p', `Peligro inmediato: ${report.immediateDanger ? 'Sí' : 'No'} · ${dateFormat.format(new Date(report.createdAt))}`, 'metadata')
  );
  return card;
}

async function loadQueue() {
  const version = ++queueVersion;
  refresh.disabled = true;
  queue.setAttribute('aria-busy', 'true');
  message(queueStatus, 'Cargando reportes…');
  try {
    const response = await request('api/rescue-reports/queue');
    const reports = await response.json();
    if (version !== queueVersion) return;
    queue.replaceChildren(...reports.map(reportCard));
    document.querySelector('#queue-count').textContent = `Reportes: ${reports.length}`;
    message(queueStatus, reports.length ? 'Cola actualizada.' : 'Todavía no hay reportes. Registra el primero.');
  } catch {
    if (version === queueVersion) {
      message(queueStatus, 'No se pudo cargar la cola. Los reportes mostrados pueden estar desactualizados. Intenta con Actualizar.', 'error');
    }
  } finally {
    if (version === queueVersion) {
      refresh.disabled = false;
      queue.setAttribute('aria-busy', 'false');
    }
  }
}

for (const field of form.querySelectorAll('input[required]:not([type="number"]), textarea[required]')) {
  field.addEventListener('input', () => field.setCustomValidity(''));
}

form.addEventListener('submit', async event => {
  event.preventDefault();
  if (saving) return;
  for (const field of form.querySelectorAll('input[required]:not([type="number"]), textarea[required]')) {
    field.setCustomValidity(field.value.trim() ? '' : 'Escribe un valor; no solo espacios.');
  }
  if (!form.reportValidity()) return;

  const fields = new FormData(form);
  const report = {
    animalDescription: fields.get('animalDescription').trim(),
    municipalitySector: fields.get('municipalitySector').trim(),
    situationDescription: fields.get('situationDescription').trim(),
    condition: fields.get('condition'),
    immediateDanger: fields.has('immediateDanger'),
    quantity: Number(fields.get('quantity'))
  };
  saving = true;
  submit.disabled = true;
  message(formStatus, 'Registrando reporte…');
  try {
    await request('api/rescue-reports', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(report)
    });
    form.reset();
    message(formStatus, 'Reporte registrado. Su prioridad fue calculada por la aplicación.', 'ok');
    await loadQueue();
  } catch {
    message(formStatus, 'No se pudo confirmar el registro. Actualiza la cola antes de reintentar para evitar duplicados. Tus campos se conservaron.', 'error');
  } finally {
    saving = false;
    submit.disabled = false;
  }
});

async function checkHealth() {
  const status = document.querySelector('#health-status');
  try {
    await request('health');
    message(status, 'Servicio disponible', 'ok');
  } catch {
    message(status, 'Servicio no disponible', 'error');
  }
}

refresh.addEventListener('click', () => {
  void loadQueue();
  void checkHealth();
});
void loadQueue();
void checkHealth();
