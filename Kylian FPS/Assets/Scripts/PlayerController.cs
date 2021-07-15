using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Variables


    [Header("Character Properties")]

    [SerializeField]
    float runSpeed = 10f;

    [SerializeField]
    float fuelConsumeWhileJump = 20;

    [SerializeField]
    float movementMultiplier = 10f;

    [SerializeField]
    float airSpeedControl = 0.4f;

    [SerializeField]
    float groundCheckRadius = 0.4f;

    [SerializeField]
    LayerMask groundLayer;

    [SerializeField]
    float groundDrag = 6f;

    [SerializeField]
    float airDrag = 2f;

    [SerializeField]
    float jumpForce = 5f;

    [SerializeField]
    bool isGrounded;

    float playerHeight = 2f;

    Vector3 moveDirection;
    Vector3 slopeMoveDirection;

    [Header("Wall Jump Properties")]

    [SerializeField]
    float minJumpHeight = 1.5f;

    [SerializeField]
    float wallDistance = 0.5f;

    [SerializeField]
    float wallRunGravity = 1.5f;

    [SerializeField]
    float wallJumpForce;

    public static bool wallOnLeft = false;

    public static bool wallOnRight = false;

    RaycastHit leftWallHit;
    RaycastHit rightWallHit;

    public static event Action onWallRun;
    public static event Action onNotWallRun;

    [Header("References")]

    [SerializeField]
    Transform groundCheck;

    [SerializeField]
    Rigidbody playerRB;

    [SerializeField]
    Transform orientation;

    [SerializeField]
    InputManager inputManager;

    [SerializeField]
    GameManager gameManager;

    RaycastHit slopeRaycast;

    private bool ifOnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeRaycast, playerHeight * 0.5f + 0.5f))
        {
            if (slopeRaycast.normal != Vector3.up)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    #endregion

    private void Start()
    {
        playerRB = GetComponent<Rigidbody>();
        playerRB.freezeRotation = true;

        inputManager = GetComponent<InputManager>();
    }

    private void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
        slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeRaycast.normal);

        ProcessInput();
        ControlDrag();
        CheckForRunnableWall();
    }

    void ProcessInput()
    {
        moveDirection = orientation.forward * InputManager.verticalMove + orientation.right * InputManager.horizontalMove;

        if (inputManager.OnNormalJumpPressed() && isGrounded  && gameManager.fuel > fuelConsumeWhileJump)
        {
            Jump();
        }
    }

    void ControlDrag()
    {
        if (isGrounded)
        {
            playerRB.drag = groundDrag;
        }
        else
        {
            playerRB.drag = airDrag;
        }
    }

    private void FixedUpdate()
    {
        Movement();
    }

    void Movement()
    {
        if (isGrounded && !ifOnSlope())
        {
            playerRB.AddForce(moveDirection.normalized * runSpeed * movementMultiplier, ForceMode.Acceleration);
        }
        else if (isGrounded && ifOnSlope())
        {
            playerRB.AddForce(slopeMoveDirection.normalized * runSpeed * movementMultiplier, ForceMode.Acceleration);
        }
        else
        {
            playerRB.AddForce(moveDirection.normalized * runSpeed * movementMultiplier * airSpeedControl, ForceMode.Acceleration);
        }
    }

    void Jump()
    {
        playerRB.velocity = new Vector3(playerRB.velocity.x, 0, playerRB.velocity.z);
        playerRB.AddForce(transform.up * jumpForce, ForceMode.Impulse);

        gameManager.fuel -= fuelConsumeWhileJump;
    }

    #region WallRun

    bool CanWallRun()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight);
    }

    void CheckForRunnableWall()
    {
        wallOnLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallDistance);
        wallOnRight = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallDistance);

        if (CanWallRun())
        {
            if (wallOnLeft)
            {
                StartWallRun();
            }
            else if (wallOnRight)
            {
                StartWallRun();
            }
            else
            {
                StopWallRun();
            }
        }
        else
        {
            StopWallRun();
        }
    }

    void StartWallRun()
    {
        playerRB.useGravity = false;

        playerRB.AddForce(Vector3.down * wallRunGravity, ForceMode.Force);

        onWallRun?.Invoke();

        if (inputManager.OnWallJumpPressed() && gameManager.fuel > fuelConsumeWhileJump)
        {
            if (wallOnLeft)
            {
                Vector3 wallJumpDir = transform.up + leftWallHit.normal;
                playerRB.velocity = new Vector3(playerRB.velocity.x, 0, playerRB.velocity.z);
                playerRB.AddForce(wallJumpDir * wallJumpForce * 100, ForceMode.Force);
            }
            else if (wallOnRight)
            {
                Vector3 wallJumpDir = transform.up + rightWallHit.normal;
                playerRB.velocity = new Vector3(playerRB.velocity.x, 0, playerRB.velocity.z);
                playerRB.AddForce(wallJumpDir * wallJumpForce * 100, ForceMode.Force);
            }
            gameManager.fuel -= fuelConsumeWhileJump;
        }
    }

    void StopWallRun()
    {
        playerRB.useGravity = true;
        onNotWallRun?.Invoke();
    }

    #endregion
}
