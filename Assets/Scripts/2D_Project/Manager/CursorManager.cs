using UnityEngine;

public enum CursorAnimState
{
    None = 0,
    Idle,
    ClickDown,
    ClickUp,
    Hover
}


public class CursorManager : MonoBehaviour
{
    [SerializeField] private RectTransform RectTransform_Cursor;
    [SerializeField] private Animator Animator_Cursor;

    private CursorAnimState _currentAnimState;

    private void Awake()
    {
        Cursor.visible = false;
        //진짜 커서를 숨기기
    }

    private void Update()
    {
        MoveCursor();
        CheckMouseClick();
    }

    private void MoveCursor()
    {
        RectTransform_Cursor.position = Input.mousePosition;
    }

    private void CheckMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SetState(CursorAnimState.ClickDown);
        }
        if(Input.GetMouseButtonUp(0))
        {
            SetState(CursorAnimState.ClickUp);
        }
    }

    private void SetState(CursorAnimState newstate)
    {
        _currentAnimState = newstate;
        switch (_currentAnimState)
        {
            case CursorAnimState.Idle:
                break;
            case CursorAnimState.ClickDown:
                Animator_Cursor.SetTrigger("ClickDown");
                break;
            case CursorAnimState.ClickUp:
                Animator_Cursor.SetTrigger("ClickUp");
                break;
            case CursorAnimState.Hover:
                Animator_Cursor.SetTrigger("Hover");
                break;
        }
    }
}
