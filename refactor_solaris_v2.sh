#!/usr/bin/env bash
# =============================================================================
# Solaris Platform — Script de Refactoring v2 (rutas absolutas)
# Proyecto: /home/devstar/projects/solaris_platform
# =============================================================================
# USO:
#   chmod +x refactor_solaris_v2.sh
#   bash refactor_solaris_v2.sh
#
# ⚠️  Hacer commit/backup ANTES de ejecutar.
# =============================================================================

set -euo pipefail

# ── Raíz absoluta del proyecto ────────────────────────────────────────────────
ROOT="/home/devstar/projects/solaris_platform"
SRC="$ROOT/src"
TESTS="$ROOT/tests"

# ── Colores ───────────────────────────────────────────────────────────────────
GREEN='\033[0;32m'; YELLOW='\033[1;33m'
BLUE='\033[0;34m';  RED='\033[0;31m'; NC='\033[0m'

log_section() { echo -e "\n${BLUE}══════════════════════════════════════${NC}"; echo -e "${BLUE}  $1${NC}"; echo -e "${BLUE}══════════════════════════════════════${NC}"; }
log_ok()      { echo -e "  ${GREEN}✔${NC}  $1"; }
log_skip()    { echo -e "  ${YELLOW}→${NC}  $1 (ya correcto, se omite)"; }
log_warn()    { echo -e "  ${YELLOW}⚠${NC}   $1"; }
log_err()     { echo -e "  ${RED}✘${NC}  $1"; }

# ── Helpers ───────────────────────────────────────────────────────────────────
safe_rename() {
    local src="$1" dst="$2"
    if [ ! -e "$src" ]; then
        log_err "NO ENCONTRADO: ${src#$ROOT/}"
        return
    fi
    if [ -e "$dst" ]; then
        log_skip "$(basename "$dst")"
        return
    fi
    mv "$src" "$dst"
    log_ok "${src#$ROOT/}  →  $(basename "$dst")"
}

safe_delete() {
    local target="$1"
    if [ -e "$target" ]; then
        rm -f "$target"
        log_ok "ELIMINADO: ${target#$ROOT/}"
    fi
}

ensure_dir() {
    local dir="$1"
    [ -d "$dir" ] && return
    mkdir -p "$dir"
    log_ok "DIR creado: ${dir#$ROOT/}"
}

# ── Verificar que el proyecto existe ─────────────────────────────────────────
if [ ! -d "$ROOT" ]; then
    echo -e "${RED}ERROR: No se encontró el proyecto en $ROOT${NC}"
    exit 1
fi

echo ""
echo -e "${GREEN}╔══════════════════════════════════════════════════════╗${NC}"
echo -e "${GREEN}║   Solaris Platform — Refactoring v2                  ║${NC}"
echo -e "${GREEN}║   Rutas: $ROOT  ║${NC}"
echo -e "${GREEN}╚══════════════════════════════════════════════════════╝${NC}"
echo ""

# =============================================================================
# PASO 1 — Eliminar archivos basura
# =============================================================================
log_section "PASO 1: Eliminando archivos temporales y basura"

safe_delete "$SRC/SolarisPlatform.Infrastructure/DependencyInjection_Instructions.cs"
safe_delete "$SRC/SolarisPlatform.Infrastructure/DependencyInjection_Snippet.cs"
safe_delete "$SRC/SolarisPlatform.API/appsettings.Development.json.bak"
safe_delete "$SRC/SolarisPlatform.API/solaris-run.log"

# =============================================================================
# PASO 2 — API: Controllers (plural → singular)
# =============================================================================
log_section "PASO 2: API — Renombrando Controllers"

CTRL="$SRC/SolarisPlatform.API/Controllers"

safe_rename "$CTRL/Catalogos/CatalogoControllers.cs"          "$CTRL/Catalogos/CatalogoController.cs"
safe_rename "$CTRL/Proyectos/FaseHitoWbsControllers.cs"       "$CTRL/Proyectos/FaseHitoController.cs"
safe_rename "$CTRL/Proyectos/OtReporteKpiControllers.cs"      "$CTRL/Proyectos/OrdenTrabajoController.cs"
safe_rename "$CTRL/Proyectos/PresupuestoGanttCCControllers.cs" "$CTRL/Proyectos/PresupuestoController.cs"
safe_rename "$CTRL/Proyectos/TareaCuadrillaControllers.cs"    "$CTRL/Proyectos/TareaController.cs"
safe_rename "$CTRL/RRHH/RrhhControllers.cs"                   "$CTRL/RRHH/RrhhController.cs"

# =============================================================================
# PASO 3 — API: Middleware (nombre más claro)
# =============================================================================
log_section "PASO 3: API — Renombrando Middleware"

safe_rename \
    "$SRC/SolarisPlatform.API/Middleware/ExceptionHandlingMiddleware.cs" \
    "$SRC/SolarisPlatform.API/Middleware/GlobalExceptionMiddleware.cs"

# =============================================================================
# PASO 4 — Application: DTOs (capitalización consistente → Dtos)
# =============================================================================
log_section "PASO 4: Application — Estandarizando DTOs"

DTOS="$SRC/SolarisPlatform.Application/DTOs"

safe_rename "$DTOS/Catalogos/CatalogoDTOs.cs"    "$DTOS/Catalogos/CatalogoDtos.cs"
safe_rename "$DTOS/Proyectos/ProyectosDTOs.cs"   "$DTOS/Proyectos/ProyectosDtos.cs"

# =============================================================================
# PASO 5 — Application: Interfaces (consolidar + renombrar)
# =============================================================================
log_section "PASO 5: Application — Reorganizando Interfaces"

APP="$SRC/SolarisPlatform.Application"

# Mover IRrhhServices de Interfaces/RRHH/ → Common/Interfaces/RRHH/
ensure_dir "$APP/Common/Interfaces/RRHH"

if [ -f "$APP/Interfaces/RRHH/IRrhhServices.cs" ]; then
    safe_rename \
        "$APP/Interfaces/RRHH/IRrhhServices.cs" \
        "$APP/Common/Interfaces/RRHH/IRrhhServices.cs"
    rmdir "$APP/Interfaces/RRHH" 2>/dev/null && log_ok "DIR eliminado: Application/Interfaces/RRHH/ (vacío)"
    rmdir "$APP/Interfaces"      2>/dev/null && log_ok "DIR eliminado: Application/Interfaces/ (vacío)"
fi

# Renombrar interfaces en Common/Interfaces/
safe_rename "$APP/Common/Interfaces/IServices.cs"                      "$APP/Common/Interfaces/IBaseService.cs"
safe_rename "$APP/Common/Interfaces/ICatalogoServices.cs"              "$APP/Common/Interfaces/ICatalogoService.cs"
safe_rename "$APP/Common/Interfaces/Proyectos/IProyectosServices.cs"   "$APP/Common/Interfaces/Proyectos/IProyectoService.cs"

# =============================================================================
# PASO 6 — Application: Common/Models
# =============================================================================
log_section "PASO 6: Application — Renombrando CommonModels"

safe_rename "$APP/Common/Models/CommonModels.cs"        "$APP/Common/Models/BaseModels.cs"
safe_rename "$APP/Common/Mappings/MappingProfile.cs"    "$APP/Common/Mappings/BaseMappingProfile.cs"

# =============================================================================
# PASO 7 — Application: Validators (plural → singular, RRHH → Empleado)
# =============================================================================
log_section "PASO 7: Application — Estandarizando Validators"

VAL="$APP/Validators"

safe_rename "$VAL/AuthValidators.cs"             "$VAL/AuthValidator.cs"
safe_rename "$VAL/Catalogos/CatalogoValidators.cs" "$VAL/Catalogos/CatalogoValidator.cs"
safe_rename "$VAL/RRHH/RrhhValidators.cs"        "$VAL/RRHH/EmpleadoValidator.cs"

# =============================================================================
# PASO 8 — Application: MappingProfiles (plural → singular)
# =============================================================================
log_section "PASO 8: Application — Estandarizando MappingProfiles"

MAPS="$APP/Mappings"

safe_rename "$MAPS/CatalogosMappingProfile.cs"   "$MAPS/CatalogoMappingProfile.cs"
safe_rename "$MAPS/ProyectosMappingProfile.cs"   "$MAPS/ProyectoMappingProfile.cs"
safe_rename "$MAPS/RrhhMappingProfile.cs"        "$MAPS/EmpleadoMappingProfile.cs"

# =============================================================================
# PASO 9 — Domain: Interfaces (plural → singular, IRepositories → IUnitOfWork)
# =============================================================================
log_section "PASO 9: Domain — Estandarizando Interfaces de repositorios"

DOM_IFACES="$SRC/SolarisPlatform.Domain/Interfaces"

safe_rename "$DOM_IFACES/IRepositories.cs"                       "$DOM_IFACES/IUnitOfWork.cs"
safe_rename "$DOM_IFACES/Catalogos/ICatalogoRepositories.cs"     "$DOM_IFACES/Catalogos/ICatalogoRepository.cs"
safe_rename "$DOM_IFACES/Proyectos/IProyectosRepositories.cs"    "$DOM_IFACES/Proyectos/IProyectoRepository.cs"
safe_rename "$DOM_IFACES/RRHH/IRrhhRepositories.cs"              "$DOM_IFACES/RRHH/IRrhhRepository.cs"

# IProyectosServices en Domain es un duplicado → marcar para revisión manual
if [ -f "$DOM_IFACES/Proyectos/IProyectosServices.cs" ]; then
    mv "$DOM_IFACES/Proyectos/IProyectosServices.cs" \
       "$DOM_IFACES/Proyectos/_IProyectosServices.REVISAR.cs"
    log_warn "IProyectosServices.cs en Domain → renombrado a _IProyectosServices.REVISAR.cs"
    log_warn "Comparar con Application/Common/Interfaces/Proyectos/IProyectoService.cs y eliminar el duplicado"
fi

# =============================================================================
# PASO 10 — Domain: Entidades (Entidades → Entities, español → inglés técnico)
# =============================================================================
log_section "PASO 10: Domain — Estandarizando nombres de Entities"

ENT="$SRC/SolarisPlatform.Domain/Entities"

safe_rename "$ENT/Catalogos/Catalogos.cs"                          "$ENT/Catalogos/CatalogoEntities.cs"
safe_rename "$ENT/Proyectos/CuadrillaRecursoEntidades.cs"          "$ENT/Proyectos/CuadrillaRecursoEntities.cs"
safe_rename "$ENT/Proyectos/GanttCentroCostoEntidades.cs"          "$ENT/Proyectos/GanttCentroCostoEntities.cs"
safe_rename "$ENT/Proyectos/KpiAlertaEntidades.cs"                 "$ENT/Proyectos/KpiAlertaEntities.cs"
safe_rename "$ENT/Proyectos/OrdenTrabajoReporteEntidades.cs"       "$ENT/Proyectos/OrdenTrabajoEntities.cs"
safe_rename "$ENT/Proyectos/PresupuestoCostoEntidades.cs"          "$ENT/Proyectos/PresupuestoCostoEntities.cs"
safe_rename "$ENT/Proyectos/ProyectoEntidades.cs"                  "$ENT/Proyectos/ProyectoEntities.cs"
safe_rename "$ENT/Proyectos/WbsTareaEntidades.cs"                  "$ENT/Proyectos/WbsTareaEntities.cs"
safe_rename "$ENT/RRHH/RrhhEntities.cs"                            "$ENT/RRHH/EmpleadoEntities.cs"

# =============================================================================
# PASO 11 — Domain: Enums (Enums.cs genérico → CommonEnums.cs)
# =============================================================================
log_section "PASO 11: Domain — Estandarizando Enums"

ENUMS="$SRC/SolarisPlatform.Domain/Enums"

safe_rename "$ENUMS/Enums.cs"              "$ENUMS/CommonEnums.cs"
safe_rename "$ENUMS/RRHH/RrhhEnums.cs"    "$ENUMS/RRHH/EmpleadoEnums.cs"

# =============================================================================
# PASO 12 — Infrastructure: Repositories (plural → singular)
# =============================================================================
log_section "PASO 12: Infrastructure — Estandarizando Repositories"

REPOS="$SRC/SolarisPlatform.Infrastructure/Persistence/Repositories"

safe_rename "$REPOS/Catalogos/CatalogoRepositories.cs"    "$REPOS/Catalogos/CatalogoRepository.cs"
safe_rename "$REPOS/Proyectos/ProyectosRepositories.cs"   "$REPOS/Proyectos/ProyectoRepository.cs"
safe_rename "$REPOS/RRHH/RrhhRepositories.cs"             "$REPOS/RRHH/EmpleadoRepository.cs"
safe_rename "$REPOS/SeguridadRepositories.cs"             "$REPOS/SeguridadRepository.cs"

# =============================================================================
# PASO 13 — Infrastructure: Services (plural → singular, nombres descriptivos)
# =============================================================================
log_section "PASO 13: Infrastructure — Estandarizando Services"

SVCS="$SRC/SolarisPlatform.Infrastructure/Services"

safe_rename "$SVCS/Catalogos/CatalogoServices.cs"                    "$SVCS/Catalogos/CatalogoService.cs"
safe_rename "$SVCS/Proyectos/FaseHitoServices.cs"                    "$SVCS/Proyectos/FaseHitoService.cs"
safe_rename "$SVCS/Proyectos/OtReporteKpiServices.cs"                "$SVCS/Proyectos/OrdenTrabajoService.cs"
safe_rename "$SVCS/Proyectos/PresupuestoGanttCentroCostoServices.cs" "$SVCS/Proyectos/PresupuestoService.cs"
safe_rename "$SVCS/Proyectos/WbsTareaCuadrillaServices.cs"           "$SVCS/Proyectos/WbsTareaService.cs"
safe_rename "$SVCS/TokenPasswordServices.cs"                         "$SVCS/TokenService.cs"
safe_rename "$SVCS/RRHH/RrhhServices.cs"                             "$SVCS/RRHH/EmpleadoService.cs"

# =============================================================================
# PASO 14 — Infrastructure: Configurations (plural → singular)
# =============================================================================
log_section "PASO 14: Infrastructure — Estandarizando Configurations"

CFGS="$SRC/SolarisPlatform.Infrastructure/Persistence/Configurations"

safe_rename "$CFGS/Catalogos/CatalogoConfigurations.cs"    "$CFGS/Catalogos/CatalogoConfiguration.cs"
safe_rename "$CFGS/Proyectos/ProyectosConfigurations.cs"   "$CFGS/Proyectos/ProyectoConfiguration.cs"
safe_rename "$CFGS/RRHH/RrhhConfigurations.cs"             "$CFGS/RRHH/EmpleadoConfiguration.cs"
safe_rename "$CFGS/EmpresaCatalogosConfigurations.cs"      "$CFGS/EmpresaConfiguration.cs"
safe_rename "$CFGS/SeguridadConfigurations.cs"             "$CFGS/SeguridadConfiguration.cs"

# =============================================================================
# PASO 15 — Tests: crear estructura mínima
# =============================================================================
log_section "PASO 15: Tests — Creando estructura"

TEST="$TESTS/SolarisPlatform.Tests"

ensure_dir "$TEST/Unit/Domain"
ensure_dir "$TEST/Unit/Application"
ensure_dir "$TEST/Unit/Infrastructure"
ensure_dir "$TEST/Integration"

touch "$TEST/Unit/Domain/.gitkeep"
touch "$TEST/Unit/Application/.gitkeep"
touch "$TEST/Unit/Infrastructure/.gitkeep"
touch "$TEST/Integration/.gitkeep"

log_ok "Estructura de tests creada"

# =============================================================================
# PASO 16 — .gitignore: agregar entradas faltantes
# =============================================================================
log_section "PASO 16: Actualizando .gitignore"

GITIGNORE="$ROOT/.gitignore"

for entry in "*.bak" "*.log" "**/DependencyInjection_*.cs" "*.REVISAR.cs" "obj/" "bin/"; do
    if ! grep -qF "$entry" "$GITIGNORE" 2>/dev/null; then
        echo "$entry" >> "$GITIGNORE"
        log_ok "Agregado a .gitignore: $entry"
    else
        log_skip "$entry ya estaba en .gitignore"
    fi
done

# =============================================================================
# PASO 17 — Actualizar namespaces en todos los archivos .cs renombrados
# =============================================================================
log_section "PASO 17: Actualizando referencias internas (namespaces y using)"

# Función para reemplazar texto en todos los .cs del proyecto
replace_in_cs() {
    local old="$1" new="$2"
    # grep primero para verificar si hay ocurrencias
    local matches
    matches=$(grep -rl "$old" "$SRC" --include="*.cs" 2>/dev/null || true)
    if [ -n "$matches" ]; then
        echo "$matches" | xargs sed -i "s/$old/$new/g"
        log_ok "Reemplazado: '$old' → '$new'"
    fi
}

# Controllers
replace_in_cs "CatalogoControllers"           "CatalogoController"
replace_in_cs "FaseHitoWbsControllers"        "FaseHitoController"
replace_in_cs "OtReporteKpiControllers"       "OrdenTrabajoController"
replace_in_cs "PresupuestoGanttCCControllers" "PresupuestoController"
replace_in_cs "TareaCuadrillaControllers"     "TareaController"
replace_in_cs "RrhhControllers"               "RrhhController"

# Middleware
replace_in_cs "ExceptionHandlingMiddleware"   "GlobalExceptionMiddleware"

# DTOs
replace_in_cs "CatalogoDTOs"    "CatalogoDtos"
replace_in_cs "ProyectosDTOs"   "ProyectosDtos"

# Interfaces Application
replace_in_cs "IServices\b"              "IBaseService"
replace_in_cs "ICatalogoServices\b"      "ICatalogoService"
replace_in_cs "IProyectosServices\b"     "IProyectoService"
replace_in_cs "IRrhhServices\b"          "IRrhhService"

# Interfaces Domain
replace_in_cs "IRepositories\b"          "IUnitOfWork"
replace_in_cs "ICatalogoRepositories\b"  "ICatalogoRepository"
replace_in_cs "IProyectosRepositories\b" "IProyectoRepository"
replace_in_cs "IRrhhRepositories\b"      "IRrhhRepository"

# Validators
replace_in_cs "AuthValidators\b"         "AuthValidator"
replace_in_cs "CatalogoValidators\b"     "CatalogoValidator"
replace_in_cs "RrhhValidators\b"         "EmpleadoValidator"

# MappingProfiles
replace_in_cs "CatalogosMappingProfile"  "CatalogoMappingProfile"
replace_in_cs "ProyectosMappingProfile"  "ProyectoMappingProfile"
replace_in_cs "RrhhMappingProfile"       "EmpleadoMappingProfile"
replace_in_cs "\.MappingProfile\b"       ".BaseMappingProfile"

# Repositories (implementations)
replace_in_cs "CatalogoRepositories\b"   "CatalogoRepository"
replace_in_cs "ProyectosRepositories\b"  "ProyectoRepository"
replace_in_cs "RrhhRepositories\b"       "EmpleadoRepository"
replace_in_cs "SeguridadRepositories\b"  "SeguridadRepository"

# Services
replace_in_cs "CatalogoServices\b"                     "CatalogoService"
replace_in_cs "FaseHitoServices\b"                     "FaseHitoService"
replace_in_cs "OtReporteKpiServices\b"                 "OrdenTrabajoService"
replace_in_cs "PresupuestoGanttCentroCostoServices\b"  "PresupuestoService"
replace_in_cs "WbsTareaCuadrillaServices\b"            "WbsTareaService"
replace_in_cs "TokenPasswordServices\b"                "TokenService"
replace_in_cs "RrhhServices\b"                         "EmpleadoService"

# Configurations
replace_in_cs "CatalogoConfigurations\b"        "CatalogoConfiguration"
replace_in_cs "ProyectosConfigurations\b"       "ProyectoConfiguration"
replace_in_cs "RrhhConfigurations\b"            "EmpleadoConfiguration"
replace_in_cs "EmpresaCatalogosConfigurations\b" "EmpresaConfiguration"
replace_in_cs "SeguridadConfigurations\b"       "SeguridadConfiguration"

# Entities
replace_in_cs "CuadrillaRecursoEntidades"   "CuadrillaRecursoEntities"
replace_in_cs "GanttCentroCostoEntidades"   "GanttCentroCostoEntities"
replace_in_cs "KpiAlertaEntidades"          "KpiAlertaEntities"
replace_in_cs "OrdenTrabajoReporteEntidades" "OrdenTrabajoEntities"
replace_in_cs "PresupuestoCostoEntidades"   "PresupuestoCostoEntities"
replace_in_cs "ProyectoEntidades"           "ProyectoEntities"
replace_in_cs "WbsTareaEntidades"           "WbsTareaEntities"
replace_in_cs "RrhhEntities\b"              "EmpleadoEntities"

# Enums
replace_in_cs "CommonEnums\b"   "CommonEnums"   # ya correcto pero por si acaso
replace_in_cs "RrhhEnums\b"     "EmpleadoEnums"

# Models
replace_in_cs "CommonModels\b"  "BaseModels"

log_ok "Namespaces y referencias actualizados"

# =============================================================================
# RESUMEN
# =============================================================================
echo ""
echo -e "${GREEN}╔══════════════════════════════════════════════════════╗${NC}"
echo -e "${GREEN}║   ✅  Refactoring v2 completado                       ║${NC}"
echo -e "${GREEN}╚══════════════════════════════════════════════════════╝${NC}"
echo ""
echo -e "${YELLOW}VERIFICAR AHORA:${NC}"
echo ""
echo "  1. Compilar el proyecto:"
echo "     cd ~/projects/solaris_platform/src/SolarisPlatform.API"
echo "     dotnet build 2>&1 | grep -E '^.*error' | head -30"
echo ""
echo "  2. Revisar el duplicado de IProyectosServices:"
echo "     cat $SRC/SolarisPlatform.Domain/Interfaces/Proyectos/_IProyectosServices.REVISAR.cs"
echo "     # Si es redundante con IProyectoService.cs en Application → eliminar:"
echo "     rm $SRC/SolarisPlatform.Domain/Interfaces/Proyectos/_IProyectosServices.REVISAR.cs"
echo ""
echo "  3. Verificar estructura resultante:"
echo "     find $SRC -name '*.cs' | grep -v '/obj/' | sort"
echo ""
echo "  4. Hacer commit de los cambios:"
echo "     cd ~/projects/solaris_platform"
echo "     git add -A && git commit -m 'refactor: rename files to .NET 10 best practices'"
echo ""
