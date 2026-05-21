using UnityEngine;

public class DaniTech_MainUI : DaniTechUIBase
{

    [SerializeField] private DaniTechUIButton Btn_OpenInventory;
    [SerializeField] private DaniTechUIButton Btn_OpenGameBook;
    [SerializeField] private DaniTechUIButton Btn_Pause;

    private bool _isPaused = false;

    private void OnEnable()
    {
        Btn_OpenInventory?.BindOnClickButtonEvent(OnClick_OpenInventory);
        Btn_OpenGameBook?.BindOnClickButtonEvent(OnClick_OpenGameBook);
        Btn_Pause?.BindOnClickButtonEvent(OnClick_Pause);

    }

    private void OnDisable()
    {
        Btn_Pause?.UnBindOnClickButtonEvent(OnClick_Pause);
    }

    public void OnClick_OpenGameBook()
    {
        DaniTechUIManager.Instance.OpenContentUI(DaniTechUIType.GameBookUI);
    }
    public void OnClick_OpenInventory()
    {
        DaniTechUIManager.Instance.OpenInventoryPopup();
        DaniTechGameManager.Inst.SaveData();
    }
    public void OnClick_Pause()
    {
        if (_isPaused)
        {
            _isPaused = false;
            Time.timeScale = 1f;
        }
        else
        {
            _isPaused = true;
            Time.timeScale = 0f;
        }

    }
}
