using UnityEngine;

public class PlayerNavigation : MonoBehaviour
{
    public GameObject navigationTarget;
    [SerializeField] GameObject arrow;

    void Update()
    {
        if (navigationTarget != null)
        {
            arrow.gameObject.SetActive(true);
            this.transform.LookAt(navigationTarget.transform.position);
        }
        else
        {
            arrow.gameObject.SetActive(false);
        }
    }
}
