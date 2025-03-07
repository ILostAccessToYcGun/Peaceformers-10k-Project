using System.Collections;
using UnityEngine;
using KinematicCharacterController;
using static UnityEngine.GraphicsBuffer;

public class StriderEnemy : MonoBehaviour, ICharacterController
{
    private Vector3 spawnPos;

    [Header("Components")]
    [SerializeField] private KinematicCharacterMotor motor;
    [SerializeField] private Transform root;
    [SerializeField] private Transform player;
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private Transform modelBase;

    [Header("Movement")]
    [SerializeField] private float detectionRange = 20f;
    [SerializeField] private float orbitDistance = 5f;
    [SerializeField] private float moveSpeed = 15f;
    [SerializeField] private float turnSpeed = 5f;
    [Space]
    [SerializeField] private float orbitRadius = 10f;
    [SerializeField] private float orbitSpeed = 2f;
    private float orbitAngle;
    [Header("Movement Noise")]
    [SerializeField] private float noiseStrength = 1f;
    [SerializeField] private float noiseFrequency = 2f;

    private Vector3 _requestedMovement;
    private Quaternion _requestedRotation;

    void Start()
    {
        player = FindAnyObjectByType<PlayerMovement>().transform;
        spawnPos = transform.position;
        motor.CharacterController = this;
    }

    void Update()
    {
        UpdateVelocity(ref _requestedMovement, Time.deltaTime);
        BaseRotation(Time.deltaTime);
    }

    public void UpdateBody(float deltaTime) { }

    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        if (player == null) return;

        Vector3 targetPosition = spawnPos;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            if (distanceToPlayer > orbitDistance)
            {
                // Move towards player
                targetPosition = player.position;
            }
            else
            {
                // Orbit around player
                orbitAngle += orbitSpeed * deltaTime;
                Vector3 orbitOffset = new Vector3(Mathf.Cos(orbitAngle), 0, Mathf.Sin(orbitAngle)) * orbitRadius;
                targetPosition = player.position + orbitOffset;
            }
        }

        Vector3 directionToTarget = (targetPosition - transform.position).normalized;

        // Apply random noise
        float noiseX = Mathf.PerlinNoise(Time.time * noiseFrequency, 0) * 2 - 1;
        float noiseZ = Mathf.PerlinNoise(0, Time.time * noiseFrequency) * 2 - 1;
        Vector3 noiseOffset = new Vector3(noiseX, 0, noiseZ) * noiseStrength;
        directionToTarget += noiseOffset;
        directionToTarget.Normalize();

        _requestedMovement = directionToTarget * moveSpeed;
        Vector3 targetVelocity = _requestedMovement;
        currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, deltaTime * turnSpeed);
    }

    void BaseRotation(float deltaTime)
    {
        if (player == null) return;

        Vector3 target = (Vector3.Distance(transform.position, player.position) <= detectionRange) ? player.position : spawnPos;

        Vector3 directionToTarget = (target - transform.position).normalized;
        directionToTarget.y = 0;

        if (directionToTarget != Vector3.zero)
        {
            _requestedRotation = Quaternion.LookRotation(directionToTarget, Vector3.up);
            modelBase.rotation = Quaternion.Slerp(modelBase.rotation, _requestedRotation, deltaTime * turnSpeed);
        }
    }

    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime) { }
    public void BeforeCharacterUpdate(float deltaTime) { }
    public void PostGroundingUpdate(float deltaTime) { }
    public void AfterCharacterUpdate(float deltaTime) { }
    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport) { }
    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport) { }
    public bool IsColliderValidForCollisions(Collider coll) => true;
    public void OnDiscreteCollisionDetected(Collider hitCollider) { }
    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport) { }
    public Transform GetCameraTarget() => cameraTarget;
}
