using UnityEngine;
using KinematicCharacterController;
using UnityEngine.UI;
using System.Collections;


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
    public bool Sprint;
}

public class PlayerMovement : MonoBehaviour, ICharacterController
{

    [Header("Components")]
    [SerializeField] private KinematicCharacterMotor motor;
    [SerializeField] private Transform root;
    [SerializeField] private PlayerBattery pb;
    [Space]

    [SerializeField] private Transform cameraTarget;
    [Space]

    [Space]
    [SerializeField] private Transform playerLegs;
    [SerializeField] private float legRotationSpeed = 8f;
    [SerializeField] private GameObject[] boostVisuals;
    [SerializeField] private float frequency = 5f;
    [SerializeField] private float scaleOffset = 0.05f;
    [SerializeField] private float baseScale = 0.95f;

    [Header("Speeds")]
    [SerializeField] private float walkSpeed = 20f;
    [SerializeField] private float walkResponse = 25f;

    [SerializeField] private float airSpeed = 15f;
    [SerializeField] private float airAcceleration = 90f;
    [Space]
    [Range(1f,2f)]
    [SerializeField] private float sprintMultiplier = 1.3f;

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

    [Header("Boost")]
    [SerializeField] private float maxBoost = 100f;
    [SerializeField] private float boostCapacity = 100f;
    [SerializeField] private float boostGain = 2f;
    [SerializeField] private float dashBoostLoss = 20f;
    [SerializeField] private float sprintBoostLoss = 4f;
    [SerializeField] private float jumpBoostLoss = 20f;
    [Space]
    [SerializeField] private Image boostBarFill;
    [SerializeField] private Image boostBarLoss;
    [SerializeField] private float lossLerpSpeed = 2f;
    [Space]
    [SerializeField] private Color goodboostColor;
    [SerializeField] private Color watchOutColor;
    [SerializeField] private Color criticalColor;


    private CharacterState _state;
    private CharacterState _lastState;
    private CharacterState _tempState;

    private Quaternion _requestedRotation;

    private Vector3 _requestedMovement;

    private bool _requestedJump;

    private bool _requestedSustainedJump;

    private bool _requestedDash;

    private bool _requestedSprint;

    private float _timeSinceUngrounded;
    private float _timeSinceJumpRequest;
    private bool _ungroundedDueToJump;

    private Vector3 startPos;

    [Space]
    [Header("Movement Modified Stats")]
    [SerializeField] public float M_walkSpeed;
    [SerializeField] public float M_sprintMultiplier;

    [SerializeField] public float M_airSpeed;
    [SerializeField] public float M_airAcceleration;
    [SerializeField] public float M_jumpSpeed;

    [SerializeField] public float M_dashSpeed;
    [SerializeField] public float M_dashBetweenCooldown;
    [SerializeField] public float M_dashRecoveryCooldown;
    [SerializeField] public int M_maxDashes;

    [SerializeField] public float M_maxBoost;
    [SerializeField] public float M_boostGain;
    [SerializeField] public float M_dashBoostLoss;
    [SerializeField] public float M_sprintBoostLoss;
    [SerializeField] public float M_jumpBoostLoss;








    public void ResetPos()
    {
        print("Resetting position");
        this.transform.position = startPos;
    }


    public void Initialize()
    {
        startPos = this.transform.position;

        _lastState = _state;

        dashes = M_maxDashes;
        boostCapacity = M_maxBoost;

        motor.CharacterController = this;
        pb = GetComponentInParent<PlayerBattery>();



        M_walkSpeed = walkSpeed;
        M_sprintMultiplier = sprintMultiplier;

        M_airSpeed = airSpeed;
        M_airAcceleration = airAcceleration;
        M_jumpSpeed = jumpSpeed;

        M_dashSpeed = dashSpeed;
        M_dashBetweenCooldown = dashBetweenCooldown;
        M_dashRecoveryCooldown = dashRecoveryCooldown;
        M_maxDashes = maxDashes;

        M_maxBoost = maxBoost;
        M_boostGain = boostGain;
        M_dashBoostLoss = dashBoostLoss;
        M_sprintBoostLoss = sprintBoostLoss;
        M_jumpBoostLoss = jumpBoostLoss;

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
        _requestedJump = _requestedJump || input.Jump && boostCapacity > M_jumpBoostLoss;
        if (_requestedJump && !wasRequestingJump)
        {
            _timeSinceJumpRequest = 0f;
        }


        _requestedSustainedJump = input.JumpSustain;

        _requestedDash = _requestedDash || input.Dash;

        _requestedSprint = input.Sprint && boostCapacity > 0;

    }

    public void BoostVisual(float deltaTime)
    {
        if(_requestedSprint || _requestedDash)
        {
            foreach(GameObject booster in boostVisuals)
            {
                booster.SetActive(true);
                float scale = baseScale + Mathf.Sin(Time.time * frequency) * scaleOffset;
                booster.transform.localScale = new Vector3(scale, scale, scale);
            }
        }
        else
        {
            foreach (GameObject booster in boostVisuals)
            {
                booster.SetActive(false);
            }
        }

        float boostRatio = boostCapacity / M_maxBoost;

        boostBarFill.fillAmount = boostRatio;

        if (boostCapacity > (M_maxBoost / 2))
            boostBarFill.color = goodboostColor;
        else if (boostCapacity > (M_maxBoost / 4))
            boostBarFill.color = watchOutColor;
        else
            boostBarFill.color = criticalColor;

        StopAllCoroutines();
        StartCoroutine(LerpBarLoss(boostRatio));
    }

    private IEnumerator LerpBarLoss(float targetFill)
    {
        float startFill = boostBarLoss.fillAmount;
        float elapsedTime = 0f;

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * lossLerpSpeed;
            boostBarLoss.fillAmount = Mathf.Lerp(startFill, targetFill, elapsedTime);
            yield return null;
        }

        boostBarLoss.fillAmount = targetFill;
    }

    public void UpdateBody(float deltaTime)
    {
        if (M_dashRecoveryCooldown > 0f && dashes < M_maxDashes)
            M_dashRecoveryCooldown -= deltaTime;
        else if (M_dashRecoveryCooldown <= 0f && dashes < M_maxDashes)
        {
            if (dashes < M_maxDashes)
                dashes++;
            M_dashRecoveryCooldown = 2f;
        }
        else if (M_dashRecoveryCooldown < 0f && dashes == M_maxDashes)
        {
            M_dashRecoveryCooldown = 0f;
        }

        if (M_dashBetweenCooldown > 0f)
            M_dashBetweenCooldown -= deltaTime;
        else if (M_dashBetweenCooldown < 0f)
            M_dashBetweenCooldown = 0f;

        if (dashes > M_maxDashes)
            dashes = M_maxDashes;

        if(_requestedSprint)
        {
            boostCapacity = Mathf.Clamp(boostCapacity - (M_sprintBoostLoss * deltaTime), 0, M_maxBoost);
            pb.LoseBattery(M_sprintBoostLoss * deltaTime * 0.025f);
        }
        else
        {
            boostCapacity = Mathf.Clamp(boostCapacity + (M_boostGain * deltaTime), 0, M_maxBoost);
        }
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
            var targetVelocity = groundedMovement * M_walkSpeed;

            //if sprinting
            targetVelocity *= (_requestedSprint && boostCapacity > 0) ? M_sprintMultiplier : 1f;

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
                var movementForce = planarMovement * M_airAcceleration * deltaTime;
                var targetPlanarVelocity = currentPlanarVelocity + movementForce;

                //if sprinting
                targetPlanarVelocity *= (_requestedSprint && boostCapacity > 0) ? M_sprintMultiplier : 1f;

                targetPlanarVelocity = Vector3.ClampMagnitude(targetPlanarVelocity, M_airSpeed);

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
            var canCoyoteJump = _timeSinceUngrounded < coyoteTime && !_ungroundedDueToJump && boostCapacity >= M_jumpBoostLoss;

            //we JUMPIN
            if (grounded || canCoyoteJump)
            {
                boostCapacity -= M_jumpBoostLoss;
                pb.LoseBattery(M_jumpBoostLoss * 0.025f);
                _requestedJump = false;

                //unstick that thang
                motor.ForceUnground(time: 0f);
                _ungroundedDueToJump = true;
                _state.Grounded = false;

                //Set minimum vertical speed to the jump speed
                var currentVerticalSpeed = Vector3.Dot(currentVelocity, motor.CharacterUp);
                var targetVerticalSpeed = Mathf.Max(currentVerticalSpeed, M_jumpSpeed);

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
            var canDash = (M_dashBetweenCooldown == 0f) && (dashes > 0) && boostCapacity >= M_dashBoostLoss;

            if (canDash)
            {
                M_dashBetweenCooldown = 0.2f;
                dashes--;
                boostCapacity -= M_dashBoostLoss;
                _requestedDash = false;
                pb.LoseBattery(M_dashBoostLoss * 0.025f);


                var dashDirection = _requestedMovement.normalized;

                if (dashDirection.sqrMagnitude == 0f)
                {
                    dashDirection = Vector3.ProjectOnPlane(root.forward, motor.CharacterUp).normalized;
                }

                var dashVelocity = dashDirection * M_dashSpeed;

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

        if (_requestedMovement.sqrMagnitude > 0.01f) 
        {
            Quaternion targetLegRotation = Quaternion.LookRotation(_requestedMovement, motor.CharacterUp);
            playerLegs.rotation = Quaternion.Lerp(playerLegs.rotation, targetLegRotation, legRotationSpeed * deltaTime);
        }
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
