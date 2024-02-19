using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SkillShots : MonoBehaviour
{
    private Inventory inventory;
    private WeaponManager weaponManager;

    [SerializeField] private Transform swordTip;
    [SerializeField] private SkillSO[] defaultSkill;
    [SerializeField] private Transform playerCam;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource playerAudio;
    [SerializeField] private AudioClip fireballSound;
    [SerializeField] private AudioClip lightningSound;
    [SerializeField] private AudioClip explosionSound;

    public List<EnemyAI> enemiesHit;

    public Collider enemyCollider;

    private int currentWeapon;
    private bool readyToUseFirstSkill;
    private bool readyToUseSecondSkill;
    private bool readyToUseThirdSkill;

    private SkillSO firstSkill;
    private SkillSO secondSkill;
    private SkillSO thirdSkill;

    [SerializeField] private float explosionSpawnRange; 

    

    KeyCode firstSkillKey = KeyCode.Q;
    KeyCode secondSkillKey = KeyCode.E;
    KeyCode thirdSkillKey = KeyCode.R;

    private void Awake()
    {
        inventory = GetComponent<Inventory>();
        weaponManager = GetComponent<WeaponManager>();
        playerAudio = GetComponent<AudioSource>();
    }

    private void Start()
    {
        //Invoke("EquipDefaultSkill", 0.1f);

        readyToUseFirstSkill = true;
        readyToUseSecondSkill = true;
        readyToUseThirdSkill = true;
    }

    private void Update()
    {
        if (GameManager.isGameOver || GameManager.isPaused) { return; }

        currentWeapon = weaponManager.currentlyEquippedWeapon;
        
        
        if(Input.GetKeyDown(firstSkillKey) && currentWeapon == 0 && inventory.GetSkill(0) && readyToUseFirstSkill) 
        {
            animator.SetTrigger("usingSkill");
            firstSkill = inventory.GetSkill(0);
            swordTip = weaponManager.currentWeaponObject.transform.GetChild(0);

            Debug.Log(firstSkill.name);

            readyToUseFirstSkill = false;

            playerAudio.PlayOneShot(fireballSound);
            GameObject skill = Instantiate(firstSkill.skillPrefab, swordTip.position, Quaternion.LookRotation(playerCam.forward));

            skill.GetComponent<Rigidbody>().AddForce(playerCam.forward * 300f, ForceMode.Force);

            
            skill.GetComponent<SkillCollision>().skillShot = GetComponent<SkillShots>();

            Destroy(skill, 5f);

            Invoke("ResetFirstSkillCooldown", firstSkill.cooldown);

        }

        if (Input.GetKeyDown(secondSkillKey) && currentWeapon == 0 && inventory.GetSkill(1) && readyToUseSecondSkill)
        {
            animator.SetTrigger("usingSkill");
            secondSkill = inventory.GetSkill(1);
            swordTip = weaponManager.currentWeaponObject.transform.GetChild(0);

            Debug.Log(secondSkill.name);

            readyToUseSecondSkill = false;

            playerAudio.PlayOneShot(lightningSound);
            GameObject skill = Instantiate(secondSkill.skillPrefab, swordTip.position, Quaternion.LookRotation(playerCam.forward));


            skill.GetComponent<SkillCollision>().skillShot = GetComponent<SkillShots>();

            Destroy(skill, 2f);

            Invoke("ResetSecondSkillCooldown", secondSkill.cooldown);

        }

        if (Input.GetKeyDown(thirdSkillKey) && currentWeapon == 0 && inventory.GetSkill(2) && readyToUseThirdSkill)
        {
            animator.SetTrigger("usingSkill");
            thirdSkill = inventory.GetSkill(2);
            swordTip = weaponManager.currentWeaponObject.transform.GetChild(0);

            Debug.Log(thirdSkill.name);

            readyToUseThirdSkill = false;

            RaycastHit hit;
            GameObject skill;

            if (Physics.Raycast(playerCam.position, playerCam.forward, out hit, explosionSpawnRange))
            {
                playerAudio.PlayOneShot(explosionSound);
                skill = Instantiate(thirdSkill.skillPrefab, hit.point, Quaternion.LookRotation(playerCam.forward));
                skill.GetComponent<SkillCollision>().skillShot = GetComponent<SkillShots>();
                Destroy(skill, 2.5f);
            }

            Invoke("ResetThirdSkillCooldown", thirdSkill.cooldown);

            

        }


    }

    public void CollisionWithFireball(Collider other)
    {
        enemyCollider = other;
        EnemyAI enemy = enemyCollider.gameObject.GetComponent<EnemyAI>();
        enemy.TakeDamage(firstSkill.damage);
    }

    public void CollisionWithLightning(Collider other)
    {
        enemyCollider = other;
        EnemyAI enemy = enemyCollider.gameObject.GetComponent<EnemyAI>();
        enemy.TakeDamage(secondSkill.damage);
        enemy.StunEnemy();
    }

    public void CollisionWithExplosion(Collider other)
    {
        enemyCollider = other;
        EnemyAI enemy = enemyCollider.gameObject.GetComponent<EnemyAI>();
        enemy.TakeDamage(thirdSkill.damage);
    }


    private void EquipDefaultSkill()
    {
        inventory.AddSkill(defaultSkill[0]);
        inventory.AddSkill(defaultSkill[1]);
        inventory.AddSkill(defaultSkill[2]);
    }


    private void ResetFirstSkillCooldown()
    {
        readyToUseFirstSkill = true;
    }

    private void ResetSecondSkillCooldown()
    {
        readyToUseSecondSkill = true;
    }

    private void ResetThirdSkillCooldown()
    {
        readyToUseThirdSkill = true;
    }

}
