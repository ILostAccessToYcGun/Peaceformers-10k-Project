using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;

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

    public bool GetUIOpenBool() { return isUIOpen; }
    public void SetUIOpenBool(bool toggle) { isUIOpen = toggle; }

    public void ToggleSettlementUI()
    {
        BackOutOfCurrentUI(0);
        if (settlementIsShowing)
        {
            LeanTween.move(settlementUI, hiddenPos, timeToMove).setEase(LeanTweenType.easeOutCubic);
        }
        else
        {
            LeanTween.move(settlementUI, shownPos, timeToMove/2).setEase(LeanTweenType.easeOutBack);
        }

        settlementIsShowing = !settlementIsShowing;
    }

    public void ToggleInventoryUI()
    {
        if (secondaryInventoryIsShowing)
        {
            LeanTween.move(secondaryInventoryUI, hiddenPos_Sinv, timeToMove).setEase(LeanTweenType.easeOutCubic);
            SetUIOpenBool(false);
            secondaryInventoryIsShowing = false;
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
            //Debug.Log("weeee");
            TogglePlayerQuestUI();
            return;
        }
        if (settlementQuestIsShowing && blackList != 4)
        {
            ToggleSettlementQuestUI();
            return;
        }
    }




    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    currentOpenUI
        //}
    }
}
