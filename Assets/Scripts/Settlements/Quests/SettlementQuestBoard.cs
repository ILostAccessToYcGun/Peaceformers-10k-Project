using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SettlementQuestBoard : QuestBoard
{
    [Header("Current Settlement")]
    [SerializeField] Settlement currentViewingSettlement;
    [SerializeField] QuestGiver currentViewingQuestGiver;
    [SerializeField] TextMeshProUGUI currentSettlementName;
    [SerializeField] Image currentSettlementIcon;
    [Space]
    [Header("Slider")]
    [SerializeField] Slider currentSettlementUpkeep;
    [SerializeField] TextMeshProUGUI currentSettlementUpkeepText;
    [SerializeField] float currentSettlementMaxUpkeep;
    [SerializeField] bool lookingAtSettlement;
    

    public Settlement GetCurrentViewingSettlement() { return currentViewingSettlement; }    
    public QuestGiver GetCurrentViewingQuestGiver() { return currentViewingQuestGiver; }
    public void SetCurrentViewingQuestGiver(QuestGiver newQuestGiver) { currentViewingQuestGiver = newQuestGiver; }
    
    public void SetCurrentViewingSettlement(QuestGiver qg)
    {
        SetLookingAtSettlementBool(true);
        SetCurrentViewingQuestGiver(qg);
        currentViewingSettlement = qg.GetSettlement();
        currentSettlementMaxUpkeep = currentViewingSettlement.GetMaxUpKeep();
        currentSettlementName.text = currentViewingSettlement.GetSettlementName();
        currentSettlementIcon.sprite = currentViewingSettlement.GetIcon();
    }

    public void ResetCurrentViewingSettlement()
    {
        SetLookingAtSettlementBool(false);
        SetCurrentViewingQuestGiver(null);
        currentViewingSettlement = null;
        currentSettlementMaxUpkeep = 0f;
        currentSettlementName.text = "Settlement Name";
    }

    public void SetLookingAtSettlementBool(bool toggle) { lookingAtSettlement = toggle; }

    #region _Quests_

    public void SetQuests(List<QuestObject> questsToCopy)
    {
        ResetQuests();
        foreach (QuestObject quest in questsToCopy)
        {
            AddQuestToBoard(quest);
        }
    }

    public void ResetQuests()
    {
        for (int i = 0; i < questsDisplays.Count; i++)
        {
            RemoveQuestFromBoard(scrollContent.GetComponentInChildren<QuestDisplay>(), RemoveType.Clear);
        }
    }

    public override QuestDisplay AddQuestToBoard(QuestObject newObject = null)
    {
        Debug.Log(baseQuestObject);
        if (newObject == null)
        {
            newObject = Instantiate(baseQuestObject);
            //newObject.SetResourceCount(Random.Range(0, newObject.GetResourceRequirement()));
        }

        GameObject questUI = Instantiate(baseQuestUI, scrollContent.transform);
        SettlementQuestDisplay newDisplay = questUI.GetComponent<SettlementQuestDisplay>();

        newObject.SetCorrespondingSettlementQuestDisplayUI(newDisplay);

        newDisplay.SetAbandonVerification(abandonVerification);
        newDisplay.SetAbandonConfirmButton(abandonConfirmButton);
        newDisplay.SetParentQuestBoard(this); //streamline this eventually
        newDisplay.SetOtherQuestBoard(otherQuestBoard);
        newDisplay.SetQuestObject(newObject);
        questsDisplays.Add(newDisplay);

        switch (newObject.state)
        {
            case QuestObject.QuestState.Avalible:
                newDisplay.RemoveButtonAbandon();
                newDisplay.RemoveButtonHandIn();
                newDisplay.AddButtonAccept();
                break;
            case QuestObject.QuestState.InProgress:
                newDisplay.RemoveButtonAccept();
                newDisplay.AddButtonHandIn();
                newDisplay.AddButtonAbandon();
                break;
            case QuestObject.QuestState.Completed:
                Debug.Log("WAT");
                break;
        }
        return newDisplay;
    }

    public override void RemoveQuestFromBoard(QuestDisplay removeQuest, RemoveType removeType = RemoveType.Clear, bool isDestroyQuest = true)
    {

        if (removeQuest == null) { return; }
        questsDisplays.Remove(removeQuest);

        switch(removeType)
        {
            case RemoveType.Remove:
                if (currentViewingQuestGiver != null)
                {
                    currentViewingQuestGiver.RemoveQuestFromGiver(removeQuest.questObject, removeType);
                }
                else
                {
                    removeQuest.questObject.GetParentQuestGiver().RemoveQuestFromGiver(removeQuest.questObject, removeType);
                }
                
                break;
            case RemoveType.Hand_In:
                currentViewingQuestGiver.RemoveQuestFromGiver(removeQuest.questObject, removeType);
                break;
            case RemoveType.Clear:
                //currentViewingQuestGiver.RemoveQuestFromGiver(removeQuest.questObject, removeType);
                break;
        }
        if (isDestroyQuest)
            Destroy(removeQuest.gameObject);

    }

    #endregion


    private void Update()
    {
        if (lookingAtSettlement)
        {
            currentSettlementUpkeep.value = currentViewingSettlement.GetCurrentUpKeep() / currentSettlementMaxUpkeep;
            currentSettlementUpkeepText.text = "Up-Keep (" + (Mathf.Floor(currentSettlementUpkeep.value * 1000f) / 10f).ToString() + "%)";
        }
    }
}
