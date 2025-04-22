using UnityEngine;
using UnityEngine.UI;

public class BaseInteractable : MonoBehaviour
{
    [SerializeField] protected Player player;
    [SerializeField] protected PlayerMovement playerMovement;
    [SerializeField] protected float playerDetectionRange;
    [SerializeField] protected InteractionPrompt interactionPrompt;
    [Space]
    [SerializeField] protected GameObject interactionPromptImage;
    [SerializeField] protected Image interactionFill;
    [SerializeField] protected bool requestedInteract;
    [SerializeField] protected bool initializeAction;
    [SerializeField] protected float interactHoldTime;



    protected virtual void OpenPrompt()
    {
        print("Open Item prompt");
    }

    protected virtual void InteractDistanceCheck()
    {
        float distance = Vector3.Distance(transform.position, playerMovement.gameObject.transform.position);
        if (distance <= playerDetectionRange && !initializeAction)
        {
            player.AddInteractionPrompt(interactionPrompt, distance);
            requestedInteract = true;
            interactionPrompt.RequestInteraction(interactHoldTime, ref interactionPromptImage, ref interactionFill, () => OpenPrompt()); //when the thing is filled
        }
        else if (distance >= playerDetectionRange && requestedInteract)
        {
            interactionPrompt.DisableInteraction();
            requestedInteract = false;
            initializeAction = false;
        }
        else
        {
            interactionPrompt.SetHold(false);
            interactionPrompt.SetRequest(false);
        }
    }

    protected virtual void Awake()
    {
        player = FindAnyObjectByType<Player>();
        playerMovement = FindAnyObjectByType<PlayerMovement>();
        interactionPrompt = GetComponentInChildren<InteractionPrompt>();

    }

    protected virtual void Update()
    {
        InteractDistanceCheck();
    }
}
