using UnityEngine;

public class PausePopupUI : DaniTechUIBase
{
    [SerializeField] private DaniTechUIButton Btn_Resume;
    [SerializeField] private DaniTechUIButton Btn_SaveAndLoad;
    [SerializeField] private DaniTechUIButton Btn_ExitToTitle;
    [SerializeField] private DaniTechUIButton Btn_QuitGame;

    private void OnEnable()
    {
        Btn_Resume?.BindOnClickButtonEvent(OnClick_Resume);
        Btn_SaveAndLoad?.BindOnClickButtonEvent(OnClick_SaveAndLoad);
        Btn_ExitToTitle?.BindOnClickButtonEvent(OnClick_ExitToTitle);
        Btn_QuitGame?.BindOnClickButtonEvent(OnClick_QuitGame);
    }

    private void OnDisable()
    {
        Btn_Resume?.UnBindOnClickButtonEvent(OnClick_Resume);
        Btn_SaveAndLoad?.UnBindOnClickButtonEvent(OnClick_SaveAndLoad);
        Btn_ExitToTitle?.UnBindOnClickButtonEvent(OnClick_ExitToTitle);
        Btn_QuitGame?.UnBindOnClickButtonEvent(OnClick_QuitGame);
    }

    private void OnClick_Resume()
    {
        Time.timeScale = 1f;
        DaniTechUIManager.Instance.CloseContentUI(DaniTechUIType.PausePopupUI);
    }

    private void OnClick_SaveAndLoad()
    {
        DaniTechGameManager.Inst.SaveData();
        DaniTechUIManager.Instance.OpenSimplePopup("저장되었습니다.");
    }

    private void OnClick_ExitToTitle()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    private void OnClick_QuitGame()
    {
        Time.timeScale = 1f;
        DaniTechGameManager.Inst.SaveAndEndGame();
    }
}

