using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class DaniTech_DialogueUI : DaniTechUIBase
{
    [SerializeField] private GameObject Layout_CharacterName;
    [SerializeField] private Text Text_Character;
    [SerializeField] private Text Text_Description;
    [SerializeField] private DaniTechUIButton Button_Next;
    [SerializeField] private float _typingDelay = 0.03f;

    // 선택지 관련 - 프리팹에서 새로 등록해야 하는 부분
    [SerializeField] private GameObject Layout_Selection;  
    [SerializeField] private GameObject Prefab_SelectionButton; 
    [SerializeField] private Transform Transform_SelectionRoot;

    private string _currentDialogueId;
    private Queue<string> _descriptionQueue = new Queue<string>();
    private bool _isOpenedThisFrame = false;
    private CancellationTokenSource _typingCts;
    private bool _isTyping = false;
    private string _currentFullText = "";

    private List<SelectionButton> _createdSelectionButtonList = new List<SelectionButton>();
    private bool _isSelectionShowing = false;

    private void OnEnable()
    {
        Button_Next.BindOnClickButtonEvent(OnClick_Next);
        _isOpenedThisFrame = true;
    }
    private void OnDisable()
    {
        if (_typingCts != null)
        {
            _typingCts.Cancel();
        }
        HideSelection();
    }
    private void Update()
    {
        if (_isOpenedThisFrame == true)
        {
            _isOpenedThisFrame = false;
            return;
        }

        // 선택지가 떠 있는 동안은 F키로 넘기지 못하게 막는다
        if (_isSelectionShowing == true)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            OnClick_Next();
        }
    }

    // 다이얼로그에서 Next 버튼이 눌러질때 호출된다
    public void OnClick_Next()
    {
        // 선택지가 떠 있으면 Next 동작을 막는다 (버튼으로만 진행)
        if (_isSelectionShowing == true)
        {
            return;
        }

        if (_isTyping == true)
        {
            if (_typingCts != null)
            {
                _typingCts.Cancel();
            }
            Text_Description.text = _currentFullText;
            _isTyping = false;
            return;
        }

        // 다음 대사가 있는지 체크한다
        bool isNextDescriptionExist = CheckAndSetDescription();

        if (isNextDescriptionExist)
        {
            return;
        }

        // 대사가 없다면, 현재 다이얼로그에 선택지가 있는지 먼저 체크한다
        bool isSelectionExist = CheckAndShowSelection();
        if (isSelectionExist)
        {
            return;
        }

        // 선택지도 없다면, 다음으로 이어지는 다이얼로그가 있는지 체크한다
        bool isNextDialogueExist = CheckAndStartNextDialogue();
        if (isNextDialogueExist == false)
        {
            DaniTechUIManager.Instance.CloseContentUI(DaniTechUIType.DNDialogueUI);
        }
    }

    // 현재 다이얼로그에 선택지가 있다면 버튼들을 생성해서 보여준다
    private bool CheckAndShowSelection()
    {
        var dialogueData = DaniTechGameDataManager.Instance.GetDNDialogueData(_currentDialogueId);
        if (dialogueData == null)
        {
            return false;
        }

        var selectionNameList = dialogueData.SelectionNameList;
        var selectionDialogueIdList = dialogueData.SelectionDialogueIdList;

        // 선택지 이름이 없으면 선택지가 없는 다이얼로그
        if (selectionNameList == null || selectionNameList.Count == 0)
        {
            return false;
        }

        ShowSelection(selectionNameList, selectionDialogueIdList);
        return true;
    }

    private void ShowSelection(List<string> selectionNameList, List<string> selectionDialogueIdList)
    {
        ClearSelection();

        _isSelectionShowing = true;
        // 선택지가 뜰 때는 Next 버튼을 숨긴다
        Button_Next.gameObject.SetActive(false);
        Layout_Selection.SetActive(true);

        for (int i = 0; i < selectionNameList.Count; i++)
        {
            string selectionName = selectionNameList[i];
            // 이름 개수에 맞춰서 안전하게 대상 다이얼로그 Id를 꺼낸다
            string targetDialogueId = (i < selectionDialogueIdList.Count) ? selectionDialogueIdList[i] : string.Empty;

            var gObj = Instantiate(Prefab_SelectionButton, Transform_SelectionRoot);
            var selectionButton = gObj.GetComponent<SelectionButton>();
            if (selectionButton == null)
            {
                Debug.LogWarning("선택지 버튼 프리팹에 SelectionButton이 없습니다.");
                continue;
            }
            selectionButton.InitSelection(selectionName, targetDialogueId);
            selectionButton.BindSelectEvent(OnClick_Selection);

            _createdSelectionButtonList.Add(selectionButton);
        }
    }

    private void OnClick_Selection(string selectedDialogueId)
    {
        HideSelection();

        if (string.IsNullOrEmpty(selectedDialogueId))
        {
            DaniTechUIManager.Instance.CloseContentUI(DaniTechUIType.DNDialogueUI);
            return;
        }

        StartDialogue(selectedDialogueId);
    }

    private void HideSelection()
    {
        _isSelectionShowing = false;
        if (Layout_Selection != null)
        {
            Layout_Selection.SetActive(false);
        }
        if (Button_Next != null)
        {
            Button_Next.gameObject.SetActive(true);
        }
        ClearSelection();
    }

    private void ClearSelection()
    {
        foreach (var button in _createdSelectionButtonList)
        {
            if (button != null)
            {
                Destroy(button.gameObject);
            }
        }
        _createdSelectionButtonList.Clear();
    }

    private bool CheckAndStartNextDialogue()
    {
        var dialogueData = DaniTechGameDataManager.Instance.GetDNDialogueData(_currentDialogueId);
        if (dialogueData == null)
        {
            Debug.LogWarning($"다이얼로그 데이터가 존재하지 않습니다 {dialogueData}");
            return false;
        }

        // 현재 데이터를 기준으로 다음 다이얼로그가 있는지 체크해보고, 있다면 다음 다이얼로그를 시작한다!
        string nextDialogueId = dialogueData.NextDialogueId;
        if (string.IsNullOrEmpty(nextDialogueId) == false)
        {
            StartDialogue(nextDialogueId);
            return true;
        }

        return false;
    }

    // 다이얼로그를 시작하는 메서드 (외부에서 UIManager를 통해 다이얼로그 시작을 요청할때도 쓴다!)
    public void StartDialogue(string dialogeId)
    {
        var dialogueData = DaniTechGameDataManager.Instance.GetDNDialogueData(dialogeId);
        if (dialogueData == null)
        {
            Debug.LogWarning($"다이얼로그 데이터가 존재하지 않습니다 {dialogueData}");
            return;
        }

        // 현재 진행중인 다이얼로그 Id는 다음 다이얼로그가 있는지 체크할 때 쓸 수 있도록 보관한다
        _currentDialogueId = dialogeId;
        bool canGiveItem = true;

        if (string.IsNullOrEmpty(dialogueData.UseItemId) == false)
        {
            canGiveItem = DaniTechGameManager.Inst.UseItem(dialogueData.UseItemId);

            if (canGiveItem == false)
            {
                HideSelection();
                SetCurrentDialogueDescription("인벤토리에 츄르가 없어!");
                SetCharacterName(null);
                return;
            }
        }

        if (canGiveItem == true && string.IsNullOrEmpty(dialogueData.GiveItemId) == false)
        {
            DaniTechGameManager.Inst.AddItem(dialogueData.GiveItemId, 1);
        }
        HideSelection();

        // 혹시 현재 대사가 너무 길거나 다음 페이지 처리가 필요할 때 <np> 키워드로 잘라주자!
        if (dialogueData.Description.Contains("<np>"))
        {
            string[] dialogueDescriptionList = dialogueData.Description.Split("<np>");
            foreach(string desc in dialogueDescriptionList)
            {
                _descriptionQueue.Enqueue(desc);
            }
            CheckAndSetDescription();
        }
        else
        {
            // Np 태그가 없다면 바로 다이얼로그 UI를 세팅하자
            SetCurrentDialogueDescription(dialogueData.Description);
        }

        SetCharacterName(dialogueData.CharacterDataId);
    }

    private bool CheckAndSetDescription()
    {
        bool isNextDescriptionExsist = (_descriptionQueue.Count > 0);
        if (isNextDescriptionExsist)
        {
            string desc = _descriptionQueue.Dequeue();
            SetCurrentDialogueDescription(desc);
        }

        return isNextDescriptionExsist;
    }

    private void SetCharacterName(string characterDataId)
    {
        // 캐릭터 정보가 있다면 말하는 이의 추가 정보를 표기해줄 수 있도록 연동하는 부분
        bool isActive = (string.IsNullOrEmpty(characterDataId) == false);
        Layout_CharacterName.SetActive(isActive);

        if (isActive)
        {
            var characterData = DaniTechGameDataManager.Instance.GetCharacterData(characterDataId);
            if(characterData != null)
            {
                Text_Character.text = characterData.Name;
            }
        }
    }

    private void SetCurrentDialogueDescription(string description)
    {
        _currentFullText = description;
        PlayTypingEffect(description).Forget();
    }

    private async UniTaskVoid PlayTypingEffect(string description)
    {
        if (_typingCts != null)
        {
            _typingCts.Cancel();
        }

        _typingCts = new CancellationTokenSource();
        var token = _typingCts.Token;

        _isTyping = true;
        Text_Description.text = "";

        for (int i = 0; i < description.Length; i++)
        {
            Text_Description.text += description[i];
            bool isCanceled = await UniTask.Delay(
                (int)(_typingDelay * 1000), cancellationToken: token).SuppressCancellationThrow();
            if (isCanceled) return;
        }

        _isTyping = false;
    }
}
