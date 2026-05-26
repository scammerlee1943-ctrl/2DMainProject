using UnityEngine;


public enum EntityAnimState
{
    None = 0,
    Idle,
    Walk,
    Run,
    Jump,
    Stun,
    Charge,
    Fallen

}

public class EntityAnimatorController : MonoBehaviour
{

    [SerializeField] private Animator Animator_Entity;
    private EntityAnimState _currentAnimState;
    private Player _player;

    private void Awake()
    {
        _player = GetComponent<Player>();
        if(_player == null)
        {
            Debug.LogWarning($"Player 컴포넌트를 찾을 수 없습니다 : {gameObject.name}");
            return;
        }
        _player.OnStateChanged += SetState;
    }

    private void OnDestroy()
    {
        if (_player == null) return;
        _player.OnStateChanged -= SetState;
    }
    // 외부에서 쉽게 변경을 요청하려고
    // 이 상태에 따른 애니메이션 재생을 여기서만 모아서 해줄려고
    public void SetState(EntityAnimState newState) // 새로운 상태
    {
        if (newState == _currentAnimState)
        {
            return;
        }

        //비교를 했는데, 같은 값이 아니고, 이제 동작을 바꿔도 된다면 이렇게 대입
        _currentAnimState = newState;
        ResetAllAnimParameters();

        switch (_currentAnimState)
        {
            case EntityAnimState.Idle:
                break;
            case EntityAnimState.Walk:
                Animator_Entity.SetBool("IsWalk", true);
                break;
            case EntityAnimState.Run:
                Animator_Entity.SetBool("IsRun", true);
                break;
            case EntityAnimState.Jump:
                Animator_Entity.SetBool("IsJump", true);
                break;
            case EntityAnimState.Charge:
                Animator_Entity.SetBool("IsCharge", true);
                break;
            case EntityAnimState.Stun:
                Animator_Entity.SetBool("IsStun", true);
                break;
            case EntityAnimState.Fallen:
                Debug.Log("Fallen 상태 진입!");
                Animator_Entity.SetBool("IsFallen", true);
                break;
            default:
                break;
        }
    }

    private void ResetAllAnimParameters()
    {
        Animator_Entity.SetBool("IsWalk", false);
        Animator_Entity.SetBool("IsRun", false);
        Animator_Entity.SetBool("IsJump", false);
        Animator_Entity.SetBool("IsCharge", false);
        Animator_Entity.SetBool("IsStun", false);
        Animator_Entity.SetBool("IsFallen", false);
    }
}