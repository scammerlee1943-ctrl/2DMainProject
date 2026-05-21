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
    public void OnClick_OpenSetting()
    {
        DaniTechUIManager.Instance.OpenSetting();
    }

    public void OnClick_OpenMusic()
    {
        DaniTechUIManager.Instance.OpenMusicVolume();
    }

    public void OnClick_OpenSoundVolume()
    {
        DaniTechUIManager.Instance.OpenSound();
    }

    public void OnClick_OpenSave()
    {
        DaniTechUIManager.Instance.OpenSave();
    }
}




