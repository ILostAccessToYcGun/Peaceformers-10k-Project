using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : Healthbar
{
    [SerializeField] PlayerBattery pb;
    [SerializeField] float damageToEnergyLoss;

    public override void LoseHealth(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);
        pb.LoseBattery(amount * damageToEnergyLoss);
        UpdateHealthbar();
    }

}
