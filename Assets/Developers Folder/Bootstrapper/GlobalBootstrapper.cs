using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class GlobalBootstrapper : MonoBehaviour
{

    [SerializeField] private GameObject _playerPrefab;
    private HealthBar _playerHealthBar;
    private ManaBar _playerManaBar;
    [SerializeField] private GameObject _player;

    [SerializeField] private GameObject _endscreenPrefab;
    [SerializeField] private GameObject _mainScreenPrefab;
    [SerializeField] private GameObject _gamePausePrefab;

    [SerializeField] private GameObject _gamePause;
    [SerializeField] private GameObject _mainScreen;
    [SerializeField] private EndScreen _endscreen;

    private ManagerSFX _managerSFX;
    private ManagerVFX _managerVFX;
    private ManagerUI _managerUI;

    private SaveLoadManager _saveLoadManager;


    private void Awake()
    {

        //Manager Instantiation
        _managerVFX = new ManagerVFX();


        _endscreen = Instantiate(_endscreenPrefab).GetComponent<EndScreen>();
        _endscreen.Initialize(this);
        _endscreen.Hide();

        _saveLoadManager = new SaveLoadManager(new RepositoryJson());


        _gamePause = Instantiate(_gamePausePrefab);
        _gamePause.GetComponentInChildren<Button>().onClick.AddListener(_saveLoadManager.SaveInRepo);
        _gamePause.SetActive(false);

        _mainScreen = Instantiate(_mainScreenPrefab);
        Button[] btns = _mainScreen.GetComponentsInChildren<Button>();

        var data = _saveLoadManager.LoadFromRepo();
        if (data != null) { btns[0].onClick.AddListener(delegate { LoadFromSave(data); } ); }
        else { btns[0].interactable = false;}

        btns[1].onClick.AddListener(LoadNewGame);
       // btns[2].onClick.AddListener();
        btns[3].onClick.AddListener(Application.Quit);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) { _gamePause.SetActive(true); Time.timeScale = 0; }
    }

    public async void Reload()
    { 
        Destroy(_player);
        await SceneManager.UnloadSceneAsync("CyberpunkScene");
        LoadNewGame();
    }

    private void LoadNewGame()
    {

        EntityAgregator.Clear();
        _mainScreen.SetActive(false);

        //Manager Instantiation
        _managerVFX = new ManagerVFX();
        _managerUI = new ManagerUI();

        // Player Instantiation
        _player = Instantiate(_playerPrefab, Vector3.up, Quaternion.identity);
        PlayerCreation(_player);

        EntityAgregator.AddEntity(_player, Enum_EntityType.Player);

        _playerHealthBar = GameObject.Find("HealthBar")?.GetComponent<HealthBar>();
        _playerManaBar = GameObject.Find("ManaBar")?.GetComponent<ManaBar>(); ;

        // Game Load
        LoadGameScene();
    }

    private void LoadFromSave(List<EntitySaveData> data)
    {
        EntityAgregator.Clear();
        _mainScreen.SetActive(false);

        //Manager Instantiation
        _managerVFX = new ManagerVFX();
        _managerUI = new ManagerUI();

   
        EntitySaveData playerData = data
                .FirstOrDefault(x => x.EntityType == Enum_EntityType.Player);
        data.Remove(playerData);

        _player = Instantiate(_playerPrefab, playerData.Position, playerData.Rotation);
        PlayerCreation(_player, playerData.CoreData);

        EntityAgregator.AddEntity(_player, Enum_EntityType.Player);

        _playerHealthBar = GameObject.Find("HealthBar")?.GetComponent<HealthBar>();
        _playerManaBar = GameObject.Find("ManaBar")?.GetComponent<ManaBar>(); ;

        // Game Load
        LoadGameScene(data);
    }




    private void PlayerCreation(GameObject player, CoreData playerData = null)
    {
        // Entity Core
        EntityCoreSystem entityCoreSystem = new EntityCoreSystem();
        if (playerData != null)
        {
            entityCoreSystem = PlayerCoreCreation(player, playerData);
        }
        else 
        {
            entityCoreSystem = PlayerCoreCreation(player);
        }

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

    private EntityCoreSystem PlayerCoreCreation(GameObject entity)
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

    private EntityCoreSystem PlayerCoreCreation(GameObject entity, CoreData coreData)
    {
        EntityCoreSystem entityCoreSystem = entity.AddComponent<EntityCoreSystem>();

        HealthBar healthBar = entity?.GetComponentInChildren<HealthBar>();
        ManaBar manaBar = entity?.GetComponentInChildren<ManaBar>();

        entityCoreSystem.Initialize(new HealthSystem(_managerUI, coreData.HealthData.MaxHealth,healthBar, coreData.HealthData.Health),
            new DamageCalculationSystem(),
            new ManaSystem(_managerUI, coreData.ManaData.MaxMana, 0.5f, manaBar, coreData.ManaData.Mana ),
            new StatSystem(coreData.StatData.Agility, coreData.StatData.Attack, coreData.StatData.Luck, coreData.StatData.Defence, coreData.StatData.Intelligence),
            new EntityStatesSystem(), // <---- current state here after it released in game
            new Movable());
        try
        {
            IHealthSystem healthSystem = (entityCoreSystem.GetHealthSystem());
            ((HealthSystem)healthSystem).OnDamaged += entity.GetComponent<AnimatorController>().PlayHitAnimation;
            ((HealthSystem)healthSystem).OnDead += entity.GetComponent<AnimatorController>().PlayDeathAnimation;

            Debug.Log("Initial HP:" + healthSystem.GetHp());
        }
        catch { Debug.Log("Damage/Death anim assignation error!"); }


        return entityCoreSystem;
    }

    private void LoadGameScene(List<EntitySaveData> data = null)
    {
        if (data == null)
        {
            SceneManager.sceneLoaded += OnNewSceneLoaded;
        }
        else 
        {
            UnityEngine.Events.UnityAction<UnityEngine.SceneManagement.Scene, LoadSceneMode> handler = (scene, mode) => OnSceneLoadedFromSave(scene, mode, data);

            SceneManager.sceneLoaded += handler;
        }
        SceneManager.LoadScene("CyberpunkScene", LoadSceneMode.Additive);
    }

    private void OnNewSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "CyberpunkScene")
        {
            SceneManager.sceneLoaded -= OnNewSceneLoaded;
            _managerUI.Initialize();
            SceneBootstrapper boot = new SceneBootstrapper();
            boot.Initialize(_managerSFX, _managerUI);
        }
    }
    private void OnSceneLoadedFromSave(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode, List<EntitySaveData> data)
    {
        // Create the same Action signature to properly unsubscribe
        UnityEngine.Events.UnityAction<UnityEngine.SceneManagement.Scene, LoadSceneMode> handler = (s, m) => OnSceneLoadedFromSave(s, m, data);
        SceneManager.sceneLoaded -= handler;

        if (scene.name == "CyberpunkScene")
        {
            _managerUI.Initialize();
            SceneBootstrapper boot = new SceneBootstrapper();
            boot.Initialize(_managerSFX, _managerUI, data);
        }
    }

}
