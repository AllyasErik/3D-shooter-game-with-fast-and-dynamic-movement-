using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private WeaponSO[] weapons;
    [SerializeField] private SkillSO[] skills;
    private int weaponInventorySize = 3;
    private int skillInventorySize = 3;

    private PlayerHUD playerHUD;

    private void Awake()
    {
        playerHUD = GetComponent<PlayerHUD>();
        weapons = new WeaponSO[weaponInventorySize];
        skills = new SkillSO[skillInventorySize];
    }


    public void AddWeapon(WeaponSO newWeapon)
    {
        int newWeaponIndex = (int)newWeapon.weaponType;

        if (weapons[newWeaponIndex] != null)
        {
            RemoveWeapon(newWeaponIndex);
        }
        weapons[newWeaponIndex] = newWeapon;

        //Update weapon UI 
        playerHUD.UpdateWeaponUI(newWeapon, newWeaponIndex);

    }

    public void AddSkill(SkillSO newSkill)
    {
        int newSkillIndex = (int)newSkill.skillIndex;

        skills[newSkillIndex] = newSkill;

        //Update weapon UI 
        playerHUD.UpdateSkillUI(newSkill, newSkillIndex);

    }

    public void RemoveWeapon(int weaponIndex)
    {
        weapons[weaponIndex] = null;
    }

    public WeaponSO GetWeapon(int weaponIndex)
    {
        return weapons[weaponIndex];
    }

    public SkillSO GetSkill(int skillIndex)
    {
        return skills[skillIndex];
    }
}
