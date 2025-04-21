using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DayNightCycleManager : MonoBehaviour
{

    // 7 mins per day

    //420s per day, 24hrs/day
    //17.5s per hour
    //2.91666s per 10 minutes
    //0.29166s per minute
    
    [Space]
    [Header("Managers")]
    [SerializeField] CalendarManger cm;
    [SerializeField] MapDirector md;
    [SerializeField] EnemyDirector ed;
    [SerializeField] PlayerUIToggler ui;
    [SerializeField] PlayerUpgrades up;

    public enum twelveHour { AM, PM }
    
    [Space]
    [Header("Time")]
    [SerializeField] bool updateTime = true;
    [SerializeField] twelveHour twelveHourClock = twelveHour.AM;
    [SerializeField] float time;
    [SerializeField] float totalTime;
    [SerializeField] public int hour = 6;
    [SerializeField] public int minute;

    [Space]
    [Header("User Interface")]
    [SerializeField] TextMeshProUGUI timeUI;
    [SerializeField] int UIUpdateFrequency;

    [Space]
    [Header("Light")]
    [SerializeField] Light sunLight;
    [SerializeField] float sunLightAngle;
    [SerializeField] Light moonLight;
    [SerializeField] float moonLightAngle;

    [Space]
    [Header("Inbetween")]
    [SerializeField] Image dayEndPanel;

    [Space]
    [Header("Other Elements")]
    [SerializeField] QuestBoard playerQuestBoard;
    [SerializeField] List<Settlement> settlements;
    [SerializeField] Camp playerCamp;
    [SerializeField] Inventory playerInventory;
    [SerializeField] float addQuestCoolDownTimer; //make this private
    [SerializeField] float addQuestCoolDown;

    #region _Time_

    public void ToggleTime(bool toggle) { updateTime = toggle; }

    public void SetTime(int _hour, int _minute)
    {
        hour = _hour;
        minute = _minute;
        totalTime = ((twelveHourClock == twelveHour.PM ? 12 : 0) + hour * 60) + minute;
        sunLightAngle = ((totalTime / 1440f) * -360f) - 90f;
        sunLight.gameObject.transform.localEulerAngles = new Vector3(sunLightAngle, 90f, 0);


        moonLightAngle = ((totalTime / 1440f) * -360f) - 270f;
        moonLight.gameObject.transform.localEulerAngles = new Vector3(moonLightAngle, 90f, 0);
        UpdateTimeUI();
    }

    public void UpdateTime()
    {
        minute++;
        if (minute >= 60)
        {
            minute = 0;
            hour++;
            if (hour > 12)
                hour = 1;

            if (hour >= 12)
            {
                if (twelveHourClock == twelveHour.AM)
                    twelveHourClock = twelveHour.PM;
                else
                    twelveHourClock = twelveHour.AM;
            }
        }
        if (minute % UIUpdateFrequency == 0)
            UpdateTimeUI();
        EndOfDayCheck();
    }

    #endregion

    #region _UI_

    public void DayEndPanel(bool enable)
    {
        if (enable)
            ui.BackOutOfCurrentUI();
        ToggleTime(!enable);
        dayEndPanel.gameObject.SetActive(enable);
        ui.SetUIOpenBool(enable);

        if (!enable)
        {
            up.ClearBlacklist();
            FindAnyObjectByType<PlayerHealthBar>().dead = false;
        }
    }

    public void UpdateTimeUI()
    {
        string hourZero;
        string minuteZero;

        if (hour < 10)
            hourZero = "0";
        else
            hourZero = null;

        if (minute < 10)
            minuteZero = "0";
        else
            minuteZero = null;

        timeUI.text = hourZero + hour + ":" + minuteZero + minute + twelveHourClock;
    }

    #endregion

    #region _Light_
    public void UpdateLightIntensity()
    {
        if (totalTime <= 1080)
        {
            sunLight.intensity = Mathf.Clamp(Mathf.Lerp(sunLight.intensity, (totalTime <= 720 ? totalTime * 1.5f / 720 : 2f - ((totalTime / 720)) * 0.5f), Time.deltaTime / 10f), 0f, 1f);
            moonLight.intensity = Mathf.Lerp(moonLight.intensity, 0, Time.deltaTime / 10f);
        }
        else
        {
            sunLight.intensity = Mathf.Lerp(sunLight.intensity, 0, Time.deltaTime / 10f);
            moonLight.intensity = Mathf.Clamp(Mathf.Lerp(moonLight.intensity, (totalTime / 720f) - 1f, Time.deltaTime / 10f), 0f, 0.75f);
        }
    }

    public void UpdateLightAngle()
    {
        sunLightAngle = Mathf.Lerp(sunLightAngle, ((totalTime / 1440f) * -360f) - 90f, Time.deltaTime);
        sunLight.gameObject.transform.localEulerAngles = new Vector3(sunLightAngle, 90f, 0);
        moonLightAngle = Mathf.Lerp(moonLightAngle, ((totalTime / 1440f) * -360f) - 270f, Time.deltaTime);
        moonLight.gameObject.transform.localEulerAngles = new Vector3(moonLightAngle, 90f, 0);
    }

    #endregion

    #region _Day_Methods_

    public void BeginDay()
    {
        PlayerHealthBar php = FindAnyObjectByType<PlayerHealthBar>();
        php.SetHealth(php.GetMaxHealth());

        SetTime(6, 0);
        addQuestCoolDownTimer = 0f;
        moonLight.intensity = 0;
        sunLight.intensity = 0.7f;
        
        md.GenerateNodes();
        ed.GenerateEnemies();
        CampEnemySpawnCheck();
    }

    public void EndOfDayCheck()
    {
        if (hour == 12 && twelveHourClock == twelveHour.AM)
            EndDay();
    }

    public void EndDay()
    {
        //we wanna run a check to see if the player is not within X distance to the camp
        if (!playerCamp.SafeDistanceCheck())
        {
            //if the player is outside the distance range, randomly remove half their items
            playerInventory.RemoveHalfInventory();
            FindAnyObjectByType<PlayerMovement>().ResetPos();
        }

        if (cm.IncrementDayCount())
            return;

        if (!(hour == 12 && twelveHourClock == twelveHour.AM))
        {
            float difference = 1465f - totalTime;
            Debug.Log(difference * 0.2f);
            foreach (Settlement set in settlements)
            {
                //1465 total time
                //lose 0.06 upkeep per second
                //3.428649798 minutes per second
                //0.2057189879 loss per minute
                //-0.2 per minute

                if (!set.LoseMeter(difference * 0.01f))
                    return;
            }
        }

        DayEndPanel(true);
        up.SelectSemiRandomUpgrades();

        ed.AddEnemyCountEntry();
        ed.IncreaseDifficulty();
        md.DestroyWorldItems();
        
        List<QuestDisplay> currentQuests = playerQuestBoard.GetQuests();
        for (int i = 0; i < currentQuests.Count; i++)
        {
            currentQuests[i].UpdateDaysLeft();
            if (!currentQuests[i].questObject.CheckQuestValidity())
            {
                //quest has expired, immediatly abandon it
                currentQuests[i].otherQuestBoard.RemoveQuestFromBoard(currentQuests[i].questObject.GetCorrespondingSettlementQuestDisplayUI(), QuestBoard.RemoveType.Remove);
                currentQuests[i].parentQuestBoard.RemoveQuestFromBoard(currentQuests[i].questObject.GetCorrespondingPlayerQuestDisplayUI(), QuestBoard.RemoveType.Remove);
                i--;
            }
        }
        SettlementEnemySpawnCheck();

        //we should probably reduce each settlement's upkeep by the amount they would have lost if we end early, so we dont scam the system

        
        
    }

    public void SettlementEnemySpawnCheck()
    {
        GameObject highestUpkeep = settlements[0].gameObject;
        float highestUpkeepCount = 0;
        foreach (Settlement settlement in settlements)
        {
            if (settlement.GetCurrentUpKeep() > highestUpkeepCount)
            {
                highestUpkeepCount = settlement.GetCurrentUpKeep();
                highestUpkeep = settlement.gameObject;
            }
        }

        foreach (Settlement settlement in settlements)
        {
            if (settlement.currentlyEndangered)
            {
                settlement.currentlyEndangered = false;
                settlement.panicEnemies += ed.SpawnSettlementEnemeies(settlement.gameObject, highestUpkeep);
            }

            foreach (Settlement otherSettlement in settlements)
            {
                if (otherSettlement != settlement)
                {
                    if (otherSettlement.GetCurrentUpKeep() - settlement.GetCurrentUpKeep() >= 30f)
                        settlement.panicEnemies += ed.SpawnSettlementEnemeies(settlement.gameObject, otherSettlement.gameObject);
                }
            }
        }
    }

    public void CampEnemySpawnCheck()
    {
        EnemyCamp[] camps = FindObjectsByType<EnemyCamp>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        
        foreach (EnemyCamp camp in camps)
        {
            camp.UpdateCamp();
        }
    }

    #endregion

    public void AllocateQuests()
    {
        List<Settlement> whiteListQuestAdd = new List<Settlement>();
        List<int> questCounts = new List<int>();
        for (int i = 0; i < settlements.Count; ++i) //loop through the settlements
        {
            questCounts.Add(settlements[i].questGiver.quests.Count);
        }

        int smallest = questCounts[0];
        foreach (int num in questCounts)
        {
            if (num < smallest)
                smallest = num;
        }

        for (int i = 0; i < questCounts.Count; ++i)
        {
            if (questCounts[i] - smallest <= 1)
                whiteListQuestAdd.Add(settlements[i]); 
        }

        Settlement settlementRecievingQuest = whiteListQuestAdd[Random.Range(0, whiteListQuestAdd.Count)];
        settlementRecievingQuest.questGiver.AddQuestToGiver();
    }

    private void Update()
    {
        if (updateTime)
        {
            if (time >= 0.29166f)
            {
                UpdateTime();
                time = 0;
            }
            totalTime += Time.deltaTime / 0.29166f;
            addQuestCoolDownTimer -= Time.deltaTime / 0.29166f;
            if (addQuestCoolDownTimer <= 0)
            {
                AllocateQuests();
                addQuestCoolDownTimer = addQuestCoolDown;
            }
            UpdateLightAngle();
            UpdateLightIntensity();
            time += Time.deltaTime;
        }
    }

    public void InitializeManger()
    {
        cm = FindAnyObjectByType<CalendarManger>();
        md = FindAnyObjectByType<MapDirector>();
        ed = FindAnyObjectByType<EnemyDirector>();
        ed.ResetDifficulty();
        ui = FindAnyObjectByType<PlayerUIToggler>();
        up = FindAnyObjectByType<PlayerUpgrades>();

        md.GenerateCamps();
        BeginDay();

        if (UIUpdateFrequency == 0)
            UIUpdateFrequency = 20;
    }

    private void Awake()
    {
        SetTime(6, 0);
        //InitializeManger();

        GameObject.FindObjectOfType<AudioManager>().Play("Ambience");
        GameObject.FindObjectOfType<AudioManager>().Play("Game_Music");
    }
}
