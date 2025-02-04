using UnityEngine;
using UnityEngine.UI;

public class InteractionPrompt : MonoBehaviour
{
    [SerializeField] private GameObject interactionPrompt;
    [SerializeField] private Image interactionFill;
    [SerializeField] private float holdTimeRequired;
    private float currentHoldTime;
    [Space]
    [SerializeField] private bool holdingInteract;
    private bool interactionRequested;

    void Start()
    {
        DisableInteraction();
    }

    void Update()
    {
        if (interactionRequested)
        {
            if(holdingInteract)
            {
                currentHoldTime = Mathf.Clamp(currentHoldTime + Time.deltaTime, 0, holdTimeRequired);
            }
            else
            {
                currentHoldTime = Mathf.Clamp(currentHoldTime - Time.deltaTime, 0, holdTimeRequired);
            }

            float holdRatio = currentHoldTime / holdTimeRequired;
            interactionFill.fillAmount = holdRatio;
        }
    }

    public void RequestInteraction(float holdTime)
    {
        interactionRequested = true;
        holdTimeRequired = holdTime;
        interactionPrompt.SetActive(true);
    }

    public void DisableInteraction()
    {
        interactionRequested = false;
        interactionPrompt.SetActive(false);
        currentHoldTime = 0;
    }

    public void SetHold(bool isHoldingInteract)
    {
        holdingInteract = isHoldingInteract;
    }
}
