using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

[CreateAssetMenu(fileName = "New Quest", menuName = "Quest")]

public class QuestObject : ScriptableObject
{
    public enum QuestState { Avalible, InProgress, Completed }

    [Header("Other Objects")]
    public Settlement parentSettlement;
    public QuestGiver parentQuestGiver;
    public DayNightCycleManager time;
    public CalendarManger calendar;
    public QuestDisplay correspondingPlayerQuestDisplayUI;
    public QuestDisplay correspondingSettlementQuestDisplayUI;
    [Space]
    [Header("Completion")]
    [SerializeField] public QuestState state;
    [SerializeField] int resourceCount;
    [SerializeField] int resourcesRequired;
    [SerializeField] float completionPercentage;
    [SerializeField] Item.Name desiredResourceName;
    [SerializeField] float upKeepGain;
    [Space]
    [Header("Time")]  
    [SerializeField] int dayDue;
    [SerializeField] Vector2 timeDue;       //x is hour, y is minutes
    [Space]
    [Header("QuestDisplay")]
    [SerializeField] string description;
    [Space]
    [SerializeField] bool isTuto = false;


    #region _Other_Objects_

    public void SetParentSettlement(Settlement newSettlement) { parentSettlement = newSettlement; SetDescription(); }
    public Settlement GetParentSettlement() { return parentSettlement; }

    public void SetParentQuestGiver(QuestGiver newQuestGiver) { parentQuestGiver = newQuestGiver; }
    public QuestGiver GetParentQuestGiver() { return parentQuestGiver; }

    public void SetTimeManager() { time = FindAnyObjectByType<DayNightCycleManager>(); ; }
    public DayNightCycleManager GetTimeManager() { return time; }

    public void SetCalenderManager() { calendar = FindAnyObjectByType<CalendarManger>(); }
    public CalendarManger GetCalenderManager() { return calendar; }

    public void SetCorrespondingPlayerQuestDisplayUI(QuestDisplay newDisplay) { correspondingPlayerQuestDisplayUI = newDisplay; }
    public QuestDisplay GetCorrespondingPlayerQuestDisplayUI() { return correspondingPlayerQuestDisplayUI; }

    public void SetCorrespondingSettlementQuestDisplayUI(QuestDisplay newDisplay) { correspondingSettlementQuestDisplayUI = newDisplay; }
    public QuestDisplay GetCorrespondingSettlementQuestDisplayUI() { return correspondingSettlementQuestDisplayUI; }

    #endregion

    #region _Completion_

    public void SetState(QuestState newState)
    {
        state = newState;
        //TODO: update UI here ???
    }
   
    public void SetResourceCount(int _resourceCount) { resourceCount = _resourceCount; }
    public int GetResourceCount() { return resourceCount; }

    public void SetResourceRequirement(int _resourcesRequired) { resourcesRequired = _resourcesRequired; }
    public int GetResourceRequirement() { return resourcesRequired; }
    public void RandomizeResourceRequirement(int min, int max)
    {
        SetResourceRequirement(Random.Range(min, max + 1));
        //Debug.Log(GetResourceRequirement());
    }

    public void SetCompletionPercentage()
    { 
        completionPercentage = (float)GetResourceCount() / (float)GetResourceRequirement(); 
        //TODO: Update UI here?
    }
    public float GetCompletionPercentage() { return completionPercentage; }

    public void SetResource(Item.Name _resourceName) { desiredResourceName = _resourceName; }
    public Item.Name GetResource() { return desiredResourceName; }
    public void RandomizeResource()
    {
        SetResource((Item.Name)Random.Range(0, 3));
        //Debug.Log(GetResource());
    }


    public void SetUpKeepGain(float newGain) { upKeepGain = newGain; }
    public float GetUpKeepGain() { return upKeepGain; }

    public void RandomizeGain(float minGain, float maxGain) // in %
    {
        SetUpKeepGain((Mathf.Floor(Random.Range(minGain * 10f, maxGain * 10f))) / 10f);
        //Debug.Log(GetUpKeepGain());
    }

    #endregion

    #region _Time_

    public void SetDayDue(int day) { dayDue = day; }
    public int GetDayDue() { return dayDue; }
    public void RandomizeDayDue(int min, int max)
    {
        if (calendar == null) { return; }
        int currentDate = calendar.date;
        SetDayDue(Random.Range(currentDate + min, currentDate + max + 1));
    }


    public void SetTimeDue(Vector2 time) { timeDue = time; }
    public Vector2 GetTimeDue() { return timeDue; }

    #endregion

    #region _Description_

    public void SetDescription()
    {
        if (parentSettlement == null) { return; }
        string suffix = "";
        if (dayDue == 1 || dayDue == 21 || dayDue == 31)
            suffix = "st";
        else if (dayDue == 2 || dayDue == 22)
            suffix = "nd";
        else if (dayDue == 3 || dayDue == 23)
            suffix = "rd";
        else
            suffix = "th";

        description = "Collect " + resourcesRequired + " " + desiredResourceName + " and Deliver to " + parentSettlement.GetSettlementName()
            + " before the " + dayDue + suffix + " of November.\n\n" + "Will add " + GetUpKeepGain() +  "% to the settlement's upkeep meter";

        //description = "Collect " + resourcesRequired + " " + desiredResourceName + " and Deliver to " + parentSettlement.GetSettlementName()
        //    + " before " + timeDue.x + ":" + (timeDue.y < 10 ? "0" : "") + timeDue.y + " on the " + dayDue + suffix + " of November.\n\n" +
        //    "Will add " + GetUpKeepGain() + "% to the settlement's upkeep meter";
    }

    public string GetDescription() { return description; }

    #endregion


    public void SetUpQuest(int minResource, int maxResource)
    {
        //SetParentSettlement();
        SetState(QuestState.Avalible);
        RandomizeResource();
        RandomizeResourceRequirement(minResource, maxResource);
        RandomizeDayDue(3, 4);
        RandomizeGain(10, 30);  
        SetDescription(); //kinda pointless here
    }

    public void AcceptQuest() //I dont think the accept quest stuff should be handled here
    {
        //SetTimeDue(new Vector2(time.hour, time.minute));
        //RandomizeDayDue(1, 4);
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
        if (calendar.date < GetDayDue())
        {
            Debug.Log("quest is still valid");
            return true;
            
        }
        else if (calendar.date == GetDayDue())
        {
            if (time.hour < GetTimeDue().x && time.minute < GetTimeDue().y)
            {
                Debug.Log("you're cutting it close now buddy");
                return true;
            }
        }
        Debug.Log("uh oh quest gone, the people have been angered");
        return false;
    }

    private void Awake()
    {
        Debug.Log("Roo");
        SetTimeManager();
        SetCalenderManager();
        if (!isTuto)
        {
            SetUpQuest(1, 10);
        }
        
        
    }
}
