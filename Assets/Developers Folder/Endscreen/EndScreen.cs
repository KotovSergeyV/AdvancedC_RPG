using UnityEngine;

public class EndScreen : MonoBehaviour
{

    [SerializeField] Bootstrapper _bootstrapper;

    public void Initialize(Bootstrapper bootstrapper)
    {
        _bootstrapper = bootstrapper;
    }
    public void Hide() { gameObject.SetActive(false); }
    public void Show() { gameObject.SetActive(true); }

    public void ReloadGame() { _bootstrapper.Reload(); }
}
