using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{

    public static InteractionManager Instance {  get; set; }

    public Weapon hoveredWeapon = null;
    private AmmoBox hoveredAmmoBox = null;

    public Throwable hoveredThrowable = null;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        else
        {
            Instance = this;
        }
    }

    private void Update()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if(Physics.Raycast(ray,out hit))
        {
            GameObject objectHitByRaycast = hit.transform.gameObject;
            if(objectHitByRaycast.GetComponent<Weapon>() && objectHitByRaycast.GetComponent<Weapon>().isActiveWeapon == false)
            {
                hoveredWeapon = objectHitByRaycast.gameObject.GetComponent<Weapon>();
                hoveredWeapon.GetComponent<Outline>().enabled = true; 

                if (Input.GetKeyDown(KeyCode.F))
                {
                    WeaponManager.Instance.PickupWeapon(objectHitByRaycast.gameObject);
                   
                }
            }
            else
            {
                if (hoveredWeapon)
                {
                    hoveredWeapon.GetComponent<Outline>().enabled = false;
                }
            }

            // AmmoBox
            if (objectHitByRaycast.GetComponent<AmmoBox>())
            {
                hoveredAmmoBox = objectHitByRaycast.gameObject.GetComponent<AmmoBox>();
                hoveredAmmoBox.GetComponent<Outline>().enabled = true;

                if (Input.GetKeyDown(KeyCode.F))
                {
                    WeaponManager.Instance.PickupAmmo(hoveredAmmoBox);
                    Destroy(objectHitByRaycast.gameObject);
                }
            }
            else
            {
                if (hoveredAmmoBox)
                {
                    hoveredAmmoBox.GetComponent<Outline>().enabled = false;
                }
            }

            // Throwable
            if (objectHitByRaycast.GetComponent<Throwable>())
            {
                hoveredThrowable = objectHitByRaycast.gameObject.GetComponent<Throwable>();
                hoveredThrowable.GetComponent<Outline>().enabled = true;

                if (Input.GetKeyDown(KeyCode.F))
                {
                    WeaponManager.Instance.PickupThrowable(hoveredThrowable);
                    Destroy(objectHitByRaycast.gameObject);
                }
            }
            else
            {
                if (hoveredThrowable)
                {
                    hoveredThrowable.GetComponent<Outline>().enabled = false;
                }
            }

        }
    }
}
