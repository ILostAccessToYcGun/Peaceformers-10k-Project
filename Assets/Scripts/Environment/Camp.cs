using UnityEngine;
using UnityEngine.UI;
using System;

public class Camp : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float playerDetectionRange;
    [SerializeField] private InteractionPrompt interactionPrompt;
    [Space]
    [SerializeField] private GameObject interactionPromptImage;
    [SerializeField] private Image interactionFill;
    [SerializeField] private bool requestedInteract;
    [SerializeField] private bool initializeAction;

    void Start()
    {
        
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, player.position) <= playerDetectionRange && !initializeAction)
        {
            requestedInteract = true;
            interactionPrompt.RequestInteraction(1f, ref interactionPromptImage, ref interactionFill, () => OpenCampPrompt());
        }
        else if (Vector3.Distance(transform.position, player.position) >= playerDetectionRange && requestedInteract)
        {
            interactionPrompt.DisableInteraction();
            requestedInteract = false;
            initializeAction = false;
        }
    }

    void OpenCampPrompt()
    {
        print("Open camp prompt");
    }
}
