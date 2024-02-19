using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] private InventoryUI[] inventoryUI;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI scoreText;

    public void UpdateHealthUI(int health)
    {
        healthText.text = health.ToString();
    }

    public void UpdateWeaponUI(WeaponSO newWeapon, int inventorySlot)
    {
        inventoryUI[inventorySlot].UpdateInfo(newWeapon.weaponIcon);
    }

    public void UpdateSkillUI(SkillSO newSkill, int inventorySlot)
    {
        inventorySlot += 3;
        inventoryUI[inventorySlot].UpdateInfo(newSkill.skillIcon);
    }

    public void UpdateScoreUI(int score)
    {
        scoreText.text = score.ToString();
    }

}
