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

    private Action OnHoverButtonEvent;
    private bool _useClickAnimation = true;

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
        transform.DOKill();
        if (Button_Base != null)
        {
            Button_Base.onClick.RemoveAllListeners();
        }
        OnHoverButtonEvent = null;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PlayHoverAnimation();
        PlayHoverSound();
        OnHoverButtonEvent?.Invoke();

        if (CursorManager.Inst != null)
            CursorManager.Inst.SetHover(true);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        PlayUnHoverAnimation();

        if (CursorManager.Inst != null)
            CursorManager.Inst.SetHover(false);
    }

    public void BindOnHoverButtonEvent(Action onHoverCallback)
    {
        OnHoverButtonEvent += onHoverCallback;
    }
    public void UnBindOnHoverButtonEvent(Action onHoverCallback)
    {
        OnHoverButtonEvent -= onHoverCallback;
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

        Button_Base.onClick.AddListener(onClickCallback.Invoke);

    }

    public void UnBindOnClickButtonEvent(Action onClickCallback)
    {
        if (Button_Base == null) return;

        Button_Base.onClick.RemoveListener(onClickCallback.Invoke);
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
        if (_useClickAnimation == true)
        {
            PlayClickAnimation();
        }
        PlayClickSound();
    }

    private void PlayClickAnimation()
    {
        transform.DOKill();
        transform.localScale = Vector3.one;
        transform.localRotation = Quaternion.identity;

        Sequence sequence = DOTween.Sequence();
        //Sequence 는 DOTween에서 여러 애니메이션을 묶어주는 컨테이너
        sequence.Append(transform.DOScaleX(0.85f, 0.1f).SetEase(Ease.OutBack));
        sequence.Join(transform.DOScaleY(0.85f, 0.1f).SetEase(Ease.OutBack));
        sequence.Join(transform.DORotate(new Vector3(0f, 0f, 5f * GetRandomDir()), 0.1f).SetEase(Ease.OutBack));
        sequence.Append(transform.DOScaleX(1f, 0.2f).SetEase(Ease.OutBack));
        sequence.Join(transform.DOScaleY(1f, 0.2f).SetEase(Ease.OutBack));
        sequence.Join(transform.DORotate(Vector3.zero, 0.1f).SetEase(Ease.OutBack));
    }

    public void SetUseClickAnimation(bool use)
    {
        _useClickAnimation = use;
    }

    private void PlayHoverSound()
    {
        int randomIndex = UnityEngine.Random.Range(1, 3);
        string randomKey = $"SFX_UI_Hover_{randomIndex}";

        PlayButtonSound(randomKey);
    }

    private void PlayClickSound()
    {
        int randomIndex = UnityEngine.Random.Range(1, 8);
        string randomKey = $"SFX_UI_Click_{randomIndex}";

        PlayButtonSound(randomKey);
    }

    private void PlayButtonSound(string soundKey)
    {
        if (DaniTechSoundManager.Inst != null)
        {
            DaniTechSoundManager.Inst.PlaySFX(soundKey);
        }
        else
        {
            Debug.LogWarning("사운드 매니저를 찾을 수 없습니다!!");
        }
    }

    private void PlayHoverAnimation()
    {
        transform.DOKill();
        transform.DOScaleX(1.1f, 0.2f).SetEase(Ease.OutBack);
        transform.DOScaleY(1.1f, 0.35f).SetEase(Ease.OutBack);
        transform.DORotate(new Vector3(0f, 0f, 5f * GetRandomDir()), 0.1f).SetEase(Ease.OutBack);
    }

    private void PlayUnHoverAnimation()
    {
        transform.DOKill();
        transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
        transform.DORotate(Vector3.zero, 0.1f).SetEase(Ease.OutBack);
    }

    private float GetRandomDir()
    {
        return UnityEngine.Random.Range(0, 2) == 0 ? 1f : -1f;
    }

}
