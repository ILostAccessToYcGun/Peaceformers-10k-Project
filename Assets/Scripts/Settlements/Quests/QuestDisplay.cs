using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestDisplay : MonoBehaviour
{
    public QuestObject questObject;
    public QuestBoard questBoard;
    public Image settlementIcon;
    public TextMeshProUGUI description;
    public Slider completionSlider;
    public Image sliderFill;
    public TextMeshProUGUI completionPercentage;
    public TextMeshProUGUI daysLeft;
    [SerializeField] protected GameObject abandonVerification;
    [SerializeField] Button abandonConfirmButton;


    public void SetUpQuestDisplay()
    {
        //settlementIcon = questObject.parentSettlement.icon
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

    public void SetQuestBoard(QuestBoard newBoard)
    {
        questBoard = newBoard;
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
        questBoard.RemoveQuest(this);
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
        daysLeft.text = (questObject.GetDayDue() - questObject.calendar.GetDayCount()).ToString();
    }


}
