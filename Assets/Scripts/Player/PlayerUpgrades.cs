using NUnit.Framework;
using Unity.Burst;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerUpgrades : MonoBehaviour
{
    [SerializeField] PlayerMovement movement;
    [SerializeField] PlayerGun gun;
    [SerializeField] PlayerBattery battery;
    [SerializeField] PlayerHealthBar healthBar;
    [SerializeField] Button upgrade1;
    [SerializeField] Button upgrade2;
    [SerializeField] Button upgrade3;

    [SerializeField] int tempMaxDashHold = 0;
    [SerializeField] int tempBulletDamageHold = 0;
    [SerializeField] int tempMaxAmmoHold = 0;

    [SerializeField] List<int> methodIndexBlacklist;


    //basically, all upgrade will cap out at 2x, meaning *2 damage or /2 cd
    //because its basically 30 days of upgrades, each upgrade should cap out at around 10 instances

    #region _Movement_



    public void Mobility()
    {
        movement.M_walkSpeed = Mathf.Clamp(movement.M_walkSpeed + 1.8f, 18f, 36f);
        movement.M_sprintMultiplier = Mathf.Clamp(movement.M_sprintMultiplier + 0.16f, 1.6f, 3.2f);
        movement.M_airSpeed = Mathf.Clamp(movement.M_airSpeed + 1.5f, 15f, 30f);
        movement.M_airAcceleration = Mathf.Clamp(movement.M_airAcceleration + 4.5f, 45f, 90f);
        movement.M_jumpSpeed = Mathf.Clamp(movement.M_jumpSpeed + 2f, 20f, 40f);
    }

    public void Dash()
    {
        movement.M_dashSpeed = Mathf.Clamp(movement.M_dashSpeed + 4.5f, 45f, 90f);
        movement.M_dashBetweenCooldown = Mathf.Clamp(movement.M_dashBetweenCooldown - 0.01f, 0.1f, 0.2f);
        movement.M_dashRecoveryCooldown = Mathf.Clamp(movement.M_dashRecoveryCooldown - 0.015f, 0.15f, 0.3f);
        tempMaxDashHold += 1;
        if (tempMaxDashHold == 5)
        {
            movement.M_maxDashes = Mathf.Clamp(movement.M_maxDashes + 1, 2, 4);
            tempMaxDashHold = 0;
        }
    }
    public void Boost()
    {
        movement.M_maxBoost = Mathf.Clamp(movement.M_maxBoost + 10f, 100f, 200f);
        movement.M_boostGain = Mathf.Clamp(movement.M_boostGain + 0.5f, 5f, 10f);
        movement.M_dashBoostLoss = Mathf.Clamp(movement.M_dashBoostLoss - 1f, 10f, 20f);
        movement.M_sprintBoostLoss = Mathf.Clamp(movement.M_sprintBoostLoss - 0.45f, 4.5f, 9f);
        movement.M_jumpBoostLoss = Mathf.Clamp(movement.M_jumpBoostLoss - 1f, 10f, 20f);
    }

    #endregion

    #region _Gun_

    public void Bullets()
    {
        ++tempBulletDamageHold;
        if (tempBulletDamageHold == 2)
        {
            gun.M_baseDmg = Mathf.Clamp(gun.M_baseDmg + 1, 5, 10);
            tempBulletDamageHold = 0;
        }
        gun.M_bulletForce = Mathf.Clamp(gun.M_bulletForce + 6f, 60f, 120f);
        gun.M_range = Mathf.Clamp(gun.M_range + 2f, 20f, 40f);
    }

    public void Magazine()
    {
        ++tempMaxAmmoHold;
        if (tempMaxAmmoHold == 2)
        {
            gun.M_maxAmmo = Mathf.Clamp(gun.M_maxAmmo + 2, 25, 50);
            tempMaxAmmoHold = 0;
        }
        else
            gun.M_maxAmmo = Mathf.Clamp(gun.M_maxAmmo + 3, 25, 50);
        
        gun.M_timeBetweenShots = Mathf.Clamp(gun.M_timeBetweenShots - 0.0055f, 0.055f, 0.11f);
        gun.M_reloadTime = Mathf.Clamp(gun.M_reloadTime - 0.075f, 0.75f, 1.5f);
    }

    #endregion

    #region _Battery_

    public void Battery()
    {
        battery.M_maxBattery = Mathf.Clamp(gun.M_reloadTime + 10f, 100f, 200f);
        battery.M_passiveLossRate = Mathf.Clamp(gun.M_reloadTime - 0.025f, 0.25f, 0.5f);
    }

    #endregion

    #region _Health_Bar_

    public void Frame()
    {
        healthBar.M_maxHealth = Mathf.Clamp(gun.M_reloadTime + 10f, 100f, 200f);
        healthBar.M_damageToEnergyLoss = Mathf.Clamp(gun.M_reloadTime - 0.005f, 0.05f, 0.1f);
    }

    #endregion


    public void SelectSemiRandomUpgrades()
    {
        //basically it will choose one of the methods above to apply to the button's function
        //then it will black list that method, so it cant get added to another upgrade,
        //meaning you cant get 2 instances of the same upgrade on the same screen


        //this needs to be fixed, methods are potentialy missing the onClick?

        
        Button target = upgrade1;

        for (int i = 0; i < 3; ++i)
        {
            switch (i)
            {
                case 1:
                    target = upgrade2;
                    break;
                case 2:
                    target = upgrade3;
                    break;
            }
        }
        
        target.onClick.RemoveAllListeners();
        bool isValidUpgrade = false;
        int choice = 0;
        while (!isValidUpgrade)
        {
            choice = Random.Range(0, 7);
            if (methodIndexBlacklist.Count < 1) return;
            foreach (int num in methodIndexBlacklist)
            {
                if (choice == num)
                {
                    isValidUpgrade = false;
                    break;
                }
                else
                    isValidUpgrade = true;
            }
        }

        switch (choice)
        {
            case 0:
                target.onClick.AddListener(this.Mobility);
                methodIndexBlacklist.Add(0);
                break;
            case 1:
                target.onClick.AddListener(this.Dash);
                methodIndexBlacklist.Add(1);
                break;
            case 2:
                target.onClick.AddListener(this.Boost);
                methodIndexBlacklist.Add(2);
                break;
            case 3:
                target.onClick.AddListener(this.Bullets);
                methodIndexBlacklist.Add(3);
                break;
            case 4:
                target.onClick.AddListener(this.Magazine);
                methodIndexBlacklist.Add(4);
                break;
            case 5:
                target.onClick.AddListener(this.Battery);
                methodIndexBlacklist.Add(5);
                break;
            case 6:
                target.onClick.AddListener(this.Frame);
                methodIndexBlacklist.Add(6);
                break;
        }
    }

    public void ClearBlacklist()
    {
        methodIndexBlacklist = new List<int>();
    }
}
