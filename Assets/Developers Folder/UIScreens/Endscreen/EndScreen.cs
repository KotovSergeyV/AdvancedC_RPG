using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EndScreen
{
    private GameObject _endScreen;

    public void Initialize(GameObject endScreen, UnityAction ReloadGameAction)
    {
        _endScreen = GameObject.Instantiate(endScreen);
        Button[] btns = _endScreen.GetComponentsInChildren<Button>();
        btns[0].onClick.AddListener(ReloadGameAction);
    }
    public void Hide() { _endScreen.SetActive(false); }
    public void Show() 
    { 
        _endScreen.SetActive(true);
    }
}
