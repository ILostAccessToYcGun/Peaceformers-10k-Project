using System.Reflection;
using UnityEngine;

public class Quest : MonoBehaviour
{
    /*//Quests have a 
     * time limit
     * complete percentage
     * parent settlement
     * random material required
     * completed bool

     */
    [Header("Other Objects")]
    public Settlement parentSettlement;
    public DayNightCycleManager time;
    public CalendarManger calendar;
    [Space]
    [Header("Completion")]
    [SerializeField] bool isCompleted;
    [SerializeField] int resourceCount;
    [SerializeField] int resourcesRequired;
    [SerializeField] float completionPercentage;
    [SerializeField] Item.Name desiredResource;
    [Space]
    [Header("Time")]  
    [SerializeField] int dayDue;
    [SerializeField] Vector2 timeDue;       //x is hour, y is minutes

    #region _Parent_Settlement_

    public void SetParentSettlement(Settlement newSettlement) { parentSettlement = newSettlement; }
    public Settlement GetParentSettlement() { return parentSettlement; }

    #endregion

    #region _Completion_

    public void ToggleCompletionStatus(bool toggle)
    {
        isCompleted = toggle;
        //TODO: update UI here
    }

    public void SetResourceCount(int _resourceCount) { resourceCount = _resourceCount; }
    public int GetResourceCount() { return resourceCount; }

    public void SetResourceRequirement(int _resourcesRequired) { resourcesRequired = _resourcesRequired; }
    public int GetResourceRequirement() { return resourcesRequired; }
    public void RandomizeResourceRequirement(int min, int max)
    {
        SetResourceRequirement(Random.Range(min, max + 1));
        Debug.Log(GetResourceRequirement());
    }

    public void SetCompletionPercentage()
    { 
        completionPercentage = (float)GetResourceCount() / (float)GetResourceRequirement(); 
        //TODO: Update UI here?
    }
    public float GetCompletionPercentage() { return completionPercentage; }

    public void SetResource(Item.Name _resourceName) { desiredResource = _resourceName; }
    public Item.Name GetResource() { return desiredResource; }
    public void RandomizeResource()
    {
        SetResource((Item.Name)Random.Range(0, 3));
        Debug.Log(GetResource());
    }

    #endregion

    #region _Time_

    public void SetDayDue(int day) { dayDue = day; }
    public int GetDayDue() { return dayDue; }
    public void RandomizeDayDue(int min, int max)
    {
        int currentDate = calendar.date;
        SetDayDue(Random.Range(currentDate + min, currentDate + max + 1));
    }


    public void SetTimeDue(Vector2 time) { timeDue = time; }
    public Vector2 GetTimeDue() { return timeDue; }

    #endregion

    public void AcceptQuest()
    {
        ToggleCompletionStatus(false);
        RandomizeResource();
        RandomizeResourceRequirement(1, 10);
        SetTimeDue(new Vector2(time.hour, time.minute));
        RandomizeDayDue(1, 3);
        //add to quest board UI

        Debug.Log("gl hf");
    }

    public void HandInQuest()
    {
        //Set settlment values
        //Clear quest UI entry
        //Die
        Debug.Log("Quest Complete POG");
    }

    public bool CheckQuestValidity()
    {
        if (calendar.date <= GetDayDue())
        {
            if (time.hour < GetTimeDue().x && time.minute < GetTimeDue().y)
            {
                Debug.Log("quest is still valid");
                return true;
            }
        }
        Debug.Log("uh oh quest gone, the people have been angered");
        return false;
    }

    private void Awake()
    {
        time = FindAnyObjectByType<DayNightCycleManager>();
        calendar = FindAnyObjectByType<CalendarManger>();
    }
}
