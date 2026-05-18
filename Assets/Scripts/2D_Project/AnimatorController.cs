using UnityEngine;


public enum EntityAnimState
{
    None = 0,
    Idle,
    Walk,
    Run,
    Jump,
    Charge

}

public class AnimatorController : MonoBehaviour
{
    [SerializeField] private Animator Animator_Entity;

    private EntityAnimState _currentAnimState;

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
    }
}