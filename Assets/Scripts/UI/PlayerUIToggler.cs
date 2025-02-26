using NUnit.Framework;
using System.Collections.Generic;
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
    [SerializeField] private Inventory inventoryUI;
    [SerializeField] private bool inventoryIsShowing = false;

    [Space]
    [Header("PlayerQuestUI")]
    [SerializeField] private QuestBoard playerQuestBoard;
    [SerializeField] private bool playerQuestIsShowing = false;

    [Space]
    [Header("SettlementQuestUI")]
    [SerializeField] private SettlementQuestBoard settlementQuestBoard;
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
        BackOutOfCurrentUI(1);
        if (inventoryIsShowing)
        {
            inventoryUI.HideInventory();
            SetUIOpenBool(false);
        }
        else
        {
            inventoryUI.ShowInventory();
            SetUIOpenBool(true);
        }
        inventoryIsShowing = !inventoryIsShowing;
        //if (settlementIsShowing)
        //{
        //    LeanTween.move(settlementUI, hiddenPos, timeToMove).setEase(LeanTweenType.easeOutCubic);
        //}
        //else
        //{
        //    LeanTween.move(settlementUI, shownPos, timeToMove / 2).setEase(LeanTweenType.easeOutBack);
        //}
        //
        //settlementIsShowing = !settlementIsShowing;
    }

    public void TogglePlayerQuestUI()
    {
        BackOutOfCurrentUI(2);
        if (playerQuestIsShowing)
        {
            playerQuestBoard.gameObject.SetActive(false);
            SetUIOpenBool(false);
        }
            
        else
        {
            playerQuestBoard.gameObject.SetActive(true);
            SetUIOpenBool(true);
        }
        playerQuestIsShowing = !playerQuestIsShowing;
    }
    
    
    public void ToggleSettlementQuestUI()
    {
        BackOutOfCurrentUI(3);
        if (settlementQuestIsShowing)
        {
            settlementQuestBoard.gameObject.SetActive(false);
            SetUIOpenBool(false);
        }
            
        else
        {
            settlementQuestBoard.gameObject.SetActive(true);
            SetUIOpenBool(true);
        }
        settlementQuestIsShowing = !settlementQuestIsShowing;
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
        if (playerQuestIsShowing && blackList != 2)
        {
            //Debug.Log("weeee");
            TogglePlayerQuestUI();
            return;
        }
        if (settlementQuestIsShowing && blackList != 3)
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
