using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [Header("Movement")]
    private float movementSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float moveSpeedMultiplier;
    public float groundDrag;
    public float slideSpeed;
    public float wallRunningSpeed;
    public float swingingSpeed;
    public float maxSpeedDifferenceForLerping;

    private float desiredMovementSpeed;
    private float lastDesiredMovementSpeed;

    [Header("References")]
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform raycastStartPoint;
    [SerializeField] private Transform playerObj;
    Rigidbody rb;

    float horizontalInput;
    float verticalInput;

    [Header("KeyBinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.C;

    Vector3 moveDirection;


    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;
    private float crouchForce = 5f;

    [Header("Ground Check")]
    public float playerHeight;
    [SerializeField] private LayerMask whatIsGround;
    bool grounded;

    [Header("Slope Movement")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;
    private float slopeSpeedMultiplier = 20f;
    

    public MovementState movementState;
    public enum MovementState
    {
        walking,
        sprinting,
        air,
        wallrunning,
        sliding,
        crouching,
        swinging
    }

    public bool sliding;
    public bool wallrunning;
    public bool swinging;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        
    }

    private void Start()
    {
        rb.freezeRotation = true;
        readyToJump = true;

        startYScale = playerObj.localScale.y;
    }

    private void Update()
    {
        // Check if there is ground below the player
        grounded = Physics.Raycast(raycastStartPoint.transform.position, Vector3.down, playerHeight * 0.5f, whatIsGround);

        MyInput();
        SpeedControl();
        StateHandler();

        // Handle drag so that the player doesn't slide on ground
        if (grounded)
        {
            rb.drag = groundDrag;
        }
        else
            rb.drag = 0;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {

        if (GameManager.isGameOver || GameManager.isPaused) { return; }


        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if(Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke("ResetJump", jumpCooldown);
        }

        // Start crouching
        if(Input.GetKeyDown(crouchKey)) {
            playerObj.localScale = new Vector3(playerObj.localScale.x, crouchYScale, playerObj.localScale.z);
            rb.AddForce(Vector3.down * crouchForce, ForceMode.Impulse);
        }
        // Stop crouching
        if (Input.GetKeyUp(crouchKey))
        {
            playerObj.localScale = new Vector3(playerObj.localScale.x, startYScale, playerObj.localScale.z);
        }
    }

    private void StateHandler()
    {
        // Mode - Swinging
        if (swinging)
        {
            movementState = MovementState.swinging;

            desiredMovementSpeed = swingingSpeed;
        }

        // Mode - Wallrunning
        else if(wallrunning)
        {
            movementState = MovementState.wallrunning;

            desiredMovementSpeed = wallRunningSpeed;
        }


        // Mode - Sliding
        else if(sliding)
        {
            movementState = MovementState.sliding;

            if(OnSlope() && rb.velocity.y < 0.1f)
            {
                desiredMovementSpeed = slideSpeed;
            }
            else
            {
                desiredMovementSpeed = sprintSpeed;
            }
        }

        // Mode - Crouching
        else if (grounded && Input.GetKey(crouchKey))
        {
            movementState = MovementState.crouching;
            desiredMovementSpeed = crouchSpeed;
        }

        // Mode - Sprinting
        else if (grounded && Input.GetKey(sprintKey))
        {
            movementState = MovementState.sprinting;
            desiredMovementSpeed = sprintSpeed;
        }

        // Mode - Walking
        else if(grounded)
        {
            movementState = MovementState.walking;
            desiredMovementSpeed = walkSpeed;
        }

        // Mode - Air
        else
        {
            movementState = MovementState.air;
        }

        if(Mathf.Abs(desiredMovementSpeed - lastDesiredMovementSpeed) > maxSpeedDifferenceForLerping && movementSpeed != 0)
        {
            StopAllCoroutines();
            StartCoroutine(SmoothlyLerpMoveSpeed());
        }
        else
        {
            movementSpeed = desiredMovementSpeed;
        }

        lastDesiredMovementSpeed = desiredMovementSpeed;
    }

    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        float time = 0;
        float speedDifference = Mathf.Abs(desiredMovementSpeed - movementSpeed);  
        float startValue = movementSpeed;

        while(time < speedDifference)
        {
            movementSpeed = Mathf.Lerp(startValue, desiredMovementSpeed, time / speedDifference);
            time += Time.deltaTime * 3;
            yield return null;
        }

        movementSpeed = desiredMovementSpeed;
    }


    private void MovePlayer()
    {
        //if (swinging) return;

        // Calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // On slope
        if(OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection(moveDirection) * movementSpeed * slopeSpeedMultiplier, ForceMode.Force);

            if(rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }

        else if(grounded)
        {
            rb.AddForce(moveDirection.normalized * movementSpeed * moveSpeedMultiplier, ForceMode.Force);
        }
        else if(!grounded && !swinging)
        {
            rb.AddForce(moveDirection.normalized * movementSpeed * moveSpeedMultiplier * airMultiplier, ForceMode.Force);
        }
        else if(swinging)
        {
            rb.AddForce(moveDirection.normalized * swingingSpeed* moveSpeedMultiplier, ForceMode.Force);
        }

        // Turn off gravity while on slope
        if(!wallrunning) rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {

        // Limiting speed on slope
        if (OnSlope() && !exitingSlope)
        { 
            if (rb.velocity.magnitude > movementSpeed)
            {
                rb.velocity = rb.velocity.normalized * movementSpeed;
            }
            // Display player speed in console for testing
            //Debug.Log(rb.velocity.magnitude);
        }

        // Limiting speed on ground or in air
        else
        {
            Vector3 velocityXZ = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            // Display player speed in console for testing
            //Debug.Log(velocityXZ.magnitude);

            // Limit velocity if needed
            if (velocityXZ.magnitude > movementSpeed)
            {
                Vector3 limitedVel = velocityXZ.normalized * movementSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
        
    }

    private void Jump()
    {
        exitingSlope = true;

        // Reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }


    public bool OnSlope()
    {
        if(Physics.Raycast(raycastStartPoint.transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }
    
}
