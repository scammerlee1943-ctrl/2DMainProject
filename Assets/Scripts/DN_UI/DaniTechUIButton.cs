using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

public class DaniTechUIButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Button Button_Base;
    [SerializeField] private Text Text_Base;
    [SerializeField] private Image Image_Base;
    [SerializeField] private Image Image_Select;

    private void Awake()
    {
        // 1-2) 이 오브젝트가 생성될 때, 한번 컴포넌트를 찾아서 캐싱하자
        InitUIButton();
        SetDefaultUI();
    }

    private void OnEnable()
    {
        BindOnClickButtonEvent(OnClickSetSelectUI);
    }

    private void OnDisable()
    {
        Button_Base.onClick.RemoveAllListeners();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PlayHoverAnimation();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        PlayUnHoverAnimation();
    }


    private void SetDefaultUI()
    {
        if(Image_Select != null)
        {
            Image_Select.gameObject.SetActive(false);
        }
    }

    private void InitUIButton()
    {
        if(Button_Base != null)
        {
            return;
        }

        // 1-1) 외부에서도 등록할 수 있고,
            // 누군가 누락했다면 등록안해도 알아서 찾아주도록 로직을 넣어 놨다
        var button = this.gameObject.GetComponentInChildren<Button>();
        if(button != null)
        {
            this.Button_Base = button;
        }
    }

    public void BindOnClickButtonEvent(Action onClickCallback)
    {
        if(Button_Base == null) return;

        Button_Base.onClick.AddListener(new UnityEngine.Events.UnityAction(onClickCallback));

    }

    public void UnBindOnClickButtonEvent(Action onClickCallback)
    {
        if (Button_Base == null) return;

        Button_Base.onClick.RemoveListener(new UnityEngine.Events.UnityAction(onClickCallback));
    }

    public void ChangeButtonText(string buttonStr)
    {
        // 혹시 이버튼을 동적으로, 코드에서 텍스트를 수정해야할 때 사용
        if (Text_Base == null) return;

        Text_Base.text = buttonStr;
    }

    private void OnClickSetSelectUI()
    {
        if(Image_Select != null)
        {
            bool currentActive = Image_Select.gameObject.activeSelf;
            Image_Select.gameObject.SetActive(!currentActive);
        }
        PlayClickAnimation();

    }

    private void PlayClickAnimation()
    {
        // 진행중인 애니메이션 있으면 초기화
        transform.DOKill();
        transform.localScale = Vector3.one;
        transform.localRotation = Quaternion.identity;

        // 크기 X, Y 따로 트윈 (스쿼시 & 스트레치)
        transform.DOScaleX(0.85f, 0.1f).SetEase(Ease.OutBack)
            .OnComplete(() => transform.DOScaleX(1f, 0.2f).SetEase(Ease.OutBack));
        transform.DOScaleY(0.85f, 0.1f).SetEase(Ease.OutBack)
            .OnComplete(() => transform.DOScaleY(1f, 0.2f).SetEase(Ease.OutBack));

        // 랜덤 방향으로 살짝 회전
        float randomDir = UnityEngine.Random.Range(0, 2) == 0 ? 1f : -1f;
        transform.DORotate(new Vector3(0f, 0f, 5f * randomDir), 0.1f).SetEase(Ease.OutBack)
            .OnComplete(() => transform.DORotate(Vector3.zero, 0.1f).SetEase(Ease.OutBack));
    }
    private void PlayHoverAnimation()
    {
        transform.DOKill();
        // 크기 살짝 커지기
        transform.DOScaleX(1.1f, 0.2f).SetEase(Ease.OutBack);
        transform.DOScaleY(1.1f, 0.35f).SetEase(Ease.OutBack);
        // 랜덤 방향 살짝 회전
        float randomDir = UnityEngine.Random.Range(0, 2) == 0 ? 1f : -1f;
        transform.DORotate(new Vector3(0f, 0f, 5f * randomDir), 0.1f).SetEase(Ease.OutBack);
    }

    private void PlayUnHoverAnimation()
    {
        transform.DOKill();
        // 원래 크기로
        transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
        // 회전 원래대로
        transform.DORotate(Vector3.zero, 0.1f).SetEase(Ease.OutBack);
    }

}
