using System;
using UnityEngine;

public class SelectionButton : MonoBehaviour
{
    [SerializeField] private DaniTechUIButton Button_Self;

    // 이 선택지를 누르면 이어질 다이얼로그 Id
    private string _targetDialogueId;
    private event Action<string> OnSelectEvent;

    private void OnEnable()
    {
        Button_Self.BindOnClickButtonEvent(OnClick_Select);
    }

    private void OnDisable()
    {
        OnSelectEvent = null;
    }

    // 부모(다이얼로그 UI)가 이 선택지를 세팅할 때 호출
    public void InitSelection(string selectionName, string targetDialogueId)
    {
        _targetDialogueId = targetDialogueId;
        Button_Self.ChangeButtonText(selectionName);
        Button_Self.SetUseClickAnimation(false);
    }

    // 부모가 콜백을 등록한다
    public void BindSelectEvent(Action<string> onSelectEvent)
    {
        OnSelectEvent = onSelectEvent;
    }

    private void OnClick_Select()
    {
        OnSelectEvent?.Invoke(_targetDialogueId);
    }
}