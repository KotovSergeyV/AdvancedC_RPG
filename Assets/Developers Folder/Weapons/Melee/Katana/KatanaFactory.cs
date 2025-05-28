using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


public abstract class KatanaFactory
{
    private AsyncOperationHandle<GameObject> _handleKatana;
    private AsyncOperationHandle<Material> _handleMaterial;
    
    protected AssetReference _materialKey; 

    public async Task<GameObject> GetKatanaAsync()
    {
        SetMaterial();
        _handleKatana = Addressables.LoadAssetAsync<GameObject>("CyberKatanaPrefab");
        GameObject katana = await _handleKatana.Task;
        Addressables.Release(_handleKatana);
        if (katana == null) return null;

        _handleMaterial = Addressables.LoadAssetAsync<Material>(_materialKey);
        Material material = await _handleMaterial.Task;
        Addressables.Release(_handleMaterial);
        if (material == null) return null;

        katana.GetComponent<MeshRenderer>().material = material;

        return katana;
    }

    protected abstract void SetMaterial();
}


public class LightningKatanaFactory : KatanaFactory
{
    protected override void SetMaterial()
    {
        _materialKey = new AssetReference("KatanaYellowMat");
    }
}

public class FireKatanaFactory : KatanaFactory
{
    protected override void SetMaterial()
    {
        _materialKey = new AssetReference("KatanaRedMat");
    }
}

public class WindKatanaFactory : KatanaFactory
{
    protected override void SetMaterial()
    {
        _materialKey = new AssetReference("KatanaGreenMat");
    }
}

public class SpaceKatanaFactory : KatanaFactory
{
    protected override void SetMaterial()
    {
        _materialKey = new AssetReference("KatanaPurpleMat");
    }
}