using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    public bool isOccupied;

    public void OccupiedToggle(bool toggle)
    {
        isOccupied = toggle;
    }
}
