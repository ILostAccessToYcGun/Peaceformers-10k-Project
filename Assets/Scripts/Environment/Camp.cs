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
    [SerializeField] private bool initializeAction;

    void Start()
    {
        
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, player.position) <= playerDetectionRange && !initializeAction)
        {
            interactionPrompt.RequestInteraction(1f, ref interactionPromptImage, ref interactionFill, () => initializeAction = true);
        }
        else
        {
            interactionPrompt.DisableInteraction();
            initializeAction = false;
        }

        if(initializeAction)
        {
            print("Open camp prompt");
        }
    }
}
