using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    [SerializeField] private float pickupRange;
    [SerializeField] private Transform playerCam;
    [SerializeField] private LayerMask pickupLayer;
    [SerializeField] private Inventory inventory;
    [SerializeField] private WeaponManager weaponManager;
    [SerializeField] private AudioSource playerAudio;
    [SerializeField] private AudioClip pickupSound;

    public KeyCode pickupKey = KeyCode.F;

    private void Awake()
    {
        inventory = GetComponent<Inventory>();
        weaponManager = GetComponent<WeaponManager>();
        playerAudio = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (GameManager.isGameOver || GameManager.isPaused) { return; }

        if (Input.GetKeyDown(pickupKey))
        {
            RaycastHit hit;

            if(Physics.Raycast(playerCam.position, playerCam.forward, out hit, pickupRange, pickupLayer))
            {
                Debug.Log("Hit: " + hit.transform.name);
                WeaponSO weaponToPickuUp;
                SkillSO skillToPickUp;
                
                if(hit.transform.GetComponent<WeaponToPickup>())
                {
                    weaponToPickuUp = hit.transform.GetComponent<WeaponToPickup>().weaponToPickup;
                    inventory.AddWeapon(weaponToPickuUp);
                    weaponManager.UnequipWeapon();
                    weaponManager.EquipWeapon(weaponToPickuUp);
                    playerAudio.PlayOneShot(pickupSound);
                }
                if(hit.transform.GetComponent<SkillToPickUp>())
                {
                    skillToPickUp = hit.transform.GetComponent<SkillToPickUp>().skillToPickup;
                    inventory.AddSkill(skillToPickUp);
                }

                
                Destroy(hit.transform.gameObject);
            }
        }
    }
}
