using System.Collections.Concurrent;
using FuncProject.Models;

namespace FuncProject;

public class AssetStore
{
    private readonly ConcurrentDictionary<string, AssetResponse> _assets = new();

    public AssetResponse? GetByAssetTag(string assetTag)
    {
        _assets.TryGetValue(assetTag, out var asset);
        return asset;
    }

    public AssetList GetAll()
    {
        return new AssetList { Items = _assets.Values.ToList() };
    }

    public (AssetResponse? Asset, bool AlreadyExists) Create(AssetCreate asset)
    {
        var response = new AssetResponse
        {
            Id = Guid.NewGuid().ToString(),
            Name = asset.Name,
            Description = asset.Description,
            Department = asset.Department,
            PurchasePrice = asset.PurchasePrice,
            AssetTag = asset.AssetTag,
            Type = asset.Type,
            AssignedTo = asset.AssignedTo,
            PurchaseDate = asset.PurchaseDate,
            Status = asset.Status
        };

        if (!_assets.TryAdd(asset.AssetTag, response))
            return (null, true);

        return (response, false);
    }
}
