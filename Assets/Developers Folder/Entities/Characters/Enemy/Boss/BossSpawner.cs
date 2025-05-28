using System;
using System.Threading.Tasks;
using UnityEngine;

public class BossSpawner : MonoBehaviour    
{

    public async void SpawnBoss(GameObject bossPrefab, KatanaFactory katanaFactory, 
        MagicProjectileFactory magicFactory, ManagerUI managerUI)
    {
        var katana = await katanaFactory.GetKatanaAsync();
        GameObject boss = Instantiate(bossPrefab, transform.position, transform.rotation );
        if (katana is not null)
            Instantiate(katana, boss.GetComponentInChildren<WeaponSocketMarker>().transform); 
        else 
            Debug.LogError("Katana not found");
        boss.GetComponent<BossController>().MagicFactory = magicFactory;
        EntityCoreCreator.EntityCoreCreation(boss, managerUI, 500, 500, 
            1, 3, 15, 3, 3, 3);
        
    }
}
