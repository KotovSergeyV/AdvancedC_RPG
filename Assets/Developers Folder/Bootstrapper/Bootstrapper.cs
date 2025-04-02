using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Bootstrapper : MonoBehaviour
{

    [SerializeField] private GameObject _playerPrefab;
    private HealthBar _playerHealthBar;
    [SerializeField] private GameObject _player;

    private ManagerSFX _managerSFX;
    private ManagerVFX _managerVFX;
    private ManagerUI _managerUI;


    private void Awake()
    {
        //Manager Instantiation
        _managerVFX = new ManagerVFX();
        _managerSFX = new ManagerSFX();
        _managerUI = new ManagerUI();

        // Player Instantiation
        _player = Instantiate(_playerPrefab, Vector3.up, Quaternion.identity);
        PlayerCreation(_player);

        _playerHealthBar = GameObject.Find("HealthSlider")?.GetComponent<HealthBar>();

        // Game Load
        LoadGameScene();
    }

    private void PlayerCreation(GameObject player)
    {
        // Entity Core
        EntityCoreSystem entityCoreSystem = EntityCoreCreation(player);

        // Magic System
        MagicCaster magicCaster = player.AddComponent<MagicCaster>();
        magicCaster.Initialize(entityCoreSystem.GetManaSystem(), _managerSFX, _managerVFX);

        //Input-connected Systems
        PlayerController playerController = player.AddComponent<PlayerController>();
        playerController.Initialize(magicCaster, _managerSFX);
        player.AddComponent<CameraController>();
        player.AddComponent<PlayerJump>();
    }

    private EntityCoreSystem EntityCoreCreation(GameObject entity)
    {
        EntityCoreSystem entityCoreSystem = entity.AddComponent<EntityCoreSystem>();

        entityCoreSystem.Initialize(new HealthSystem(_managerUI, 100), new DamageCalculationSystem(), new ManaSystem(100),
            new StatSystem(1, 1, 1, 1, 1), new EntityStatesSystem(), new Movable());
        try {
            IHealthSystem healthSystem = (entityCoreSystem.GetHealthSystem());
            ((HealthSystem)healthSystem).OnDamaged += entity.GetComponent<AnimatorController>().PlayHitAnimation;
            ((HealthSystem)healthSystem).OnDead += entity.GetComponent<AnimatorController>().PlayDeathAnimation;
            Debug.Log("Initial HP:" + healthSystem.GetHp());
        }
        catch { Debug.Log("Damage/Death anim assignation error!"); }


        return entityCoreSystem;
    }

    private void LoadGameScene()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene("Test_Copy_Graybox_Level_2", LoadSceneMode.Additive);
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Test_Copy_Graybox_Level_2")
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            _managerUI.Initialize();
            InitializeEnemies();
        }
    }

    private void InitializeEnemies()
    {
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            EntityCoreCreation(enemy);
            if (enemy.GetComponent<GunnerAI>())
            {
                enemy.GetComponent<GunnerAI>().Initialize(_managerSFX);
            }
        }
    }
}
