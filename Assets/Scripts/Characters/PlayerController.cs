using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, IPlayerControlled
{
    [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private float _runSpeed = 6f;
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private IAnimatorController _animatorController;

    private InputSystem_Actions _inputs;

    private Vector2 _moveInput;
    private bool _isRunning = false;

    void Awake()
    {
        _inputs = new InputSystem_Actions();
    }

    void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _animatorController = GetComponent<IAnimatorController>();
    }

    private void OnEnable()
    {
        _inputs.Player.Move.performed += OnMove;
        _inputs.Player.Move.canceled += OnMove;
        _inputs.Player.Sprint.performed += OnRun;
        _inputs.Player.Sprint.canceled += OnRun;
        _inputs.Player.Attack.started += OnAttack;
        _inputs.Player.Attack.canceled += OnAttack;
        _inputs.Enable();
    }

    private void OnDisable()
    {
        _inputs.Disable();
    }

    void Update()
    {
        Move(_moveInput);
    }

    public void Move(Vector2 direction)
    {
        float speed = _isRunning ? _runSpeed : _moveSpeed;
        Vector3 move = new Vector3(direction.x, 0, direction.y);

        _characterController.Move(move * speed * Time.deltaTime);

        if (direction.magnitude > 0)
        {
            if (!_isRunning)
            {
                _animatorController.PlayWalkAnimation(true);
                _animatorController.PlayRunAnimation(_isRunning);
            }
            else
            {
                _animatorController.PlayWalkAnimation(false);
                _animatorController.PlayRunAnimation(_isRunning);
            }
        }
        else
        {
            _animatorController.PlayWalkAnimation(false);
            _animatorController.PlayRunAnimation(false);
        }
    }

    public void GoToTarget(Transform target, float speed)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Move(new Vector2(direction.x, direction.z));
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _moveInput = context.ReadValue<Vector2>();
            _animatorController.SetFloatToAnimation(_moveInput.x, _moveInput.y);
        }
        else if (context.canceled)
        {
            _moveInput = Vector2.zero;
        }
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        _isRunning = context.performed;

        if (context.canceled)
        {
            _isRunning = false;
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        //логика атаки
        Debug.Log("Attack");
        _animatorController.PlayAttackAnimation();
    }
}