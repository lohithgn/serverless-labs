using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using FuncProject.Models;

namespace FuncProject;

public class AssetEndpoints
{
    private readonly ILogger<AssetEndpoints> _logger;
    private readonly AssetStore _store;

    public AssetEndpoints(ILogger<AssetEndpoints> logger, AssetStore store)
    {
        _logger = logger;
        _store = store;
    }

    [Function("AddAsset")]
    public IActionResult AddAsset(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "assets")]
        HttpRequest req,
        [Microsoft.Azure.Functions.Worker.Http.FromBody] AssetCreate asset)
    {
        _logger.LogInformation("Processing add asset request for tag: {AssetTag}", asset.AssetTag);

        var (created, alreadyExists) = _store.Create(asset);

        if (alreadyExists)
        {
            _logger.LogWarning("Asset already exists: {AssetTag}", asset.AssetTag);
            return new ConflictObjectResult(new { detail = $"Asset with tag '{asset.AssetTag}' already exists" });
        }

        _logger.LogInformation("Asset created successfully: {AssetTag}", asset.AssetTag);
        return new ObjectResult(created) { StatusCode = StatusCodes.Status201Created };
    }

    [Function("GetAsset")]
    public IActionResult GetAsset(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "assets/{assetTag}")]
        HttpRequest req,
        string assetTag)
    {
        _logger.LogInformation("Processing get asset request for tag: {AssetTag}", assetTag);

        var asset = _store.GetByAssetTag(assetTag);
        if (asset is null)
        {
            return new NotFoundObjectResult(new { detail = $"Asset with tag '{assetTag}' not found" });
        }

        return new OkObjectResult(asset);
    }

    [Function("ListAssets")]
    public IActionResult ListAssets(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "assets")]
        HttpRequest req)
    {
        _logger.LogInformation("Processing list assets request");

        var assets = _store.GetAll();
        return new OkObjectResult(assets);
    }
}
