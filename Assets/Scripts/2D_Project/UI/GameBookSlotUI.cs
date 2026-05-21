using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UI;

public class GameBookSlotUI : MonoBehaviour
{
    [Header("슬롯 기본 정보")]
    [SerializeField] private Image Image_MainIcon;
    [SerializeField] private Text Text_MainName;
    [SerializeField] private GameObject GObj_Selected;
    [SerializeField] private DaniTechUIButton Button_SlotClick;

    private event Action<string> _onClickSlot;

    public string GetSlotDataId()
    {
        return _slotDataId;
    }



    private string _slotDataId; // 슬롯이 자기가 살아있는동안 어떤 슬롯인지 DataId를 보관

    private void OnEnable()
    {
        Button_SlotClick.BindOnClickButtonEvent(OnClick_GameBookSlot);
    }
 

    private void OnClick_GameBookSlot()
    {
        _onClickSlot?.Invoke(_slotDataId);
    }

    private void OnDisable()
    {
        _onClickSlot = null;
    }
    public void InitSlot(string dataId, Action<string> onClickCallback) //TODO : 카테고리에 따라 다른 데이터를 
    {

        var ItemData = DaniTechGameDataManager.Instance.GetDNItemData(dataId);
        if ( ItemData == null ) return;

        Text_MainName.text = ItemData.Name;//아이템 데이터에 적혀있는 이름 출력


        string iconPath = ItemData.IconPath;
        if (string.IsNullOrEmpty(iconPath) == true) return;// 비웠을 수 있으니 체크

        DaniTechGameUtil.LoadAndSetSpriteImage(Image_MainIcon, iconPath).Forget();

        //데이터를 잘 받아왔으면, 보관해두자
        

        _slotDataId = dataId;
        _onClickSlot += onClickCallback;

        //TODO 슬롯 로드가 들어갈 예정
        //Text_MainName.text =
    }

    public void SetSelectedUI(bool isSelect)
    {
        GObj_Selected.SetActive(isSelect);
    }


}