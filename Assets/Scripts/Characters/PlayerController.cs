using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using Unity.Cinemachine;

public class PlayerController : Movement, IPlayerControlled
{
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private CinemachineInputAxisController _cameraInputController;
    private Camera _mainCamera;
    private CursorLockMode _cursorLockMode;

    private InputSystem_Actions _inputs;
    private Vector2 _moveInput;
    private bool _isRunning = false;

    [SerializeField] private float _attackCooldown;
    private float _lastAttackTime;

    protected override void Awake()
    {
        _inputs = new InputSystem_Actions();
        base.Awake();
    }

    void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _mainCamera = Camera.main;
        _lastAttackTime = -_attackCooldown;
    }

    private void OnEnable()
    {
        _inputs.Player.Move.performed += OnMove;
        _inputs.Player.Move.canceled += OnMove;
        _inputs.Player.Sprint.performed += OnRun;
        _inputs.Player.Sprint.canceled += OnRun;
        _inputs.Player.Attack.performed += OnAttack;
        _inputs.Player.Attack.canceled += OnAttack;
        _inputs.Player.Cast.started += OnCastMagic;
        _inputs.Player.Cast.canceled += OnCastMagic;
        _inputs.Player.RotateCamera.performed += OnRotateCamera;
        _inputs.Player.RotateCamera.canceled += OnRotateCamera;

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
            Move(_moveInput);
            _target = null;
            StopMoving();
        }

        if (_target == null) return;

        float distanceToTarget = Vector3.Distance(transform.position, _target.position);

        if (distanceToTarget <= _attackRange)
        {
            transform.LookAt(_target);
            _agent.isStopped = true;

            if (Time.time - _lastAttackTime >= _attackCooldown)
            {
                Debug.Log("Attacking");
                _lastAttackTime = Time.time;
                AnimatorController?.PlayAttackAnimationByTrigger();
                AnimatorController?.PlayRunAnimation(false);
                AnimatorController?.PlayWalkAnimation(false);
            }
        }
        else
        {
            _agent.isStopped = false;
        }
    }

    public void Move(Vector2 direction)
    {
        if (_mainCamera != null)
        {
            Transform cameraTransform = _mainCamera.transform;
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;

            forward.y = 0;
            right.y = 0;

            forward.Normalize();
            right.Normalize();

            Vector3 moveDirection = forward * direction.y + right * direction.x;
            moveDirection.Normalize();

            float speed = _isRunning ? _runSpeed : _walkSpeed;
            _characterController.Move(moveDirection * speed * Time.deltaTime);

            if (direction.magnitude > 0)
            {
                transform.forward = moveDirection;
                AnimatorController?.PlayWalkAnimation(!_isRunning);
                AnimatorController?.PlayRunAnimation(_isRunning);
            }
            else
            {
                AnimatorController?.PlayWalkAnimation(false);
                AnimatorController?.PlayRunAnimation(false);
            }
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _moveInput = context.ReadValue<Vector2>();
            AnimatorController?.SetFloatToAnimation(_moveInput.x, _moveInput.y);
            _characterController.enabled = true;
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
        if (context.performed)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    _target = hit.collider.transform;
                    base.GoToTarget(_target, _runSpeed);
                    _characterController.enabled = false;
                    _agent.isStopped = false;
                    transform.LookAt(_target);
                }
            }
        }
    }

    public void OnCastMagic(InputAction.CallbackContext context)
    {
        Debug.Log("Cast!");
    }

    public void OnRotateCamera(InputAction.CallbackContext context)
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

    public override void StopMoving()
    {
        if (_agent == null) return;

        _agent.isStopped = true;
        _agent.ResetPath();
    }
}
