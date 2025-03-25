using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : Healthbar
{
    [SerializeField] PlayerBattery pb;
    [SerializeField] float damageToEnergyLoss;
    [SerializeField] public float M_damageToEnergyLoss;

    public override void Start()
    {
        base.Start();
        M_damageToEnergyLoss = damageToEnergyLoss;
    }

    public override void LoseHealth(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth - amount, 0, M_maxHealth);
        pb.LoseBattery(amount * M_damageToEnergyLoss);
        UpdateHealthbar();

        if (currentHealth <= 0)
        {
            pb.dayNightCycleManager.EndDay();
        }
    }

}
