using UnityEngine;

public class SideUI : DaniTechUIBase
{

    [SerializeField] private DaniTechUIButton Btn_OpenSetting;
    [SerializeField] private DaniTechUIButton Btn_OpenMusic;
    [SerializeField] private DaniTechUIButton Btn_OpenSoundVolume;
    [SerializeField] private DaniTechUIButton Btn_OpenSave;

    private void OnEnable()
    {
        Btn_OpenSetting?.BindOnClickButtonEvent(OnClick_OpenSetting);
        Btn_OpenMusic?.BindOnClickButtonEvent(OnClick_OpenMusic);
        Btn_OpenSoundVolume?.BindOnClickButtonEvent(OnClick_OpenSoundVolume);
        Btn_OpenSave?.BindOnClickButtonEvent(OnClick_OpenSave);
    }
    private void OnDisable()
    {
        Btn_OpenSetting?.UnBindOnClickButtonEvent(OnClick_OpenSetting);
        Btn_OpenMusic?.UnBindOnClickButtonEvent(OnClick_OpenMusic);
        Btn_OpenSoundVolume?.UnBindOnClickButtonEvent(OnClick_OpenSoundVolume);
        Btn_OpenSave?.UnBindOnClickButtonEvent(OnClick_OpenSave);
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




