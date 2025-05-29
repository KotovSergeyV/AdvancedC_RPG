using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class SettingsScreen : MonoBehaviour
{
    [Header("UI Elements")]

    [SerializeField] private Toggle peaceModeToggle;

    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private Button closeButton;

    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider voiceVolumeSlider;

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;

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


        Peacemode isPeaceful = FindAnyObjectByType<Peacemode>();
        peaceModeToggle.isOn = isPeaceful.PeacefulMode;
        peaceModeToggle.onValueChanged.AddListener(delegate { isPeaceful.PeacefulMode = !isPeaceful.PeacefulMode;  });

        fullscreenToggle.isOn = Screen.fullScreen;
        fullscreenToggle.onValueChanged.AddListener(SetFullScreen);

        qualityDropdown.onValueChanged.AddListener(SetQuality);

        closeButton.onClick.AddListener(CloseSettings);

        musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        voiceVolumeSlider.onValueChanged.AddListener(SetVoiceVolume);

        musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
        masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        voiceVolumeSlider.value = PlayerPrefs.GetFloat("VoiceVolume", 1f);
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


    private void SetMusicVolume(float volume)
    {
        if (volume == 0)
        {
            audioMixer.SetFloat("MusicVolume", -80);
            PlayerPrefs.SetFloat("MusicVolume", volume);
        }
        else{
            audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 40);
            PlayerPrefs.SetFloat("MusicVolume", volume);
        }
    }

    private void SetSFXVolume(float volume)
    {
        if (volume == 0)
        {
            audioMixer.SetFloat("SFXVolume", -80);
            PlayerPrefs.SetFloat("SFXVolume", volume);
        }
        else{
            audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 40);
            PlayerPrefs.SetFloat("SFXVolume", volume);
        }
    }

    private void SetMasterVolume(float volume)
    {
        if (volume == 0)
        {
            audioMixer.SetFloat("MasterVolume", -80);
            PlayerPrefs.SetFloat("MasterVolume", volume);
        }
        else{
            audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 40);
            PlayerPrefs.SetFloat("MasterVolume", volume);
        }
    }

    private void SetVoiceVolume(float volume)
    {
        if (volume == 0)
        {
            audioMixer.SetFloat("VoiceVolume", -80);
            PlayerPrefs.SetFloat("VoiceVolume", volume);
        }
        else{
            audioMixer.SetFloat("VoiceVolume", Mathf.Log10(volume) * 40);
            PlayerPrefs.SetFloat("VoiceVolume", volume);
        }
    }

    private void CloseSettings()
    {
        gameObject.SetActive(false);
        if (_mainScreen != null)
            _mainScreen.SetActive(true);
    }
}
