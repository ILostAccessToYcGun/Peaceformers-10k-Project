using System.Collections;
using UnityEngine;
using KinematicCharacterController;
using static UnityEngine.GraphicsBuffer;
using System.Collections.Generic;

public class TankEnemy : MonoBehaviour, ICharacterController
{
    private Vector3 spawnPos;

    [Header("Components")]
    [SerializeField] private KinematicCharacterMotor motor;
    [SerializeField] private Transform root;
    [SerializeField] private List<Transform> targets;
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private Transform modelBase;
    [SerializeField] private StationaryEnemy gun;

    [Header("Movement")]
    [SerializeField] private float detectionRange = 20f;
    [SerializeField] private float orbitDistance = 5f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float turnSpeed = 1f;
    [Space]
    [SerializeField] private float orbitRadius = 10f;
    [SerializeField] private float orbitSpeed = 2f;
    private float orbitAngle;
    [Header("Movement Noise")]
    [SerializeField] private float noiseStrength = 1f;
    [SerializeField] private float noiseFrequency = 2f;

    private Vector3 _requestedMovement;
    private Quaternion _requestedRotation;

    [SerializeField] private Transform currentTarget;
    [SerializeField] public EnemyDirector ed;

    void Start()
    {
        //player = FindAnyObjectByType<PlayerMovement>().transform;

        ed = FindAnyObjectByType<EnemyDirector>();
        spawnPos = transform.position;
        motor.CharacterController = this;
        targets = ed.targetList;
    }

    void Update()
    {
        UpdateVelocity(ref _requestedMovement, Time.deltaTime);
        BaseRotation(Time.deltaTime);
    }

    public Transform CompareTargetDistances()
    {
        Transform closestTransform = targets[0];
        float smallestDistance = detectionRange;
        foreach (Transform target in targets)
        {
            float dist = Vector3.Distance(transform.position, target.position);
            if (dist < smallestDistance)
            {
                smallestDistance = dist;
                closestTransform = target;
                if (gun.healthBar.GetCurrentHealth() < gun.healthBar.GetMaxHealth())
                {
                    if (target.gameObject.layer == LayerMask.NameToLayer("Player"))
                        return closestTransform;
                }
                else if (gun.isPrioSettlements)
                {
                    if (target.gameObject.tag == "Settlement")
                    {
                        Debug.Log(closestTransform.name);
                        return closestTransform;
                    }
                }
            }
        }
        Debug.Log(closestTransform.name);
        return closestTransform;
    }

    public void UpdateBody(float deltaTime) { }

    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        
        currentTarget = CompareTargetDistances();
        if (currentTarget == null) return;

        float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);

        //stopping
        if (distanceToTarget <= gun.detectionRange) 
        {
            currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, deltaTime * turnSpeed * 2);
            if (!gun.isReloading) { return; }
        }

        Vector3 targetPosition = spawnPos;
        


        if (distanceToTarget <= detectionRange)
        {
            if (distanceToTarget > orbitDistance)
            {
                // Move towards player
                //targetPosition = currentTarget.position;
                targetPosition = Vector3.Lerp(targetPosition, currentTarget.position, deltaTime * turnSpeed);
            }
            else
            {
                // Orbit around player
                orbitAngle += orbitSpeed * deltaTime;
                Vector3 orbitOffset = new Vector3(Mathf.Cos(orbitAngle), 0, Mathf.Sin(orbitAngle)) * orbitRadius;
                targetPosition = currentTarget.position + orbitOffset;
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
        _requestedRotation = Quaternion.LookRotation(_requestedMovement, Vector3.up);
        modelBase.rotation = Quaternion.Slerp(modelBase.rotation, _requestedRotation, deltaTime * turnSpeed);
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
