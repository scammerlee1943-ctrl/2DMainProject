using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI2 : DaniTechUIBase
{
    [SerializeField] private Text Text_Description;
    [SerializeField] private DaniTechUIButton Button_Next;


    private string _currentDialogueId = string.Empty;
    private Queue<string> _descriptionQueue = new Queue<string>();


    private void OnEnable()
    {
        Button_Next.BindOnClickButtonEvent(OnClick_Next);
    }

    public void OnClick_Next()
    {
        bool isNextDescriptionOpened = CheckAndSetDescription();

        if (isNextDescriptionOpened)
        {
            return;
        }

        bool isNextDialogueExist = CheckAndStartNextDialogue();
        if (isNextDialogueExist == false)
        {
            DaniTechUIManager.Instance.CloseContentUI(DaniTechUIType.DNDialogueUI);
        }

    }



    private bool CheckAndStartNextDialogue()
    {
        var curDialogueData = DaniTechGameDataManager.Instance.GetDNDialogueData(_currentDialogueId);
        if (curDialogueData == null)
        {
            Debug.LogWarning($"{_currentDialogueId} 현재 다이얼로그 데이터가 존재하지 않습니다");
            return false;
        }
        string nextDialogueId = curDialogueData.NextDialogueId;
        if (string.IsNullOrEmpty(nextDialogueId) == false)
        {
            StartDialogue(nextDialogueId);
            return true;
        }

        return false;
    }



    public void StartDialogue(string dialogueDataId)
    {
        var dialogueData = DaniTechGameDataManager.Instance.GetDNDialogueData(dialogueDataId);
        if (dialogueData == null)
        {
            Debug.LogWarning($"{dialogueDataId}다이얼로그 데이터가 존재하지 않습니다");
            return;
        }

        _currentDialogueId = dialogueDataId;

        if (dialogueData.Description.Contains("<np>"))
        {
            string[] dialogueDescArr = dialogueData.Description.Split("<np>");
            foreach (string desc in dialogueDescArr)
            {
                _descriptionQueue.Enqueue(desc);
            }
            CheckAndSetDescription();
        }
        else
        {
            Text_Description.text = dialogueData.Description;
        }
    }

    private bool CheckAndSetDescription()
    {
        bool isNextDescriptionExsit = (_descriptionQueue.Count > 0);
        if (isNextDescriptionExsit) 
        {
            string desc = _descriptionQueue.Dequeue();
            Text_Description.text = desc;
            
        }



        return isNextDescriptionExsit;
    }
}
