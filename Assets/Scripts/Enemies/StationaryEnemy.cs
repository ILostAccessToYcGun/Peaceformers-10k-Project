using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class StationaryEnemy : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] public EnemyDirector ed;
    [SerializeField] public Healthbar healthBar;
    [SerializeField] public MapDirector md;

    [SerializeField] private Transform modelTransform;
    [SerializeField] private Transform[] shootingPoints;
    [SerializeField] private Material bulletTrailMaterial;
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private GameObject bulletPrefab;

    [SerializeField] private GameObject gameObjectToDestory;
    [SerializeField] public GameObject parentSettlement;
    [SerializeField] public EnemyCamp enemyCamp;
    [SerializeField] ParticleSystem[] bulletSpit;
    [Space]
    [Header("Detection")]
    [SerializeField] private List<Transform> targets;
    [SerializeField] private Transform currentTarget;
    [SerializeField] public float detectionRange;
    [SerializeField] private float rotationSpeed;
    [Space]
    [Header("Weapon")]
    [SerializeField] private int maxAmmo = 25;
    [SerializeField] private int currentAmmo;
    [SerializeField] private int baseDmg = 8;
    [SerializeField] private int modDmg;
    [SerializeField] private float fireForce = 60f;
    [SerializeField] private float timeBetweenShots = 0.04f;
    [SerializeField] private float reloadTime = 5f;
    [SerializeField] private float bulletSpread = 3f;
    private bool isFiring;
    public bool isReloading;

    private bool _requestedShoot;

    private Coroutine firingCoroutine;

    [SerializeField] private bool alive = true;

    [Space]
    [Header("Specialization")]
    [SerializeField] public bool isPrioSettlements = false;
    [SerializeField] public bool isCampEnemy = false;

    AudioManager audioManager;

    void Start()
    {
        ////currentTarget = FindAnyObjectByType<PlayerMovement>().transform;
        ed = FindAnyObjectByType<EnemyDirector>();
        md = FindAnyObjectByType<MapDirector>();

        modDmg = baseDmg;
        SetModDmg(ed.damageMultiplier);
        healthBar.SetMaxHealth(healthBar.GetMaxHealth() * ed.healthMultiplier);
        healthBar.ScaleUI(); //
        audioManager = GameObject.FindObjectOfType<AudioManager>();
        targets = ed.targetList;
        if (isCampEnemy)
          ++enemyCamp.campEnemiesAlive;
        else
          ++ed.enemiesAlive;

        currentAmmo = maxAmmo;
    }

    void Update()
    {
        if (healthBar.GetCurrentHealth() == 0) { alive = false; }
        
        if(alive)
            DetectTargets();
        else
        {
            print(transform.name + ": im dead!!");
            if (!isCampEnemy)
                --ed.enemiesAlive;

            if (parentSettlement != null)
            {
                Debug.Log("settlement enemy has died, deducting some upkeep");
                Settlement set = parentSettlement.GetComponent<Settlement>();
                set.LoseMeter(3f);
                --set.panicEnemies;
            }
            else if (enemyCamp != null)
            {
                --enemyCamp.GetComponent<EnemyCamp>().campEnemiesAlive;
            }

            Debug.Log("Deade");
            md.SpawnNode(2, this.transform.position);

            audioManager.Play("Kill_Enemy");

            Destroy((gameObjectToDestory != null ? gameObjectToDestory : this.gameObject));
        }
    }

    public Transform CompareTargetDistances()
    {
        Transform closestTransform = null;
        float smallestDistance = detectionRange;
        foreach (Transform target in targets)
        {
            float dist = Vector3.Distance(transform.position, target.position);
            
            if (dist < smallestDistance)
            {
                smallestDistance = dist;
                closestTransform = target;
                if (healthBar.GetCurrentHealth() < healthBar.GetMaxHealth())
                {
                    if (target.gameObject.layer == LayerMask.NameToLayer("Player"))
                        return closestTransform;
                }
                else if (isPrioSettlements)
                {
                    if (target.gameObject.tag == "Settlement")
                    {
                        if (parentSettlement == null)
                            return closestTransform;
                        else if(target.gameObject != parentSettlement)
                            return closestTransform;
                    }
                }
            }
        }
        if (parentSettlement != null)
            return null;
        else
            return closestTransform;
    }

    void DetectTargets() //we also want to detect other settlements or other settlement's enemies
    {
        
        currentTarget = CompareTargetDistances();
        if (currentTarget == null) return;

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
        GameObject.FindObjectOfType<AudioManager>().Play("Enemy_Fire");

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
            b.GetComponent<Rigidbody>().linearVelocity = barrel.right * fireForce;
            b.GetComponent<Bullet>().baseDmg = modDmg;
            b.GetComponent<Bullet>().source = transform;

            if (barrel == shootingPoints[0])
                bulletSpit[0].Play();
            else
                bulletSpit[1].Play();

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
