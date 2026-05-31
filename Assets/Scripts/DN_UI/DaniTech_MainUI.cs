using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class DaniTech_MainUI : DaniTechUIBase
{

    [SerializeField] private DaniTechUIButton Btn_OpenInventory;
    [SerializeField] private DaniTechUIButton Btn_OpenGameBook;
    [SerializeField] private DaniTechUIButton Btn_Pause;
    [SerializeField] private DaniTechUIButton Btn_Toggle;
    [SerializeField] private RectTransform RectTransform_ToggleArrow;
    [SerializeField] private RectTransform RectTransform_Buttons;                            
    [SerializeField] private float _hiddenY = 100f;             
    [SerializeField] private float _shownY = 0f;                


    private bool _isPaused = false;
    private bool _isOpen = true;

    private void OnEnable()
    {
        Btn_OpenInventory?.BindOnClickButtonEvent(OnClick_OpenInventory);
        Btn_OpenGameBook?.BindOnClickButtonEvent(OnClick_OpenGameBook);
        Btn_Pause?.BindOnClickButtonEvent(OnClick_Pause);
        Btn_Toggle?.BindOnClickButtonEvent(OnClick_Toggle);

    }

    private void OnDisable()
    {
        Btn_Pause?.UnBindOnClickButtonEvent(OnClick_Pause);
    }

    public void OnClick_Toggle()
    {
        _isOpen = !_isOpen;
        if (_isOpen == true)
        {
            RectTransform_ToggleArrow.DORotate(Vector3.zero, 0.2f);
            RectTransform_Buttons.DOAnchorPosY(_shownY, 0.3f).SetEase(Ease.OutBack);
        }
        else
        {
            RectTransform_ToggleArrow.DORotate(new Vector3(0f, 0f, 180f), 0.2f);
            RectTransform_Buttons.DOAnchorPosY(_hiddenY, 0.3f).SetEase(Ease.OutBack);
        }
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
