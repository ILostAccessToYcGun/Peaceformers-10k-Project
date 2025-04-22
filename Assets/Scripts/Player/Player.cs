using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerCharacter;
    [SerializeField] private PlayerCamera playerCamera;
    [SerializeField] private PlayerGun playerGun;
    [SerializeField] private PlayerHealthBar playerhp;
    [Space]
    [SerializeField] private CameraSpring cameraSpring;
    [SerializeField] private CameraLean cameraLean;
    [Space]
    [SerializeField] private PlayerUIToggler playerUIToggler;
    [SerializeField] private InteractionPrompt interactionPrompt;
    [SerializeField] private List<InteractionPrompt> interactionPrompts;
    [SerializeField] private List<float> interactionDistances;
    [SerializeField] private float closestPrompt;

    private PlayerActionInputs _inputActions;
    //[Space]
    //[SerializeField] public List<Upgrade> upgrades //these are scripatbel objects that get generated and added  to the player?
    //nah thats way too complicated, basically, we are gonna have an upgrades script that holds a bunch of values just to view them later
    //the upgrade script will be connected to the other player components and upgrade their M_ values, not the base values

    public void AddInteractionPrompt(InteractionPrompt newPrompt, float distance)
    {
        ////hmmmmmmmmm closestPrompt some thing something
        interactionPrompts.Add(newPrompt);
        interactionDistances.Add(distance);
    }

    public void SetInteractionPrompt()
    {
        if (interactionPrompts.Count <= 0) { return; }
        int prompt = 0;
        float smallestDistance = 20;
        for (int i = 0; i < interactionDistances.Count; i++)
        {
            if (interactionDistances[i] < smallestDistance)
            {
                prompt = i;
                smallestDistance = interactionDistances[i];
            }
        }
        ////hmmmmmmmmm closestPrompt some thing something
        interactionPrompt = interactionPrompts[prompt];
        interactionPrompts.Remove(interactionPrompts[prompt]);

        for (int i = 0; i < interactionPrompts.Count; i++)
        {
            interactionPrompts[i].SetHold(false);
            interactionPrompts[i].SetRequest(false);
            interactionPrompts[i].SetHoldTime(0);
        }
        interactionPrompts.Clear();
        interactionDistances.Clear();
    }

    void Start()
    {
        _inputActions = new PlayerActionInputs();
        _inputActions.Enable();

        //initialize other scripts
        playerCharacter.Initialize();
        playerCamera.Initialize(playerCharacter.GetCameraTarget());
        cameraSpring.Initialize();
        cameraLean.Initialize();
        playerhp.dead = false;
    }

    void OnDestroy()
    {
        _inputActions.Dispose();
    }

    void Update()
    {
        var input = _inputActions.Player;
        var ui = _inputActions.UI;
        var deltaTime = Time.deltaTime;

        //Get camera input and update Cam Rotation
        var cameraInput = new CameraInput { Look = input.Look.ReadValue<Vector2>() };
        playerCamera.UpdateRotation(cameraInput);

        //Get character input and update it
        var characterInput = new CharacterInput
        {
            Rotation = playerCamera.transform.rotation,
            Move = input.Move.ReadValue<Vector2>(),
            Jump = input.Jump.WasPressedThisFrame(),
            JumpSustain = input.Jump.IsPressed(),
            Dash = input.Dash.WasPressedThisFrame(),
            Sprint = input.Sprint.IsPressed()
        };
        playerCharacter.UpdateInput(characterInput);
        playerCharacter.UpdateBody(deltaTime);
        playerCharacter.BoostVisual(deltaTime);

        var combatInput = new CombatInput
        {
            Shoot = input.Attack.IsPressed(),
            Reload = input.Reload.WasPressedThisFrame()
        };
        playerGun.RotateGunTowardsMouse();
        if (!playerUIToggler.GetUIOpenBool())
            playerGun.UseWeapon(combatInput);


        #region _UI_

        if (ui.SettlementUI.WasPressedThisFrame())
        {
            playerUIToggler.ToggleSettlementUI();
        }
        interactionPrompt.SetHold(input.Interact.IsPressed());

        if (ui.InventoryUI.WasPressedThisFrame())
        {
            playerUIToggler.ToggleInventoryUI();
        }

        if (ui.QuestUI.WasPressedThisFrame())
        {
            playerUIToggler.TogglePlayerQuestUI();
        }

        if (ui.BackOut.WasPressedThisFrame())
        {
            if (playerUIToggler.GetUIOpenBool())
            {
                if (playerUIToggler.optionsIsShowing)
                {
                    playerUIToggler.ToggleOptionsUI();
                    playerUIToggler.TogglePauseUI();
                }
                else
                {
                    if (!playerUIToggler.endIsShowing)
                        playerUIToggler.BackOutOfCurrentUI();
                }
                    
            }
            else
                playerUIToggler.TogglePauseUI();
        }

        #endregion


    }

    void LateUpdate()
    {
        var deltaTime = Time.deltaTime;

        var cameraTarget = playerCharacter.GetCameraTarget();

        var state = playerCharacter.GetState();

        playerCamera.UpdatePosition(cameraTarget);
        cameraSpring.UpdateSpring(deltaTime, cameraTarget.up);
        cameraLean.UpdateLean(deltaTime, state.Acceleration, cameraTarget.up);

        SetInteractionPrompt();
    }
}
