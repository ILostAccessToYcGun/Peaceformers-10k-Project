using System.Collections;
using UnityEngine;

public class StationaryEnemy : MonoBehaviour
{
    [SerializeField] private Transform modelTransform;
    [SerializeField] private Transform[] shootingPoints;
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
    [SerializeField] private float bulletForce = 50f;
    [SerializeField] private float timeBetweenShots = 0.04f;
    [SerializeField] private float reloadTime = 5f;
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

    void DetectPlayer()
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
                //GameObject m = Instantiate(muzzleFlash, muzzlePoint.position, muzzlePoint.rotation);
                //m.transform.parent = muzzlePoint;
                //Destroy(m, 0.05f);

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
            GameObject b = Instantiate(bulletPrefab, barrel.position, barrel.rotation);

            Vector3 direction = (target.position - barrel.position).normalized;
            direction.y = Mathf.Max(direction.y, -0.02f);
            b.GetComponent<Rigidbody>().linearVelocity = direction * bulletForce;

            b.GetComponent<Bullet>().baseDmg = baseDmg;


            Destroy(b, 8);
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        isReloading = false;
    }
}
