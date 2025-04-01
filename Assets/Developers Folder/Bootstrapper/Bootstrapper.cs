using UnityEngine;

public class Bootstrapper : MonoBehaviour
{

    [SerializeField] private GameObject _playerPrefab;

    [SerializeField] private GameObject _player;

    private ManagerSFX _managerSFX;
    private ManagerVFX _managerVFX;


    private void Awake()
    {
        //Manager Instantiation
        _managerSFX =  Instantiate(ManagerSFX.Instance, Vector3.zero, Quaternion.identity);
        _managerVFX =  Instantiate(ManagerVFX.Instance, Vector3.zero, Quaternion.identity);

        // Player Instantiation
        _player = Instantiate(_playerPrefab, Vector3.zero, Quaternion.identity);

        EntityCoreCreation(_player);

    }


    private void EntityCoreCreation(GameObject entity)
    {
        EntityCoreSystem entityCoreSystem = entity.AddComponent<EntityCoreSystem>();
        entityCoreSystem.Initialize(new HealthSystem(), new DamageCalculationSystem(), new ManaSystem(), new StatSystem(), new EntityStatesSystem());
    }
}
