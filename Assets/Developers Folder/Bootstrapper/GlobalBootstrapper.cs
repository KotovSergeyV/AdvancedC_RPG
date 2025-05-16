using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor.Overlays;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.Timeline.DirectorControlPlayable;

public class GlobalBootstrapper : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions;


    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private GameObject _player;


    [SerializeField][Tooltip("������ ���������� ������")]
    private GameObject _mainScreenPrefab;
    [SerializeField] private GameObject _endscreenPrefab;
    [SerializeField] private GameObject _gamePausePrefab;
    [SerializeField] private GameObject _settingsScreen;

    EndScreen _endscreen;

    [Space(10)]
    [SerializeField] private ManagerSFX _managerSFX;
    [SerializeField] private ManagerVFX _managerVFX;
    [SerializeField] private ManagerUI _managerUI;

    [SerializeField] private AudioClip[] _musicClip;

    private SaveLoadManager _saveLoadManager;


    // ��������� ������ ��� �������� ��� ������ "����������"
    List<EntitySaveData> __LoadDataContainer;

    // ������ �������� MainScreen � �������/��� ������
    event Action mainScr_onLoadWithData;
    event Action mainScr_onLoadWithoutData;

    // ������ Show/Hide ���� EndGame
    event Action ACT_endScr_Hide;
    event Action ACT_endScr_Show;

    // ������ Show/Hide ���� IngamePause
    event Action ACT_pauseScr_Hide;
    event Action ACT_pauseScr_Show;


    bool _isPaused = false;


    private void Awake()
    {
// Temp - Setting Application - Temp
        Peacemode mode = gameObject.AddComponent<Peacemode>();
        mode.PeacefulMode = false;
// Temp

// Manager Instantiation
        gameObject.AddComponent<ManagerSFX>();
        gameObject.AddComponent<ManagerVFX>();
        gameObject.AddComponent<ManagerUI>();

        _managerSFX = gameObject.GetComponent<ManagerSFX>();
        _managerVFX = gameObject.GetComponent<ManagerVFX>();
        _managerUI = gameObject.GetComponent<ManagerUI>();

        _saveLoadManager = new SaveLoadManager(new RepositoryJson());

// EndScreen Instantiate
        _endscreen = new EndScreen();
        _endscreen.Initialize(_endscreenPrefab, Reload);
        ACT_endScr_Hide += _endscreen.Hide;
        ACT_endScr_Show += _endscreen.Show;
        ACT_endScr_Hide.Invoke();

// SettingsScreen Instantiate 
        var settings = Instantiate(_settingsScreen);
        settings.SetActive(false);

//  MainScreen Init

        var mainSc = new MainMenuScript();

        // ���������: ������ ������, ������ settingsScreen  (����� ������� �� Script �����),
        // ������ �� ������� ������ "����� ����", ������ �� ������� ������ "����������"
        mainSc.Initialize(_mainScreenPrefab, settings, LoadNewGame,

            delegate { LoadFromSave(__LoadDataContainer); });

        // ���� ������ "����������" ��� �������� � �������/��� ������
        mainScr_onLoadWithData += delegate { mainSc.SetLoadBtnInteractable(true); };
        mainScr_onLoadWithoutData += delegate { mainSc.SetLoadBtnInteractable(false); };

        // ���� �������� ���������� ������ ��� ������ �� �����
        mainScr_onLoadWithData += mainSc.ShowScreen;
        mainScr_onLoadWithoutData += mainSc.ShowScreen;


//  IngamePauseScreen Init
        IngamePause ingamePause = new IngamePause();
        ingamePause.Initialize(_gamePausePrefab, ExitWithSave, CloseInGameMenu);
        ACT_pauseScr_Hide += ingamePause.Hide;
        ACT_pauseScr_Show += ingamePause.Show;
        ACT_pauseScr_Hide.Invoke();


        _managerSFX.PlaySFX(_musicClip[0], transform.position, ManagerSFX.MixerGroupType.Music, null, false, 1, 0);
        // ��������� ��������� �����
        LoadMainScreen();
    }



    // DEBUG ---------------------------
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_isPaused)
            {
                _isPaused = false;
                CloseInGameMenu();
            }
            else if (!_endscreen.GetShow())
            {
                _isPaused = true;
                ShowInGameMenu();
            }
        }

    }
    private void ShowInGameMenu()
    {
        ACT_pauseScr_Show.Invoke();
        Time.timeScale = 0;
    }

    private void CloseInGameMenu()
    {
        ACT_pauseScr_Hide.Invoke();
        Time.timeScale = 1;
    }

    // ----------------------------

    public async Task Unload()
    {
        Destroy(_player);
        await SceneManager.UnloadSceneAsync("CyberpunkScene");
    }


    #region SceneManagement

    //������� �� ������ "��������� � �����" (������������� �����)
    private async void ExitWithSave()
    {
        // ��������� ������ ��� ����������+������� ������������� ���������
        var data = EntityAgregator.GenerateSaveData();
        EntityAgregator.Clear();

        // �������� ����� �����
        ACT_pauseScr_Hide.Invoke();


        // ���������� ������ � �����������
        await _saveLoadManager.SaveInRepoAsync(data);

        // Unload ������� �����
        await Unload();

        // �������� �������� ����
        await LoadMainScreen();

        Time.timeScale = 1.0f;
    }
    public async void Reload()
    {
        ACT_endScr_Hide.Invoke();
        await Unload();
        LoadNewGame();
    }

    private async Task LoadMainScreen()
    {
        // ���������� ������ ���������� ��� ������� "����������"
        var dataForLoading = await _saveLoadManager.LoadFromRepo();
        __LoadDataContainer = new List<EntitySaveData>(dataForLoading);

        _managerSFX.StopAllSFX();
        _managerSFX.PlaySFX(_musicClip[0], transform.position, ManagerSFX.MixerGroupType.Music, null, false, 1, 0);

        // ����� �������� ����
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

        _managerSFX.StopAllSFX();
        _managerSFX.PlaySFX(_musicClip[1], transform.position, ManagerSFX.MixerGroupType.Music, null, false, 1, 0);

        // Game Load
        LoadGameScene();
    }

    private void LoadFromSave(List<EntitySaveData> data)
    {

        EntitySaveData playerData = data
                .FirstOrDefault(x => x.EntityType == Enum_EntityType.Player);
        data.Remove(playerData);

        _player = Instantiate(_playerPrefab, playerData.Position, playerData.Rotation);
        PlayerCreation(_player, playerData.CoreData);

        EntityAgregator.AddEntity(_player, Enum_EntityType.Player);

        _managerSFX.StopAllSFX();
        _managerSFX.PlaySFX(_musicClip[1], transform.position, ManagerSFX.MixerGroupType.Music, null, false, 1, 0);

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
        ((HealthSystem)healthSystem).OnDead += ACT_endScr_Show.Invoke;

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
            new StatSystem(1, 1, 1, 1, 1), new EntityStatesSystem());
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
            new EntityStatesSystem() // <---- current state here after it released in game
            );
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
