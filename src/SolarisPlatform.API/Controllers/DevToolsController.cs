using Microsoft.AspNetCore.Mvc;
using SolarisPlatform.Application.Common.Models;

namespace SolarisPlatform.API.Controllers;

/// <summary>
/// Controller de herramientas de desarrollo — solo disponible en Development
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class DevToolsController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<DevToolsController> _logger;

    public DevToolsController(
        IWebHostEnvironment env,
        ILogger<DevToolsController> logger)
    {
        _env = env;
        _logger = logger;
    }

    /// <summary>
    /// Retorna la estructura de directorios y archivos del proyecto en tiempo real.
    /// Consumido por /pages/estructura.html para el explorador visual y descarga para Claude.
    /// Solo disponible en entorno Development.
    /// </summary>
    [HttpGet("estructura")]
    public ActionResult<ApiResponse<object>> GetEstructura()
    {
        if (!_env.IsDevelopment())
        {
            return NotFound(ApiResponse<object>.Fail("Endpoint no disponible en producción"));
        }

        try
        {
            // Subir desde bin/Debug/netX.0 hasta la raíz de la solución
            var projectRoot = FindSolutionRoot(_env.ContentRootPath);

            if (projectRoot == null)
            {
                return NotFound(ApiResponse<object>.Fail("No se encontró la raíz de la solución (.sln)"));
            }

            var tree = BuildTree(new DirectoryInfo(projectRoot), depth: 0, maxDepth: 10);

            return Ok(ApiResponse<object>.Ok(tree, "Estructura cargada correctamente"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al escanear estructura del proyecto");
            return StatusCode(500, ApiResponse<object>.Fail($"Error al escanear: {ex.Message}"));
        }
    }

    // ─────────────────────────────────────────────────────────────
    //  HELPERS
    // ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Sube en el árbol de directorios hasta encontrar un .sln o la raíz del disco.
    /// </summary>
    private static string? FindSolutionRoot(string startPath)
    {
        var current = new DirectoryInfo(startPath);
        while (current != null)
        {
            if (current.GetFiles("*.sln").Length > 0)
                return current.FullName;
            current = current.Parent;
        }
        // Si no hay .sln, usar ContentRoot directamente
        return startPath;
    }

    /// <summary>
    /// Construye recursivamente el árbol de nodos del proyecto.
    /// Excluye carpetas de build, cache y herramientas.
    /// </summary>
    private static object BuildTree(DirectoryInfo dir, int depth, int maxDepth)
    {
        if (depth > maxDepth)
        {
            return new
            {
                name     = dir.Name,
                type     = "directory",
                path     = NormalizePath(dir.FullName),
                children = Array.Empty<object>(),
                truncated = true
            };
        }

        // Carpetas a ignorar (build artifacts, cache, herramientas)
        var ignoreDirs = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "bin", "obj", ".git", ".vs", ".idea", "node_modules",
            ".github", ".vscode", "TestResults", ".sonarqube",
            "__pycache__", ".angular", "dist", "coverage"
        };

        // Extensiones a ignorar (binarios, secrets, user files)
        var ignoreExts = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".user", ".suo", ".db", ".lock", ".log",
            ".exe", ".dll", ".pdb", ".nupkg", ".zip",
            ".DS_Store", ".cache"
        };

        var children = new List<object>();

        // Primero carpetas (ordenadas)
        foreach (var subDir in dir.GetDirectories()
                                  .Where(d => !ignoreDirs.Contains(d.Name))
                                  .OrderBy(d => d.Name))
        {
            children.Add(BuildTree(subDir, depth + 1, maxDepth));
        }

        // Luego archivos (ordenados)
        foreach (var file in dir.GetFiles()
                                .Where(f => !ignoreExts.Contains(f.Extension))
                                .OrderBy(f => f.Name))
        {
            children.Add(new
            {
                name         = file.Name,
                type         = "file",
                path         = NormalizePath(file.FullName),
                size         = file.Length,
                lastModified = file.LastWriteTimeUtc,
                lines        = CountLines(file)
            });
        }

        return new
        {
            name     = dir.Name,
            type     = "directory",
            path     = NormalizePath(dir.FullName),
            children = children.ToArray()
        };
    }

    /// <summary>
    /// Cuenta líneas de archivos de texto pequeños (max 500KB).
    /// Retorna null para binarios o archivos grandes.
    /// </summary>
    private static int? CountLines(FileInfo file)
    {
        var textExts = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".cs", ".json", ".xml", ".html", ".css", ".js",
            ".ts", ".md", ".txt", ".yml", ".yaml", ".csproj",
            ".sln", ".razor", ".cshtml", ".sh", ".env"
        };

        if (!textExts.Contains(file.Extension)) return null;
        if (file.Length > 512_000) return null; // skip > 500KB

        try
        {
            return System.IO.File.ReadLines(file.FullName).Count();
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Normaliza separadores de path a forward slash para consistencia en el JSON.
    /// </summary>
    private static string NormalizePath(string path)
        => path.Replace('\\', '/');
}
