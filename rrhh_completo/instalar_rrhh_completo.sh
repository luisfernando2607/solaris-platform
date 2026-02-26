#!/usr/bin/env bash
# =====================================================================
# SOLARIS PLATFORM — Instalación completa RRHH
# Instala: SolarisDbContext, RrhhRepositories, y registra DI
# =====================================================================
set -e
GREEN='\033[0;32m'; YELLOW='\033[1;33m'; CYAN='\033[0;36m'; RED='\033[0;31m'; RESET='\033[0m'
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
BASE="$HOME/projects/solaris_platform/src"

echo ""
echo -e "${CYAN}  ════ SOLARIS RRHH — Instalación Completa ════${RESET}"
echo ""

# ── 1. SolarisDbContext.cs ─────────────────────────────────────────────
DBCTX="$BASE/SolarisPlatform.Infrastructure/Persistence/Context/SolarisDbContext.cs"
if [ -f "$DBCTX" ]; then
  cp "$DBCTX" "${DBCTX}.bak.$(date +%Y%m%d%H%M%S)"
  cp "$SCRIPT_DIR/SolarisDbContext.cs" "$DBCTX"
  echo -e "${GREEN}  ✓${RESET} SolarisDbContext.cs actualizado"
else
  echo -e "${RED}  ✗ No se encontró SolarisDbContext.cs en: $DBCTX${RESET}"; exit 1
fi

# ── 2. RrhhRepositories.cs ──────────────────────────────────────────────
REPO_DIR="$BASE/SolarisPlatform.Infrastructure/Persistence/Repositories/RRHH"
mkdir -p "$REPO_DIR"
[ -f "$REPO_DIR/RrhhRepositories.cs" ] && cp "$REPO_DIR/RrhhRepositories.cs" "$REPO_DIR/RrhhRepositories.cs.bak.$(date +%Y%m%d%H%M%S)"
cp "$SCRIPT_DIR/RrhhRepositories.cs" "$REPO_DIR/RrhhRepositories.cs"
echo -e "${GREEN}  ✓${RESET} RrhhRepositories.cs instalado"

# ── 3. DependencyInjection.cs — agregar repos RRHH ─────────────────────
DI_FILE="$BASE/SolarisPlatform.Infrastructure/DependencyInjection.cs"
[ -f "$DI_FILE" ] || { echo -e "${RED}  ✗ DependencyInjection.cs no encontrado${RESET}"; exit 1; }

if grep -q "IDepartamentoRepository" "$DI_FILE"; then
  echo -e "${YELLOW}  ⚠ Repositorios RRHH ya registrados en DI — omitiendo${RESET}"
else
  cp "$DI_FILE" "${DI_FILE}.bak.$(date +%Y%m%d%H%M%S)"
  python3 - "$DI_FILE" << 'PYEOF'
import sys, re

path = sys.argv[1]
with open(path) as f:
    c = f.read()

# Bloque de repos a insertar
repos = '''
        // RRHH — Repositorios
        services.AddScoped<IDepartamentoRepository,         DepartamentoRepository>();
        services.AddScoped<IPuestoRepository,               PuestoRepository>();
        services.AddScoped<IEmpleadoRepository,             EmpleadoRepository>();
        services.AddScoped<IAsistenciaRepository,           AsistenciaRepository>();
        services.AddScoped<ISolicitudAusenciaRepository,    SolicitudAusenciaRepository>();
        services.AddScoped<ISaldoVacacionesRepository,      SaldoVacacionesRepository>();
        services.AddScoped<IConceptoNominaRepository,       ConceptoNominaRepository>();
        services.AddScoped<IPeriodoNominaRepository,        PeriodoNominaRepository>();
        services.AddScoped<IRolPagoRepository,              RolPagoRepository>();
        services.AddScoped<IParametroNominaRepository,      ParametroNominaRepository>();
        services.AddScoped<IPrestamoRepository,             PrestamoRepository>();
        services.AddScoped<IEvaluacionRepository,           EvaluacionRepository>();
        services.AddScoped<ICapacitacionRepository,         CapacitacionRepository>();
'''

# Insertar antes de AddAuthorization
anchor = 'services.AddAuthorization();'
if anchor in c:
    c = c.replace(anchor, repos + '\n        ' + anchor, 1)
else:
    c = c.replace('        return services;', repos + '\n        return services;', 1)

# Agregar usings si no están
usings_needed = [
    'using SolarisPlatform.Infrastructure.Persistence.Repositories.RRHH;',
]
last_using_pos = max((m.end() for m in re.finditer(r'^using .+;', c, re.MULTILINE)), default=0)
missing = [u for u in usings_needed if u not in c]
if missing and last_using_pos:
    c = c[:last_using_pos] + '\n' + '\n'.join(missing) + c[last_using_pos:]

with open(path, 'w') as f:
    f.write(c)
print('OK')
PYEOF
  echo -e "${GREEN}  ✓${RESET} Repositorios RRHH registrados en DependencyInjection.cs"
fi

# ── 4. Verificar RrhhMappingProfile en Application DI ──────────────────
APP_DI="$BASE/SolarisPlatform.Application/DependencyInjection.cs"
if [ -f "$APP_DI" ]; then
  if ! grep -q "RrhhMappingProfile" "$APP_DI"; then
    cp "$APP_DI" "${APP_DI}.bak.$(date +%Y%m%d%H%M%S)"
    # Insertar después del último AddProfile existente
    python3 - "$APP_DI" << 'PYEOF'
import sys, re
path = sys.argv[1]
with open(path) as f: c = f.read()
# Buscar el último AddProfile y añadir después
if 'AddProfile<MappingProfile>' in c:
    c = c.replace(
        'cfg.AddProfile<MappingProfile>();',
        'cfg.AddProfile<MappingProfile>();\n            cfg.AddProfile<RrhhMappingProfile>();',
        1
    )
# Agregar using si falta
if 'using SolarisPlatform.Application.Mappings;' not in c:
    last = max((m.end() for m in re.finditer(r'^using .+;', c, re.MULTILINE)), default=0)
    if last: c = c[:last] + '\nusing SolarisPlatform.Application.Mappings;' + c[last:]
with open(path, 'w') as f: f.write(c)
print('OK')
PYEOF
    echo -e "${GREEN}  ✓${RESET} RrhhMappingProfile registrado en Application/DependencyInjection.cs"
  else
    echo -e "${GREEN}  ✓${RESET} RrhhMappingProfile ya registrado"
  fi
fi

# ── 5. Compilar ─────────────────────────────────────────────────────────
echo ""
echo -e "${CYAN}  Compilando...${RESET}"
cd "$BASE/SolarisPlatform.API"
dotnet build 2>&1 | grep -E "error CS|Build succeeded|Error\(s\)" | head -20

echo ""
echo -e "${GREEN}  ✓ Proceso completado${RESET}"
echo ""
