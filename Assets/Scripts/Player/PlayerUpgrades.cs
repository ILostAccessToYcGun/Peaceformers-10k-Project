using NUnit.Framework;
using Unity.Burst;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class PlayerUpgrades : MonoBehaviour
{
    [SerializeField] PlayerMovement movement;
    [SerializeField] PlayerGun gun;
    [SerializeField] PlayerBattery battery;
    [SerializeField] PlayerHealthBar healthBar;
    [SerializeField] List<GameObject> upgradeButtons;
    [SerializeField] Button upgradeConfirm;

    [SerializeField] int tempMaxDashHold = 0;
    [SerializeField] int tempBulletDamageHold = 0;
    [SerializeField] int tempMaxAmmoHold = 0;

    [SerializeField] List<int> methodIndexBlacklist;


    //basically, all upgrade will cap out at 2x, meaning *2 damage or /2 cd
    //because its basically 30 days of upgrades, each upgrade should cap out at around 10 instances

    #region _Movement_



    public void Mobility()
    {
        Debug.Log("Mobility Upgrade");
        movement.M_walkSpeed = Mathf.Clamp(movement.M_walkSpeed + 1.8f, 18f, 36f);
        movement.M_sprintMultiplier = Mathf.Clamp(movement.M_sprintMultiplier + 0.16f, 1.6f, 3.2f);
        movement.M_airSpeed = Mathf.Clamp(movement.M_airSpeed + 1.5f, 15f, 30f);
        movement.M_airAcceleration = Mathf.Clamp(movement.M_airAcceleration + 4.5f, 45f, 90f);
        movement.M_jumpSpeed = Mathf.Clamp(movement.M_jumpSpeed + 2f, 20f, 40f);
    }

    public void MobilitySelect()
    {
        Debug.Log("Mobility Select");
        SelectUpgrade(Mobility);
    }

    public void Dash()
    {
        Debug.Log("Dash Upgrade");
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
    public void DashSelect()
    {
        Debug.Log("Dash Select");
        SelectUpgrade(Dash);
    }

    public void Boost()
    {
        Debug.Log("Boost Upgrade");
        movement.M_maxBoost = Mathf.Clamp(movement.M_maxBoost + 10f, 100f, 200f);
        movement.M_boostGain = Mathf.Clamp(movement.M_boostGain + 0.5f, 5f, 10f);
        movement.M_dashBoostLoss = Mathf.Clamp(movement.M_dashBoostLoss - 1f, 10f, 20f);
        movement.M_sprintBoostLoss = Mathf.Clamp(movement.M_sprintBoostLoss - 0.45f, 4.5f, 9f);
        movement.M_jumpBoostLoss = Mathf.Clamp(movement.M_jumpBoostLoss - 1f, 10f, 20f);
    }
    public void BoostSelect()
    {
        Debug.Log("Boost Select");
        SelectUpgrade(Boost);
    }

    #endregion

    #region _Gun_

    public void Bullets()
    {
        Debug.Log("Bullet Upgrade");
        ++tempBulletDamageHold;
        if (tempBulletDamageHold == 2)
        {
            gun.M_baseDmg = Mathf.Clamp(gun.M_baseDmg + 1, 5, 10);
            tempBulletDamageHold = 0;
        }
        gun.M_bulletForce = Mathf.Clamp(gun.M_bulletForce + 6f, 60f, 120f);
        gun.M_range = Mathf.Clamp(gun.M_range + 2f, 20f, 40f);
    }
    public void BulletsSelect()
    {
        Debug.Log("Bullets Select");
        SelectUpgrade(Bullets);
    }

    public void Magazine()
    {
        Debug.Log("Magazine Upgrade");
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
    public void MagazineSelect()
    {
        Debug.Log("Magazine Select");
        SelectUpgrade(Magazine);
    }

    #endregion

    #region _Battery_

    public void Battery()
    {
        Debug.Log("Battery Upgrade");
        battery.M_maxBattery = Mathf.Clamp(gun.M_reloadTime + 10f, 100f, 200f);
        battery.M_passiveLossRate = Mathf.Clamp(gun.M_reloadTime - 0.025f, 0.25f, 0.5f);
    }
    public void BatterySelect()
    {
        Debug.Log("Battery Select");
        SelectUpgrade(Battery);
    }

    #endregion

    #region _Health_Bar_

    public void Frame() 
    {
        Debug.Log("Frame Upgrade");
        healthBar.M_maxHealth = Mathf.Clamp(gun.M_reloadTime + 10f, 100f, 200f);
        healthBar.M_damageToEnergyLoss = Mathf.Clamp(gun.M_reloadTime - 0.005f, 0.05f, 0.1f);
    }
    public void FrameSelect()
    {
        Debug.Log("Frame Select");
        SelectUpgrade(Frame);
    }

    #endregion

    public void SelectUpgrade(UnityEngine.Events.UnityAction func)
    {
        Debug.Log(func);
        upgradeConfirm.onClick.RemoveListener(Mobility);
        upgradeConfirm.onClick.RemoveListener(Dash);
        upgradeConfirm.onClick.RemoveListener(Boost);
        upgradeConfirm.onClick.RemoveListener(Bullets);
        upgradeConfirm.onClick.RemoveListener(Magazine);
        upgradeConfirm.onClick.RemoveListener(Battery);
        upgradeConfirm.onClick.RemoveListener(Frame);
        upgradeConfirm.onClick.AddListener(func);
    }


    public void SelectSemiRandomUpgrades()
    {
        //basically it will choose one of the methods above to apply to the button's function
        //then it will black list that method, so it cant get added to another upgrade,
        //meaning you cant get 2 instances of the same upgrade on the same screen


        //this needs to be fixed, methods are potentialy missing the onClick?

        
        

        foreach (GameObject buttons in upgradeButtons)
        {
            Button target = buttons.GetComponent<Button>();
            TextMeshProUGUI title = buttons.GetComponentInChildren<TextMeshProUGUI>();
            Debug.Log(target);

            target.onClick.RemoveAllListeners();
            bool isValidUpgrade = false;
            int choice = 0;
            while (!isValidUpgrade)
            {
                choice = Random.Range(0, 7);
                if (methodIndexBlacklist.Count < 1) break;
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
                    title.text = "Mobility Upgrade";
                    target.onClick.AddListener(this.MobilitySelect);
                    methodIndexBlacklist.Add(0);
                    break;
                case 1:
                    title.text = "Dash Upgrade";
                    target.onClick.AddListener(this.DashSelect);
                    methodIndexBlacklist.Add(1);
                    break;
                case 2:
                    title.text = "Boost Upgrade";
                    target.onClick.AddListener(this.BoostSelect);
                    methodIndexBlacklist.Add(2);
                    break;
                case 3:
                    title.text = "Bullets Upgrade";
                    target.onClick.AddListener(this.BulletsSelect);
                    methodIndexBlacklist.Add(3);
                    break;
                case 4:
                    title.text = "Magazine Upgrade";
                    target.onClick.AddListener(this.MagazineSelect);
                    methodIndexBlacklist.Add(4);
                    break;
                case 5:
                    title.text = "Battery Upgrade";
                    target.onClick.AddListener(this.BatterySelect);
                    methodIndexBlacklist.Add(5);
                    break;
                case 6:
                    title.text = "Frame Upgrade";
                    target.onClick.AddListener(this.FrameSelect);
                    methodIndexBlacklist.Add(6);
                    break;
            }
        }
        
        
    }

    public void ClearBlacklist()
    {
        methodIndexBlacklist = new List<int>();
    }
}
