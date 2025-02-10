using UnityEngine;
using UnityEngine.UI;
using System;
using Unity.VisualScripting;

public class Camp : BaseInteractable
{
    protected override void Update()
    {
        base.Update();
    }

    protected override void OpenPrompt()
    {
        print("Open camp prompt");
    }

    protected override void Awake()
    {
        InteractionPrompt tempPrompt = interactionPrompt;
        base.Awake();
        interactionPrompt = tempPrompt;
    }

}
