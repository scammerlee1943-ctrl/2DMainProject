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

        Btn_OpenSetting?.BindOnHoverButtonEvent(OnHover_RandomSideButton);
        Btn_OpenMusic?.BindOnHoverButtonEvent(OnHover_RandomSideButton);
        Btn_OpenSoundVolume?.BindOnHoverButtonEvent(OnHover_RandomSideButton);
        Btn_OpenSave?.BindOnHoverButtonEvent(OnHover_RandomSideButton);
    }
    private void OnHover_RandomSideButton()
    {
        int randomIndex = Random.Range(1, 3);

        string randomKey = $"SFX_UI_Hover_{randomIndex}";

        if (DaniTechSoundManager.Inst != null)
        {
            DaniTechSoundManager.Inst.PlaySFX(randomKey);
        }
        else
        {
            Debug.LogWarning("사운드 매니저를 찾을 수 없습니다!!");
        }
    }
    public void OnClick_OpenSetting()
    {
        //DaniTechSoundManager.Inst.PlaySFX();
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




