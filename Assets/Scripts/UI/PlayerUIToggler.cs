using UnityEngine;

public class PlayerUIToggler : MonoBehaviour
{
    [Header("SettlementUI")]
    [SerializeField] private RectTransform settlementUI;
    [SerializeField] private Inventory inventoryUI;
    [SerializeField] private Vector3 shownPos;
    [SerializeField] private Vector3 hiddenPos;
    [SerializeField] private float timeToMove = 0.8f;
    [SerializeField] private bool settlementIsShowing = true;
    [SerializeField] private bool inventoryIsShowing = false;
    [SerializeField] private UIManager uiManager;

    public void ToggleSettlementUI()
    {
        if(settlementIsShowing)
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
        if (inventoryIsShowing)
            inventoryUI.HideInventory();
        else
            inventoryUI.ShowInventory();

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
}
