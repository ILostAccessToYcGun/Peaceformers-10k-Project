using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using static QuestBoard;


public class QuestGiver : MonoBehaviour
{
    //im very dizzy with where I need to go with this, i will do UI then come back

    //OKAY i've decided. I'm going to do this quest display system the same way as the player's quest board.
    /*
     * meaning this script will just HOLD, GENERATE AND DELETE QUESTS
     * I will make another script which will take in this script and interpret the variables and data
     *  into the right spots and instantiate the right quest objects
     *  
     * this other script will be very similar to the QuestBoard script, but different because of the different layout
     
     
     
     */

    //this script will be given to the settlements
    [SerializeField] Settlement currentSettlement;
    [SerializeField] SettlementQuestBoard settlementQuestBoard;
    //[SerializeField] Slider settlementUpKeepMeter;
    //[SerializeField] Image settlementIcon;
    //[SerializeField] float currentSettlementMaxUpkeep;

    public enum QuestStatus { InProgress, Completed } //????
    [SerializeField] List<QuestObject> quests;
    //[SerializeField] GameObject baseQuestUI;
    [SerializeField] QuestObject baseQuestObject;

    public void AddQuestToGiver(QuestObject newObject = null)
    {
        Debug.Log(baseQuestObject);
        if (newObject == null)
        {
            newObject = Instantiate(baseQuestObject, this.transform);
            newObject.SetParentSettlement(currentSettlement); //I wish there was a better way of setting it up but oh well
            newObject.SetParentQuestGiver(this);
            //newObject.SetResourceCount(Random.Range(0, newObject.GetResourceRequirement()));
        }

        quests.Add(newObject);

        if (settlementQuestBoard.GetCurrentViewingSettlement() == currentSettlement)
        {
            settlementQuestBoard.SetQuests(quests);
        }

    }

    public void RemoveQuestFromGiver(QuestObject removeObject, RemoveType removeType)
    {
        switch (removeType)
        {
            case RemoveType.Remove:
                currentSettlement.LoseMeter(removeObject.GetUpKeepGain()); //might change this later to lesser values
                quests.Remove(removeObject);
                Destroy(removeObject); //huh 
                break;
            case RemoveType.Hand_In:
                if ((float)removeObject.GetResourceCount() / (float)removeObject.GetResourceRequirement() > 0f)
                {
                    currentSettlement.GainMeter(removeObject.GetUpKeepGain() * removeObject.GetResourceCount() / removeObject.GetResourceRequirement());
                    quests.Remove(removeObject);
                    Destroy(removeObject); //huh 
                }
                else
                {
                    RemoveQuestFromGiver(removeObject, RemoveType.Remove);
                }
                break;
            case RemoveType.Clear:
                quests.Remove(removeObject);
                Destroy(removeObject); //huh 
                break;
        }
    }



    //the idea is that this is one piece of UI that will switch which settlement to look at based on which one you interact with.
    public void SetSettlement() { currentSettlement = GetComponent<Settlement>(); }
    public Settlement GetSettlement() { return currentSettlement; }

    public void ToggleSettlementQuestBoardVisibility()
    {
        if (settlementQuestBoard.gameObject.activeSelf)
        {
            settlementQuestBoard.ResetCurrentViewingSettlement();
            settlementQuestBoard.gameObject.SetActive(false);
        }
            
        else
        {
            //pass in the quests list and remake the settlement quest board based on it
            settlementQuestBoard.SetCurrentViewingSettlement(this);
            settlementQuestBoard.SetQuests(quests);
            Debug.Log("twice?");
            settlementQuestBoard.gameObject.SetActive(true);
        }
            
    }

    private void Awake()
    {
        //settlementQuestBoard = FindAnyObjectByType<SettlementQuestBoard>();
        SetSettlement();
    }
}
