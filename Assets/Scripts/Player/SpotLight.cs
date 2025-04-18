using UnityEngine;

public class SpotLight : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] Light gunLight;
    [SerializeField] DayNightCycleManager dayManager;

    private void Update()
    {
        if (dayManager.hour < 8 && dayManager.twelveHourClock == DayNightCycleManager.twelveHour.AM
            || dayManager.hour >= 5 && dayManager.twelveHourClock == DayNightCycleManager.twelveHour.PM)
            gunLight.intensity = 10;
        else
            gunLight.intensity = 0;
    }
}
