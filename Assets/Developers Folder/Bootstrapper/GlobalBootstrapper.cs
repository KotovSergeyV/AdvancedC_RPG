using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor.Overlays;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.Events;
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
    [SerializeField] private GameObject _gamePausePrefab;

    [SerializeField][Tooltip("Префаб начального экрана")] private GameObject _mainScreenPrefab;

    [SerializeField] private GameObject _gamePause;

    [SerializeField] private GameObject _settingsScreen;
    [SerializeField] private EndScreen _endscreen;
    [Space(10)]
    [SerializeField] private ManagerSFX _managerSFX;
    [SerializeField] private ManagerVFX _managerVFX;
    [SerializeField] private ManagerUI _managerUI;

    private SaveLoadManager _saveLoadManager;


    // Контейнер данных для загрузки для кнопки "Продолжить"
    List<EntitySaveData> __LoadDataContainer;

    // Ивенты загрузки MainScreen с данными/без данных
    event Action mainScr_onLoadWithData;
    event Action mainScr_onLoadWithoutData;


    private void Awake()
    {

        //Manager Instantiation
        gameObject.AddComponent<ManagerSFX>();
        gameObject.AddComponent<ManagerVFX>();
        gameObject.AddComponent<ManagerUI>();

        _managerSFX = gameObject.GetComponent<ManagerSFX>();
        _managerVFX = gameObject.GetComponent<ManagerVFX>();
        _managerUI = gameObject.GetComponent<ManagerUI>();


        _endscreen = Instantiate(_endscreenPrefab).GetComponent<EndScreen>();
        _endscreen.Initialize(this);
        _endscreen.Hide();

        _saveLoadManager = new SaveLoadManager(new RepositoryJson());



// - MainScreen Init-
        
        var mainSc = new MainMenuScript();

        // Параметры: Префаб экрана, объект settingsScreen  (будет заменен на Script позже),
        // Ссылка на функцию кнопки "новая игра", Ссылка на функцию кнопки "Продолжить"
        mainSc.Initialize(_mainScreenPrefab, _settingsScreen, LoadNewGame,
            delegate { LoadFromSave(__LoadDataContainer); });

        // Бинд кнопки "Продолжить" для загрузки с данными/без данных
        mainScr_onLoadWithData += delegate { mainSc.SetLoadBtnInteractable(true); };
        mainScr_onLoadWithoutData += delegate { mainSc.SetLoadBtnInteractable(false); };

        // Бинд загрузки начального экрана при выходе из сцены
        mainScr_onLoadWithData += mainSc.ShowScreen;
        mainScr_onLoadWithoutData += mainSc.ShowScreen;

// ------------------




        _gamePause = Instantiate(_gamePausePrefab);
        _gamePause.SetActive(false);
        var btns1 = _gamePause.GetComponentsInChildren<Button>();
        btns1[0].onClick.AddListener(ExitWithSave);
        btns1[1].onClick.AddListener(CloseInGameMenu);


        _settingsScreen = Instantiate(_settingsScreen);
        _settingsScreen.SetActive(false);


        // Загрузить начальный экран
        LoadMainScreen();
    }



    // DEBUG ---------------------------
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {

            if (_gamePause.activeSelf == true)
            {
                CloseInGameMenu();
            }
            else
            {
                _gamePause.SetActive(true);
                Time.timeScale = 0;
            }
        }
    }

    private void CloseInGameMenu()
    {
        _gamePause.SetActive(false);
        Time.timeScale = 1;
    }

    // ----------------------------

    public async Task Unload()
    {
        Destroy(_player);
        await SceneManager.UnloadSceneAsync("CyberpunkScene");
    }


    #region SceneManagement

    //Функция на кнопку "Сохранить и выйти" (внутриигровая пауза)
    private async void ExitWithSave()
    {
        // Генерация данных для сохранения+очистка отслеживаемых сущностей
        var data = EntityAgregator.GenerateSaveData();
        EntityAgregator.Clear();

        _gamePause.SetActive(false);    // заменится позже

        // Сохранение данных в репозиторий
        await _saveLoadManager.SaveInRepoAsync(data);

        // Unload текущей сцены
        await Unload();

        // Загрузка главного меню
        await LoadMainScreen();

        Time.timeScale = 1.0f;
    }
    public async void Reload()
    {
        await Unload();
        LoadNewGame();
    }

    private async Task LoadMainScreen()
    {
        // Обновление данных контейнера для функции "Продолжить"
        var dataForLoading = await _saveLoadManager.LoadFromRepo();
        __LoadDataContainer = new List<EntitySaveData>(dataForLoading);

        // Вызов главного меню
        if (__LoadDataContainer != null && __LoadDataContainer.Count > 0) mainScr_onLoadWithData?.Invoke();
        else mainScr_onLoadWithoutData?.Invoke();
    }

    private void LoadNewGame()
    {

        EntityAgregator.Clear();


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

        Debug.LogWarning("DATA ON LOAD:");
        Debug.LogWarning(data.Count);

        EntitySaveData playerData = data
                .FirstOrDefault(x => x.EntityType == Enum_EntityType.Player);
        data.Remove(playerData);
        Debug.LogWarning(playerData);
        Debug.LogWarning("ENEMY DATA ON LOAD:");
        Debug.LogWarning(data.Count);

        _player = Instantiate(_playerPrefab, playerData.Position, playerData.Rotation);
        PlayerCreation(_player, playerData.CoreData);

        EntityAgregator.AddEntity(_player, Enum_EntityType.Player);

        _playerHealthBar = GameObject.Find("HealthBar")?.GetComponent<HealthBar>();
        _playerManaBar = GameObject.Find("ManaBar")?.GetComponent<ManaBar>(); ;

        // Game Load
        LoadGameScene(data);
    }

    #endregion


    private void PlayerCreation(GameObject player, CoreData playerData = null)
    {
        // Entity Core
        EntityCoreSystem entityCoreSystem;
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
            UnityEngine.Events.UnityAction<UnityEngine.SceneManagement.Scene, LoadSceneMode> handler = (scene, mode) => OnSceneLoadedFromSave(scene, mode, __LoadDataContainer);

            SceneManager.sceneLoaded += handler;
        }
        SceneManager.LoadScene("CyberpunkScene", LoadSceneMode.Additive);
    }

    private void OnNewSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "CyberpunkScene")
        {
            SceneManager.sceneLoaded -= OnNewSceneLoaded;
            
            SceneBootstrapper boot = new SceneBootstrapper();
            boot.Initialize(_managerSFX, _managerUI);
            _managerUI.Initialize();
        }
    }
    private void OnSceneLoadedFromSave(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode, List<EntitySaveData> data)
    {
        UnityEngine.Events.UnityAction<UnityEngine.SceneManagement.Scene, LoadSceneMode> handler =
            (s, m) => OnSceneLoadedFromSave(s, m, __LoadDataContainer);
        SceneManager.sceneLoaded -= handler;

        if (scene.name == "CyberpunkScene")
        {
            SceneBootstrapper boot = new SceneBootstrapper();
            Debug.Log("!!!!!!!! "+_managerSFX + "   " + _managerUI + "   " + data);
            boot.Initialize(_managerSFX, _managerUI, data);
            _managerUI.Initialize();
        }
        __LoadDataContainer = null;
    }

}
