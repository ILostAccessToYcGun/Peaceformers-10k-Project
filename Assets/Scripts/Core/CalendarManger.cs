using TMPro;
using UnityEngine;

public class CalendarManger : MonoBehaviour
{
    public int date; //current date 1st, 13th, etc
    public int endDate; // the day the game ends 31st

    public enum Month { January, February, March, April, May, June, July, August, September, October, November, December }
    public Month currentMonth = Month.November;
    public TextMeshProUGUI dateText;


    public void CheckIfEndDayCountReached()
    {
        if (date > endDate)
        {
            Debug.Log("you won, go to game manager");
        }
    }

    public void UpdateDateText()
    {
        string suffix = "";
        if (date == 1 || date == 21 || date == 31)
            suffix = "st";
        else if (date == 2 || date == 22)
            suffix = "nd";
        else if (date == 3 || date == 23)
            suffix = "rd";
        else
            suffix = "th";

        dateText.text = currentMonth + " " + date + suffix;
    }

    public void SetDayCount(int count)
    { 
        date = count;
        UpdateDateText();
    }
    public void IncrementDayCount() 
    { 
        date++;
        UpdateDateText();
        CheckIfEndDayCountReached();
    }
    public void SetEndDayCount(int count) { endDate = count; }





    private void Awake()
    {
        SetEndDayCount(14);
        SetDayCount(1);
    }
}
