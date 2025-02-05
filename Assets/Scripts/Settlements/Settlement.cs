using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    [SerializeField] private TextMeshProUGUI nametag;
    [Space]
    [SerializeField] private Color goodHealthColor;
    [SerializeField] private Color watchOutColor;
    [SerializeField] private Color criticalColor;
    [Space]
    [Header("Interaction")]
    [SerializeField] private Transform player;
    [SerializeField] private float playerDetectionRange;
    [SerializeField] private InteractionPrompt interactionPrompt;
    [Space]
    [SerializeField] private GameObject interactionPromptImage;
    [SerializeField] private Image interactionFill;
    [SerializeField] private bool requestedInteract;
    [SerializeField] private bool initializeAction;

    void Start()
    {
        nametag.text = settlementName;
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
        Interaction();
    }

    void Interaction()
    {
        if (Vector3.Distance(transform.position, player.position) <= playerDetectionRange && !initializeAction)
        {
            requestedInteract = true;
            interactionPrompt.RequestInteraction(1f, ref interactionPromptImage, ref interactionFill, () => GainMeter(20));
        }
        else if (Vector3.Distance(transform.position, player.position) <= playerDetectionRange && requestedInteract)
        {
            interactionPrompt.DisableInteraction();
            requestedInteract = false;
            initializeAction = false;
        }
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
}
