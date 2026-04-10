using UnityEngine;

public class Weapon : MonoBehaviour
{
    public string weaponName = "X-BF7";
    public int currentAmmo = 30;
    public int magazineSize = 30;
    public int totalAmmo = 90;
    public float fireRate = 10f;
    public float damage = 20f;
    public float range = 100f;
    public float spread = 2f;
    public bool automatic = true;
}