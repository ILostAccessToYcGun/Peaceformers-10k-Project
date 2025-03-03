using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBattery : MonoBehaviour
{
    [SerializeField] private RectTransform mainBar;
    [SerializeField] private Image batteryFill;
    [SerializeField] private Image batteryLoss;
    [SerializeField] private float lossLerpSpeed = 2f;
    [Space]
    private float maxBattery = 100f;
    [SerializeField] private float currentBattery;

    [Space]
    [SerializeField] private Color goodHealthColor;
    [SerializeField] private Color watchOutColor;
    [SerializeField] private Color criticalColor;

    [Space]
    [SerializeField] private DayNightCycleManager dayNightCycleManager;


    void Start()
    {
        currentBattery = maxBattery;
        UpdateBattery();
    }

    void Update()
    {
        //LoseBattery(0.29166f);
        LoseBattery(0.29166f/100f/2f);

        if (currentBattery <= 0f)
        {
            //end day
            dayNightCycleManager.DayEndPanel(true);
        }
    }

    public void SetBattery(float newHealth)
    {
        currentBattery = Mathf.Clamp(newHealth, 0, maxBattery);
        UpdateBattery();
    }

    public void LoseBattery(float amount)
    {
        currentBattery = Mathf.Clamp(currentBattery - amount, 0, maxBattery);
        UpdateBattery();
    }

    public void GainBattery(float amount)
    {
        currentBattery = Mathf.Clamp(currentBattery + amount, 0, maxBattery);
        UpdateBattery();
    }

    private void UpdateBattery()
    {
        float healthRatio = currentBattery / maxBattery;

        batteryFill.fillAmount = healthRatio;


        if (currentBattery > 30)
            batteryFill.color = goodHealthColor;
        else if (currentBattery > 20)
            batteryFill.color = watchOutColor;
        else
            batteryFill.color = criticalColor;

        StopAllCoroutines();
        StartCoroutine(LerpbatteryLoss(healthRatio));
    }

    private IEnumerator LerpbatteryLoss(float targetFill)
    {
        float startFill = batteryLoss.fillAmount;
        float elapsedTime = 0f;

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * lossLerpSpeed;
            batteryLoss.fillAmount = Mathf.Lerp(startFill, targetFill, elapsedTime);
            yield return null;
        }

        batteryLoss.fillAmount = targetFill;
    }
}
