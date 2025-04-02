using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;



public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineInputAxisController _cameraInputController;
    private CursorLockMode _cursorLockMode;
    private InputSystem_Actions _inputs;

    [SerializeField] private Texture2D _defaultCursor;
    [SerializeField] private Texture2D _highlightCursor;
    [SerializeField] private Vector2 _cursorHotspot;

    private AsyncOperationHandle<Texture2D> _dCursorLoadHandle;
    private AsyncOperationHandle<Texture2D> _hCursorLoadHandle;

    private Camera _mainCamera;

    private void Awake()
    {
        _cameraInputController = GetComponentInChildren<CinemachineInputAxisController>();
        LoadCursorsTexture("DefaultCursor", _defaultCursor);

        _inputs = new InputSystem_Actions();
        _mainCamera = Camera.main;
    }

    async void LoadCursorsTexture(string cursorAddress, Texture2D cursor)
    {
        _dCursorLoadHandle = Addressables.LoadAssetAsync<Texture2D>("DefaultCursor");
        _hCursorLoadHandle = Addressables.LoadAssetAsync<Texture2D>("HighlightCursor");
        await _dCursorLoadHandle.Task;
        await _hCursorLoadHandle.Task;

        if (_dCursorLoadHandle.Status == AsyncOperationStatus.Succeeded) { _defaultCursor = _dCursorLoadHandle.Result; }
        else { Debug.Log("Error assigning default texture!"); }
        if (_hCursorLoadHandle.Status == AsyncOperationStatus.Succeeded) { _highlightCursor = _hCursorLoadHandle.Result; }
        else { Debug.Log("Error assigning highlight texture!"); }
    }

    private void OnEnable()
    {
        _inputs.Player.RotateCamera.performed += OnRotateCamera;
        _inputs.Player.RotateCamera.canceled += OnRotateCamera;
        _inputs.Enable();
    }

    private void OnDisable()
    {
        _inputs.Disable();
    }

    private void OnRotateCamera(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _cameraInputController.enabled = true;
            _cursorLockMode = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if (context.canceled)
        {
            _cameraInputController.enabled = false;
            _cursorLockMode = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void Update()
    {
        UpdateCursor();
    }

    private void UpdateCursor()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                Cursor.SetCursor(_highlightCursor, _cursorHotspot, CursorMode.Auto);
            }
            else
            {
                Cursor.SetCursor(_defaultCursor, _cursorHotspot, CursorMode.Auto);
            }
        }
        else
        {
            Cursor.SetCursor(_defaultCursor, _cursorHotspot, CursorMode.Auto);
        }
    }
}
