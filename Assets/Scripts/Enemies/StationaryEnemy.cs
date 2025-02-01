using System.Collections;
using UnityEngine;

public class StationaryEnemy : MonoBehaviour
{
    [SerializeField] private Transform shootingPoint;
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
    [SerializeField] private float timeBetweenShots = 0.04f;
    [SerializeField] private float reloadTime = 5f;
    private bool isFiring;
    private bool isReloading;

    private bool _requestedShoot;

    void Start()
    {
        currentAmmo = maxAmmo;
    }

    void Update()
    {
        DetectPlayer();
    }

    void DetectPlayer()
    {
        if (Vector3.Distance(transform.position, target.position) <= detectionRange)
        {
            //can target be seen (not on other side of a wall for instance)?
            RaycastHit hitInfo;
            if (Physics.Linecast(new Vector3(transform.position.x, (transform.position.y + 1.5f), transform.position.z),
                new Vector3(target.position.x, (target.position.y + 1.5f),
                    target.position.z), out hitInfo))
            {

                //can see target and met all other criteria so add to agro list
                if (hitInfo.transform == target) 
                {
                    //requesting fire
                    if (!isFiring && !isReloading)
                    {
                        _requestedShoot = true;
                        StartCoroutine(ContinuousFire());
                    }
                    Vector3 targetPoint = target.position;
                    Vector3 direction = (targetPoint - transform.position).normalized;
                    direction.y = 0f;

                    if (direction != Vector3.zero)
                    {
                        Quaternion targetRotation = Quaternion.LookRotation(direction);
                        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
                    }
                }
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
                print("cant shoot for shit");
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
        if (target == null) return;

        Healthbar hp = target.GetComponentInChildren<Healthbar>();
        if (hp != null)
        {
            hp.LoseHealth(baseDmg);
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
        print("Reloading");
        isReloading = true;
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        isReloading = false;
    }
}
