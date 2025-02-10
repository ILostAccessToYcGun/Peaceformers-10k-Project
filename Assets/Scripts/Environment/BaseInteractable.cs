using UnityEngine;
using UnityEngine.UI;

public class BaseInteractable : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private float playerDetectionRange;
    [SerializeField] protected InteractionPrompt interactionPrompt;
    [Space]
    [SerializeField] private GameObject interactionPromptImage;
    [SerializeField] private Image interactionFill;
    [SerializeField] private bool requestedInteract;
    [SerializeField] private bool initializeAction;
    [SerializeField] private float interactHoldTime;



    protected virtual void OpenPrompt()
    {
        print("Open Item prompt");
    }

    protected void InteractDistanceCheck()
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
            Debug.Log(":OOOOOOOOOOOOO");
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
