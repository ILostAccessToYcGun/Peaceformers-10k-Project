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

    [SerializeField] Settlement currentSettlement;
    [SerializeField] SettlementQuestBoard settlementQuestBoard;
    [SerializeField] QuestBoard playerQuestBoard;

    public enum QuestStatus { InProgress, Completed } //????
    [SerializeField] public List<QuestObject> quests;
    //[SerializeField] GameObject baseQuestUI;
    [SerializeField] QuestObject baseQuestObject;

    [SerializeField] PlayerUIToggler playerUIToggler;

    public void AddQuestToGiver(QuestObject newObject = null)
    {   
        //Debug.Log("add quest");
        if (newObject == null)
        {
            newObject = Instantiate(baseQuestObject, this.transform);
            newObject.SetParentSettlement(currentSettlement); //I wish there was a better way of setting it up but oh well
            newObject.SetParentQuestGiver(this);
            //newObject.SetResourceCount(Random.Range(0, newObject.GetResourceRequirement()));
        }

        Debug.Log("Quest accepted");
        newObject.SetState(QuestObject.QuestState.InProgress);
        playerQuestBoard.AddQuestToBoard(newObject);
        playerQuestBoard.UpdateQuests();
        settlementQuestBoard.AddQuestToBoard(newObject);
        settlementQuestBoard.UpdateQuests();
            //correspondingPlayerBoardDisplay = otherQuestBoard.AddQuestToBoard(this.questObject);


        quests.Add(newObject);

        if (settlementQuestBoard.GetCurrentViewingSettlement() == currentSettlement)
        {
            //settlementQuestBoard.SetQuests(quests);
        }

    }

    public void RemoveQuestFromGiver(QuestObject removeObject, RemoveType removeType)
    {
        switch (removeType)
        {
            case RemoveType.Remove:
                float modifier = 1f;
                if (removeObject.GetDayDue() - removeObject.calendar.GetDayCount() == 0)
                    modifier = 1.5f;
                currentSettlement.LoseMeter(removeObject.GetUpKeepGain() * modifier); //might change this later to lesser values
                quests.Remove(removeObject);
                Destroy(removeObject); //huh 
                break;
            case RemoveType.Hand_In:
                if ((float)removeObject.GetResourceCount() / (float)removeObject.GetResourceRequirement() > 0f)
                {
                    currentSettlement.GainMeter(removeObject.GetUpKeepGain() * removeObject.GetResourceCount() / removeObject.GetResourceRequirement());
                    quests.Remove(removeObject);
                    //Destroy(removeObject); //huh 
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

    public void SetSettlement() { currentSettlement = GetComponent<Settlement>(); }
    public Settlement GetSettlement() { return currentSettlement; }

    public void ToggleSettlementQuestBoardVisibility()
    {
        if (playerUIToggler.GetSettlementQuestBool())
        {
            settlementQuestBoard.ResetCurrentViewingSettlement();
            playerUIToggler.ToggleSettlementQuestUI();
            //settlementQuestBoard.gameObject.SetActive(false);
            settlementQuestBoard.ClearQuestBoard();
        }
            
        else
        {
            //pass in the quests list and remake the settlement quest board based on it
            settlementQuestBoard.SetCurrentViewingSettlement(this);
            settlementQuestBoard.SetQuests(quests);
            playerUIToggler.ToggleSettlementQuestUI();
            //settlementQuestBoard.gameObject.SetActive(true);
        }
            
    }

    private void Start()
    {
        playerUIToggler = FindAnyObjectByType<PlayerUIToggler>();
    }
    private void Awake()
    {
        //settlementQuestBoard = FindAnyObjectByType<SettlementQuestBoard>();
        SetSettlement();
    }
}
