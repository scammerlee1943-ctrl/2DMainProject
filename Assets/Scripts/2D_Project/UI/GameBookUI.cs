using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class GameBookUI : DaniTechUIBase
{
    [Header("동적 생성할 프리팹")]
    [SerializeField] private GameObject Prefab_Slot; //동적 생성된다는 것을 알기 위해 프리팹이라는 단어 명시

    [Header("디테일 정보 영역")]
    [SerializeField] private Image Image_MainIcon;
    [SerializeField] private Text Text_MainName;
    [SerializeField] private Text Text_Description;
    [SerializeField] private DaniTechUIButton Button_CloseUI;


    //[Header("부가 정보")]
   // [SerializeField] private GameObject Layout_SubInfo; // 그 안에 있는 UI요소를 직접 하나하나 껐다 켰다 하는 게 아니라, 그 레이아웃에 대표 오브젝트만 껐다 켰다 하는게 압도적으로 편하다!!

    [Header("슬롯 리스트 영역")]
    [SerializeField] private Transform Transform_SlotRoot; //스크롤뷰

    private Dictionary<string, GameBookSlotUI> _SlotList = new Dictionary<string, GameBookSlotUI>();

    private void OnEnable()
    {
        ReadItemListAndCreateSlot();

        Button_CloseUI.BindOnClickButtonEvent(OnClick_CloseGameBookUI);
    }

    public void OnClick_CloseGameBookUI()
    {
        DaniTechUIManager.Instance.CloseContentUI(DaniTechUIType.GameBookUI);
    }

    private void OnDisable()
    {

        if (_SlotList.Count > 0)
        {
            foreach(var slotKv in _SlotList)
            {
                var slot = slotKv.Value; //컴포넌트인데, 얘로 gameObject를 받아올 수 있다!
                DestroyImmediate(slot.gameObject);
            }
            _SlotList.Clear();
        }
    }

    private void ReadItemListAndCreateSlot()
    {
        var dataList = DaniTechGameDataManager.Instance.ItemDataList;
        foreach(var dataKv in dataList)
        {
            var data = dataKv.Value;
            if(data == null) continue; // 데이터가 Null일수 있으니 체크

            CreateGameBookSlot(data.Id);
        }
    }
    private void CreateGameBookSlot(string dataId)
    {

        var gObj = Instantiate(Prefab_Slot, Transform_SlotRoot);
        if(gObj == null) return;


        // 게임 오브젝트는 생성이 됐다.
        var SlotComponent = gObj.GetComponent<GameBookSlotUI>();
        if(SlotComponent == null) return;

        // 동적 생성된 자식 슬롯(게임오브젝트) 안에 있는 컴포넌트도 잘 가져왔다.
        SlotComponent.InitSlot(dataId, OnClickchildSlotSelected);
        _SlotList.Add(dataId, SlotComponent);

    }

    public void OnClickchildSlotSelected(string slotDataId)
    {
        var currentSelectedData = DaniTechGameDataManager.Instance.GetDNItemData(slotDataId);
        if(currentSelectedData == null) return;

        DaniTechGameUtil.LoadAndSetSpriteImage(Image_MainIcon, currentSelectedData.IconPath).Forget();

        //Image_MainIcon;
        Text_MainName.text = currentSelectedData.Name;
        Text_Description.text = currentSelectedData.Description;


        foreach(var slotKv in _SlotList)
        {
            var slot = slotKv.Value;
            var dataId = slot.GetSlotDataId();
            slot.SetSelectedUI(slotDataId == dataId);

        }
    }
}
