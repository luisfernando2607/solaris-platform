/**
 * curl.js
 * Generador de comandos curl y copiado rápido desde el API Explorer.
 */

let currentToken = null;

// Intenta leer el token que pudo haber guardado test.html
try { currentToken = localStorage.getItem('solaris-token'); } catch { /* sin acceso a storage */ }

/* ── Genera el comando curl ─────────────────────────── */
function generateCurl() {
  const method    = document.getElementById('curlMethod').value;
  const path      = document.getElementById('curlPath').value || '/api/Auth/me';
  const url       = `${window.location.origin}${path}`;
  const tokenDot  = document.getElementById('tokenDot');
  const tokenHint = document.getElementById('tokenHintText');

  let cmd = `curl -X ${method} \\\n  '${url}' \\\n  -H 'Content-Type: application/json'`;

  if (currentToken) {
    cmd += ` \\\n  -H 'Authorization: Bearer ${currentToken.substring(0, 20)}…'`;
    tokenDot.className   = 'token-dot has-token';
    tokenHint.textContent = 'Token detectado — endpoints protegidos disponibles';
  } else {
    tokenDot.className   = 'token-dot';
    tokenHint.textContent = 'Sin token — usa /api/Auth/login para obtener uno';
  }

  if (['POST', 'PUT', 'PATCH'].includes(method)) {
    cmd += ` \\\n  -d '{}'`;
  }

  document.getElementById('curlText').textContent = cmd;
}

/* ── Copia el curl generado al portapapeles ─────────── */
function copyCurl() {
  const text = document.getElementById('curlText').textContent;
  navigator.clipboard.writeText(text).then(() => {
    const btn = document.getElementById('copyBtn');
    btn.textContent = '✓ Copiado';
    btn.classList.add('copied');
    setTimeout(() => { btn.textContent = 'Copiar'; btn.classList.remove('copied'); }, 1800);
  });
}

/**
 * Inyecta path y método desde las tarjetas del API Explorer,
 * genera el curl y lo copia al portapapeles de inmediato.
 */
function quickCopy(path, method) {
  document.getElementById('curlPath').value  = path;
  document.getElementById('curlMethod').value = method;
  generateCurl();
  copyCurl();
  document.querySelector('.devtools-grid')?.scrollIntoView({ behavior: 'smooth', block: 'nearest' });
}

// Expone al scope global (usados en onclick inline del HTML)
window.generateCurl = generateCurl;
window.copyCurl     = copyCurl;
window.quickCopy    = quickCopy;

// Render inicial
generateCurl();

// Detecta cambios de token hechos desde otra pestaña (ej. test.html)
setInterval(() => {
  try {
    const t = localStorage.getItem('solaris-token');
    if (t !== currentToken) { currentToken = t; generateCurl(); }
  } catch { /* sin acceso a storage */ }
}, 3_000);
