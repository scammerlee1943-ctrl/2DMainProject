using UnityEngine;

public class LobbyUI : DaniTechUIBase
{
    [SerializeField] private DaniTechUIButton Button_GameStart;
    [SerializeField] private DaniTechUIButton Button_GameQuit;

    private void OnEnable()
    {
        Button_GameStart.BindOnClickButtonEvent(OnClick_GameStart);
        Button_GameQuit.BindOnClickButtonEvent(OnClick_GameQuit);

        DaniTechUIManager.Instance.OpenContentUI(DaniTechUIType.SideUI);
    }

    public void OnClick_GameStart()
    {
        DaniTechUIManager.Instance.OpenContentUI(DaniTechUIType.MenuUI);
        DaniTechUIManager.Instance.CloseContentUI(DaniTechUIType.LobbyUI);
    }

    public void OnClick_GameQuit()
    {
        DaniTechGameManager.Inst.SaveAndEndGame();


    }


}
