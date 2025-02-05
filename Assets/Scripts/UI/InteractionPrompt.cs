using UnityEngine;
using UnityEngine.UI;
using System;

public class InteractionPrompt : MonoBehaviour
{
    [SerializeField] private GameObject interactionPrompt;
    [SerializeField] private Image interactionFill;
    [SerializeField] private float holdTimeRequired;
    private float currentHoldTime;
    [Space]
    [SerializeField] private bool holdingInteract;
    private bool interactionRequested;
    private Action interactDone;

    void Start()
    {
        
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

            if (currentHoldTime >= holdTimeRequired)
            {
                print("Finished interaction");
                interactDone?.Invoke();
                DisableInteraction();
            }
        }
    }

    public void RequestInteraction(float holdTime, ref GameObject interactionImg, ref Image interactionPromptFill, Action interactionDone)
    {
        interactionRequested = true;
        holdTimeRequired = holdTime;
        interactionPrompt = interactionImg;
        interactionFill = interactionPromptFill;
        interactionPrompt.SetActive(true);
        interactDone = interactionDone;
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
