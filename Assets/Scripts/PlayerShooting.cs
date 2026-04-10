using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("VFX Settings")]
    public GameObject impactPrefab;
    public GameObject muzzleFlashPrefab;

    private Weapon weapon;
    private Transform firePoint;
    private Camera cam;
    private float nextTimeToFire;
    
    private PlayerInputActions controls;
    private PlayerMovement playerMovement;
    private WeaponSway weaponSway;

    private void Awake()
    {
        controls = new PlayerInputActions();
        cam = Camera.main;
        playerMovement = GetComponentInParent<PlayerMovement>();
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    void Update()
    {
        if (weapon == null) { FindWeapon(); return; }
        HandleShooting();
    }

    void FindWeapon()
    {
        weapon = GetComponentInChildren<Weapon>(true);
        if (weapon != null)
        {
            weaponSway = weapon.GetComponent<WeaponSway>();
            firePoint = weapon.transform.Find("FirePoint");
            if (firePoint == null) firePoint = weapon.transform;
        }
    }

    void HandleShooting()
    {
        if (playerMovement != null && (playerMovement.IsRunning || !playerMovement.IsGrounded)) return;

        bool shootInput = weapon.automatic ? controls.Player.Shoot.ReadValue<float>() > 0 : controls.Player.Shoot.triggered;

        if (shootInput && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + (1f / weapon.fireRate);
            Shoot();
        }
    }

    void Shoot()
    {
      
        if (muzzleFlashPrefab != null && firePoint != null)
        {
            GameObject flash = Instantiate(muzzleFlashPrefab, firePoint.position, firePoint.rotation, firePoint);
            Destroy(flash, 0.1f);
        }

        
        float adsMultiplier = (weaponSway != null && weaponSway.EstaMirando) ? 0.2f : 1.0f;
        float currentSpread = weapon.spread * adsMultiplier;

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Vector3 dir = ray.direction;
        dir.x += Random.Range(-currentSpread, currentSpread) * 0.01f;
        dir.y += Random.Range(-currentSpread, currentSpread) * 0.01f;

        
        if (Physics.Raycast(cam.transform.position, dir, out RaycastHit hit, weapon.range))
        {

            if (impactPrefab) 
            {
                Instantiate(impactPrefab, hit.point, Quaternion.LookRotation(hit.normal));
            }
        }
    }
}