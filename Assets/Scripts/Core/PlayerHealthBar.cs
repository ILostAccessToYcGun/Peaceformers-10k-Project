using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : Healthbar
{
    [SerializeField] PlayerBattery pb;
    [SerializeField] float damageToEnergyLoss;
    [SerializeField] public float M_damageToEnergyLoss;
    [SerializeField] public bool dead = false;

    public override void Start()
    {
        base.Start();
        M_damageToEnergyLoss = damageToEnergyLoss;
    }

    public override void LoseHealth(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth - amount, -0.1f, M_maxHealth);
        pb.LoseBattery(amount * M_damageToEnergyLoss);
        UpdateHealthbar();

        if (currentHealth <= 0)
        {
            if (!dead)
            {
                dead = true;
                Debug.Log("die");
                pb.dayNightCycleManager.EndDay();
            }
        }
    }

}
