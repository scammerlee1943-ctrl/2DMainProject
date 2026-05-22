using DG.Tweening.Core.Easing;
using System;
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

    public event Action<EntityAnimState> OnStateChanged;

    private Rigidbody2D _rigidBody;
    private float _horizontalInput;
    private float _currentJumpForce = 0f;
    private float _jumpDirection;

    private bool _isGrounded;
    private bool _lookRight = true;
    private bool _isCharging = false;
    private bool _isJumping;
    private bool _isWallBouncing = false;

    private Vector3 _originVisualLocalPos;



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
            if (_isWallBouncing)
            {
                _isWallBouncing = false;
                CancelInvoke(nameof(ResetWallBouncing));
            }
            if (_isJumping)
            {
                _currentJumpForce = _minJumpForce;
            }
            _isJumping = false;
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
            _isCharging = true;
            GetJumpDirection();
            _rigidBody.linearVelocity = new Vector2(0f, _rigidBody.linearVelocity.y);
        }

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

        if (Input.GetKeyUp(KeyCode.Space) && _isCharging)
        {
            Jump();
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
        if (_isCharging == false && _isJumping == false)
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
        if (_isGrounded && !_isCharging && !_isJumping)
        {
            _rigidBody.linearVelocity = new Vector2(_horizontalInput * _moveSpeed, _rigidBody.linearVelocity.y);
        }

        else if (_isWallBouncing)
        {
            float airFriction = 0.95f;
            _rigidBody.linearVelocity = new Vector2(_rigidBody.linearVelocity.x * airFriction, _rigidBody.linearVelocity.y);
        }
    }

    private void Jump()
    {
        float radian = _jumpAngle * Mathf.Deg2Rad;
        float dirX = Mathf.Cos(radian) * _jumpDirection;
        float dirY = Mathf.Sin(radian);

        Vector2 jumpVelocity = new Vector2(dirX, dirY) * _currentJumpForce;

        _isJumping = true;

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
        OnStateChanged?.Invoke(newState);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (_isJumping)
        {
            Vector2 normal = collision.contacts[0].normal;

            bool isWall = Mathf.Abs(normal.x) > Mathf.Abs(normal.y);

            if (isWall)
            {
                BounceOffWall(normal);
            }

        }
    }

    private void BounceOffWall(Vector2 normal)
    {
        _isJumping = true;
        _isWallBouncing = true;

        float bounceX = normal.x * (_minJumpForce * 0.7f);
        float bounceY = _minJumpForce * 0.6f;

        _rigidBody.linearVelocity = new Vector2(bounceX, bounceY);
        _currentJumpForce = _minJumpForce;

        bool shouldLookRight = normal.x > 0;
        if (_lookRight != shouldLookRight)
        {
            Flip();
        }

        Invoke(nameof(ResetWallBouncing), 0.15f);
        CancelInvoke(nameof(ResetWallBouncing));
    }

    private void ResetWallBouncing()
    {
        _isWallBouncing = false;
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