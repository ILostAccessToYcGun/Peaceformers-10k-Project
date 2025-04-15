using UnityEngine;
using UnityEngine.UI;

public class PlayerNavigation : MonoBehaviour
{
    public GameObject navigationTarget;
    [SerializeField] GameObject arrow;

    //[SerializeField] Button drapnir;
    //[SerializeField] Button midgard;
    //[SerializeField] Button rubicon;
    //[SerializeField] Button camp;
    //i wanted to try and deselect the buttons here when u click them but ohwell


    public void SetNavTarget(GameObject target)
    {
        if (navigationTarget == target)
        {
            navigationTarget = null;
        }
            
        else
            navigationTarget = target;
    }


    void Update()
    {
        if (navigationTarget != null)
        {
            arrow.gameObject.SetActive(true);
            this.transform.LookAt(navigationTarget.transform.position);
        }
        else
            arrow.gameObject.SetActive(false);
    }
}
