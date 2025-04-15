using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerJump : AnimatorController
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
        _inputs.Player.Jump.started += OnJump;
        _inputs.Enable();
    }

    private void OnDisable()
    {
        _inputs.Disable();
    }

    void Update()
    {
        if (_characterController.enabled)
        {

            if (_characterController.isGrounded && _velocity.y <= 0)
            {
                _velocity.y = -2f;
            }

            _velocity.y -= gravity * Time.deltaTime;
            _characterController.Move(_velocity * Time.deltaTime);
        }

        _isGrounded = _characterController.isGrounded;
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (_isGrounded)
        {
            _velocity.y = Mathf.Sqrt(jumpHeight * 2f * gravity);
            PlayJumpAnimation();
        }
    }
}