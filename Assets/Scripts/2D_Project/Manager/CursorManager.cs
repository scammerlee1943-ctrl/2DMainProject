using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField] private RectTransform RectTransform_Cursor;
    [SerializeField] private Animator Animator_Cursor;

    private void Awake()
    {
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