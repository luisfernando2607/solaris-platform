#!/usr/bin/env bash
# =====================================================================
# SOLARIS PLATFORM — Parche DI para módulo RRHH
# Agrega el registro de servicios y repositorios RRHH al
# DependencyInjection.cs de Infrastructure
# =====================================================================

set -e
RED='\033[0;31m'; GREEN='\033[0;32m'; YELLOW='\033[1;33m'; CYAN='\033[0;36m'; RESET='\033[0m'

# ── Detectar archivo DI ───────────────────────────────────────────────
DI_FILE=""
SEARCH_PATHS=(
  "$HOME/projects/solaris_platform/src/SolarisPlatform.Infrastructure/DependencyInjection.cs"
  "$(pwd)/src/SolarisPlatform.Infrastructure/DependencyInjection.cs"
  "$(pwd)/SolarisPlatform.Infrastructure/DependencyInjection.cs"
)

for p in "${SEARCH_PATHS[@]}"; do
  if [ -f "$p" ]; then DI_FILE="$p"; break; fi
done

if [ -z "$DI_FILE" ]; then
  echo ""
  echo -e "${YELLOW}  No se encontró DependencyInjection.cs automáticamente.${RESET}"
  echo -n "  Ingresa la ruta completa al archivo: "
  read -r DI_FILE
  [ -f "$DI_FILE" ] || { echo -e "${RED}  ✗ Archivo no encontrado.${RESET}"; exit 1; }
fi

echo ""
echo -e "${CYAN}  Archivo detectado:${RESET} $DI_FILE"
echo ""

# ── Verificar que ya no está aplicado ────────────────────────────────
if grep -q "IDepartamentoService" "$DI_FILE" 2>/dev/null; then
  echo -e "${YELLOW}  ⚠ El bloque RRHH ya parece estar registrado en este archivo.${RESET}"
  echo -e "    Verifica manualmente que los servicios estén registrados."
  exit 0
fi

# ── Backup ───────────────────────────────────────────────────────────
BAK="${DI_FILE}.bak.$(date +%Y%m%d%H%M%S)"
cp "$DI_FILE" "$BAK"
echo -e "${GREEN}  ✓ Backup creado:${RESET} $BAK"

# ── Bloque RRHH a insertar ────────────────────────────────────────────
BLOQUE_RRHH='
        // ==========================================
        // RRHH — Servicios
        // ==========================================
        services.AddScoped<IDepartamentoService,    DepartamentoService>();
        services.AddScoped<IPuestoService,          PuestoService>();
        services.AddScoped<IEmpleadoService,        EmpleadoService>();
        services.AddScoped<IAsistenciaService,      AsistenciaService>();
        services.AddScoped<IConceptoNominaService,  ConceptoNominaService>();
        services.AddScoped<INominaService,          NominaService>();
        services.AddScoped<IPrestamoService,        PrestamoService>();
        services.AddScoped<IEvaluacionService,      EvaluacionService>();
        services.AddScoped<ICapacitacionService,    CapacitacionService>();
        services.AddScoped<IRrhhDashboardService,   RrhhDashboardService>();

        // RRHH — Repositorios
        services.AddScoped<IDepartamentoRepository, DepartamentoRepository>();
        services.AddScoped<IPuestoRepository,       PuestoRepository>();
        services.AddScoped<IEmpleadoRepository,     EmpleadoRepository>();
'

# ── Bloque using a insertar (al inicio del archivo) ───────────────────
BLOQUE_USING='using SolarisPlatform.Application.Interfaces.RRHH;
using SolarisPlatform.Infrastructure.Services.RRHH;
using SolarisPlatform.Infrastructure.Persistence.Repositories.RRHH;
using SolarisPlatform.Domain.Interfaces.RRHH;'

# Insertar usings después del último using existente
python3 - "$DI_FILE" "$BLOQUE_USING" "$BLOQUE_RRHH" << 'PYEOF'
import sys, re

di_path      = sys.argv[1]
using_block  = sys.argv[2]
service_block = sys.argv[3]

with open(di_path, 'r') as f:
    content = f.read()

# 1. Insertar usings después del último bloque de usings
last_using = 0
for m in re.finditer(r'^using .+;', content, re.MULTILINE):
    last_using = m.end()

if last_using > 0:
    insert_pos = last_using
    content = content[:insert_pos] + '\n// ── RRHH ──────────────────────────────────────────────────────────────\n' + using_block + content[insert_pos:]

# 2. Insertar bloque de servicios ANTES de "services.AddAuthorization();"
anchor = 'services.AddAuthorization();'
if anchor in content:
    content = content.replace(anchor, service_block + '\n        ' + anchor, 1)
else:
    # fallback: antes del último return services;
    content = content.replace('        return services;', service_block + '\n\n        return services;', 1)

with open(di_path, 'w') as f:
    f.write(content)

print("OK")
PYEOF

echo -e "${GREEN}  ✓ Bloque RRHH insertado correctamente${RESET}"
echo ""
echo -e "  ${YELLOW}IMPORTANTE:${RESET} Verifica que existan las implementaciones concretas:"
echo -e "  ${CYAN}Infrastructure/Services/RRHH/${RESET}          → DepartamentoService, PuestoService, EmpleadoService..."
echo -e "  ${CYAN}Infrastructure/Persistence/Repositories/RRHH/${RESET} → DepartamentoRepository, PuestoRepository, EmpleadoRepository"
echo ""
echo -e "  Luego ejecuta:"
echo -e "  ${CYAN}cd ~/projects/solaris_platform/src/SolarisPlatform.API && dotnet run${RESET}"
echo ""
