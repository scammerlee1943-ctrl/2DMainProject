using Unity.Collections;
using UnityEditor.Build.Pipeline;
using UnityEngine;

// +) 어떤 컴포넌트가 필수로 필요하다는 것을 강제할 수 있다
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

    [Header("지면 체크 설정")]
    [SerializeField] private Transform _groundCheck;    // 발 밑에 배치할 빈 오브젝트
    [SerializeField] private float _checkRadius = 0.5f; // 체크 범위
    [SerializeField] private LayerMask _groundLayer;    // 지면으로 인식할 레이어 (Platforms 등)

    [Header("애니메이터")]
    [SerializeField] private AnimatorController AnimatorController_Entity;

    private Rigidbody2D _rigidBody;
    private bool _isGrounded;
    private float _horizontalInput;
    private bool _lookRight = true;
    private float _currentJumpForce = 0f;
    private bool _canJump = true;
    private bool _isCharging = false;
    private float _jumpDirection;
    private bool _isJumping = false;

    void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();

        // 2D 캐릭터가 물리 충돌 시 회전해서 넘어지는 것 방지
        _rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }



    void Update()
    {
        if(_isCharging == false)
        {
            // 1. 입력 받기 (Update에서 수행)
            _horizontalInput = Input.GetAxisRaw("Horizontal");
        }
        else
        {
            _horizontalInput = 0f;
        }

        // 2. 점프 입력
        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded && _canJump)
        {
            _rigidBody.linearVelocity = new Vector2(0f, _rigidBody.linearVelocity.y);
        }

        if (Input.GetKey(KeyCode.Space) && _isGrounded && _canJump)
        {
            _isCharging = true;
            _jumpDirection = _lookRight ? 1f : -1f;
            _currentJumpForce += _chargeSpeed * Time.deltaTime;
            _currentJumpForce = Mathf.Clamp(_currentJumpForce, _minJumpForce, _maxJumpForce);

            ChangePlayerState(EntityAnimState.Charge);
        }
        if(Input.GetKeyUp(KeyCode.Space) && _isCharging)
        {
            _canJump = true;
            Jump();

        }


        // 3. 캐릭터 방향 전환 (Flip)
        if (_horizontalInput > 0 && !_lookRight)
        {
            Flip();
        }
        else if (_horizontalInput < 0 && _lookRight)
        {
            Flip();
        }

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

    private void ChangePlayerState(EntityAnimState newState)
    {
        // 이런 곳에 UI나 플레이어의 별도 처리를 넣어줄 수도 있다


        // 우선 애니메이션만 바꿔 봅시다
        AnimatorController_Entity.SetState(newState);
    }

    void FixedUpdate()
    {
        _isGrounded = Physics2D.OverlapCircle(_groundCheck.position, _checkRadius, _groundLayer);
        if (_isGrounded && _rigidBody.linearVelocity.y <= 0.01f)
        {
            _isJumping = false;
        }
        Move();
    }

    void Move()
    {
        if (_isGrounded && !_isCharging && !_isJumping)
        {
            // Y축 속도는 유지하면서 X축 속도만 변경 (관성 유지)
        _rigidBody.linearVelocity = new Vector2(_horizontalInput * _moveSpeed, _rigidBody.linearVelocity.y);
        }
    }

    void Jump()
    {
        float radian = _jumpAngle * Mathf.Deg2Rad;

        float dirX = Mathf.Cos(radian) * _jumpDirection;
        float dirY = Mathf.Sin(radian);

        Vector2 jumpVelocity = new Vector2(dirX, dirY) * _currentJumpForce;

        _isJumping = true;

        _rigidBody.linearVelocity = Vector2.zero;
        _rigidBody.linearVelocity = jumpVelocity;

        ChangePlayerState(EntityAnimState.Jump);

        _isCharging = false;
        _canJump = false;
        Invoke("ResetJump", 0.2f);
    }
    void ResetJump()
    {
        _canJump = true;
        _currentJumpForce = 0;
    }

    void Flip()
    {
        _lookRight = !_lookRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    // 에디터 뷰에서 지면 체크 범위를 시각적으로 확인
    private void OnDrawGizmos()
    {
        if (_groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_groundCheck.position, _checkRadius);
        }
    }

    // 6) 적 충돌 시 처리를 해보자
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 6-1) 플레이어의 > 콜리전에 충돌한 객체가 어떤 Tag인지 1차 검사한다.
        // 지면 같은 오브젝트와 점프시 충돌이 계속 오므로 이렇게 태그로 먼저 비교하는게 좋다
        // 중단점을 찍어보면서 확인 추천
        if (collision.gameObject.CompareTag("Enemy") == false)
        {
            return;
        }

        // 6-2) 충돌한 몬스터의 정보를 받아오려고 시도해보자
        var enemyComponent = collision.gameObject.GetComponent<DaniTech_2DEnemy>();
        if (enemyComponent == null)
        {
            Debug.Log($"충돌한 적 객체에서 컴포넌트를 찾을 수 없습니다 : {gameObject.name}");
            return;
        }

        // 6-3) 충돌된 오브젝트를 플레이어가 직접 제거하는게 아니라, Id로 게임오브젝트매니저한테 삭제를 요청한다
        DaniTechGameObjectManager.Inst.RequestDestroyEntityObject(enemyComponent.EntityInstancId);

    }
}
