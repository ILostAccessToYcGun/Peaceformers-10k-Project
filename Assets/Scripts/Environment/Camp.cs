using UnityEngine;
using UnityEngine.UI;
using System;
using Unity.VisualScripting;

public class Camp : BaseInteractable
{
    [SerializeField] private GameObject campPrompt;
    [SerializeField] private PlayerUIToggler ui;

    protected override void Update()
    {
        base.Update();
    }

    protected override void OpenPrompt()
    {
        if (campPrompt.activeSelf == true)
        {
            ClosePrompt();
            ui.SetUIOpenBool(false);
        }
        else
        {
            print("Open camp prompt");
            campPrompt.SetActive(true);
            ui.SetUIOpenBool(true);
        }
    }

    public void ClosePrompt()
    {
        campPrompt.SetActive(false);
    }

    protected override void Awake()
    {
        InteractionPrompt tempPrompt = interactionPrompt;
        base.Awake();
        interactionPrompt = tempPrompt;
    }

    public bool SafeDistanceCheck()
    {
        return Vector3.Distance(this.transform.position, playerMovement.transform.position) <= playerDetectionRange;
    }

}
