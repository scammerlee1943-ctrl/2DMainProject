using UnityEngine;
using UnityEngine.UI;

public class SoundVolumeUI : DaniTechUIBase
{
    [SerializeField] private Slider Slider_BGMVolume;
    [SerializeField] private Slider Slider_SFXVolume;
    [SerializeField] private DaniTechUIButton Button_Close;

    private void OnEnable()
    {
        Button_Close?.BindOnClickButtonEvent(OnClick_Close);
        Slider_BGMVolume.onValueChanged.AddListener(OnChange_BGMVolume);
        Slider_SFXVolume.onValueChanged.AddListener(OnChange_SFXVolume);
    }

    private void OnDisable()
    {
        Slider_BGMVolume.onValueChanged.RemoveAllListeners();
        Slider_SFXVolume.onValueChanged.RemoveAllListeners();
    }
    private void OnChange_BGMVolume(float value)
    {
        DaniTechSoundManager.Inst.SetBGMVolume(value);
    }
    private void OnChange_SFXVolume(float value)
    {
        DaniTechSoundManager.Inst.SetSFXVolume(value);
    }
    private void OnClick_Close()
    {
        DaniTechUIManager.Instance.CloseContentUI(DaniTechUIType.SoundVolumeUI);
    }
}
