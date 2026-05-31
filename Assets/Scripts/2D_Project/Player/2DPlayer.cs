using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("이동 설정")]
    [SerializeField] private float _moveSpeed = 4f;
    [SerializeField] private float _minJumpHorizontalForce = 5f;
    [SerializeField] private float _maxJumpHorizontalForce = 25f;

    [Header("점프 설정")]
    [SerializeField] private float _minJumpForce = 3f;
    [SerializeField] private float _maxJumpForce = 12f;
    [SerializeField] private float _chargeSpeed = 15f;

    [Header("물리 설정")]
    [SerializeField] private float _terminalVelocity = 20f;

    [Header("차징 흔들림 설정")]
    [SerializeField] private Transform Transform_Visual;
    [SerializeField] private float _shakeAmount = 0.05f;
    [SerializeField] private float _shakeSpeed = 20f;

    [Header("지면 체크 설정")]
    [SerializeField] private Transform Transform_GroundCheck;
    [SerializeField] private float _checkRadius = 0.5f;
    [SerializeField] private LayerMask LayerMask_Ground;
    [SerializeField] private float _fallThreshold = 3f;

    public event Action<EntityAnimState> OnStateChanged;
    public event Action<float, float> OnHeightChanged;
    public event Action<bool, bool, bool> OnInputChanged;

    private Rigidbody2D _rigidBody;
    private float _horizontalInput;
    private float _currentJumpForce = 0f;
    private float _jumpDirection;
    private float _maxJumpY;
    private float _allTimeMaxHeight;

    private bool _isGrounded;
    private bool _lookRight = true;
    private bool _isCharging = false;
    private bool _isJumping;
    private bool _isFallingFromWall = false;
    private bool _isFallen = false;
    private bool _prevIsLeft;
    private bool _prevIsRight;
    private bool _prevIsCharging;

    private Vector3 _originVisualLocalPos;
    private Vector2 _previousVelocity;


    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
        _originVisualLocalPos = Transform_Visual.localPosition;
    }
    private void Start()
    {
        DaniTechGameObjectManager.Inst.RegisterLocalPlayer(this);
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
        // 지면 체크
        _isGrounded = Physics2D.OverlapCircle(Transform_GroundCheck.position, _checkRadius, LayerMask_Ground);

        // 최대 낙하 속도 제한
        if (_rigidBody.linearVelocity.y < -_terminalVelocity)
        {
            _rigidBody.linearVelocity = new Vector2(_rigidBody.linearVelocity.x, -_terminalVelocity);
        }

        // 점프 중 최고 높이 갱신
        if (_isJumping && transform.position.y > _maxJumpY)
        {
            _maxJumpY = transform.position.y;
        }

        // 전체 최고 높이는 별도로 갱신
        if (transform.position.y > _allTimeMaxHeight)
        {
            _allTimeMaxHeight = transform.position.y;
        }
        OnHeightChanged?.Invoke(transform.position.y, _allTimeMaxHeight);

        // 착지 처리
        if (_isGrounded && _rigidBody.linearVelocity.y <= 0.01f)
        {
            _isFallingFromWall = false;

            if (_isJumping)
            {
                _currentJumpForce = 0f;
                float fallDistance = _maxJumpY - transform.position.y;
                if (fallDistance > _fallThreshold)
                {
                    _isFallen = true;
                    DaniTechSoundManager.Inst.PlaySFX("sfx_fallen", 0.15f);
                    ChangePlayerState(EntityAnimState.Fallen);
                }
            }
            _isJumping = false;
        }

        _previousVelocity = _rigidBody.linearVelocity;
        Move();
    }

    private void HandleMoveInput()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        // 벽에 부딪혀 낙하 중이면 이동 입력 차단
        if (_isFallingFromWall)
        {
            _horizontalInput = 0f;
            return;
        }

        if (_isFallen == true)
        {
            if (_horizontalInput != 0)
            {
                _isFallen = false;
                ChangePlayerState(EntityAnimState.Idle);
            }
            _horizontalInput = 0f;
            return;
        }
        bool isLeft = _horizontalInput < 0;
        bool isRight = _horizontalInput > 0;

        if (isLeft != _prevIsLeft || isRight != _prevIsRight || _isCharging != _prevIsCharging)
        {
            _prevIsLeft = isLeft;
            _prevIsRight = isRight;
            _prevIsCharging = _isCharging;
            OnInputChanged?.Invoke(isLeft, isRight, _isCharging);
        }
    }

    private void HandleJumpInput()
    {
        // 스페이스 누르는 순간 충전 시작
        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded)
        {
            _isFallen = false;
            _isCharging = true;
            OnInputChanged?.Invoke(_horizontalInput < 0, _horizontalInput > 0, _isCharging);
            _currentJumpForce = _minJumpForce;
            _rigidBody.linearVelocity = new Vector2(0f, _rigidBody.linearVelocity.y);
        }

        // 스페이스 누르는 동안 충전
        if (Input.GetKey(KeyCode.Space) && _isGrounded && !_isJumping)
        {
            _currentJumpForce += _chargeSpeed * Time.deltaTime;
            _currentJumpForce = Mathf.Clamp(_currentJumpForce, _minJumpForce, _maxJumpForce);

            ChangePlayerState(EntityAnimState.Charge);

            if (_currentJumpForce >= _maxJumpForce)
            {
                ShakeVisual();
            }
        }

        // 스페이스 떼는 순간 점프 실행
        if (Input.GetKeyUp(KeyCode.Space) && _isCharging)
        {
            GetJumpDirection();
            Jump();
            OnInputChanged?.Invoke(_horizontalInput < 0, _horizontalInput > 0, false);
        }
    }
    private void GetJumpDirection()
    {
        if (_horizontalInput == 0)
        {
            _jumpDirection = 0f;
        }

        else if (_lookRight)
        {
            _jumpDirection = 1f;
        }

        else
        {
            _jumpDirection = -1f;
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
        if (_isCharging == false && _isJumping == false && _isFallen == false)
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
        // 지상에서만 이동 가능 (충전 중, 점프 중 제외)
        if (_isGrounded && !_isCharging && !_isJumping)
        {
            _rigidBody.linearVelocity = new Vector2(_horizontalInput * _moveSpeed, _rigidBody.linearVelocity.y);
        }
    }

    private void Jump()
    {
        _maxJumpY = transform.position.y;
        float chargeRatio = (_currentJumpForce - _minJumpForce) / (_maxJumpForce - _minJumpForce);
        float horizontalForce = 0f;


        if (_jumpDirection != 0)
        {
            horizontalForce = Mathf.Lerp(_minJumpHorizontalForce, _maxJumpHorizontalForce, chargeRatio);
        }

        else
        {
            horizontalForce = 0f;
        }

        _rigidBody.linearVelocity = new Vector2(horizontalForce * _jumpDirection, _currentJumpForce);

        _isJumping = true;
        _isCharging = false;

        DaniTechSoundManager.Inst.PlaySFX("sfx_jump", 0.4f);
        ChangePlayerState(EntityAnimState.Jump);
        Transform_Visual.localPosition = _originVisualLocalPos;
    }

    private void ShakeVisual()
    {
        // 풀 충전 시 비주얼 흔들림
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
        OnStateChanged?.Invoke(newState);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (_isJumping)
        {
            Vector2 normal = collision.contacts[0].normal;

            bool isWall = Mathf.Abs(normal.x) > Mathf.Abs(normal.y);
            bool isCeiling = normal.y < -0.5f;

            if (isWall)
            {
                BounceOffWall(normal);
            }

            else if (isCeiling)
            {
                // 천장 충돌 시 수직 속도 절반으로 반사
                _rigidBody.linearVelocity = new Vector2(_rigidBody.linearVelocity.x,-_rigidBody.linearVelocity.y / 2f);
            }
        }
    }

    private void BounceOffWall(Vector2 normal)
    {
        _isFallingFromWall = true;
        _rigidBody.linearVelocity = new Vector2(-_previousVelocity.x / 2f, _previousVelocity.y);

        // 벽 방향으로 바라보기
        bool shouldLookRight = normal.x < 0;
        if (_lookRight != shouldLookRight) Flip();

        ChangePlayerState(EntityAnimState.Stun);
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