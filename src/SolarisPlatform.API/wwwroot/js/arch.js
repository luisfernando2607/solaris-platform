/**
 * arch.js
 * Accordion para las capas de arquitectura del sistema.
 */

/**
 * Expande / colapsa una capa del stack de arquitectura.
 * @param {string} id  — sufijo del layer: 'client' | 'api' | 'app' | 'domain' | 'infra' | 'db'
 */
function toggleLayer(id) {
  document.getElementById('layer-' + id)?.classList.toggle('open');
}

// Expone al scope global para los onclick inline del HTML
window.toggleLayer = toggleLayer;
