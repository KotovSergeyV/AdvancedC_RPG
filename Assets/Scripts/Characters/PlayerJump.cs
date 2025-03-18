using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerJump : MonoBehaviour
{
    [SerializeField] private float jumpHeight = 2.0f;
    [SerializeField] private float gravity = 9.81f;
    private CharacterController _characterController;
    private Vector3 _velocity;
    private bool _isGrounded;

    private InputSystem_Actions _inputs;

    void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _inputs = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        _inputs.Player.Jump.performed += OnJump;
        _inputs.Enable();
    }

    private void OnDisable()
    {
        _inputs.Disable();
    }

    void Update()
    {
        _isGrounded = _characterController.isGrounded;

        if (_characterController.enabled)
        {

            if (_isGrounded && _velocity.y < 0)
            {
                _velocity.y = -2f;
            }

            _velocity.y -= gravity * Time.deltaTime;
            _characterController.Move(_velocity * Time.deltaTime);
        }
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (_isGrounded)
        {
            _velocity.y = Mathf.Sqrt(jumpHeight * 2f * gravity);
            Debug.Log("JUMP");
        }
    }
}