using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [Header("이동 설정")]
    [SerializeField] private float _moveSpeed = 8f;

    [Header("점프 설정")]
    [SerializeField] private float _minJumpForce = 5f;
    [SerializeField] private float _maxJumpForce = 20f;
    [SerializeField] private float _chargeSpeed = 30f;

    [Header("점프 각도 설정")]
    [Range(15f, 75f)][SerializeField] private float _jumpAngle = 35f;

    [Header("차징 흔들림 설정")]
    [SerializeField] private Transform Transform_Visual;
    [SerializeField] private float _shakeAmount = 0.05f;
    [SerializeField] private float _shakeSpeed = 20f;

    [Header("지면 체크 설정")]
    [SerializeField] private Transform Transform_GroundCheck;
    [SerializeField] private float _checkRadius = 0.5f;
    [SerializeField] private LayerMask LayerMask_Ground;

    [Header("애니메이터")]
    [SerializeField] private EntityAnimatorController AnimatorController_Entity;

    private Rigidbody2D _rigidBody;
    private bool _isGrounded;
    private float _horizontalInput;
    private bool _lookRight = true;
    private float _currentJumpForce = 0f;
    private bool _isCharging = false;
    private float _jumpDirection;
    private Vector3 _originVisualLocalPos;

    public bool IsJumping { get; private set; }


    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
        _originVisualLocalPos = Transform_Visual.localPosition;
    }

    private void Update()
    {
        HandleMoveInput();
        HandleJumpInput();
        HandleFlip();
        HandleAnimationState();
    }

    private void FixedUpdate()
    {
        _isGrounded = Physics2D.OverlapCircle(Transform_GroundCheck.position, _checkRadius, LayerMask_Ground);

        if (_isGrounded && _rigidBody.linearVelocity.y <= 0.01f)
        {
            if (IsJumping)
            {
                _currentJumpForce = 0f;
            }
            IsJumping = false;
        }

        Move();
    }
    private void HandleMoveInput()
    {
        if (_isCharging == false)
        {
            _horizontalInput = Input.GetAxisRaw("Horizontal");
        }
        else
        {
            _horizontalInput = 0f;
        }
    }

    private void HandleJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded)
        {
            _rigidBody.linearVelocity = new Vector2(0f, _rigidBody.linearVelocity.y);
        }

        if (Input.GetKey(KeyCode.Space) && _isGrounded && !IsJumping)
        {
            _isCharging = true;
            _jumpDirection = _lookRight ? 1f : -1f;
            _currentJumpForce += _chargeSpeed * Time.deltaTime;
            _currentJumpForce = Mathf.Clamp(_currentJumpForce, _minJumpForce, _maxJumpForce);

            ChangePlayerState(EntityAnimState.Charge);

            if (_currentJumpForce >= _maxJumpForce)
            {
                ShakeVisual();
            }
        }

        if (Input.GetKeyUp(KeyCode.Space) && _isCharging)
        {
            Jump();
        }
    }

    private void HandleFlip()
    {
        if (_horizontalInput > 0 && !_lookRight)
        {
            Flip();
        }
        else if (_horizontalInput < 0 && _lookRight)
        {
            Flip();
        }
    }

    private void HandleAnimationState()
    {
        if (_isCharging == false && IsJumping == false)
        {
            bool isMoving = (_horizontalInput != 0);
            ChangePlayerState(isMoving ? EntityAnimState.Walk : EntityAnimState.Idle);

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                ChangePlayerState(EntityAnimState.Run);
            }
        }
    }

    private void Move()
    {
        if (_isGrounded && !_isCharging && !IsJumping)
        {
            _rigidBody.linearVelocity = new Vector2(_horizontalInput * _moveSpeed, _rigidBody.linearVelocity.y);
        }
    }

    private void Jump()
    {
        float radian = _jumpAngle * Mathf.Deg2Rad;
        float dirX = Mathf.Cos(radian) * _jumpDirection;
        float dirY = Mathf.Sin(radian);

        Vector2 jumpVelocity = new Vector2(dirX, dirY) * _currentJumpForce;

        IsJumping = true;

        _rigidBody.linearVelocity = Vector2.zero;
        _rigidBody.linearVelocity = jumpVelocity;

        ChangePlayerState(EntityAnimState.Jump);

        Transform_Visual.localPosition = _originVisualLocalPos;

        _isCharging = false;
    }
    private void ShakeVisual()
    {
        float shakeX = Mathf.Sin(Time.time * _shakeSpeed) * _shakeAmount;
        Transform_Visual.localPosition = _originVisualLocalPos + new Vector3(shakeX, 0f, 0f);
    }

    private void Flip()
    {
        _lookRight = !_lookRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    private void ChangePlayerState(EntityAnimState newState)
    {
        AnimatorController_Entity.SetState(newState);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") == false)
        {
            return;
        }

        var enemyComponent = collision.gameObject.GetComponent<DaniTech_2DEnemy>();
        if (enemyComponent == null)
        {
            Debug.Log($"충돌한 적 객체에서 컴포넌트를 찾을 수 없습니다 : {gameObject.name}");
            return;
        }

        DaniTechGameObjectManager.Inst.RequestDestroyEntityObject(enemyComponent.EntityInstancId);
    }
    private void OnDrawGizmos()
    {
        if (Transform_GroundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(Transform_GroundCheck.position, _checkRadius);
        }
    }
}