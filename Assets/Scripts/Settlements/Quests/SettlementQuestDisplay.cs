using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettlementQuestDisplay : QuestDisplay
{
    [SerializeField] Button acceptButton;
    [SerializeField] Button abandonButton;
    [SerializeField] Button handInButton;

    public override void ButtonAbandonedQuest()
    {
        Debug.Log("abandon");
        RemoveButtonAbandon();
        RemoveButtonHandIn();
        AddButtonAccept();

        //TESTING//
        //questObject.SetResourceCount(questObject.GetResourceCount() + 1);
        //UpdateSliderUI();

        questBoard.RemoveQuest(this);
    }

    public void ButtonHandInQuest()
    {
        Debug.Log("hand in quest");
        RemoveButtonAbandon();
        RemoveButtonHandIn();
        AddButtonAccept();

        //TESTING//
        //questObject.SetResourceCount(questObject.GetResourceCount() + 1);
        //UpdateSliderUI();

        questBoard.RemoveQuest(this);
    }

    public void ButtonAcceptQuest()
    {
        Debug.Log("Quest accepted");
        RemoveButtonAccept();
        AddButtonHandIn();
        AddButtonAbandon();
    }

    #region _Buttons_
    public void AddButtonAccept() { acceptButton.gameObject.SetActive(true); }
    public void RemoveButtonAccept() { acceptButton.gameObject.SetActive(false); }
    public void AddButtonAbandon() { abandonButton.gameObject.SetActive(true); }
    public void RemoveButtonAbandon() { abandonButton.gameObject.SetActive(false); }
    public void AddButtonHandIn() { handInButton.gameObject.SetActive(true); }
    public void RemoveButtonHandIn() { handInButton.gameObject.SetActive(false); }

    #endregion

}
