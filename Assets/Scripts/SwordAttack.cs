using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    [SerializeField] private float attackDelay;

    [SerializeField] private WeaponManager weaponManager;
    [SerializeField] private Inventory inventory;
    [SerializeField] private Transform playerCam;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource playerAudio;
    [SerializeField] private AudioClip swordSound;

    [SerializeField] private GameObject impactEffect;

    private WeaponSO meleeWeapon;

    private KeyCode swordAttackKey = KeyCode.Mouse0;

    bool readyToAttack;
    int attackCount;

    private void Awake()
    {
        inventory = GetComponent<Inventory>();
        weaponManager = GetComponent<WeaponManager>();
        playerAudio = GetComponent<AudioSource>();
    }

    private void Start()
    {
        readyToAttack = true;
        attackCount = 0;
    }

    private void Update()
    {
        if (GameManager.isGameOver || GameManager.isPaused) { return; }

        meleeWeapon = inventory.GetWeapon(0);

        if(Input.GetKeyDown(swordAttackKey) && weaponManager.currentlyEquippedWeapon == 0) 
        {
            MeleeAttack();
        }
    }

    private void MeleeAttack()
    {
        if (!readyToAttack) return;

        readyToAttack = false;

        playerAudio.PlayOneShot(swordSound,1f);

        Invoke("ResetAttack", meleeWeapon.fireRate);
        Invoke("ImpactRaycast", attackDelay);

        if(attackCount == 0)
        {
            animator.SetTrigger("attack 1");
            attackCount++;
        }
        else
        {
            animator.SetTrigger("attack 2");
            attackCount = 0;
        }


    }

    private void ImpactRaycast()
    {
        RaycastHit hit;
        if(Physics.Raycast(playerCam.position, playerCam.forward, out hit, meleeWeapon.range))
        {
            Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            EnemyAI enemy = hit.transform.GetComponent<EnemyAI>();
            if(enemy != null)
            {
                enemy.TakeDamage(meleeWeapon.damage);
            }
        }
    }

    private void ResetAttack()
    {
        readyToAttack = true;
    }

}
