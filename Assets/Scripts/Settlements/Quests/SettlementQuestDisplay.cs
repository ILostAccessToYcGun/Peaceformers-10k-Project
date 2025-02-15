using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettlementQuestDisplay : QuestDisplay
{
    [SerializeField] Button acceptButton;
    [SerializeField] Button abandonButton;
    [SerializeField] Button handInButton;

    public override void ConfirmAbandon()
    {
        Debug.Log("settlement quest");
        RemoveButtonAbandon();
        RemoveButtonHandIn();
        AddButtonAccept();

        abandonVerification.SetActive(false);
        questBoard.RemoveQuest(this);

        // remove the quest from the settlement quest list
        // update player quest board
    }

    public void ButtonAcceptQuest()
    {
        Debug.Log("Quest accepted");
        RemoveButtonAccept();
        AddButtonHandIn();
        AddButtonAbandon();

        // update player quest board
    }


    public void ButtonHandInQuest()
    {
        Debug.Log("hand in quest");
        RemoveButtonAbandon();
        RemoveButtonHandIn();
        AddButtonAccept();

        // remove the quest from the settlement quest list
        // update player quest board

        questBoard.RemoveQuest(this);
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
