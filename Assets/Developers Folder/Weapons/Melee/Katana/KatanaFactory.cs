using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


public abstract class KatanaFactory
{
    private AsyncOperationHandle<GameObject> _handleKatana;
    private AsyncOperationHandle<Material> _handleMaterial;
    
    protected AssetReference _materialKey; 
    protected Color _color = Color.white; 

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
        
        SetColor();
        katana.GetComponentInChildren<TrailRenderer>().startColor = _color;
        katana.GetComponentInChildren<TrailRenderer>().endColor = 
            new Color(_color.r*0.7f,_color.g*0.7f,_color.b*0.7f, 1);
        
        return katana;
    }

    protected abstract void SetMaterial();
    protected abstract void SetColor();
}


public class LightningKatanaFactory : KatanaFactory
{
    protected override void SetMaterial()
    {
        _materialKey = new AssetReference("KatanaYellowMat");
    }
    protected override void SetColor()
    {
        _color = new Color(.9f, .9f, 0, 1);
    }
}

public class FireKatanaFactory : KatanaFactory
{
    protected override void SetMaterial()
    {
        _materialKey = new AssetReference("KatanaRedMat");
    }
    protected override void SetColor()
    {
        _color = new Color(.8f, .1f, 0, 1);
    }
}

public class WindKatanaFactory : KatanaFactory
{
    protected override void SetMaterial()
    {
        _materialKey = new AssetReference("KatanaGreenMat");
    }
    protected override void SetColor()
    {
        _color = new Color(0.1f, 0.5f, 0.1f, 1);
    }
}

public class SpaceKatanaFactory : KatanaFactory
{
    protected override void SetMaterial()
    {
        _materialKey = new AssetReference("KatanaPurpleMat");
    }
    
    protected override void SetColor()
    {
        _color = new Color(.2f, 0, .8f, 1);
    }
}