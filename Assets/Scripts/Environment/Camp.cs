using UnityEngine;

public class Camp : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float playerDetectionRange;
    [SerializeField] private InteractionPrompt interactionPrompt;

    void Start()
    {
        
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, player.position) <= playerDetectionRange)
        {
            interactionPrompt.RequestInteraction(1f);
        }
        else
        {
            interactionPrompt.DisableInteraction();
        }
    }
}
