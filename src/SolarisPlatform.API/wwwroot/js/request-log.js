/**
 * request-log.js
 * Simulador de request log en tiempo real para el DevTools dashboard.
 */

/* ── Dataset de endpoints simulados ────────────────── */
const ENDPOINTS = [
  { method: 'GET',    path: '/api/Auth/me',                    status: 200, ms: () => rand(12, 45)  },
  { method: 'POST',   path: '/api/Auth/login',                 status: 200, ms: () => rand(30, 90)  },
  { method: 'POST',   path: '/api/Auth/refresh-token',         status: 200, ms: () => rand(15, 50)  },
  { method: 'GET',    path: '/api/Usuarios',                   status: 200, ms: () => rand(20, 80)  },
  { method: 'GET',    path: '/api/Usuarios/{id}',              status: 200, ms: () => rand(10, 40)  },
  { method: 'POST',   path: '/api/Usuarios',                   status: 201, ms: () => rand(40, 120) },
  { method: 'PUT',    path: '/api/Usuarios/{id}',              status: 200, ms: () => rand(35, 100) },
  { method: 'DELETE', path: '/api/Usuarios/{id}',              status: 204, ms: () => rand(20, 60)  },
  { method: 'GET',    path: '/api/Roles',                      status: 200, ms: () => rand(10, 35)  },
  { method: 'GET',    path: '/api/Empresas',                   status: 200, ms: () => rand(15, 55)  },
  { method: 'GET',    path: '/api/Proyectos/{id}/gantt',       status: 200, ms: () => rand(50, 180) },
  { method: 'GET',    path: '/api/Proyectos/{id}/kpis',        status: 200, ms: () => rand(40, 150) },
  { method: 'GET',    path: '/api/RRHH/dashboard',             status: 200, ms: () => rand(60, 200) },
  { method: 'GET',    path: '/api/RRHH/nomina',                status: 200, ms: () => rand(30, 90)  },
  { method: 'GET',    path: '/health',                         status: 200, ms: () => rand(5,  20)  },
  { method: 'POST',   path: '/api/Empresas/{id}/sucursales',   status: 201, ms: () => rand(45, 130) },
  { method: 'PUT',    path: '/api/Roles/{id}/permisos',        status: 200, ms: () => rand(25, 70)  },
  { method: 'GET',    path: '/api/Proyectos/{id}/wbs',         status: 200, ms: () => rand(55, 170) },
  // Errores ocasionales para realismo
  { method: 'GET',    path: '/api/Usuarios/{id}',              status: 404, ms: () => rand(8,  25)  },
  { method: 'POST',   path: '/api/Auth/login',                 status: 401, ms: () => rand(20, 50)  },
  { method: 'DELETE', path: '/api/Roles/{id}',                 status: 403, ms: () => rand(10, 30)  },
];

/* ── Estado interno ─────────────────────────────────── */
let logRunning  = false;
let logInterval = null;
let logEntries  = [];

function rand(min, max) {
  return Math.floor(Math.random() * (max - min + 1)) + min;
}

/* ── Renderiza las entradas del log ─────────────────── */
function renderLog() {
  const log   = document.getElementById('reqLog');
  const empty = document.getElementById('logEmpty');

  // Limpia entradas anteriores
  log.querySelectorAll('.req-entry').forEach(el => el.remove());

  if (logEntries.length === 0) {
    empty.style.display = 'block';
    document.getElementById('logCount').textContent = '0 entradas';
    return;
  }

  empty.style.display = 'none';

  logEntries.slice(0, 20).forEach((e, i) => {
    const div = document.createElement('div');
    div.className = 'req-entry' + (i === 0 ? ' new' : '');

    const sc = e.status >= 500 ? 's5'
             : e.status >= 400 ? 's4'
             : 's2';

    div.innerHTML = `
      <span class="req-method rm-${e.method}">${e.method}</span>
      <span class="req-path">${e.path}</span>
      <span class="req-status ${sc}">${e.status}</span>
      <span class="req-ms">${e.ms}ms</span>
    `;
    log.insertBefore(div, empty);
  });

  document.getElementById('logCount').textContent = `${logEntries.length} entradas`;
}

/* ── Agrega una entrada simulada ─────────────────────── */
function addLogEntry() {
  const ep    = ENDPOINTS[Math.floor(Math.random() * ENDPOINTS.length)];
  const ms    = ep.ms();
  const entry = {
    ...ep,
    ms,
    time: new Date().toLocaleTimeString('es-EC', { hour12: false }),
  };
  logEntries.unshift(entry);
  if (logEntries.length > 50) logEntries.pop();
  renderLog();
}

/* ── Toggle play / pausa ─────────────────────────────── */
function toggleRequestLog() {
  logRunning = !logRunning;
  const btn   = document.getElementById('logToggleBtn');
  const badge = document.getElementById('logBadge');

  if (logRunning) {
    btn.textContent = '⏸ Pausar simulación';
    btn.classList.add('active');
    badge.textContent         = 'LIVE';
    badge.style.color         = 'var(--green)';
    badge.style.borderColor   = 'rgba(34,197,94,.4)';
    badge.style.background    = 'rgba(34,197,94,.06)';
    logInterval = setInterval(addLogEntry, 1200);
    addLogEntry();
  } else {
    btn.textContent = '▶ Reanudar simulación';
    btn.classList.remove('active');
    badge.textContent       = 'PAUSED';
    badge.style.color       = 'var(--muted)';
    badge.style.borderColor = '';
    badge.style.background  = '';
    clearInterval(logInterval);
  }
}

/* ── Limpia el log ───────────────────────────────────── */
function clearLog() {
  logEntries = [];
  renderLog();
}

// Expone al scope global (usados en onclick inline del HTML)
window.toggleRequestLog = toggleRequestLog;
window.clearLog         = clearLog;
