using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public Transform weaponHolder;  // Empty GameObject inside Player to hold the weapon
    private GameObject currentWeapon;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 3f))
            {
                if (hit.collider.CompareTag("Weapon") && currentWeapon == null)
                {
                    PickupWeapon(hit.collider.gameObject);
                }
                else if (currentWeapon != null)
                {
                    DropWeapon();
                }
            }
        }
    }

    void PickupWeapon(GameObject weapon)
    {
        currentWeapon = weapon;
        weapon.transform.SetParent(weaponHolder);
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;
        weapon.GetComponent<Rigidbody>().isKinematic = true;
    }

    void DropWeapon()
    {
        currentWeapon.transform.SetParent(null);
        currentWeapon.GetComponent<Rigidbody>().isKinematic = false;
        currentWeapon = null;
    }
}
