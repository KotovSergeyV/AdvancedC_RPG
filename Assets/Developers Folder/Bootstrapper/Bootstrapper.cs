using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class Bootstrapper : MonoBehaviour
{

    [SerializeField] private GameObject _playerPrefab;
    private HealthBar _playerHealthBar;
    private ManaBar _playerManaBar;
    [SerializeField] private GameObject _player;
    [SerializeField] private GameObject _endscreenPrefab;

    [SerializeField] private EndScreen _endscreen;

    private ManagerSFX _managerSFX;
    private ManagerVFX _managerVFX;
    private ManagerUI _managerUI;


    private void Awake()
    {

        _endscreen = Instantiate(_endscreenPrefab).GetComponent<EndScreen>();
        _endscreen.Initialize(this);

        Load();
    }

    public async void Reload()
    { 
        Destroy(_player);
        await SceneManager.UnloadSceneAsync("CyberpunkScene");
        Load();
    }

    private void Load()
    {
        
        _endscreen.Hide();

        //Manager Instantiation
        _managerVFX = new ManagerVFX();
        _managerSFX = new ManagerSFX();
        _managerUI = new ManagerUI();

        // Player Instantiation
        _player = Instantiate(_playerPrefab, Vector3.up, Quaternion.identity);
        PlayerCreation(_player);

        _playerHealthBar = GameObject.Find("HealthBar")?.GetComponent<HealthBar>();
        _playerManaBar = GameObject.Find("ManaBar")?.GetComponent<ManaBar>(); ;

        // Game Load
        LoadGameScene();
    }

    private void PlayerCreation(GameObject player)
    {
        // Entity Core
        EntityCoreSystem entityCoreSystem = EntityCoreCreation(player);

        // EndScreen
        IHealthSystem healthSystem = (entityCoreSystem.GetHealthSystem());
        ((HealthSystem)healthSystem).OnDead += _endscreen.Show;

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

        HealthBar healthBar = entity?.GetComponentInChildren<HealthBar>();
        ManaBar manaBar = entity?.GetComponentInChildren<ManaBar>();

        entityCoreSystem.Initialize(new HealthSystem(_managerUI, 100, healthBar), new DamageCalculationSystem(), new ManaSystem(_managerUI, 100, 0.5f, manaBar),
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
        SceneManager.LoadScene("CyberpunkScene", LoadSceneMode.Additive);
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "CyberpunkScene")
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
