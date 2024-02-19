using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] private Transform weaponHolder;
    private Inventory inventory;
    public int currentlyEquippedWeapon = 0;
    public GameObject currentWeaponObject;
    public GameObject rangedWeaponObject;
    public GameObject meleeWeaponObject;
    public GameObject grapplerWeaponObject;

    [SerializeField] private WeaponSO[] defaultMeleeWeapons = null;
    [SerializeField] private RobotType robotType;
    [SerializeField] private Animator animator;

    KeyCode meleeWeaponSlot = KeyCode.Alpha1;
    KeyCode rangedWeaponSlot = KeyCode.Alpha2;
    KeyCode grapplerWeaponSlot = KeyCode.Alpha3;

    private void Awake()
    {
        inventory = GetComponent<Inventory>();
    }

    private void Start()
    {
        Invoke("EquipDefaultMeleeWeapon", 0.1f);
    }

    private void Update()
    {
        if (GameManager.isGameOver || GameManager.isPaused) { return; }

        //Debug.Log(currentlyEquippedWeapon);
        if (Input.GetKeyDown(meleeWeaponSlot) && currentlyEquippedWeapon != 0 && inventory.GetWeapon(0)) 
        {
            PlayAnimations();
            Invoke("EquipMeleeWeapon", 0.2f);
        }
        if (Input.GetKeyDown(rangedWeaponSlot) && currentlyEquippedWeapon != 1 && inventory.GetWeapon(1))
        {
            PlayAnimations();
            Invoke("EquipRangedWeapon", 0.2f);
        }
        if (Input.GetKeyDown(grapplerWeaponSlot) && currentlyEquippedWeapon != 2 && inventory.GetWeapon(2))
        {
            PlayAnimations();
            Invoke("EquipGrappler", 0.2f);
        }
    }

    private void EquipMeleeWeapon()
    {
        UnequipWeapon();
        EquipWeapon(inventory.GetWeapon(0));
    }

    private void EquipRangedWeapon()
    {
        UnequipWeapon();
        EquipWeapon(inventory.GetWeapon(1));
    }
    private void EquipGrappler()
    {
        UnequipWeapon();
        EquipWeapon(inventory.GetWeapon(2));
    }


    public void EquipWeapon(WeaponSO weapon)
    {
        currentlyEquippedWeapon = (int)weapon.weaponType;
        //currentWeaponObject = Instantiate(weapon.weaponPrefab, weaponHolder);

        switch((int)weapon.weaponType)
        {
            case 0:
                meleeWeaponObject = Instantiate(weapon.weaponPrefab, weaponHolder);
                currentWeaponObject = meleeWeaponObject;
                break;
            case 1:
                rangedWeaponObject = Instantiate(weapon.weaponPrefab, weaponHolder);
                currentWeaponObject = rangedWeaponObject;
                break;
            case 2:
                grapplerWeaponObject = Instantiate(weapon.weaponPrefab, weaponHolder);
                currentWeaponObject = grapplerWeaponObject;
                break;
            default:
                break;
        }

    }

    public void UnequipWeapon()
    {
        Destroy(currentWeaponObject);
    }

    private void EquipDefaultMeleeWeapon()
    {
        inventory.AddWeapon(defaultMeleeWeapons[(int)robotType]);
        EquipWeapon(inventory.GetWeapon(0));
    }

    public enum RobotType
    {
        Blue, Green, Yellow
    }

    private void StartSwitchAnimation()
    {
        animator.SetBool("switching", true);
    }

    private void EndSwitchAnimation()
    {
        animator.SetBool("switching", false);
    }

    public void PlayAnimations()
    {
        StartSwitchAnimation();
        Invoke("EndSwitchAnimation", 0.1f);
    }
}
