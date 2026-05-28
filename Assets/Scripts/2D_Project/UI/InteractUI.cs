using UnityEngine;
using UnityEngine.UI;

public class InteractUI : DaniTechUIBase
{
    [SerializeField] private Text Text_InputDescription;
    [SerializeField] private Animator Animator_AKey;
    [SerializeField] private Animator Animator_DKey;
    [SerializeField] private Animator Animator_LeftKey;
    [SerializeField] private Animator Animator_RightKey;
    [SerializeField] private Animator Animator_SpaceKey;

    private void OnEnable()
    {
        if (DaniTechGameObjectManager.Inst == null) return;
        var player = DaniTechGameObjectManager.Inst.GetLocalPlayer();
        if (player == null) return;
        player.OnInputChanged += UpdateInputDisplay;
    }

    private void OnDisable()
    {
        var player = DaniTechGameObjectManager.Inst.GetLocalPlayer();
        if (player == null) return;
        player.OnInputChanged -= UpdateInputDisplay;
    }

    public void UpdateInputDisplay(bool isLeft, bool isRight, bool isCharging)
    {
        Animator_AKey.SetBool("IsPressed", isLeft);
        Animator_LeftKey.SetBool("IsPressed", isLeft);
        Animator_DKey.SetBool("IsPressed", isRight);
        Animator_RightKey.SetBool("IsPressed", isRight);
        Animator_SpaceKey.SetBool("IsPressed", isCharging);

        if (isLeft && isCharging)
            Text_InputDescription.text = "왼쪽 점프 충전 중";
        else if (isRight && isCharging)
            Text_InputDescription.text = "오른쪽 점프 충전 중";
        else if (isLeft)
            Text_InputDescription.text = "왼쪽 이동 중";
        else if (isRight)
            Text_InputDescription.text = "오른쪽 이동 중";
        else if (isCharging)
            Text_InputDescription.text = "점프 충전 중";
        else
            Text_InputDescription.text = "";
    }
}