using Cysharp.Threading.Tasks;
using NUnit.Framework.Interfaces;
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

    private event Action<string, EGameBookCategory> _onClickSlot;


    private string _slotDataId; // 슬롯이 자기가 살아있는동안 어떤 슬롯인지 DataId를 보관
    private EGameBookCategory _curSlotCategory;

    public string GetSlotDataId()
    {
        return _slotDataId;
    }



    private void OnEnable()
    {
        Button_SlotClick.BindOnClickButtonEvent(OnClick_GameBookSlot);
    }
 

    public void OnClick_GameBookSlot()
    {
        _onClickSlot?.Invoke(_slotDataId, _curSlotCategory);
    }

    private void OnDisable()
    {
        _onClickSlot = null;
    }

    private void SetSlotUI(string dataName, string iconPath)
    {
        Text_MainName.text = dataName;//아이템 데이터에 적혀있는 이름 출력

        if (string.IsNullOrEmpty(iconPath) == true) return;// 비웠을 수 있으니 체크

        DaniTechGameUtil.LoadAndSetSpriteImage(Image_MainIcon, iconPath).Forget();
    }
    public void InitSlot(string dataId, EGameBookCategory curCategory,Action<string, EGameBookCategory> onClickCallback) //TODO : 카테고리에 따라 다른 데이터를 
    {
        if(curCategory == EGameBookCategory.ItemCategory)
        {
            var itemData = DaniTechGameDataManager.Instance.GetDNItemData(dataId);
            if (itemData == null) return;

            SetSlotUI(itemData.Name, itemData.IconPath);
        }

        else if(curCategory == EGameBookCategory.MonsterCategory)
        {
            var monsterData = DaniTechGameDataManager.Instance.GetDNMonsterData(dataId);
            if (monsterData == null) return;

            SetSlotUI(monsterData.Name, monsterData.IconPath);
        }

        else if (curCategory == EGameBookCategory.ArtifactCategory)
        {
            var artifactData = DaniTechGameDataManager.Instance.GetArtifactData(dataId);
            if (artifactData == null) return;

            SetSlotUI(artifactData.Name, artifactData.IconPath);
        }

        //데이터를 잘 받아왔으면, 보관해두자
        

        _slotDataId = dataId;
        _curSlotCategory = curCategory;
        _onClickSlot += onClickCallback;

        //TODO 슬롯 로드가 들어갈 예정
        //Text_MainName.text =
    }

    public void SetSelectedUI(bool isSelect)
    {
        GObj_Selected.SetActive(isSelect);
    }


}