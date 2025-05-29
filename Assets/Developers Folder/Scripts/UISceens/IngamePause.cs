using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class IngamePause
{
    private GameObject _ingamePause;

    public void Initialize(GameObject ingamePause, UnityAction ExitToMainMenu, UnityAction ContinueGame) 
    { 
        _ingamePause = GameObject.Instantiate(ingamePause);
        Button[] btns = _ingamePause.GetComponentsInChildren<Button>();
        btns[0].onClick.AddListener(ExitToMainMenu);
        btns[1].onClick.AddListener(ContinueGame);
    }

    public void Show() { _ingamePause.SetActive(true); }
    public void Hide() { _ingamePause.SetActive(false); }
}
