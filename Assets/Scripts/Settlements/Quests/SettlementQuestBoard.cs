using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SettlementQuestBoard : QuestBoard
{

    public void SetCurrentViewingSettlement(QuestGiver qg)
    {
        //TODO:
        //set settlement name
        //set settlement icon
        //update the upkeep bar to be in sync with the settlement
        //clear and rebuild the quests in the content
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


        
        newDisplay.SetQuestBoard(this); //streamline this eventually
        newDisplay.SetQuestObject(newObject);
        quests.Add(newDisplay);

        
        //instantiate quest UI
        //add something to the scrollQuestParent
    }

    public void RemoveQuest(QuestDisplay removeQuest)
    {
        quests.Remove(removeQuest);
        Destroy(removeQuest.gameObject);
        //remove something to the scrollQuestParent

        //I need to show a confirmation prompt to the plyer to make sure they want to adandon the quest.
    }
}
