using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon SO", order = 1)]
public class WeaponSO : ScriptableObject
{
    public string weaponName;
    public Sprite weaponIcon;
    public GameObject weaponPrefab;
    public float fireRate;
    public float bulletSpread;
    public int bulletsPerTap;
    public float timeBetweenShots;

    public int damage;

    public float range;
    public WeaponType weaponType;
}

public enum WeaponType
{
    Melee, Ranged, Grappler
}
