using System.Collections;
using UnityEngine;

public class StationaryEnemy : MonoBehaviour
{
    [SerializeField] private Transform modelTransform;
    [SerializeField] private Transform[] shootingPoints;
    [SerializeField] private Material bulletTrailMaterial;
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private GameObject bulletPrefab;
    [Space]
    [Header("Detection")]
    [SerializeField] private Transform target;
    [SerializeField] private float detectionRange;
    [SerializeField] private float rotationSpeed;
    [Space]
    [Header("Weapon")]
    [SerializeField] private int maxAmmo = 25;
    [SerializeField] private int currentAmmo;
    [SerializeField] private int baseDmg = 8;
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
        currentAmmo = maxAmmo;
    }

    void Update()
    {
        if(alive)
            DetectPlayer();
        else
        {
            print(transform.name + ": im dead!!");
        }
    }

    void DetectPlayer() //we also want to detect other settlements or other settlement's enemies
    {
        if (Vector3.Distance(transform.position, target.position) <= detectionRange)
        {

            RaycastHit hitInfo;
            if (Physics.Linecast(new Vector3(modelTransform.position.x, (modelTransform.position.y + 1.5f), modelTransform.position.z),
                new Vector3(target.position.x, (target.position.y + 1.5f),
                    target.position.z), out hitInfo))
            {

                if (hitInfo.transform == target)
                {
                    //requesting fire
                    if (!isFiring && !isReloading)
                    {
                        _requestedShoot = true;
                        firingCoroutine = StartCoroutine(ContinuousFire());
                    }
                    Vector3 targetPoint = target.position;
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
            RaycastHit hit;

            GameObject m = Instantiate(muzzleFlash, barrel.position, barrel.rotation);
            m.transform.parent = barrel;
            Destroy(m, 0.05f);

            GameObject b = Instantiate(bulletPrefab, barrel.position, barrel.rotation);
            b.GetComponent<Rigidbody>().linearVelocity = barrel.right * bulletForce;
            b.GetComponent<Bullet>().baseDmg = baseDmg;
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
}
