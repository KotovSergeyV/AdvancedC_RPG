using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineInputAxisController _cameraInputController;
    private CursorLockMode _cursorLockMode;
    private InputSystem_Actions _inputs;

    [SerializeField] private Texture2D _defaultCursor;
    [SerializeField] private Texture2D _highlightCursor;
    [SerializeField] private Vector2 _cursorHotspot;

    private Camera _mainCamera;

    private void Awake()
    {
        _inputs = new InputSystem_Actions();
        _mainCamera = Camera.main;
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
