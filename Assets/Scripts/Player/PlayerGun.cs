using System.Collections;
using UnityEngine;

public struct CombatInput
{
    public bool Shoot;
    public bool Reload;
}

public class PlayerGun : MonoBehaviour
{
    [Header("Aiming")]
    [SerializeField] private Transform playerRoot;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float rotationSpeed = 10f;
    private Transform currentTarget;
    private Vector2 lookInput;
    [Space]
    [Header("Shooting")]
    [SerializeField] private Transform shootingPoint;
    [Space]
    [Header("Current Weapon")]
    [SerializeField] private int maxAmmo = 100;
    [SerializeField] private int currentAmmo;
    [SerializeField] private int baseDmg = 8;
    [SerializeField] private float range = 100f;
    [SerializeField] private float timeBetweenShots = 0.04f;
    [SerializeField] private float reloadTime = 1.5f;
    [Space]
    [Header("Visuals")]
    [SerializeField] private Transform muzzlePoint;
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private GameObject hitVfx;

    private float currentReloadTime;

    private bool _requestedShoot;
    private bool isFiring = false;

    private bool _requestedReload;
    private bool isReloading = false;
    

    void Start()
    {
        currentAmmo = maxAmmo;
    }

    public void RotateGunTowardsMouse()
    {
        if (playerCamera == null || playerRoot == null) return;

        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform.CompareTag("Target"))
            {
                if (Vector3.Distance(transform.position, hit.transform.position) <= range)
                    currentTarget = hit.transform;
                else
                    currentTarget = null;
            }
            else
            {
                currentTarget = null;
            }
        }

        Vector3 targetPoint = currentTarget ? currentTarget.position : ray.GetPoint(range);
        Vector3 direction = (targetPoint - playerRoot.position).normalized;
        direction.y = 0f;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    public void UseWeapon(CombatInput input)
    {
        _requestedShoot = input.Shoot && !isReloading;

        _requestedReload = input.Reload;

        if (_requestedShoot && !isFiring && !isReloading)
        {
            StartCoroutine(ContinuousFire());
        }

        if (_requestedReload && !isReloading)
        {
            StartCoroutine(Reload());
        }
    }

    IEnumerator ContinuousFire()
    {
        isFiring = true;

        while (_requestedShoot)
        {
            if (currentAmmo > 0)
            {
                GameObject m = Instantiate(muzzleFlash, muzzlePoint.position, muzzlePoint.rotation);
                m.transform.parent = muzzlePoint;
                Destroy(m, 0.05f);

                FireBullet();
                currentAmmo--;
            }
            else
            {
                _requestedShoot = false;
            }

            yield return new WaitForSeconds(timeBetweenShots);
        }

        isFiring = false;
    }

    private void FireBullet()
    {
        if (currentTarget == null) return;

        Healthbar hp = currentTarget.GetComponentInChildren<Healthbar>();
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
        Debug.Log("Reloading...");
        isReloading = true;
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        isReloading = false;
        Debug.Log("Done Reloading.");
    }
}
