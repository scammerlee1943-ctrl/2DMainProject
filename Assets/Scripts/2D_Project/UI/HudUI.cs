using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudUI : DaniTechUIBase
{
    [SerializeField] private Text Text_CurrentHeight;
    [SerializeField] private Text Text_MaxHeight;

    [SerializeField] private GameObject Prefab_HudHeight;
    [SerializeField] private Transform Transform_Root;

    private void Start()
    {
        if (DaniTechGameObjectManager.Inst == null) return;
        var player = DaniTechGameObjectManager.Inst.GetLocalPlayer();
        if (player == null) return;
        player.OnHeightChanged += UpdateHeight;
    }

    private void OnEnable()
    {

    }
    private void OnDisable()
    {
        var player = DaniTechGameObjectManager.Inst.GetLocalPlayer();
        if (player == null) return;
        player.OnHeightChanged -= UpdateHeight;
    }
    public void UpdateHeight(float currentHeight, float maxHeight)
    {
        Text_CurrentHeight.text = $"현재 높이: {(int)currentHeight}m";
        Text_MaxHeight.text = $"최고 높이: {(int)maxHeight}m";
    }

    private Dictionary<int, HudSlotUI> _hudSlotList = new Dictionary<int, HudSlotUI>();
    public void AddHudSlot(int instanceId)
    {
        CreateHudSlot(instanceId);
    }

    private void CreateHudSlot(int instanceId)
    {
        var gObj = Instantiate(Prefab_HudHeight, Transform_Root);
        if (gObj == null) return;


        // 게임 오브젝트는 생성이 됐다.
        var slotComponent = gObj.GetComponent<HudSlotUI>();
        if (slotComponent == null) return;

        //// 동적 생성된 자식 슬롯(게임오브젝트) 안에 있는 컴포넌트도 잘 가져왔다.
        //SlotComponent.InitSlot(dataId, curCategory, OnClickchildSlotSelected);
        _hudSlotList.Add(instanceId, slotComponent);
    }
    public void RemoveHudSlot()
    {

    }
}
