using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerCharacter;
    [SerializeField] private PlayerCamera playerCamera;
    [SerializeField] private PlayerGun playerGun;
    [Space]
    [SerializeField] private CameraSpring cameraSpring;
    [SerializeField] private CameraLean cameraLean;
    [Space]
    [SerializeField] private PlayerUIToggler playerUIToggler;

    private PlayerActionInputs _inputActions;

    void Start()
    {
        _inputActions = new PlayerActionInputs();
        _inputActions.Enable();

        //initialize other scripts
        playerCharacter.Initialize();
        playerCamera.Initialize(playerCharacter.GetCameraTarget());
        cameraSpring.Initialize();
        cameraLean.Initialize();
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

        var combatInput = new CombatInput
        {
            Shoot = input.Attack.IsPressed(),
            Reload = input.Reload.WasPressedThisFrame()
        };
        playerGun.RotateGunTowardsMouse();
        playerGun.UseWeapon(combatInput);

        if(ui.SettlementUI.WasPressedThisFrame())
        {
            playerUIToggler.ToggleSettlementUI();
        }
    }

    void LateUpdate()
    {
        var deltaTime = Time.deltaTime;

        var cameraTarget = playerCharacter.GetCameraTarget();

        var state = playerCharacter.GetState();

        playerCamera.UpdatePosition(cameraTarget);
        cameraSpring.UpdateSpring(deltaTime, cameraTarget.up);
        cameraLean.UpdateLean(deltaTime, state.Acceleration, cameraTarget.up);
    }
}
