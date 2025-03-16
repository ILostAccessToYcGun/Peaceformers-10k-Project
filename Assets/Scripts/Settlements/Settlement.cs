using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Settlement : BaseInteractable
{
    [Header("Basics")]
    [SerializeField] private string settlementName;
    [SerializeField] private float currentUpkeep;
    private float maxUpkeep = 100;

    [SerializeField] private float lossRate;
    [Space]
    [SerializeField] private float stayTime = 3f; //for when you drop off supplies
    private bool stationary = false;
    private float currentStayTime = 0;

    [Space]
    [Header("UI")]
    [SerializeField] private Image upkeepMeter;
    [SerializeField] private TextMeshProUGUI nametag;
    [Space]
    [SerializeField] private Color goodHealthColor;
    [SerializeField] private Color watchOutColor;
    [SerializeField] private Color criticalColor;
    [SerializeField] private QuestGiver questGiver;


    [SerializeField] public int panicEnemies;
    [SerializeField] public bool currentlyEndangered;



    protected override void Awake()
    {
        base.Awake();
        questGiver = GetComponent<QuestGiver>();
    }
    protected void Start()
    {
        nametag.text = settlementName;
        transform.name = settlementName;
        currentUpkeep = maxUpkeep;
    }

    protected override void Update()
    {
        if (!stationary) 
        {
            currentStayTime = 0;
            if (!currentlyEndangered)
                currentUpkeep -= lossRate * Time.deltaTime;
        }
        else
        { 
            currentStayTime += Time.deltaTime;
            if(currentStayTime >= stayTime)
            {
                stationary = false;
            }
        }

        currentUpkeep = Mathf.Clamp(currentUpkeep, 0, maxUpkeep);
        Upkeepbar();
        base.Update();
    }

    void Upkeepbar()
    {
        float barRatio = currentUpkeep / maxUpkeep;

        upkeepMeter.fillAmount = barRatio;

        if (currentUpkeep > 50)
            upkeepMeter.color = goodHealthColor;
        else if (currentUpkeep > 20)
            upkeepMeter.color = watchOutColor;
        else //critical
            upkeepMeter.color = criticalColor;

        CheckEndangerment();
    }

    public void GainMeter(float value)
    {
        currentUpkeep = Mathf.Clamp(currentUpkeep + value, 0, maxUpkeep);
        stationary = true;
    }

    public void LoseMeter(float value)
    {
        currentUpkeep = Mathf.Clamp(currentUpkeep - value, 0, maxUpkeep);
        stationary = true;
    }

    protected override void OpenPrompt()
    {
        print("Open QuestGiver prompt");
        questGiver.ToggleSettlementQuestBoardVisibility();
    }

    protected void CheckEndangerment()
    {
        if (panicEnemies > 0) { return; } //if we got enemies we dont care about the endagerment, because the player will fix that. right?
        //this gets set to false after the panic enemies are defeated AND the settlement is back to above 25%
        //the DayManager is where this gets set. 

        if (currentUpkeep <= 25)
        {
            currentlyEndangered = true;
        }
        else
        {
            currentlyEndangered = false;
        }
    }

    public string GetSettlementName() { return settlementName; }
    public float GetCurrentUpKeep() { return currentUpkeep; }
    public float GetMaxUpKeep() { return maxUpkeep; }
}
