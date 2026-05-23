using UnityEngine;

public class MusicSelectUI : DaniTechUIBase
{
    [SerializeField] private DaniTechUIButton Button_Music_1;
    [SerializeField] private DaniTechUIButton Button_Music_2;

    [SerializeField] private DaniTechUIButton Button_Close;

    private void OnEnable()
    {
        Button_Music_1?.BindOnClickButtonEvent(OnClick_PlayMusic_1);
        Button_Music_2?.BindOnClickButtonEvent(OnClick_PlayMusic_2);

        Button_Music_1?.BindOnHoverButtonEvent(OnHover_RandomMusicButton);
        Button_Music_2?.BindOnHoverButtonEvent(OnHover_RandomMusicButton);

        Button_Close?.BindOnClickButtonEvent(OnClick_CloseUI);

        Button_Close?.BindOnHoverButtonEvent(OnHover_RandomMusicButton);
    }

    private void OnClick_PlayMusic_1()
    {
        if (DaniTechSoundManager.Inst != null)
        {
            DaniTechSoundManager.Inst.PlayBGM("BGM_Main_01");
            Debug.Log("1번 배경음악 재생!");
        }
    }
    private void OnClick_PlayMusic_2()
    {
        if (DaniTechSoundManager.Inst != null)
        {
            DaniTechSoundManager.Inst.PlayBGM("BGM_Main_02");
            Debug.Log("2번 배경음악 재생!");
        }
    }

    private void OnClick_CloseUI()
    {
        if(DaniTechUIManager.Instance != null)
        {
            DaniTechUIManager.Instance.CloseContentUI(DaniTechUIType.MusicSelectUI);
        }
    }
    private void OnHover_RandomMusicButton()
    {
        int randomIndex = Random.Range(1, 3);
        string randomKey = $"SFX_UI_Hover_{randomIndex}";

        if (DaniTechSoundManager.Inst != null)
        {
            DaniTechSoundManager.Inst.PlaySFX(randomKey);
        }
    }
}
