using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public enum EGameBookCategory
{
    None = 0,
    ItemCategory,
    MonsterCategory,
    ArtifactCategory
}

public class GameBookUI : DaniTechUIBase
{
    [Header("동적 생성할 프리팹")]
    [SerializeField] private GameObject Prefab_Slot; //동적 생성된다는 것을 알기 위해 프리팹이라는 단어 명시

    [Header("디테일 정보 영역")]
    [SerializeField] private Image Image_MainIcon;
    [SerializeField] private Text Text_MainName;
    [SerializeField] private Text Text_Description;

    [Header("상단 카테고리")]
    [SerializeField] private DaniTechUIButton Button_ItemCategory;
    [SerializeField] private DaniTechUIButton Button_ArtifactCategory;
    [SerializeField] private DaniTechUIButton Button_MonsterCategory;

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
        Button_ItemCategory.BindOnClickButtonEvent(OnClick_ItemCategory);
        Button_ArtifactCategory.BindOnClickButtonEvent(OnClick_ArtifactCategory);
        Button_MonsterCategory.BindOnClickButtonEvent(OnClick_MonsterCategory);
    }

    private void OnDisable()
    {
        DestroyAndSlotList();
    }
    private void DestroyAndSlotList()
    {
        if (_SlotList.Count > 0)
        {
            foreach (var slotKv in _SlotList)
            {
                var slot = slotKv.Value; //컴포넌트인데, 얘로 gameObject를 받아올 수 있다!
                DestroyImmediate(slot.gameObject);
            }
            _SlotList.Clear();
        }
    }

    public void OnClick_CloseGameBookUI()
    {
        DaniTechUIManager.Instance.CloseContentUI(DaniTechUIType.GameBookUI);
    }

    public void OnClick_ItemCategory()
    {
        SetGameBookLayoutByCategory(EGameBookCategory.ItemCategory);
    }

    public void OnClick_MonsterCategory()
    {
        SetGameBookLayoutByCategory(EGameBookCategory.MonsterCategory);
    }
    public void OnClick_ArtifactCategory()
    {
        SetGameBookLayoutByCategory(EGameBookCategory.ArtifactCategory);
    }

    private void SetGameBookLayoutByCategory(EGameBookCategory category)
    {
        DestroyAndSlotList();

        switch (category)
        {
            case EGameBookCategory.ItemCategory:
                ReadItemListAndCreateSlot();
                break;
            case EGameBookCategory.MonsterCategory:
                ReadMonsterListAndCreateSlot();
                break;
            case EGameBookCategory.ArtifactCategory:
                ReadArtifactListAndCreateSlot();
                break;
            default:
                break;
        }
    }
    private void ReadItemListAndCreateSlot()
    {
        var dataList = DaniTechGameDataManager.Instance.ItemDataList;
        foreach(var dataKv in dataList)
        {
            var data = dataKv.Value;
            if(data == null) continue; // 데이터가 Null일수 있으니 체크

            CreateGameBookSlot(data.Id, EGameBookCategory.ItemCategory);
        }
        SelectFirstSlot();
    }
    private void ReadMonsterListAndCreateSlot()
    {
        var dataList = DaniTechGameDataManager.Instance.MonsterDataList;
        foreach (var dataKv in dataList)
        {
            var data = dataKv.Value;
            if (data == null) continue;

            CreateGameBookSlot(data.Id, EGameBookCategory.MonsterCategory);
        }
        SelectFirstSlot();
    }

    private void ReadArtifactListAndCreateSlot()
    {
        var dataList = DaniTechGameDataManager.Instance.ArtifactDataList;
        foreach (var dataKv in dataList)
        {
            var data = dataKv.Value;
            if (data == null) continue;

            CreateGameBookSlot(data.Id, EGameBookCategory.ArtifactCategory);
        }
        SelectFirstSlot();
    }

    private void CreateGameBookSlot(string dataId, EGameBookCategory curCategory)
    {

        var gObj = Instantiate(Prefab_Slot, Transform_SlotRoot);
        if(gObj == null) return;


        // 게임 오브젝트는 생성이 됐다.
        var slotComponent = gObj.GetComponent<GameBookSlotUI>();
        if(slotComponent == null) return;

        // 동적 생성된 자식 슬롯(게임오브젝트) 안에 있는 컴포넌트도 잘 가져왔다.
        slotComponent.InitSlot(dataId,curCategory, OnClickchildSlotSelected);
        _SlotList.Add(dataId, slotComponent);

    }

    private void SelectFirstSlot()
    {
        foreach (var slotKv in _SlotList)
        {
            var slot = slotKv.Value;
            slot.OnClick_GameBookSlot();
            break;
        }

    }

    private void SetDetailInfoUI(string dataName, string dataDescription = "", string iconPath = "")
    {
        Text_MainName.text = dataName;
        Text_Description.text = dataDescription;

        if (string.IsNullOrEmpty(iconPath) == false)
        {
            DaniTechGameUtil.LoadAndSetSpriteImage(Image_MainIcon, iconPath).Forget();
        }

        Image_MainIcon.gameObject.SetActive(string.IsNullOrEmpty(iconPath) == false);
    }

    private  void OnClickchildSlotSelected(string slotDataId, EGameBookCategory selectedSlotCategory)
    {

        if(selectedSlotCategory == EGameBookCategory.ItemCategory)
        {
            var currentSelectedData = DaniTechGameDataManager.Instance.GetDNItemData(slotDataId);
            if (currentSelectedData == null) return;

            SetDetailInfoUI(currentSelectedData.Name, currentSelectedData.Description, currentSelectedData.IconPath);
        }

        else if (selectedSlotCategory == EGameBookCategory.MonsterCategory)
        {
            var currentSelectedData = DaniTechGameDataManager.Instance.GetDNMonsterData(slotDataId);
            if (currentSelectedData == null) return;

            SetDetailInfoUI(currentSelectedData.Name, currentSelectedData.Description, currentSelectedData.IconPath);
        }

        else if (selectedSlotCategory == EGameBookCategory.ArtifactCategory)
        {
            var currentSelectedData = DaniTechGameDataManager.Instance.GetArtifactData(slotDataId);
            if (currentSelectedData == null) return;

            SetDetailInfoUI(currentSelectedData.Name, currentSelectedData.Description, currentSelectedData.IconPath);
        }

        foreach(var slotKv in _SlotList)
        {
            var slot = slotKv.Value;
            var dataId = slot.GetSlotDataId();
            slot.SetSelectedUI(slotDataId == dataId);

        }
    }
}
