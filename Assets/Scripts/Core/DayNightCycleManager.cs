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
    public bool updateTime = true;
    [Space]
    public CalendarManger cm;

    public enum twelveHour { AM, PM }
    public twelveHour twelveHourClock = twelveHour.AM;
    [Space]
    public float time;
    public float totalTime;

    public int hour = 6;
    public int minute;
    [Space]
    public int UIUpdateFrequency;
    public TextMeshProUGUI timeUI;
    public Light sunLight;
    public float sunLightAngle;

    public Light moonLight;
    public float moonLightAngle;

    [Space]
    [Header("Inbetween")]
    public Image dayEndPanel;

    [Space]
    [Header("Other Elements")]
    [SerializeField] QuestBoard playerQuestBoard;
    [SerializeField] List<Settlement> settlements;

    public void DisableTime() { updateTime = false; }
    public void EnableTime() {  updateTime = true; }

    public void DayEndPanel(bool enable)
    {
        DisableTime();
        dayEndPanel.gameObject.SetActive(enable);
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

    public void UpdateLightIntensity()
    {
        //360 minutes = 0.5
        //720 minutes = 1
        //1080 minutes = 0.5 1.5
        //1440 minutes = 0   2

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
        //360 minutes = 180 degrees 6am
        //720 minutes = 90 degrees 12pm
        //1080 minutes = 0 degrees 6pm
        //1440 minutes = -90 degrees 6pm

        sunLightAngle = Mathf.Lerp(sunLightAngle, ((totalTime / 1440f) * -360f) - 90f, Time.deltaTime);
        sunLight.gameObject.transform.localEulerAngles = new Vector3(sunLightAngle, 90f, 0);
        
        moonLightAngle = Mathf.Lerp(moonLightAngle, ((totalTime / 1440f) * -360f) - 270f, Time.deltaTime);
        moonLight.gameObject.transform.localEulerAngles = new Vector3(moonLightAngle, 90f, 0);
    }

    public void BeginDay()
    {
        SetTime(6, 0);
        moonLight.intensity = 0;
        sunLight.intensity = 0.7f;
    }
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
        //totalTime++;
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
        {
            UpdateTimeUI();
        }
        EndOfDayCheck();
    }

    public void EndOfDayCheck()
    {
        if (hour == 12 && twelveHourClock == twelveHour.AM)
        {
            BeginDay();
            cm.IncrementDayCount();
            //do something like upgrades

            WorldItem[] itemsOnTheGround = FindObjectsByType<WorldItem>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            for (int i = 0; i < itemsOnTheGround.Length; i++)
            {
                Destroy(itemsOnTheGround[i].gameObject);
            }
            

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

            foreach (Settlement settlement in settlements)
            {
                if (settlement.currentlyEndangered)
                {
                    settlement.panicEnemiesGO = true;
                    settlement.currentlyEndangered = false;
                }
            }

            //spawn Settlement enemies
        }
            
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
            UpdateLightAngle();
            UpdateLightIntensity();
            time += Time.deltaTime;
        }
        
    }

    private void Awake()
    {
        cm = FindAnyObjectByType<CalendarManger>();
        BeginDay();

        if (UIUpdateFrequency == 0)
        {
            UIUpdateFrequency = 20;
        }
        
    }
}
