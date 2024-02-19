using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    private Animator animator;
    [SerializeField] private Transform orientation;

    [SerializeField] private int health;
    [SerializeField] private GameObject stunEffect;
    [SerializeField] private GameObject[] weaponToDrop;
    [SerializeField] private Score playerScore;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private Transform gunTip;

    private GameObject stunVisual;

    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;

    //Patrolling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange, stunned, alreadyDead;

    public float skillEffectLength;
    public float currentSkillEffectLength;

    [SerializeField] private GameObject projectile;

    [SerializeField] private int scoreForKill;

    private void Awake()
    {
        //player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        Invoke("GetPlayerReference", 1f);
        playerScore = FindObjectOfType<Score>();

        alreadyDead = false;
    }



    private void Update()
    {
        if (GameManager.isGameOver || GameManager.isPaused) { return; }

        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if(!playerInSightRange && !playerInAttackRange && !stunned)
        {
            Patrol();
        }
        if (playerInSightRange && !playerInAttackRange && !stunned)
        {
            ChasePlayer();
        }
        if (playerInSightRange && playerInAttackRange && !stunned)
        {
            AttackPlayer();
        }
        if(stunned)
        {
            Stunned();
        }
    }

    public void StunEnemy()
    {
        stunned = true;
        currentSkillEffectLength = skillEffectLength;
        animator.SetBool("stunned", true);
        stunVisual = Instantiate(stunEffect, transform.position + new Vector3(0f, 1f, 0f), Quaternion.identity);
        agent.SetDestination(transform.position);
    }

    private void Stunned()
    {
        if (currentSkillEffectLength <= 0)
        {
            stunned = false;
            animator.SetBool("stunned", false);
            Destroy(stunVisual);
        }
        else
        {
            currentSkillEffectLength -= Time.deltaTime;
        }

    }

    private void Patrol()
    {
        animator.SetBool("running", false);
        animator.SetBool("shooting", false);

        if (!walkPointSet)
        {
            SetWalkPoint();
        }
        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }
    }

    private void SetWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if(Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
        {
            walkPointSet = true;
        }
    }

    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);
        orientation.transform.LookAt(player);

        Vector3 targetPostition = new Vector3(player.position.x, this.transform.position.y, player.position.z);
        transform.LookAt(targetPostition);

        //transform.LookAt(player);


        if(!alreadyAttacked)
        {
            Invoke("InstantiateBullet", 0.75f);

            animator.SetBool("shooting", true);

            alreadyAttacked = true;
            Invoke("ResetAttack", timeBetweenAttacks);
            
        }
    }

    private void InstantiateBullet()
    {
        GameObject bullet = Instantiate(projectile, gunTip.position, Quaternion.identity);
        muzzleFlash.Play();
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.AddForce(orientation.transform.forward * 32f, ForceMode.Impulse);
        Destroy(bullet, 5f);
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    private void ChasePlayer()
    {
        animator.SetBool("running", true);
        animator.SetBool("shooting", false);
        agent.SetDestination(player.position);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        
        if(health <= 0 && !alreadyDead)
        {
            DestroyEnemy();
            alreadyDead = true;
        }
    }

    private void DropWeapon()
    {
        int randomWeaponIndex = Random.Range(0, weaponToDrop.Length);
        Instantiate(weaponToDrop[randomWeaponIndex], orientation.position, Quaternion.identity);
    }

    private void DestroyEnemy()
    {
        animator.SetBool("shooting", false);
        animator.SetTrigger("die");
        playerScore.AddToScore(scoreForKill);
        Destroy(gameObject, 2f);
        Destroy(stunVisual);

        Invoke("DropWeapon", 0.5f);

    }


    private void GetPlayerReference()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
