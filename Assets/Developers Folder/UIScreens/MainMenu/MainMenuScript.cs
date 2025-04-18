using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MainMenuScript
{
    [SerializeField] private GameObject _mainScreen;


    // Префаб экрана, объект settingsScreen  (будет заменен на Script позже),
    // Ссылка на функцию кнопки "новая игра", Ссылка на функцию кнопки "Продолжить"
    public void Initialize(GameObject mainScreenPrefab, GameObject settingsScreen,
        UnityAction LoadNewGame, UnityAction LoadGame)
    {
        _mainScreen = GameObject.Instantiate(mainScreenPrefab);
        Button[] btns = _mainScreen.GetComponentsInChildren<Button>();


        btns[0].onClick.AddListener(LoadGame);
        btns[0].onClick.AddListener(HideScreen);

        btns[1].onClick.AddListener(LoadNewGame);
        btns[1].onClick.AddListener(HideScreen);

        btns[2].onClick.AddListener(delegate { Setting(settingsScreen); });
        btns[2].onClick.AddListener(HideScreen);

        btns[3].onClick.AddListener(Application.Quit);

    }


    private void Setting(GameObject settingsScreen)
    {
        settingsScreen.SetActive(true);
    }

    public void SetLoadBtnInteractable(bool interactable)
    {
        Button btn = _mainScreen.GetComponentsInChildren<Button>()[0];
        btn.interactable = interactable;
    }

    void HideScreen()
    {
        _mainScreen.SetActive(false);
    }

    public void ShowScreen()
    {
        _mainScreen.SetActive(true);
    }
}
