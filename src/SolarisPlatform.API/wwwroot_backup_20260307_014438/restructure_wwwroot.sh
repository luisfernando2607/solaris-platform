#!/bin/bash
# ============================================================
#  restructure_wwwroot.sh
#  Reorganiza la estructura de wwwroot siguiendo mejores prácticas
#  Uso: bash restructure_wwwroot.sh [ruta/a/wwwroot]
# ============================================================

set -euo pipefail

# ── Colores ──────────────────────────────────────────────────
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
BOLD='\033[1m'
RESET='\033[0m'

# ── Helpers ──────────────────────────────────────────────────
ok()   { echo -e "${GREEN}  ✔ ${1}${RESET}"; }
info() { echo -e "${CYAN}  → ${1}${RESET}"; }
warn() { echo -e "${YELLOW}  ⚠ ${1}${RESET}"; }
fail() { echo -e "${RED}  ✖ ${1}${RESET}"; exit 1; }
section() { echo -e "\n${BOLD}${CYAN}══ ${1} ══${RESET}"; }

# ── Directorio objetivo ──────────────────────────────────────
WWWROOT="${1:-$(pwd)}"
[[ -d "$WWWROOT" ]] || fail "Directorio no encontrado: $WWWROOT"
WWWROOT="$(realpath "$WWWROOT")"

echo -e "\n${BOLD}╔══════════════════════════════════════════╗"
echo -e "║     wwwroot restructure  •  Solaris      ║"
echo -e "╚══════════════════════════════════════════╝${RESET}"
echo -e "  Directorio: ${CYAN}${WWWROOT}${RESET}\n"

# ── Confirmación ─────────────────────────────────────────────
read -rp "  ¿Continuar con la reestructuración? [s/N] " confirm
[[ "$confirm" =~ ^[sS]$ ]] || { echo "  Cancelado."; exit 0; }

# ── Backup ───────────────────────────────────────────────────
section "Backup"
BACKUP_DIR="${WWWROOT}/../wwwroot_backup_$(date +%Y%m%d_%H%M%S)"
cp -r "$WWWROOT" "$BACKUP_DIR"
ok "Backup creado en: $(realpath "$BACKUP_DIR")"

# ── Crear nueva estructura de carpetas ───────────────────────
section "Creando estructura de carpetas"

mkdir -p "$WWWROOT/assets/css"
ok "assets/css/"
mkdir -p "$WWWROOT/assets/img"
ok "assets/img/"
mkdir -p "$WWWROOT/assets/js"
ok "assets/js/"
mkdir -p "$WWWROOT/pages"
ok "pages/"

# ── Mover assets estáticos ───────────────────────────────────
section "Moviendo assets"

move_if_exists() {
  local src="$1" dst="$2" label="$3"
  if [[ -f "$src" ]]; then
    mv "$src" "$dst"
    ok "$label → $(realpath --relative-to="$WWWROOT" "$dst")"
  else
    warn "$label no encontrado, omitiendo"
  fi
}

move_if_exists "$WWWROOT/solaris.css"    "$WWWROOT/assets/css/solaris.css"   "solaris.css"
move_if_exists "$WWWROOT/favicon.ico"    "$WWWROOT/assets/img/favicon.ico"   "favicon.ico"
move_if_exists "$WWWROOT/logo_back.png"  "$WWWROOT/assets/img/logo_back.png" "logo_back.png"

# ── Mover páginas (no el index) ──────────────────────────────
section "Moviendo páginas a pages/"

for page in api health; do
  move_if_exists "$WWWROOT/${page}.html" "$WWWROOT/pages/${page}.html" "${page}.html"
done

# test.html va dentro de test/
move_if_exists "$WWWROOT/test.html" "$WWWROOT/test/test.html" "test.html"

# ── Actualizar referencias en archivos HTML ──────────────────
section "Actualizando referencias internas"

# Función: reemplaza referencias en un HTML dado su prefijo de retroceso
update_refs() {
  local file="$1"
  local prefix="$2"   # ej: ".." para pages/, "../.." para subcarpetas
  [[ -f "$file" ]] || return

  local fname
  fname="$(basename "$file")"

  # CSS
  sed -i "s|href=\"solaris\.css\"|href=\"${prefix}/assets/css/solaris.css\"|g"  "$file"
  sed -i "s|href=\"/solaris\.css\"|href=\"${prefix}/assets/css/solaris.css\"|g" "$file"

  # Imágenes
  sed -i "s|src=\"logo_back\.png\"|src=\"${prefix}/assets/img/logo_back.png\"|g"  "$file"
  sed -i "s|src=\"/logo_back\.png\"|src=\"${prefix}/assets/img/logo_back.png\"|g" "$file"
  sed -i "s|href=\"favicon\.ico\"|href=\"${prefix}/assets/img/favicon.ico\"|g"    "$file"
  sed -i "s|href=\"/favicon\.ico\"|href=\"${prefix}/assets/img/favicon.ico\"|g"   "$file"

  # Links entre páginas (desde pages/ → otras páginas en pages/)
  if [[ "$prefix" == ".." ]]; then
    sed -i "s|href=\"api\.html\"|href=\"api.html\"|g"       "$file"
    sed -i "s|href=\"health\.html\"|href=\"health.html\"|g" "$file"
    sed -i "s|href=\"test\.html\"|href=\"../test/test.html\"|g" "$file"
    sed -i "s|href=\"index\.html\"|href=\"../index.html\"|g"    "$file"
  fi

  ok "Referencias actualizadas: $(realpath --relative-to="$WWWROOT" "$file")"
}

# index.html está en raíz → prefix vacío, usa rutas relativas directas
if [[ -f "$WWWROOT/index.html" ]]; then
  sed -i "s|href=\"solaris\.css\"|href=\"assets/css/solaris.css\"|g"     "$WWWROOT/index.html"
  sed -i "s|href=\"/solaris\.css\"|href=\"assets/css/solaris.css\"|g"    "$WWWROOT/index.html"
  sed -i "s|src=\"logo_back\.png\"|src=\"assets/img/logo_back.png\"|g"   "$WWWROOT/index.html"
  sed -i "s|src=\"/logo_back\.png\"|src=\"assets/img/logo_back.png\"|g"  "$WWWROOT/index.html"
  sed -i "s|href=\"favicon\.ico\"|href=\"assets/img/favicon.ico\"|g"     "$WWWROOT/index.html"
  sed -i "s|href=\"/favicon\.ico\"|href=\"assets/img/favicon.ico\"|g"    "$WWWROOT/index.html"
  sed -i "s|href=\"api\.html\"|href=\"pages/api.html\"|g"                "$WWWROOT/index.html"
  sed -i "s|href=\"health\.html\"|href=\"pages/health.html\"|g"          "$WWWROOT/index.html"
  sed -i "s|href=\"test\.html\"|href=\"test/test.html\"|g"               "$WWWROOT/index.html"
  ok "Referencias actualizadas: index.html"
fi

# pages/*.html → prefix ".."
for f in "$WWWROOT/pages/"*.html; do
  update_refs "$f" ".."
done

# test/test.html → prefix ".."
update_refs "$WWWROOT/test/test.html" ".."

# ── Resultado final ──────────────────────────────────────────
section "Estructura final"

if command -v tree &>/dev/null; then
  tree -a -I ".git|bin|obj|node_modules|.vs" "$WWWROOT"
else
  find "$WWWROOT" | sed "s|$WWWROOT||" | sort
fi

echo -e "\n${GREEN}${BOLD}  ✔ Reestructuración completada exitosamente${RESET}"
echo -e "  ${YELLOW}Backup disponible en: $(realpath "$BACKUP_DIR")${RESET}"
echo -e "  ${CYAN}Revisa los HTMLs para referencias no detectadas (JS dinámico, etc.)${RESET}\n"
