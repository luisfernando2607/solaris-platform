#!/usr/bin/env bash
# =====================================================================
# SOLARIS PLATFORM — Instalar servicios RRHH faltantes
# Copia DepartamentoService, PuestoService, EmpleadoService +
# stubs de AsistenciaService, ConceptoNominaService, NominaService,
# PrestamoService, EvaluacionService, CapacitacionService,
# RrhhDashboardService
# =====================================================================
set -e
GREEN='\033[0;32m'; YELLOW='\033[1;33m'; CYAN='\033[0;36m'; RED='\033[0;31m'; RESET='\033[0m'

INFRA="$HOME/projects/solaris_platform/src/SolarisPlatform.Infrastructure"
DEST="$INFRA/Services/RRHH"
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

[ -d "$INFRA" ] || { echo -e "${RED}✗ No se encontró Infrastructure en: $INFRA${RESET}"; exit 1; }

mkdir -p "$DEST"
echo -e "${CYAN}  Destino: $DEST${RESET}"
echo ""

# ── Copiar servicios ──────────────────────────────────────────────────
ARCHIVOS=(
  "DepartamentoService.cs"
  "PuestoService.cs"
  "EmpleadoService.cs"
  "AsistenciaService.cs"
  "ConceptoNominaService.cs"
  "NominaService.cs"
  "PrestamoService.cs"
  "EvaluacionService.cs"
  "CapacitacionService.cs"
  "RrhhDashboardService.cs"
)

for archivo in "${ARCHIVOS[@]}"; do
  src="$SCRIPT_DIR/$archivo"
  dst="$DEST/$archivo"
  if [ -f "$src" ]; then
    [ -f "$dst" ] && cp "$dst" "${dst}.bak.$(date +%Y%m%d%H%M%S)"
    cp "$src" "$dst"
    echo -e "${GREEN}  ✓${RESET} $archivo"
  else
    echo -e "${YELLOW}  ⚠ No encontrado: $archivo${RESET}"
  fi
done

echo ""
echo -e "${CYAN}  Verificando que el DependencyInjection.cs tenga registrados los servicios RRHH...${RESET}"
DI_FILE="$INFRA/DependencyInjection.cs"

if ! grep -q "IDepartamentoService" "$DI_FILE"; then
  echo -e "${YELLOW}  ⚠ DependencyInjection.cs no tiene el bloque RRHH. Agregándolo...${RESET}"

  BLOQUE='
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
'

  USINGS='using SolarisPlatform.Application.Interfaces.RRHH;
using SolarisPlatform.Infrastructure.Services.RRHH;'

  python3 - "$DI_FILE" "$USINGS" "$BLOQUE" << 'PYEOF'
import sys, re
path = sys.argv[1]; usings = sys.argv[2]; bloque = sys.argv[3]
with open(path) as f: c = f.read()
last = max((m.end() for m in re.finditer(r'^using .+;', c, re.MULTILINE)), default=0)
if last: c = c[:last] + '\n// ── RRHH\n' + usings + c[last:]
anchor = 'services.AddAuthorization();'
c = c.replace(anchor, bloque + '\n        ' + anchor, 1) if anchor in c else \
    c.replace('        return services;', bloque + '\n        return services;', 1)
with open(path, 'w') as f: f.write(c)
print("OK")
PYEOF
  echo -e "${GREEN}  ✓ DependencyInjection.cs actualizado${RESET}"
else
  echo -e "${GREEN}  ✓ DependencyInjection.cs ya tiene el bloque RRHH${RESET}"
fi

# ── Verificar que el RrhhMappingProfile esté registrado ──────────────
APP_DI="$HOME/projects/solaris_platform/src/SolarisPlatform.Application/DependencyInjection.cs"
if [ -f "$APP_DI" ] && ! grep -q "RrhhMappingProfile" "$APP_DI"; then
  echo ""
  echo -e "${YELLOW}  ⚠ RrhhMappingProfile no está registrado en Application/DependencyInjection.cs${RESET}"
  echo -e "    Agrega manualmente:"
  echo -e "    ${CYAN}cfg.AddProfile<RrhhMappingProfile>();${RESET}"
  echo -e "    en el método AddApplication() de Application/DependencyInjection.cs"
fi

echo ""
echo -e "${GREEN}  ✓ Instalación completada${RESET}"
echo ""
echo -e "  Ejecuta para verificar:"
echo -e "  ${CYAN}cd ~/projects/solaris_platform/src/SolarisPlatform.API && dotnet build${RESET}"
echo ""
