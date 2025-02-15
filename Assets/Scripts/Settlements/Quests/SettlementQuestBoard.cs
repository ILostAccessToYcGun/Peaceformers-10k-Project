using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SettlementQuestBoard : QuestBoard
{
    [SerializeField] Settlement currentViewingSettlement;
    [SerializeField] TextMeshProUGUI currentSettlementName;
    [SerializeField] Image currentSettlementIcon;
    [Space]
    [Header("Slider")]
    [SerializeField] Slider currentSettlementUpkeep;
    [SerializeField] TextMeshProUGUI currentSettlementUpkeepText;
    [SerializeField] float currentSettlementMaxUpkeep;
    [SerializeField] bool lookingAtSettlement;

    public Settlement GetCurrentViewingSettlement() { return currentViewingSettlement; }
    public void SetCurrentViewingSettlement(QuestGiver qg)
    {
        SetLookingAtSettlementBool(true);
        currentViewingSettlement = qg.GetSettlement();
        currentSettlementMaxUpkeep = currentViewingSettlement.GetMaxUpKeep();
        currentSettlementName.text = currentViewingSettlement.GetSettlementName();

        //currentSettlementIcon = currentViewingSettlement.GetSettlementIcon();
        //TODO:

        //update the upkeep bar to be in sync with the settlement
        //clear and rebuild the quests in the content
    }

    public void ResetCurrentViewingSettlement()
    {
        SetLookingAtSettlementBool(false);
        currentViewingSettlement = null;
        currentSettlementMaxUpkeep = 0f;
        currentSettlementName.text = "Settlement Name";

        //currentSettlementIcon = currentViewingSettlement.GetSettlementIcon();
        //TODO:

        //update the upkeep bar to be in sync with the settlement
        //clear and rebuild the quests in the content
    }

    public void SetLookingAtSettlementBool(bool toggle) { lookingAtSettlement = toggle; }

    #region _Quests_

    public void SetQuests(List<QuestObject> questsToCopy)
    {
        ResetQuests();
        foreach (QuestObject quest in questsToCopy)
        {
            AddQuest(quest);
        }
    }

    public void ResetQuests()
    {
        for (int i = 0; i < quests.Count; i++)
        {
            RemoveQuest(scrollContent.GetComponentInChildren<QuestDisplay>());
        }
    }

    public override void AddQuest(QuestObject newObject = null)
    {
        Debug.Log(baseQuestObject);
        if (newObject == null)
        {
            newObject = Instantiate(baseQuestObject);
            newObject.SetResourceCount(Random.Range(0, newObject.GetResourceRequirement()));
        }

        GameObject questUI = Instantiate(baseQuestUI, scrollContent.transform);
        SettlementQuestDisplay newDisplay = questUI.GetComponent<SettlementQuestDisplay>();


        newDisplay.SetAbandonVerification(abandonVerification);
        newDisplay.SetAbandonConfirmButton(abandonConfirmButton);
        newDisplay.SetQuestBoard(this); //streamline this eventually
        newDisplay.SetQuestObject(newObject);
        quests.Add(newDisplay);


        //instantiate quest UI
        //add something to the scrollQuestParent
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
