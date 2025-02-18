using UnityEngine;
using UnityEngine.UI;
using System;
using Unity.VisualScripting;

public class Camp : BaseInteractable
{
    [SerializeField] private GameObject campPrompt;

    protected override void Update()
    {
        base.Update();
    }

    protected override void OpenPrompt()
    {
        print("Open camp prompt");
        campPrompt.SetActive(true);
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

}
