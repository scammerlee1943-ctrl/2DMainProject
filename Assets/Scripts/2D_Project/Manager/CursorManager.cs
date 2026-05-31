using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField] private RectTransform RectTransform_Cursor;
    [SerializeField] private Animator Animator_Cursor;

    public static CursorManager Inst { get; set; }

    private void Awake()
    {
        Inst = this;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None;
    }

    private void Update()
    {
        MoveCursor();
        CheckMouseClick();
    }

    private void MoveCursor()
    {
        if (RectTransform_Cursor == null) return;
        RectTransform_Cursor.position = Input.mousePosition;
    }
    public void SetHover(bool isHover)
    {
        if (Animator_Cursor == null) return;
        Animator_Cursor.SetBool("IsHover", isHover);
    }
    private void CheckMouseClick()
    {
        if (Animator_Cursor == null) return;

        if (Input.GetMouseButtonDown(0))
        {
            Animator_Cursor.SetBool("IsClick", true);
        }

        if (Input.GetMouseButtonUp(0))
        {
            Animator_Cursor.SetBool("IsClick", false);
        }
    }
}