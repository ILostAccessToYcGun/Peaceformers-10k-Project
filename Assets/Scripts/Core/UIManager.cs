using UnityEngine;

public class UIManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private bool isUIOpen;
    //[SerializeField] GameObject currentOpenUI

    public bool GetUIOpenBool() { return isUIOpen; }
    public void SetUIOpenBool(bool toggle) { isUIOpen = toggle; }
}
