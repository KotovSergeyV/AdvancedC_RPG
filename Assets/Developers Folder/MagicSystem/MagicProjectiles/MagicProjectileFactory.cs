using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public abstract class MagicProjectileFactory
{
    private AsyncOperationHandle<GameObject> _handleMagic;
    private AsyncOperationHandle<Material> _handleMaterial;
    private AsyncOperationHandle<AudioClip > _handleSound;
    
    protected AssetReference _materialKey; 
    protected AssetReference _audioKey; 

    public async Task<GameObject> GetMagicAsync(BossController owner, Struct_DamageData damageData )
    {

        _handleMagic = Addressables.LoadAssetAsync<GameObject>("ProjectileMagicPrefab");
        GameObject magic = await _handleMagic.Task;
        Addressables.Release(_handleMagic);
        if (magic == null) return null;
        
        SetMaterial();
        _handleMaterial = Addressables.LoadAssetAsync<Material>(_materialKey);
        Material material = await _handleMaterial.Task;
        Addressables.Release(_handleMaterial);
        if (material == null) return null;

        magic.GetComponent<MeshRenderer>().material = material;
        
        SetSound();
        _handleSound = Addressables.LoadAssetAsync<AudioClip>(_audioKey);
        AudioClip sound = await _handleSound.Task;
        Addressables.Release(_handleSound);
        if (sound == null) return null;
        
        var magicPrj = GameObject.Instantiate(magic, owner.transform.position + (owner.transform.forward + Vector3.up)*1.5f,
            owner.transform.rotation );
        magicPrj.GetComponent<MagicProjectile_Base>().Initialize(owner.gameObject, 1, 12,
            owner.transform.forward, 10, damageData, sound, owner.ManagerSFX);

        return magic;
    }

    protected abstract void SetMaterial();
    protected abstract void SetSound();
}


public class LightningMagicProjectileFactory : MagicProjectileFactory
{
    protected override void SetMaterial()
    {
        _materialKey = new AssetReference("MagicYellowMat");
    }
    protected override void SetSound()
    {
        _audioKey = new AssetReference("ElectricSound");
    }
}

public class FireMagicProjectileFactory : MagicProjectileFactory
{
    protected override void SetMaterial()
    {
        _materialKey = new AssetReference("MagicRedMat");
    }
    protected override void SetSound()
    {
        _audioKey = new AssetReference("FireSound");
    }
}

public class WindMagicProjectileFactory : MagicProjectileFactory
{
    protected override void SetMaterial()
    {
        _materialKey = new AssetReference("MagicGreenMat");
    }
    protected override void SetSound()
    {
        _audioKey = new AssetReference("WindSound");
    }
}

public class SpaceMagicProjectileFactory : MagicProjectileFactory
{
    protected override void SetMaterial()
    {
        _materialKey = new AssetReference("MagicPurpleMat");
    }
    protected override void SetSound()
    {
        _audioKey = new AssetReference("SpaceSound");
    }
}