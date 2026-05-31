using DG.Tweening;
using UnityEngine;

public class SideUI : DaniTechUIBase
{

    [SerializeField] private DaniTechUIButton Btn_OpenSetting;
    [SerializeField] private DaniTechUIButton Btn_OpenMusic;
    [SerializeField] private DaniTechUIButton Btn_OpenSoundVolume;
    [SerializeField] private DaniTechUIButton Btn_OpenSave;
    [SerializeField] private DaniTechUIButton Btn_Toggle;        
    [SerializeField] private RectTransform RectTransform_Buttons; 
    [SerializeField] private RectTransform RectTransform_ToggleArrow;

    [SerializeField] private float _hiddenX = 200f;               
    [SerializeField] private float _shownX = 0f;

    private bool _isOpen = true;

    private void OnEnable()
    {
        RectTransform_ToggleArrow.localRotation = Quaternion.Euler(0f, 0f, 90f);

        Btn_OpenSetting?.BindOnClickButtonEvent(OnClick_OpenSetting);
        Btn_OpenMusic?.BindOnClickButtonEvent(OnClick_OpenMusic);
        Btn_OpenSoundVolume?.BindOnClickButtonEvent(OnClick_OpenSoundVolume);
        Btn_OpenSave?.BindOnClickButtonEvent(OnClick_OpenSave);
        Btn_Toggle?.BindOnClickButtonEvent(OnClick_Toggle);
    }
    private void OnDisable()
    {
        Btn_OpenSetting?.UnBindOnClickButtonEvent(OnClick_OpenSetting);
        Btn_OpenMusic?.UnBindOnClickButtonEvent(OnClick_OpenMusic);
        Btn_OpenSoundVolume?.UnBindOnClickButtonEvent(OnClick_OpenSoundVolume);
        Btn_OpenSave?.UnBindOnClickButtonEvent(OnClick_OpenSave);
    }

    public void OnClick_Toggle() 
    {
        _isOpen = !_isOpen;
        if (_isOpen == true)
        {
            RectTransform_ToggleArrow.DORotate(new Vector3(0f, 0f, 90f), 0.2f);
            RectTransform_Buttons.DOAnchorPosX(_shownX, 0.3f).SetEase(Ease.OutBack);
        }
        else
        {
            RectTransform_ToggleArrow.DORotate(new Vector3(0f, 0f, -90f), 0.2f);
            RectTransform_Buttons.DOAnchorPosX(_hiddenX, 0.3f).SetEase(Ease.OutBack);
        }
    }
    public void OnClick_OpenSetting()
    {
        if (DaniTechUIManager.Instance != null)
        {
            DaniTechUIManager.Instance.OpenSetting();
        }
    }

    public void OnClick_OpenMusic()
    {
        if (DaniTechUIManager.Instance != null)
        {
            DaniTechUIManager.Instance.OpenMusicVolume();
        }
    }

    public void OnClick_OpenSoundVolume()
    {
        if (DaniTechUIManager.Instance != null)
        {
            DaniTechUIManager.Instance.OpenSound();
        }
    }

    public void OnClick_OpenSave()
    {
        if (DaniTechUIManager.Instance != null)
        {
            DaniTechUIManager.Instance.OpenSave();
        }
    }
}




