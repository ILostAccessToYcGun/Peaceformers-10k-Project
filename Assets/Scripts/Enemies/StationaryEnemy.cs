using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class StationaryEnemy : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] private EnemyDirector ed;
    [SerializeField] public Healthbar healthBar;

    [SerializeField] private Transform modelTransform;
    [SerializeField] private Transform[] shootingPoints;
    [SerializeField] private Material bulletTrailMaterial;
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private GameObject bulletPrefab;

    [SerializeField] private GameObject gameObjectToDestory;
    [Space]
    [Header("Detection")]
    [SerializeField] private List<Transform> targets;
    [SerializeField] private Transform currentTarget;
    [SerializeField] private float detectionRange;
    [SerializeField] private float rotationSpeed;
    [Space]
    [Header("Weapon")]
    [SerializeField] private int maxAmmo = 25;
    [SerializeField] private int currentAmmo;
    [SerializeField] private int baseDmg = 8;
    [SerializeField] private int modDmg;
    [SerializeField] private float bulletForce = 60f;
    [SerializeField] private float timeBetweenShots = 0.04f;
    [SerializeField] private float reloadTime = 5f;
    [SerializeField] private float bulletSpread = 3f;
    private bool isFiring;
    private bool isReloading;

    private bool _requestedShoot;

    private Coroutine firingCoroutine;

    [SerializeField] private bool alive = true;

    void Start()
    {
        //currentTarget = FindAnyObjectByType<PlayerMovement>().transform;
        ed = FindAnyObjectByType<EnemyDirector>();

        modDmg = baseDmg;
        SetModDmg(ed.damageMultiplier);
        healthBar.SetMaxHealth(healthBar.GetMaxHealth() * ed.healthMultiplier);
        healthBar.ScaleUI();

        targets = ed.targetList;
        ++ed.enemiesAlive;
        currentAmmo = maxAmmo;
    }

    void Update()
    {
        if (healthBar.GetCurrentHealth() == 0) { alive = false; }
        
        if(alive)
            DetectPlayer();
        else
        {
            print(transform.name + ": im dead!!");
            --ed.enemiesAlive;
            Destroy((gameObjectToDestory != null ? gameObjectToDestory : this.gameObject));
        }
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
                Debug.Log(dist);
                smallestDistance = dist;
                closestTransform = target;
                if (healthBar.GetCurrentHealth() < healthBar.GetMaxHealth())
                {
                    if (target.gameObject.layer == LayerMask.NameToLayer("Player"))
                    {
                        Debug.Log("erm");
                        return closestTransform;
                    }
                }
            }
        }
        return closestTransform;
    }

    void DetectPlayer() //we also want to detect other settlements or other settlement's enemies
    {
        
        currentTarget = CompareTargetDistances();

        if (Vector3.Distance(transform.position, currentTarget.position) <= detectionRange)
        {

            RaycastHit hitInfo;
            if (Physics.Linecast(new Vector3(modelTransform.position.x, (modelTransform.position.y + 1.5f), modelTransform.position.z),
                new Vector3(currentTarget.position.x, (currentTarget.position.y + 1.5f),
                    currentTarget.position.z), out hitInfo))
            {

                if (hitInfo.transform == currentTarget)
                {
                    //requesting fire
                    if (!isFiring && !isReloading)
                    {
                        _requestedShoot = true;
                        firingCoroutine = StartCoroutine(ContinuousFire());
                    }
                    Vector3 targetPoint = currentTarget.position;
                    Vector3 direction = (targetPoint - modelTransform.position).normalized;

                    if (direction != Vector3.zero)
                    {
                        Quaternion targetRotation = Quaternion.LookRotation(direction);
                        modelTransform.rotation = Quaternion.Lerp(modelTransform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
                    }
                }
            }
        }
        else
        {
            if (firingCoroutine != null)
            {
                isFiring = false;
                StopCoroutine(firingCoroutine);
            }
        }
    }

    IEnumerator ContinuousFire()
    {
        isFiring = true;

        while (_requestedShoot)
        {
            if (currentAmmo > 0)
            {
                FireBullet();
                currentAmmo--;
            }
            else
            {
                isFiring = false;
                if (!isReloading)
                {
                    StartCoroutine(Reload());
                }
                _requestedShoot = false;
            }

            yield return new WaitForSeconds(timeBetweenShots);
        }

        isFiring = false;
    }

    private void FireBullet()
    {
        foreach (Transform barrel in shootingPoints)
        {
            Vector3 spread = new Vector3(
                Random.Range(-bulletSpread, bulletSpread),
                Random.Range(-bulletSpread, bulletSpread),
                0f
            );

            Vector3 shootDirection = barrel.right + spread;
            //RaycastHit hit;

            GameObject m = Instantiate(muzzleFlash, barrel.position, barrel.rotation);
            m.transform.parent = barrel;
            Destroy(m, 0.05f);

            GameObject b = Instantiate(bulletPrefab, barrel.position, barrel.rotation);
            b.GetComponent<Rigidbody>().linearVelocity = barrel.right * bulletForce;
            b.GetComponent<Bullet>().baseDmg = modDmg;
            b.GetComponent<Bullet>().source = transform;

            Destroy(b, 5f);

            //if (Physics.Raycast(barrel.position, shootDirection, out hit, detectionRange))
            //{
            //    Healthbar hp = hit.collider.GetComponentInChildren<Healthbar>();
            //    if (hp != null)
            //    {
            //        hp.LoseHealth(baseDmg);
            //    }
            //}

            //GameObject line = new GameObject("BulletTrail");
            //LineRenderer lr = line.AddComponent<LineRenderer>();
            //lr.startWidth = 0.03f;
            //lr.endWidth = 0.03f;
            //lr.positionCount = 2;
            //lr.SetPosition(0, barrel.position);
            //lr.SetPosition(1, hit.point);
            //lr.material = bulletTrailMaterial; 

            //Destroy(line, 0.02f);
        }
    }

    Healthbar FindHealthbarInChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Healthbar health = child.GetComponent<Healthbar>();
            if (health != null)
                return health;

            health = FindHealthbarInChildren(child);
            if (health != null)
                return health;
        }

        return null;
    }

    IEnumerator Reload()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        isReloading = false;
    }

    public void SetModDmg(float addative)
    {
        modDmg = (int)((float)baseDmg + addative);
    }
}
