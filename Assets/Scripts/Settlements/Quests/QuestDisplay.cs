using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestDisplay : MonoBehaviour
{
    public QuestObject questObject;
    public QuestBoard parentQuestBoard; //this is the quest board that the quest display belongs to
    public QuestBoard otherQuestBoard; 
    public Image settlementIcon;
    public TextMeshProUGUI description;
    public Slider completionSlider;
    public Image sliderFill;
    public TextMeshProUGUI completionPercentage;
    public TextMeshProUGUI daysLeft;
    [SerializeField] protected GameObject abandonVerification;
    [SerializeField] Button abandonConfirmButton;

    [SerializeField] public QuestDisplay correspondingDisplay;


    public void SetUpQuestDisplay()
    {
        SetIcon();
        if (questObject != null)
        {
            if (questObject.GetDescription() != "")
                description.text = questObject.GetDescription();
            else
                description.text = "description is empty";
        } 
        else
            description.text = "No questObject found.";
        UpdateSliderUI();
        UpdateDaysLeft();
    }

    public void SetQuestObject(QuestObject newObject)
    {
        questObject = newObject;
        SetUpQuestDisplay();
    }

    public void SetParentQuestBoard(QuestBoard newBoard)
    {
        parentQuestBoard = newBoard;
    }

    public void SetOtherQuestBoard(QuestBoard newBoard)
    {
        otherQuestBoard = newBoard;
    }

    public void SetIcon()
    {
        if (settlementIcon != null)
            settlementIcon.sprite = questObject.parentSettlement.GetIcon();
    }

    #region _Button_
    public void ButtonAbandonedQuest()
    {
        Debug.Log("abandon");
        abandonVerification.SetActive(true);
        abandonConfirmButton.onClick.RemoveAllListeners();
        abandonConfirmButton.onClick.AddListener(this.ButtonConfirmAbandon_onClick);
        
    }

    //Handle the onClick event
    protected void ButtonConfirmAbandon_onClick()
    {
        ConfirmAbandon();
    }

    public virtual void ConfirmAbandon()
    {
        Debug.Log("quest, not settlement");
        abandonVerification.SetActive(false);
        parentQuestBoard.RemoveQuestFromBoard(this, QuestBoard.RemoveType.Remove);
        otherQuestBoard.RemoveQuestFromBoard(questObject.GetCorrespondingSettlementQuestDisplayUI(), QuestBoard.RemoveType.Remove);

        //update the settlement quest giver to not hold the quest anymore
        //do the upkeep damage
    }

    

    public void SetAbandonVerification(GameObject newVerification) { abandonVerification = newVerification; }
    public void SetAbandonConfirmButton(Button newButton) { abandonConfirmButton = newButton; }

    #endregion

    public void UpdateSliderUI()
    {
        float percent = (float)questObject.GetResourceCount() / (float)questObject.GetResourceRequirement();
        completionSlider.value = percent;

        sliderFill.color = new Color(Mathf.Clamp(50f / 255f + (1 - percent), 0, 1), Mathf.Clamp(50f/255f + percent, 0, 1), 0, 1);


        if (percent == 1)
            completionPercentage.text = "Complete (100%)";
        else
            completionPercentage.text = (Mathf.Floor(percent * 1000f) / 10f).ToString() + "%";
    }

    public void UpdateDaysLeft()
    {
        Debug.Log(questObject.GetDayDue());
        questObject.correspondingPlayerQuestDisplayUI.daysLeft.text = (questObject.GetDayDue() - questObject.calendar.GetDayCount()).ToString();
        if (questObject.correspondingSettlementQuestDisplayUI != null)
            questObject.correspondingSettlementQuestDisplayUI.daysLeft.text = (questObject.GetDayDue() - questObject.calendar.GetDayCount()).ToString();
    }


}
