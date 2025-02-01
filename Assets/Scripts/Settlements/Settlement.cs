using UnityEngine;
using UnityEngine.UI;

public class Settlement : MonoBehaviour
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
    [Space]
    [SerializeField] private Color goodHealthColor;
    [SerializeField] private Color watchOutColor;
    [SerializeField] private Color criticalColor;

    void Start()
    {
        transform.name = settlementName;
        currentUpkeep = maxUpkeep;
    }

    void Update()
    {
        if (!stationary) 
        {
            currentStayTime = 0;
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
    }

    private void Upkeepbar()
    {
        float barRatio = currentUpkeep / maxUpkeep;

        upkeepMeter.fillAmount = barRatio;

        if (currentUpkeep > 50)
            upkeepMeter.color = goodHealthColor;
        else if (currentUpkeep > 20)
            upkeepMeter.color = watchOutColor;
        else //critical
            upkeepMeter.color = criticalColor;
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

    //temp collection
    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.CompareTag("Player"))
        {
            GainMeter(20);
        }
    }
}
