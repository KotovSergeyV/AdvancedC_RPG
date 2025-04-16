using UnityEngine;

public class EndScreen : MonoBehaviour
{

    [SerializeField] GlobalBootstrapper _bootstrapper;

    public void Initialize(GlobalBootstrapper bootstrapper)
    {
        _bootstrapper = bootstrapper;
    }
    public void Hide() { gameObject.SetActive(false); }
    public void Show() { gameObject.SetActive(true); }

    public void ReloadGame() { _bootstrapper.Reload(); }
}
