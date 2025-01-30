using UnityEngine;
using KinematicCharacterController;


public struct CharacterState
{
    public bool Grounded;
    public Vector3 Velocity;
    public Vector3 Acceleration;
}

public struct CharacterInput
{
    public Quaternion Rotation;
    public Vector2 Move;
    public bool Jump;
    public bool JumpSustain;
    public bool Dash;
}

public class PlayerMovement : MonoBehaviour, ICharacterController
{
    /*TO-DO: 
     * REDO JUMPING TO RUN OFF A JUMP VALUE INT AS WELL SO WE CAN HAVE SOME SWEET SWEET DOUBLE JUMPS 
     * MAKE SURE TO IMPLEMENT STATE VELOCITY SO THE COMMENTED PORTION OF AFTERCHARACTERUPDATE WORKS
     */


    [Header("Components")]
    [SerializeField] private KinematicCharacterMotor motor;
    [SerializeField] private Transform root;
    [Space]

    [SerializeField] private Transform cameraTarget;
    [Space]

    [Header("Speeds")]
    [SerializeField] private float walkSpeed = 20f;
    [SerializeField] private float walkResponse = 25f;

    [SerializeField] private float airSpeed = 15f;
    [SerializeField] private float airAcceleration = 90f;
    [Space]

    [Header("Jumping")]
    [SerializeField] private float jumpSpeed = 20f;
    [SerializeField] private float coyoteTime = 0.2f;
    [Range(0f, 1f)]
    [SerializeField] private float jumpSustainGravity = 0.4f;
    [SerializeField] private float gravity = -90f;
    [Space]

    [Header("Dashing")]
    [SerializeField] private float dashSpeed = 40f;
    [SerializeField] private float dashBetweenCooldown = 0.2f;
    [SerializeField] private float dashRecoveryCooldown = 0.3f;
    [SerializeField] private int maxDashes = 2;
    [SerializeField] private int dashes = 2;

    private CharacterState _state;
    private CharacterState _lastState;
    private CharacterState _tempState;

    private Quaternion _requestedRotation;

    private Vector3 _requestedMovement;

    private bool _requestedJump;

    private bool _requestedSustainedJump;

    private bool _requestedDash;

    private float _timeSinceUngrounded;
    private float _timeSinceJumpRequest;
    private bool _ungroundedDueToJump;


    public void Initialize()
    {
        _lastState = _state;

        dashes = maxDashes;

        motor.CharacterController = this;
    }

    public void UpdateInput(CharacterInput input)
    {
        _requestedRotation = input.Rotation;

        //take the 2D movement input and map it to an XZ plane
        _requestedMovement = new Vector3(input.Move.x, 0f, input.Move.y);
        //clamp the movement vector to a magnitude of 1 (no funny diag movement tech)
        _requestedMovement = Vector3.ClampMagnitude(_requestedMovement, 1f);
        //orient the movement to where the player is actually facing
        _requestedMovement = input.Rotation * _requestedMovement;

        var wasRequestingJump = _requestedJump;
        _requestedJump = _requestedJump || input.Jump;
        if (_requestedJump && !wasRequestingJump)
        {
            _timeSinceJumpRequest = 0f;
        }


        _requestedSustainedJump = input.JumpSustain;

        _requestedDash = _requestedDash || input.Dash;

    }

    public void UpdateBody(float deltaTime)
    {
        if (dashRecoveryCooldown > 0f && dashes < maxDashes)
            dashRecoveryCooldown -= deltaTime;
        else if (dashRecoveryCooldown <= 0f && dashes < maxDashes)
        {
            if (dashes < maxDashes)
                dashes++;
            dashRecoveryCooldown = 2f;
        }
        else if (dashRecoveryCooldown < 0f && dashes == maxDashes)
        {
            dashRecoveryCooldown = 0f;
        }

        if (dashBetweenCooldown > 0f)
            dashBetweenCooldown -= deltaTime;
        else if (dashBetweenCooldown < 0f)
            dashBetweenCooldown = 0f;

        if (dashes > maxDashes)
            dashes = maxDashes;
    }

    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        _state.Acceleration = Vector3.zero;

        //if on the ground
        if (motor.GroundingStatus.IsStableOnGround)
        {
            _state.Grounded = true;
            _timeSinceUngrounded = 0f;
            _ungroundedDueToJump = false;

            var groundedMovement = motor.GetDirectionTangentToSurface
            (
                direction: _requestedMovement,
                surfaceNormal: motor.GroundingStatus.GroundNormal
            ) * _requestedMovement.magnitude;

            //THE SCHMOVEMENT. THE HE. THE SCHMOVER.
            var targetVelocity = groundedMovement * walkSpeed;

            var moveVelocity = Vector3.Lerp
            (
                a: currentVelocity,
                b: targetVelocity,
                t: 1f - Mathf.Exp(-walkResponse * deltaTime)
            );

            _state.Acceleration = (moveVelocity - currentVelocity) / deltaTime;

            currentVelocity = moveVelocity;
        }
        //GUNDAM AERRIALLLLLLL (in the air)
        else
        {
            _timeSinceUngrounded += deltaTime;

            //aerial movement
            if (_requestedMovement.sqrMagnitude > 0f)
            {
                //requested movement on plane
                var planarMovement = Vector3.ProjectOnPlane
                (
                    vector: _requestedMovement,
                    planeNormal: motor.CharacterUp
                ) * _requestedMovement.magnitude;

                //current velocity on the movement plane 
                var currentPlanarVelocity = Vector3.ProjectOnPlane
                (
                    vector: currentVelocity,
                    planeNormal: motor.CharacterUp
                );

                //calculate movement force
                var movementForce = planarMovement * airAcceleration * deltaTime;
                var targetPlanarVelocity = currentPlanarVelocity + movementForce;

                targetPlanarVelocity = Vector3.ClampMagnitude(targetPlanarVelocity, airSpeed);

                currentVelocity += targetPlanarVelocity - currentPlanarVelocity;
            }

            //gravity
            var effectGravity = gravity;
            var verticalSpeed = Vector3.Dot(currentVelocity, motor.CharacterUp);
            if (_requestedSustainedJump && verticalSpeed > 0f)
                effectGravity *= jumpSustainGravity;

            currentVelocity += motor.CharacterUp * effectGravity * deltaTime;
        }

        //i wanna shoop baby
        if (_requestedJump)
        {
            var grounded = motor.GroundingStatus.IsStableOnGround;
            var canCoyoteJump = _timeSinceUngrounded < coyoteTime && !_ungroundedDueToJump;

            //we JUMPIN
            if (grounded || canCoyoteJump)
            {

                _requestedJump = false;

                //unstick that thang
                motor.ForceUnground(time: 0f);
                _ungroundedDueToJump = true;
                _state.Grounded = false;

                //Set minimum vertical speed to the jump speed
                var currentVerticalSpeed = Vector3.Dot(currentVelocity, motor.CharacterUp);
                var targetVerticalSpeed = Mathf.Max(currentVerticalSpeed, jumpSpeed);

                currentVelocity += motor.CharacterUp * (targetVerticalSpeed - currentVerticalSpeed);
            }
            else
            {
                _timeSinceJumpRequest += deltaTime;

                //decide whether or not thou can coyote jump
                var canJumpLater = _timeSinceJumpRequest < coyoteTime;
                _requestedJump = canJumpLater;
            }
        }

        if (_requestedDash)
        {
            var canDash = (dashBetweenCooldown == 0f) && (dashes > 0);

            if (canDash)
            {
                dashBetweenCooldown = 0.2f;
                dashes--;
                _requestedDash = false;



                var dashDirection = _requestedMovement.normalized;

                if (dashDirection.sqrMagnitude == 0f)
                {
                    dashDirection = Vector3.ProjectOnPlane(root.forward, motor.CharacterUp).normalized;
                }

                var dashVelocity = dashDirection * dashSpeed;

                currentVelocity = new Vector3(dashVelocity.x, currentVelocity.y, dashVelocity.z);
            }
            else
            {
                _requestedDash = false;
            }
        }
    }

    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        var forward = Vector3.ProjectOnPlane
        (
            _requestedRotation * Vector3.forward,
            motor.CharacterUp
        );

        if (forward != Vector3.zero)
            currentRotation = Quaternion.LookRotation(forward, motor.CharacterUp);
    }



    public void BeforeCharacterUpdate(float deltaTime)
    {

    }

    public void PostGroundingUpdate(float deltaTime) { }

    public void AfterCharacterUpdate(float deltaTime)
    {
        // Stuff can happen after move acceleration (walk/slide movement) is applied to current velocity that
        // lowers the velocity, so after the character updates make sure move acceleration does not exceed
        // the total acceleration.
        //var totalAcceleration = (_state.Velocity - _lastState.Velocity) / deltaTime;
        //_state.Acceleration = Vector3.ClampMagnitude(_state.Acceleration, totalAcceleration.magnitude);
    }


    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport) { }

    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport) { }

    public bool IsColliderValidForCollisions(Collider coll) => true;

    public void OnDiscreteCollisionDetected(Collider hitCollider) { }

    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport) { }

    public Transform GetCameraTarget() => cameraTarget;

    public CharacterState GetState() => _state;
    public CharacterState GetLastState() => _lastState;
}
