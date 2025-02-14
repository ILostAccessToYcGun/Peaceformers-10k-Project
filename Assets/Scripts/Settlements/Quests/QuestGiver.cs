using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;


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

    public void AddQuest(QuestObject newObject = null)
    {
        Debug.Log(baseQuestObject);
        if (newObject == null)
        {
            newObject = Instantiate(baseQuestObject, this.transform);
            newObject.SetResourceCount(Random.Range(0, newObject.GetResourceRequirement()));
        }

        quests.Add(newObject);

    }

    public void RemoveQuest(QuestObject removeObject)
    {
        quests.Remove(removeObject);
        //Destroy(removeObject); //huh
        //remove something to the scrollQuestParent

        //I need to show a confirmation prompt to the plyer to make sure they want to adandon the quest.
    }



    //the idea is that this is one piece of UI that will switch which settlement to look at based on which one you interact with.
    public void SetSettlement() { currentSettlement = GetComponent<Settlement>(); }
    public Settlement GetSettlement() { return currentSettlement; }

    //private void Update()
    //{
    //    if (currentSettlement != null)
    //        UpdateMeter();
    //}

    private void Awake()
    {
        settlementQuestBoard = FindAnyObjectByType<SettlementQuestBoard>();
        SetSettlement();
    }
}
