using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponFire : MonoBehaviour
{
    
    private float fireRate;
    private float bulletsShot;
    private Inventory inventory;
    private WeaponManager weaponManager;
    [SerializeField] private Transform playerCam;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private GameObject impactEffect;
    [SerializeField] private AudioSource playerAudio;
    [SerializeField] private AudioClip shootSound;



    private WeaponSO currentWeapon;

    RaycastHit hit;

    bool readyToShoot;

    [SerializeField] private Transform gunTip;

    KeyCode shootKey = KeyCode.Mouse0;

    private void Awake()
    {
        inventory = GetComponent<Inventory>();
        weaponManager = GetComponent<WeaponManager>();
        playerAudio = GetComponent<AudioSource>();
    }

    private void Start()
    {
        readyToShoot = true;
    }

    private void Update()
    {
        if (GameManager.isGameOver || GameManager.isPaused) { return; }

        currentWeapon = inventory.GetWeapon(weaponManager.currentlyEquippedWeapon);
        if (Input.GetKey(shootKey) && readyToShoot && weaponManager.currentWeaponObject != null && currentWeapon.weaponType == WeaponType.Ranged) 
        {
            bulletsShot = 0;
            Shoot();
        }
    }


    private void Shoot()
    {
        readyToShoot = false;


        float x = Random.Range(-currentWeapon.bulletSpread, currentWeapon.bulletSpread);
        float y = Random.Range(-currentWeapon.bulletSpread, currentWeapon.bulletSpread);


        Vector3 shotDirection = playerCam.forward + new Vector3(x, y, 0);

        
        fireRate = currentWeapon.fireRate;
        gunTip = weaponManager.currentWeaponObject.transform.GetChild(0);

        muzzleFlash.transform.position = gunTip.position;
        muzzleFlash.Play();
        playerAudio.PlayOneShot(shootSound);

        if(Physics.Raycast(playerCam.position, shotDirection, out hit, currentWeapon.range))
        {
            Debug.Log(hit.transform.name);

            Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            

            EnemyAI enemy = hit.transform.GetComponent<EnemyAI>();

            if(enemy != null)
            {
                enemy.TakeDamage(currentWeapon.damage);
            }
        }

        bulletsShot++;

        Invoke("ResetShot", fireRate);

        if(bulletsShot < currentWeapon.bulletsPerTap)
        {
            Invoke("Shoot", currentWeapon.timeBetweenShots);
        }
    }

    private void ResetShot()
    {
        readyToShoot = true;
    }
    
}
