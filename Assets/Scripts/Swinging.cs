using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swinging : MonoBehaviour
{
    [SerializeField] private LineRenderer lr;
    [SerializeField] private Transform gunTip;
    [SerializeField] private Transform playerCam;
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask whatIsGrappable;
    [SerializeField] private AudioSource playerAudio;
    [SerializeField] private AudioClip grapplerSound;

    private PlayerMovement playerMovement;
    private WeaponManager weaponManager;
    private Inventory inventory;

    private float maxSwingDistance = 25f;
    private bool canSwing;
    private float swingCooldown;

    private Vector3 swingPoint;
    private SpringJoint joint;

    private KeyCode swingKey = KeyCode.Mouse0;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        weaponManager = GetComponent<WeaponManager>();
        inventory = GetComponent<Inventory>();
        playerAudio = GetComponent<AudioSource>();
    }

    private void Start()
    {
        canSwing = true;
    }

    private void Update()
    {
        if (GameManager.isGameOver || GameManager.isPaused) { return; }


        if (Input.GetKeyDown(swingKey) && weaponManager.currentlyEquippedWeapon == 2 && canSwing) StartSwing();
        if(Input.GetKeyUp(swingKey)) StopSwing();
    }

    private void LateUpdate()
    {
        DrawRope();
    }

    private void StartSwing()
    {
        canSwing = false;
        WeaponSO grappleGun = inventory.GetWeapon(2);
        maxSwingDistance = grappleGun.range;
        swingCooldown = grappleGun.fireRate;

        playerMovement.swinging = true;
        gunTip = weaponManager.currentWeaponObject.transform.GetChild(0);

        RaycastHit hit;
        if (Physics.Raycast(playerCam.position, playerCam.forward, out hit, maxSwingDistance, whatIsGrappable))
        {
            playerAudio.PlayOneShot(grapplerSound);
            swingPoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = swingPoint;

            float distanceFromPoint = Vector3.Distance(gunTip.position, swingPoint);

            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;

            joint.spring = 5f;
            joint.damper = 3f;
            joint.massScale = 3f;

            lr.positionCount = 2;
        }

        Invoke("ResetSwing", swingCooldown);
    }

    private void ResetSwing()
    {
        canSwing = true;
    }

    private void StopSwing()
    {
        playerMovement.swinging = false;

        lr.positionCount = 0;
        Destroy(joint);
    }

    private void DrawRope()
    {
        if (!joint) return;
        gunTip = weaponManager.currentWeaponObject.transform.GetChild(0);

        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, swingPoint);
    }


}
