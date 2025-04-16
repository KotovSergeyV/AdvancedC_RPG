using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SettingsScreen : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private Button closeButton;

    [SerializeField] private GameObject _mainScreen;

    private Resolution[] resolutions;

    private void Awake()
    {
        _mainScreen = GameObject.Find("MainMenu(Clone)");
    }

    private void Start()
    {
        InitializeResolutionOptions();
        InitializeQualityOptions();

        fullscreenToggle.isOn = Screen.fullScreen;
        fullscreenToggle.onValueChanged.AddListener(SetFullScreen);

        qualityDropdown.onValueChanged.AddListener(SetQuality);

        closeButton.onClick.AddListener(CloseSettings);
    }

    private void OnEnable()
    {
        _mainScreen = GameObject.Find("MainMenu(Clone)");
    }

    private void OnDisable()
    {
        if (_mainScreen != null)
            _mainScreen.SetActive(true);
    }

    private void InitializeResolutionOptions()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = $"{resolutions[i].width} x {resolutions[i].height}";
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        resolutionDropdown.onValueChanged.AddListener(SetResolution);
    }

    private void InitializeQualityOptions()
    {
        qualityDropdown.ClearOptions();

        List<string> options = new List<string>(QualitySettings.names);
        qualityDropdown.AddOptions(options);
        qualityDropdown.value = QualitySettings.GetQualityLevel();
        qualityDropdown.RefreshShownValue();
    }

    private void SetResolution(int index)
    {
        Resolution resolution = resolutions[index];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    private void SetFullScreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    private void SetQuality(int index)
    {
        QualitySettings.SetQualityLevel(index);
    }

    private void CloseSettings()
    {
        gameObject.SetActive(false);
        if (_mainScreen != null)
            _mainScreen.SetActive(true);
    }
}
