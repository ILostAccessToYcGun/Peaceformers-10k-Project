using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettlementQuestDisplay : QuestDisplay
{
    [Space]
    [Header("Buttons")]
    [SerializeField] Button acceptButton;
    [SerializeField] Button abandonButton;
    [SerializeField] Button handInButton;
    

    public override void ConfirmAbandon()
    {
        Debug.Log("settlement quest");
        questObject.SetState(QuestObject.QuestState.Completed);
        RemoveButtonAbandon();
        RemoveButtonHandIn();
        AddButtonAccept();

        abandonVerification.SetActive(false);
        parentQuestBoard.RemoveQuestFromBoard(this, QuestBoard.RemoveType.Remove);
        otherQuestBoard.RemoveQuestFromBoard(questObject.GetCorrespondingPlayerQuestDisplayUI(), QuestBoard.RemoveType.Remove);


        //MIGHT BE DONE?
        // remove the quest from the settlement quest list
        // call a method from the quest board
        // update player quest board
    }

    public void ButtonAcceptQuest()
    {
        Debug.Log("Quest accepted");
        questObject.SetState(QuestObject.QuestState.InProgress);
        RemoveButtonAccept();
        AddButtonHandIn();
        AddButtonAbandon();

        parentQuestBoard.UpdateQuests();
        correspondingDisplay = otherQuestBoard.AddQuestToBoard(this.questObject);
    }


    public void ButtonHandInQuest()
    {
        Debug.Log("hand in quest");
        questObject.SetState(QuestObject.QuestState.Completed);
        RemoveButtonAbandon();
        RemoveButtonHandIn();
        AddButtonAccept();

        

        parentQuestBoard.RemoveQuestFromBoard(this, QuestBoard.RemoveType.Hand_In);
        otherQuestBoard.RemoveQuestFromBoard(questObject.GetCorrespondingPlayerQuestDisplayUI(), QuestBoard.RemoveType.Hand_In);

        Debug.Log(questObject.GetResourceCount());

        parentQuestBoard.playerInventory.RemoveItemFromInventory(questObject.GetResource(), questObject.GetResourceCount());
        if (questObject.name != "TuToQuest")
            Destroy(questObject);
        


        parentQuestBoard.UpdateQuests();
        otherQuestBoard.UpdateQuests();
    }

    

    

    #region _Buttons_
    public void AddButtonAccept() 
    { 
        acceptButton.gameObject.SetActive(true);
    }
    public void RemoveButtonAccept() 
    {
        acceptButton.gameObject.SetActive(false); 
    }
    public void AddButtonAbandon() 
    { 
        abandonButton.gameObject.SetActive(true);
    }
    public void RemoveButtonAbandon() 
    { 
        abandonButton.gameObject.SetActive(false); 
    }
    public void AddButtonHandIn() 
    { 
        handInButton.gameObject.SetActive(true);
    }
    public void RemoveButtonHandIn() 
    { 
        handInButton.gameObject.SetActive(false); 
    }

    #endregion

}
