using UnityEngine;
using System.Linq;
using System;
using Unity.VisualScripting;
using UnityEngine.VFX;
using System.Collections;

public class Mini3PlayerController : MonoBehaviour
{
    // [HideInInspector]
    // public bool tapToJumpInstructionTriggered = true;

    [SerializeField] private Animator animator;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private Transform[] groundChecks;
    [SerializeField] private Transform[] wallChecks;
    [SerializeField] private VisualEffect[] landingVFXs;


    [SerializeField] private Mini3Manager mini3Manager;
    [SerializeField] private StageManager stageManager;
    [SerializeField] private TouchInputMini3 touchInputManager;

    [Header("Jump Parameters")]

    [SerializeField] private float runSpeed = 30F;
    [SerializeField] private float maxJumpHeight = 10f;
    [SerializeField][Range(0, 1)] private float maxJumpLength = 1f;
    [SerializeField] private float baseGravity = -80f;
    [SerializeField] private float fallGravityMod = 1.5f;
    // [SerializeField] private float jumpHoldGravityMod = 0.5f;
    // [SerializeField] private float jumpReleaseMod = 2f;
    [SerializeField] private float coyoteTime = 0.5f;
    [SerializeField] private float jumpBufferTime = 0.1f;

    [SerializeField] private Mini3PlayerContact playerContact;
    private CharacterController characterController;
    private Vector3 velocity;
    public float horizontalInput {get; private set;}
    private float jumpForce;
    private float edgePushForce;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    private bool isJumping;
    private bool landingVFXQuota = false;
    private bool gamePaused = false;
    private bool finishedGame;


    private void Start()
    {
        playerContact.GetComponent<Mini3PlayerContact>();
        characterController = GetComponent<CharacterController>();
        horizontalInput = 1f;
        finishedGame = false;
        // SetNormalParameters();
        CalculateJumpForce();
    }

    private void OnValidate()
    {
        CalculateJumpForce();
    }

    private void Update()
    {   
        if(!gamePaused)
        {
            if(
                mini3Manager.CurrentMini3State != Mini3Manager.Mini3State.GameOver
            && (mini3Manager.CurrentMini3State == Mini3Manager.Mini3State.Intro || playerContact.gameTimer.isRunning)
            && !playerContact.gameEnded
            )
            {
                UpdateJump();    
            }
            characterController.Move(velocity * Time.deltaTime);
            UpdateMovement();
            ApplyGravity();
            UpdateAnimator();
            // Debug.Log(velocity);
        }
    }

    public void HandlePowerUpActivated(float duration)
    {
        //Debug.Log($"Power-up activated for {duration} seconds!");
        // Add code here to respond to power-up activation
        // SetEnhancedParameters();
    }

    public void HandlePowerUpDeactivated()
    {
        //Debug.Log("Power-up deactivated!");
        // Add code here to respond to power-up deactivation
        // SetNormalParameters();
    } 

    private void UpdateMovement()
    {
        transform.forward = new Vector3(Mathf.Abs(horizontalInput) - 1, 0, horizontalInput);
        Vector3 movement = new Vector3(0, 0, horizontalInput * runSpeed) * Time.deltaTime;
        
        if (!IsWallBlocked() && finishedGame == false)
        {
            velocity.z = horizontalInput * runSpeed;
            ApplyEdgePush();
        }
    }

    private bool IsWallBlocked()
    {
        var res = wallChecks.Any(wallCheck => Physics.CheckSphere(wallCheck.position, 0.1f, groundLayerMask, QueryTriggerInteraction.Ignore));
        // Debug.Log("WallBlocked: " + res);
        return res;
    }

    private void UpdateJump()
    {
        bool isGrounded = IsGrounded();
        UpdateCoyoteTime(isGrounded);
        UpdateJumpBuffer();

        if (CanJump())
        {
            StartJump();
        }

        // ModifyJumpHeight();
        ResetJumpStateIfGrounded(isGrounded);
    }

    private bool IsGrounded()
    {
        // var res = groundChecks.Any(groundCheck => Physics.CheckSphere(groundCheck.position, 0.1f, groundLayerMask, QueryTriggerInteraction.Ignore));
        // // Debug.Log("Grounded: " + res);
        // return res;
        return Physics.CheckSphere(transform.position, 0.1f, groundLayerMask, QueryTriggerInteraction.Ignore);
    }

    // private bool IsStuckOnEdge()
    // {
    //     // Check if we're not grounded and not wall blocked, but very close to a ground surface
    //     if (!IsGrounded() && !IsWallBlocked())
    //     {
    //         RaycastHit hit;
    //         float rayDist = characterController.radius * 1.01f;
    //         if (Physics.Raycast(transform.position, Vector3.forward, out hit, rayDist, groundLayerMask))
    //         {
    //             Debug.DrawLine(transform.position, hit.point, Color.red); // Visualize the ray in the Unity editor
    //             return true;
    //         }
    //     }
    //     return false;
    // }

    private void ApplyEdgePush()
    {
        // Vector3 pushDirection = (transform.forward + transform.up).normalized;
        // // Debug.Log("PUSHING CHARACTER!!");
        // velocity.y -= (velocity.y - edgePushForce / Mathf.Sqrt(2)) * Time.deltaTime;
        // Debug.Log(velocity);

        RaycastHit hit;
        float rayDist = characterController.radius * 1.01f;
        if (Physics.Raycast(transform.position, transform.forward, out hit, rayDist, groundLayerMask))
        {
            // We're stuck on an edge, apply a push force
            Debug.DrawLine(transform.position, hit.point, Color.red); // Visualize the ray in the Unity editor
            Vector3 pushDirection = (hit.normal + transform.up).normalized;
            velocity += pushDirection * edgePushForce * Time.deltaTime;
            // Debug.Log("Applying edge push force");
        }
    }

    private void UpdateCoyoteTime(bool isGrounded)
    {
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
    }

    private void UpdateJumpBuffer()
    {
        if (touchInputManager.tappedOnScreen || Input.GetKey(KeyCode.Space))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }
    }

    private bool CanJump()
    {
        return ((jumpBufferCounter > 0f 
            && coyoteTimeCounter > 0f 
            && !isJumping
            && IsGrounded())
            || Physics.Raycast(transform.position, transform.forward, characterController.radius * 1.01f, groundLayerMask)) 
            && touchInputManager.tappedOnScreen
            // && tapToJumpInstructionTriggered
            ;
    }

    private void StartJump()
    {
        velocity.y = jumpForce;
        isJumping = true;
        jumpBufferCounter = 0f;
        landingVFXQuota = true;
    }

    // private void ModifyJumpHeight()
    // {
    //     if (!isJumping || velocity.y <= 0) return;

    //     float gravityMod = (jumpButtonHeld || Input.GetKey(KeyCode.Space)) ? jumpHoldGravityMod : jumpReleaseMod;
    //     velocity.y += baseGravity * gravityMod * Time.deltaTime;
    // }

    private void ResetJumpStateIfGrounded(bool isGrounded)
    {
        if (isGrounded && velocity.y < 0)
        {
            isJumping = false;
            velocity.y = 0f;
            if(landingVFXQuota == true)
            {
                // Debug.Log("Trigger landingVFX!!");
                foreach (VisualEffect landingVFX in landingVFXs)
                {
                    landingVFX.Play();
                }
                landingVFXQuota = false;
            }
            if (touchInputManager != null)
            {
                touchInputManager.tappedOnScreen = false;
            }
        }
    }

    private void ApplyGravity()
    {
        float gravityMultiplier = velocity.y < 0 ? fallGravityMod : 1f;
        velocity.y += baseGravity * gravityMultiplier * Time.deltaTime;
    }

    private void CalculateJumpForce()
    {
        float timeToApex = maxJumpLength / 2f;
        float calculatedGravity = -2f * maxJumpHeight / Mathf.Pow(timeToApex, 2);
        jumpForce = Mathf.Abs(calculatedGravity) * timeToApex;
    }

    private void UpdateAnimator()
    {
        animator.SetBool("isGrounded", IsGrounded());
        animator.SetFloat("verticalSpeed", velocity.y);
    }

    public void ResetVelocity()
    {
        velocity = Vector3.zero;
    }

    public IEnumerator SlowDownPlayer(float durationToStop, bool finished)
    {   
        if(finished)
        {
            animator.SetTrigger("finished");
        }
        else
        {
            animator.SetTrigger("failed");
        }
        float elapsedTime = 0;
        float duration = durationToStop;
        while (elapsedTime < duration)
        {
            velocity.z = Mathf.Lerp(velocity.z, 0f, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        velocity.z = 0f;
        finishedGame = true;
    }

    public void TriggerPause(float duration)
    {
        StartCoroutine(PlayerPause(duration));
    }

    private IEnumerator PlayerPause(float duration)
    {
        gamePaused = true;
        animator.speed = 0f;
        yield return new WaitForSeconds(duration);
        playerContact.gameTimer.StartTimer();
        animator.speed = 1f;
        gamePaused = false;
    }
}