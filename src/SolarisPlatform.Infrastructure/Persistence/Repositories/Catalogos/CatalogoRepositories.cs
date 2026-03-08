// =====================================================
// INFRASTRUCTURE LAYER - Repositorios de Catálogos
// FIX: Eliminado context.Update(entity) en todos los ActualizarAsync.
//      ObtenerPorIdAsync usa FirstOrDefaultAsync (con tracking activo).
//      La entidad ya está en el change tracker → EF detecta cambios
//      automáticamente. context.Update() sobre una entidad tracked con
//      nav-props cargadas (Pais, EstadoProvincia, etc.) marcaba el GRAFO
//      COMPLETO como Modified → EF generaba UPDATE sobre las tablas
//      relacionadas → DbUpdateException.
// =====================================================

using Microsoft.EntityFrameworkCore;
using SolarisPlatform.Domain.Entities.Catalogos;
using SolarisPlatform.Domain.Interfaces.Catalogos;
using SolarisPlatform.Infrastructure.Persistence.Context;

namespace SolarisPlatform.Infrastructure.Persistence.Repositories.Catalogos;

// ══════════════════════════════════════════════════════
// REPOSITORIO DE PAÍSES
// ══════════════════════════════════════════════════════
public class PaisRepository(SolarisDbContext context) : IPaisRepository
{
    public async Task<IEnumerable<Pais>> ObtenerTodosAsync(bool soloActivos = false)
    {
        var query = context.Paises.AsQueryable();
        if (soloActivos) query = query.Where(x => x.Activo);
        return await query.OrderBy(x => x.Orden).ThenBy(x => x.Nombre).ToListAsync();
    }

    public async Task<Pais?> ObtenerPorIdAsync(long id) =>
        await context.Paises.FirstOrDefaultAsync(x => x.Id == id);

    public async Task<Pais?> ObtenerPorCodigoAsync(string codigo) =>
        await context.Paises.FirstOrDefaultAsync(x => x.Codigo == codigo.ToUpper());

    public async Task<bool> ExisteCodigoAsync(string codigo, long? excludeId = null)
    {
        var query = context.Paises.Where(x => x.Codigo == codigo.ToUpper());
        if (excludeId.HasValue) query = query.Where(x => x.Id != excludeId.Value);
        return await query.AnyAsync();
    }

    public async Task<Pais> CrearAsync(Pais pais)
    {
        pais.Codigo    = pais.Codigo.ToUpper();
        pais.CodigoIso2 = pais.CodigoIso2.ToUpper();
        context.Paises.Add(pais);
        await context.SaveChangesAsync();
        return pais;
    }

    public async Task<Pais> ActualizarAsync(Pais pais)
    {
        pais.Codigo    = pais.Codigo.ToUpper();
        pais.CodigoIso2 = pais.CodigoIso2.ToUpper();
        // FIX: entidad ya tracked desde ObtenerPorIdAsync → SaveChanges detecta cambios.
        // context.Update() marcaba el grafo completo como Modified → DbUpdateException.
        await context.SaveChangesAsync();
        return pais;
    }

    public async Task EliminarAsync(long id)
    {
        var pais = await context.Paises.FindAsync(id);
        if (pais is not null)
        {
            context.Paises.Remove(pais);
            await context.SaveChangesAsync();
        }
    }
}

// ══════════════════════════════════════════════════════
// REPOSITORIO DE ESTADOS / PROVINCIAS
// ══════════════════════════════════════════════════════
public class EstadoProvinciaRepository(SolarisDbContext context) : IEstadoProvinciaRepository
{
    public async Task<IEnumerable<EstadoProvincia>> ObtenerTodosAsync(bool soloActivos = false)
    {
        var query = context.EstadosProvincias.Include(x => x.Pais).AsQueryable();
        if (soloActivos) query = query.Where(x => x.Activo);
        return await query.OrderBy(x => x.Pais.Nombre).ThenBy(x => x.Orden).ThenBy(x => x.Nombre).ToListAsync();
    }

    public async Task<IEnumerable<EstadoProvincia>> ObtenerPorPaisAsync(long paisId, bool soloActivos = false)
    {
        var query = context.EstadosProvincias.Include(x => x.Pais).Where(x => x.PaisId == paisId);
        if (soloActivos) query = query.Where(x => x.Activo);
        return await query.OrderBy(x => x.Orden).ThenBy(x => x.Nombre).ToListAsync();
    }

    public async Task<EstadoProvincia?> ObtenerPorIdAsync(long id) =>
        await context.EstadosProvincias.Include(x => x.Pais).FirstOrDefaultAsync(x => x.Id == id);

    public async Task<bool> ExisteCodigoEnPaisAsync(long paisId, string codigo, long? excludeId = null)
    {
        var query = context.EstadosProvincias.Where(x => x.PaisId == paisId && x.Codigo == codigo);
        if (excludeId.HasValue) query = query.Where(x => x.Id != excludeId.Value);
        return await query.AnyAsync();
    }

    public async Task<EstadoProvincia> CrearAsync(EstadoProvincia estado)
    {
        context.EstadosProvincias.Add(estado);
        await context.SaveChangesAsync();
        return estado;
    }

    public async Task<EstadoProvincia> ActualizarAsync(EstadoProvincia estado)
    {
        // FIX: ObtenerPorIdAsync carga con Include(x => x.Pais) → Pais queda tracked.
        // context.Update() marcaba Pais como Modified también → UPDATE en seg.pais → error.
        // Solución: solo SaveChanges, EF detecta los cambios escalares del EstadoProvincia.
        await context.SaveChangesAsync();
        return estado;
    }

    public async Task EliminarAsync(long id)
    {
        var estado = await context.EstadosProvincias.FindAsync(id);
        if (estado is not null)
        {
            context.EstadosProvincias.Remove(estado);
            await context.SaveChangesAsync();
        }
    }
}

// ══════════════════════════════════════════════════════
// REPOSITORIO DE CIUDADES
// ══════════════════════════════════════════════════════
public class CiudadRepository(SolarisDbContext context) : ICiudadRepository
{
    public async Task<IEnumerable<Ciudad>> ObtenerTodosAsync(bool soloActivos = false)
    {
        var query = context.Ciudades
            .Include(x => x.EstadoProvincia)
            .ThenInclude(x => x.Pais)
            .AsQueryable();
        if (soloActivos) query = query.Where(x => x.Activo);
        return await query.OrderBy(x => x.EstadoProvincia.Nombre).ThenBy(x => x.Nombre).ToListAsync();
    }

    public async Task<IEnumerable<Ciudad>> ObtenerPorEstadoAsync(long estadoId, bool soloActivos = false)
    {
        var query = context.Ciudades
            .Include(x => x.EstadoProvincia).ThenInclude(x => x.Pais)
            .Where(x => x.EstadoProvinciaId == estadoId);
        if (soloActivos) query = query.Where(x => x.Activo);
        return await query.OrderBy(x => x.Orden).ThenBy(x => x.Nombre).ToListAsync();
    }

    public async Task<Ciudad?> ObtenerPorIdAsync(long id) =>
        await context.Ciudades
            .Include(x => x.EstadoProvincia).ThenInclude(x => x.Pais)
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<Ciudad> CrearAsync(Ciudad ciudad)
    {
        context.Ciudades.Add(ciudad);
        await context.SaveChangesAsync();
        return ciudad;
    }

    public async Task<Ciudad> ActualizarAsync(Ciudad ciudad)
    {
        // FIX: EntadoProvincia y Pais quedan tracked desde ObtenerPorIdAsync.
        // context.Update() los marcaba Modified → doble UPDATE → error de FK.
        await context.SaveChangesAsync();
        return ciudad;
    }

    public async Task EliminarAsync(long id)
    {
        var ciudad = await context.Ciudades.FindAsync(id);
        if (ciudad is not null)
        {
            context.Ciudades.Remove(ciudad);
            await context.SaveChangesAsync();
        }
    }
}

// ══════════════════════════════════════════════════════
// REPOSITORIO DE MONEDAS
// ══════════════════════════════════════════════════════
public class MonedaRepository(SolarisDbContext context) : IMonedaRepository
{
    public async Task<IEnumerable<Moneda>> ObtenerTodosAsync(bool soloActivos = false)
    {
        var query = context.Monedas.AsQueryable();
        if (soloActivos) query = query.Where(x => x.Activo);
        return await query.OrderBy(x => x.Orden).ThenBy(x => x.Nombre).ToListAsync();
    }

    public async Task<Moneda?> ObtenerPorIdAsync(long id) =>
        await context.Monedas.FirstOrDefaultAsync(x => x.Id == id);

    public async Task<Moneda?> ObtenerPorCodigoAsync(string codigo) =>
        await context.Monedas.FirstOrDefaultAsync(x => x.Codigo == codigo.ToUpper());

    public async Task<bool> ExisteCodigoAsync(string codigo, long? excludeId = null)
    {
        var query = context.Monedas.Where(x => x.Codigo == codigo.ToUpper());
        if (excludeId.HasValue) query = query.Where(x => x.Id != excludeId.Value);
        return await query.AnyAsync();
    }

    public async Task<Moneda> CrearAsync(Moneda moneda)
    {
        moneda.Codigo = moneda.Codigo.ToUpper();
        context.Monedas.Add(moneda);
        await context.SaveChangesAsync();
        return moneda;
    }

    public async Task<Moneda> ActualizarAsync(Moneda moneda)
    {
        moneda.Codigo = moneda.Codigo.ToUpper();
        // FIX: entidad ya tracked → solo SaveChanges.
        await context.SaveChangesAsync();
        return moneda;
    }

    public async Task EliminarAsync(long id)
    {
        var moneda = await context.Monedas.FindAsync(id);
        if (moneda is not null)
        {
            context.Monedas.Remove(moneda);
            await context.SaveChangesAsync();
        }
    }
}

// ══════════════════════════════════════════════════════
// REPOSITORIO DE TIPOS DE IDENTIFICACIÓN
// ══════════════════════════════════════════════════════
public class TipoIdentificacionRepository(SolarisDbContext context) : ITipoIdentificacionRepository
{
    public async Task<IEnumerable<TipoIdentificacion>> ObtenerTodosAsync(bool soloActivos = false)
    {
        var query = context.TiposIdentificacion.Include(x => x.Pais).AsQueryable();
        if (soloActivos) query = query.Where(x => x.Activo);
        return await query.OrderBy(x => x.PaisId).ThenBy(x => x.Orden).ThenBy(x => x.Nombre).ToListAsync();
    }

    public async Task<IEnumerable<TipoIdentificacion>> ObtenerPorPaisAsync(long? paisId, bool soloActivos = false)
    {
        var query = context.TiposIdentificacion.Include(x => x.Pais)
            .Where(x => x.PaisId == null || x.PaisId == paisId);
        if (soloActivos) query = query.Where(x => x.Activo);
        return await query.OrderBy(x => x.Orden).ThenBy(x => x.Nombre).ToListAsync();
    }

    public async Task<TipoIdentificacion?> ObtenerPorIdAsync(long id) =>
        await context.TiposIdentificacion.Include(x => x.Pais).FirstOrDefaultAsync(x => x.Id == id);

    public async Task<bool> ExisteCodigoAsync(string codigo, long? excludeId = null)
    {
        var query = context.TiposIdentificacion.Where(x => x.Codigo == codigo.ToUpper());
        if (excludeId.HasValue) query = query.Where(x => x.Id != excludeId.Value);
        return await query.AnyAsync();
    }

    public async Task<TipoIdentificacion> CrearAsync(TipoIdentificacion tipo)
    {
        tipo.Codigo = tipo.Codigo.ToUpper();
        context.TiposIdentificacion.Add(tipo);
        await context.SaveChangesAsync();
        return tipo;
    }

    public async Task<TipoIdentificacion> ActualizarAsync(TipoIdentificacion tipo)
    {
        tipo.Codigo = tipo.Codigo.ToUpper();
        // FIX: Pais tracked desde ObtenerPorIdAsync → solo SaveChanges.
        await context.SaveChangesAsync();
        return tipo;
    }

    public async Task EliminarAsync(long id)
    {
        var tipo = await context.TiposIdentificacion.FindAsync(id);
        if (tipo is not null)
        {
            context.TiposIdentificacion.Remove(tipo);
            await context.SaveChangesAsync();
        }
    }
}

// ══════════════════════════════════════════════════════
// REPOSITORIO DE IMPUESTOS
// ══════════════════════════════════════════════════════
public class ImpuestoRepository(SolarisDbContext context) : IImpuestoRepository
{
    public async Task<IEnumerable<Impuesto>> ObtenerTodosAsync(long? empresaId = null, bool soloActivos = false)
    {
        var query = context.Impuestos.AsQueryable();
        if (empresaId.HasValue)
            query = query.Where(x => x.EmpresaId == null || x.EmpresaId == empresaId);
        if (soloActivos) query = query.Where(x => x.Activo);
        return await query.OrderBy(x => x.TipoImpuesto).ThenBy(x => x.Orden).ThenBy(x => x.Nombre).ToListAsync();
    }

    public async Task<Impuesto?> ObtenerPorIdAsync(long id) =>
        await context.Impuestos.FirstOrDefaultAsync(x => x.Id == id);

    public async Task<bool> ExisteCodigoAsync(string codigo, long? empresaId, long? excludeId = null)
    {
        var query = context.Impuestos.Where(x => x.Codigo == codigo && x.EmpresaId == empresaId);
        if (excludeId.HasValue) query = query.Where(x => x.Id != excludeId.Value);
        return await query.AnyAsync();
    }

    public async Task<Impuesto> CrearAsync(Impuesto impuesto)
    {
        context.Impuestos.Add(impuesto);
        await context.SaveChangesAsync();
        return impuesto;
    }

    public async Task<Impuesto> ActualizarAsync(Impuesto impuesto)
    {
        // FIX: entidad ya tracked → solo SaveChanges.
        await context.SaveChangesAsync();
        return impuesto;
    }

    public async Task EliminarAsync(long id)
    {
        var impuesto = await context.Impuestos.FindAsync(id);
        if (impuesto is not null)
        {
            context.Impuestos.Remove(impuesto);
            await context.SaveChangesAsync();
        }
    }
}

// ══════════════════════════════════════════════════════
// REPOSITORIO DE FORMAS DE PAGO
// ══════════════════════════════════════════════════════
public class FormaPagoRepository(SolarisDbContext context) : IFormaPagoRepository
{
    public async Task<IEnumerable<FormaPago>> ObtenerTodosAsync(long? empresaId = null, bool soloActivos = false)
    {
        var query = context.FormasPago.AsQueryable();
        if (empresaId.HasValue)
            query = query.Where(x => x.EmpresaId == null || x.EmpresaId == empresaId);
        if (soloActivos) query = query.Where(x => x.Activo);
        return await query.OrderBy(x => x.Orden).ThenBy(x => x.Nombre).ToListAsync();
    }

    public async Task<FormaPago?> ObtenerPorIdAsync(long id) =>
        await context.FormasPago.FirstOrDefaultAsync(x => x.Id == id);

    public async Task<bool> ExisteCodigoAsync(string codigo, long? empresaId, long? excludeId = null)
    {
        var query = context.FormasPago.Where(x => x.Codigo == codigo && x.EmpresaId == empresaId);
        if (excludeId.HasValue) query = query.Where(x => x.Id != excludeId.Value);
        return await query.AnyAsync();
    }

    public async Task<FormaPago> CrearAsync(FormaPago formaPago)
    {
        context.FormasPago.Add(formaPago);
        await context.SaveChangesAsync();
        return formaPago;
    }

    public async Task<FormaPago> ActualizarAsync(FormaPago formaPago)
    {
        // FIX: entidad ya tracked → solo SaveChanges.
        await context.SaveChangesAsync();
        return formaPago;
    }

    public async Task EliminarAsync(long id)
    {
        var forma = await context.FormasPago.FindAsync(id);
        if (forma is not null)
        {
            context.FormasPago.Remove(forma);
            await context.SaveChangesAsync();
        }
    }
}

// ══════════════════════════════════════════════════════
// REPOSITORIO DE BANCOS
// ══════════════════════════════════════════════════════
public class BancoRepository(SolarisDbContext context) : IBancoRepository
{
    public async Task<IEnumerable<Banco>> ObtenerTodosAsync(long? paisId = null, bool soloActivos = false)
    {
        var query = context.Bancos.Include(x => x.Pais).AsQueryable();
        if (paisId.HasValue) query = query.Where(x => x.PaisId == paisId);
        if (soloActivos) query = query.Where(x => x.Activo);
        return await query.OrderBy(x => x.Orden).ThenBy(x => x.Nombre).ToListAsync();
    }

    public async Task<Banco?> ObtenerPorIdAsync(long id) =>
        await context.Bancos.Include(x => x.Pais).FirstOrDefaultAsync(x => x.Id == id);

    public async Task<bool> ExisteCodigoAsync(string codigo, long? excludeId = null)
    {
        var query = context.Bancos.Where(x => x.Codigo == codigo.ToUpper());
        if (excludeId.HasValue) query = query.Where(x => x.Id != excludeId.Value);
        return await query.AnyAsync();
    }

    public async Task<Banco> CrearAsync(Banco banco)
    {
        banco.Codigo = banco.Codigo.ToUpper();
        context.Bancos.Add(banco);
        await context.SaveChangesAsync();
        return banco;
    }

    public async Task<Banco> ActualizarAsync(Banco banco)
    {
        banco.Codigo = banco.Codigo.ToUpper();
        // FIX: Pais tracked desde ObtenerPorIdAsync → solo SaveChanges.
        await context.SaveChangesAsync();
        return banco;
    }

    public async Task EliminarAsync(long id)
    {
        var banco = await context.Bancos.FindAsync(id);
        if (banco is not null)
        {
            context.Bancos.Remove(banco);
            await context.SaveChangesAsync();
        }
    }
}
