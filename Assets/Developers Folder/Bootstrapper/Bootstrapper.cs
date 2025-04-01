using UnityEditor.SearchService;
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
        _managerVFX = new ManagerVFX();
        _managerSFX = new ManagerSFX();

        // Player Instantiation
        _player = Instantiate(_playerPrefab, Vector3.up, Quaternion.identity);
        PlayerCreation(_player);

        // Game Load
        LoadGameScene();
    }


    private void PlayerCreation(GameObject player)
    {
        // Entity Core
        EntityCoreSystem entityCoreSystem = player.AddComponent<EntityCoreSystem>();
        entityCoreSystem.Initialize(new HealthSystem(), new DamageCalculationSystem(), new ManaSystem(), new StatSystem(), new EntityStatesSystem(), new Movable());

        // Magic System
        MagicCaster magicCaster = player.AddComponent<MagicCaster>();
        magicCaster.Initialize(entityCoreSystem.GetManaSystem(), _managerSFX, _managerVFX);

        //Input-connected Systems
        PlayerController playerController = player.AddComponent<PlayerController>();
        playerController.Initialize(magicCaster);
        player.AddComponent<CameraController>();
        player.AddComponent<PlayerJump>();
    }

    private void LoadGameScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Test_Copy_Graybox_Level_2", UnityEngine.SceneManagement.LoadSceneMode.Additive);
    }
}
