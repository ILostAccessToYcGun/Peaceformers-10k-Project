using NUnit.Framework;
using System.Collections.Generic;
using System.Transactions;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerUIToggler : MonoBehaviour
{
    [Header("SettlementUI")]
    [SerializeField] private RectTransform settlementUI;
    [SerializeField] private Vector3 shownPos;
    [SerializeField] private Vector3 hiddenPos;
    [SerializeField] private float timeToMove = 0.8f;
    [SerializeField] private bool settlementIsShowing = true;

    [Space]
    [Header("InventoryUI")]
    [SerializeField] private RectTransform playerInventoryUI;
    [SerializeField] private Vector3 shownPos_inv;
    [SerializeField] private Vector3 hiddenPos_inv;
    [SerializeField] private bool inventoryIsShowing = false;

    [Space]
    [Header("SecondaryInventoryUI")]
    [SerializeField] private RectTransform secondaryInventoryUI;
    [SerializeField] private Vector3 shownPos_Sinv;
    [SerializeField] private Vector3 hiddenPos_Sinv;
    [SerializeField] private InteractableInventory currentSecondInventory;
    [SerializeField] private bool secondaryInventoryIsShowing = false;

    [Space]
    [Header("PlayerQuestUI")]
    [SerializeField] private RectTransform playerQuestBoardUI;
    [SerializeField] private Vector3 shownPos_pqb;
    [SerializeField] private Vector3 hiddenPos_pqb;
    [SerializeField] private bool playerQuestIsShowing = false;

    [Space]
    [Header("SettlementQuestUI")]
    [SerializeField] private RectTransform settlementQuestBoardUI;
    [SerializeField] private Vector3 shownPos_sqb;
    [SerializeField] private Vector3 hiddenPos_sqb;
    [SerializeField] private bool settlementQuestIsShowing = false;

    [Space]
    [Header("GeneralManagement")]
    [SerializeField] private bool isUIOpen;

    [Space]
    [Header("PauseUI")]
    [SerializeField] private RectTransform pauseUI;
    [SerializeField] private Vector3 shownPos_p;
    [SerializeField] private Vector3 hiddenPos_p;
    [SerializeField] private bool pauseIsShowing = false;

    [Header("OptionsUI")]
    [SerializeField] private RectTransform optionsUI;
    [SerializeField] private Vector3 shownPos_o;
    [SerializeField] private Vector3 hiddenPos_o;
    [SerializeField] public bool optionsIsShowing = false;

    [Header("EndUI")]
    [SerializeField] private RectTransform endUI;
    [SerializeField] private TextMeshProUGUI endTitle;
    [SerializeField] private TextMeshProUGUI endStats;
    [SerializeField] private Vector3 shownPos_end;
    [SerializeField] private Vector3 hiddenPos_end;
    [SerializeField] public bool endIsShowing = false;

    public bool GetUIOpenBool() { return isUIOpen; }
    public void SetUIOpenBool(bool toggle) { isUIOpen = toggle; }

    public void ToggleSettlementUI()
    {
        GameObject.FindAnyObjectByType<AudioManager>().Play("UI_Pop");
        BackOutOfCurrentUI(0);
        if (settlementIsShowing)
            LeanTween.move(settlementUI, hiddenPos, timeToMove).setEase(LeanTweenType.easeOutCubic);
        else
            LeanTween.move(settlementUI, shownPos, timeToMove/2).setEase(LeanTweenType.easeOutBack);

        settlementIsShowing = !settlementIsShowing;
    }

    public void ToggleInventoryUI()
    {
        GameObject.FindAnyObjectByType<AudioManager>().Play("UI_Pop");
        if (secondaryInventoryIsShowing)
        {
            LeanTween.move(secondaryInventoryUI, hiddenPos_Sinv, timeToMove).setEase(LeanTweenType.easeOutCubic);
            SetUIOpenBool(false);
            secondaryInventoryIsShowing = false;

            if (GetSecondaryInventory() != null)
                GetSecondaryInventory().ToggleInventories();
        }

        BackOutOfCurrentUI(1);
        if (inventoryIsShowing)
        {
            LeanTween.move(playerInventoryUI, hiddenPos_inv, timeToMove).setEase(LeanTweenType.easeOutCubic);
            SetUIOpenBool(false);
        }
        else
        {
            LeanTween.move(playerInventoryUI, shownPos_inv, timeToMove / 2).setEase(LeanTweenType.easeOutBack);
            SetUIOpenBool(true);
        }
        inventoryIsShowing = !inventoryIsShowing;
    }

    public bool GetPlayerInventoryShowingBool() { return inventoryIsShowing; }

    public void ToggleSecondaryInventoryUI()
    {
        GameObject.FindAnyObjectByType<AudioManager>().Play("UI_Pop");
        BackOutOfCurrentUI(2);
        if (secondaryInventoryIsShowing)
        {
            LeanTween.move(secondaryInventoryUI, hiddenPos_Sinv, timeToMove).setEase(LeanTweenType.easeOutCubic);
            SetUIOpenBool(false);
        }
        else
        {
            LeanTween.move(secondaryInventoryUI, shownPos_Sinv, timeToMove / 2).setEase(LeanTweenType.easeOutBack);
            SetUIOpenBool(true);
        }
        secondaryInventoryIsShowing = !secondaryInventoryIsShowing;
    }

    public bool GetSecondaryInventoryShowingBool() { return secondaryInventoryIsShowing; }
    public InteractableInventory GetSecondaryInventory() { return currentSecondInventory; }
    public void SetSecondaryInventory(InteractableInventory newInv) { currentSecondInventory = newInv; }

    public void ToggleInventoryUIs()
    {

        if (settlementIsShowing) { ToggleSettlementUI(); }
        if (playerQuestIsShowing) { TogglePlayerQuestUI(); }
        if (settlementQuestIsShowing) { ToggleSettlementQuestUI(); }

        if (inventoryIsShowing && secondaryInventoryIsShowing)
        {
            LeanTween.move(playerInventoryUI, hiddenPos_inv, timeToMove).setEase(LeanTweenType.easeOutCubic);
            LeanTween.move(secondaryInventoryUI, hiddenPos_Sinv, timeToMove).setEase(LeanTweenType.easeOutCubic);
            SetUIOpenBool(false);
            inventoryIsShowing = false;
            secondaryInventoryIsShowing = false;
        }
        else if (inventoryIsShowing && !secondaryInventoryIsShowing)
        {
            LeanTween.move(secondaryInventoryUI, shownPos_Sinv, timeToMove / 2).setEase(LeanTweenType.easeOutBack);
            SetUIOpenBool(true);
            secondaryInventoryIsShowing = true;
        }
        else if (!inventoryIsShowing && secondaryInventoryIsShowing)
        {
            LeanTween.move(playerInventoryUI, shownPos_inv, timeToMove / 2).setEase(LeanTweenType.easeOutBack);
            SetUIOpenBool(true);
            inventoryIsShowing = true;
        }
        else
        {
            LeanTween.move(secondaryInventoryUI, shownPos_Sinv, timeToMove / 2).setEase(LeanTweenType.easeOutBack);
            LeanTween.move(playerInventoryUI, shownPos_inv, timeToMove / 2).setEase(LeanTweenType.easeOutBack);
            SetUIOpenBool(true);
            inventoryIsShowing = true;
            secondaryInventoryIsShowing = true;
        }
    }

    public void TogglePlayerQuestUI()
    {
        GameObject.FindAnyObjectByType<AudioManager>().Play("UI_Pop");
        BackOutOfCurrentUI(3);
        if (playerQuestIsShowing)
        {
            LeanTween.move(playerQuestBoardUI, hiddenPos_pqb, timeToMove).setEase(LeanTweenType.easeOutCubic);
            SetUIOpenBool(false);
        }
        else
        {
            LeanTween.move(playerQuestBoardUI, shownPos_pqb, timeToMove / 2).setEase(LeanTweenType.easeOutBack);
            SetUIOpenBool(true);
        }
        playerQuestIsShowing = !playerQuestIsShowing; 
    }

    public void ToggleSettlementQuestUI()
    {
        GameObject.FindAnyObjectByType<AudioManager>().Play("UI_Pop");
        BackOutOfCurrentUI(4);

        if (settlementQuestIsShowing)
        {
            LeanTween.move(settlementQuestBoardUI, hiddenPos_pqb, timeToMove).setEase(LeanTweenType.easeOutCubic);
            SetUIOpenBool(false);
        }
        else
        {
            LeanTween.move(settlementQuestBoardUI, shownPos_pqb, timeToMove / 2).setEase(LeanTweenType.easeOutBack);
            SetUIOpenBool(true);
        }
        settlementQuestIsShowing = !settlementQuestIsShowing;
    }

    public bool GetSettlementQuestBool() { return settlementQuestIsShowing; }

    public void TogglePauseUI()
    {
        GameObject.FindAnyObjectByType<AudioManager>().Play("UI_Pop");
        BackOutOfCurrentUI(5);
        
        if (pauseIsShowing)
        {
            Time.timeScale = 1;
            LeanTween.move(pauseUI, hiddenPos_p, timeToMove).setIgnoreTimeScale(true).setEase(LeanTweenType.easeOutCubic);
            SetUIOpenBool(false);
           
        }
        else
        {
            Time.timeScale = 0;
            LeanTween.move(pauseUI, shownPos_p, timeToMove / 2).setIgnoreTimeScale(true).setEase(LeanTweenType.easeOutBack);
            SetUIOpenBool(true);
            
        }
        pauseIsShowing = !pauseIsShowing;
    }

    public void ToggleOptionsUI()
    {
        GameObject.FindAnyObjectByType<AudioManager>().Play("UI_Pop");
        BackOutOfCurrentUI(6);

        if (optionsIsShowing)
        {
            LeanTween.move(optionsUI, hiddenPos_o, timeToMove).setIgnoreTimeScale(true).setEase(LeanTweenType.easeOutCubic);
            SetUIOpenBool(false);
            //Time.timeScale = 1;
        }
        else
        {
            Time.timeScale = 0;
            LeanTween.move(optionsUI, shownPos_o, timeToMove / 2).setIgnoreTimeScale(true).setEase(LeanTweenType.easeOutBack);
            SetUIOpenBool(true);
            //Time.timeScale = 0;
        }
        optionsIsShowing = !optionsIsShowing;
    }

    public void ShowEndUI(string title = "End", string stats = "ur trash")
    {
        endTitle.text = title;
        endStats.text = stats;
        BackOutOfCurrentUI();

        //if (endIsShowing)
        //{
        //    LeanTween.move(endUI, hiddenPos_end, timeToMove).setIgnoreTimeScale(true).setEase(LeanTweenType.easeOutCubic);
        //    SetUIOpenBool(false);
        //}
        //else
        //{
            
        //}

        Time.timeScale = 0;
        LeanTween.move(endUI, shownPos_end, timeToMove / 2).setIgnoreTimeScale(true).setEase(LeanTweenType.easeOutBack);
        SetUIOpenBool(true);
        endIsShowing = true;
    }


    public void BackOutOfCurrentUI(int blackList = -1)
    {
        if (settlementIsShowing && blackList != 0)
        {
            ToggleSettlementUI();
            return;
        }
        if (inventoryIsShowing && blackList != 1)
        {
            ToggleInventoryUI();
            return;
        }
        if (secondaryInventoryIsShowing && blackList != 2)
        {
            ToggleSecondaryInventoryUI();
            return;
        }
        if (playerQuestIsShowing && blackList != 3)
        {
            TogglePlayerQuestUI();
            return;
        }
        if (settlementQuestIsShowing && blackList != 4)
        {
            ToggleSettlementQuestUI();
            return;
        }
        if (pauseIsShowing && blackList != 5)
        {
            TogglePauseUI();
            return;
        }
        if (optionsIsShowing && blackList != 6)
        {
            ToggleOptionsUI();
            return;
        }
    }


    public void Quit()
    {
        //save a file
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
