using Unity.VisualScripting;
using UnityEngine;

public class InWorldPlayerMovement : MonoBehaviour
{   
    [Header("Movement Parameters")]
    [SerializeField] private float baseSpeed = 4f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private AnimationCurve accelerationCurve;

    [field: Header("Collisions")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private AnimationCurve slopeSpeedModifierCurve;
    [field: SerializeField] public CapsuleColliderUtility colliderUtility {get; private set;}
    
    
    [Header("References")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Animator animator;
    [SerializeField] private TouchInputInWorld touchInputManager;
    


    [Tooltip("Movement")]
    private float speedModifier = 1f;
    private float slopeSpeedModifier = 1f;
    private Vector2 movementInput;
    private Vector3 moveDirection;

    
    [Tooltip("Physics")]
    // private new CapsuleCollider collider;
    private Rigidbody rb;

 
    // private bool isGrounded;

    private void Awake()
    {
        // collider = GetComponent<CapsuleCollider>();

        colliderUtility.Initialize(gameObject);
        colliderUtility.CalculateCapsuleColliderDimensions();

        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        // Configure Rigidbody settings
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.useGravity = true;
    }

    private void OnValidate()
    {
        colliderUtility.Initialize(gameObject);
        colliderUtility.CalculateCapsuleColliderDimensions();
    }

    private void Start()
    {
        // this.gameObject.transform.position = GameManager.Instance.spawnPosition;
    }

    private void Update()
    {
        // Get input and update animator
        movementInput = touchInputManager.movementAmount;
        UpdateAnimator();

        // if(Input.GetKeyDown(KeyCode.Space))
        // {
        //     Jump();
        // }
    }

    private void FixedUpdate()
    {   
        FloatCapsule();
        HandleMovement();
    }

    private void FloatCapsule()
    {
        Vector3 capsuleColliderCenterInWorldSpace = colliderUtility.capsuleColliderData.capsuleCollider.bounds.center;
        Ray downwardsRayFromCapsuleCenter = new Ray(capsuleColliderCenterInWorldSpace, Vector3.down);

        if(Physics.Raycast(downwardsRayFromCapsuleCenter, out RaycastHit hit, 
                        colliderUtility.slopeData.FloatRayDistance, groundLayer, 
                        QueryTriggerInteraction.Ignore))
        {
            float groundAngle = Vector3.Angle(hit.normal, -downwardsRayFromCapsuleCenter.direction);

            slopeSpeedModifier = SetSlopeSpeedModifierOnAngle(groundAngle);

            if(slopeSpeedModifier == 0f)
            {
                return;
            }

            float distanceToFloatingPoint = colliderUtility.capsuleColliderData.colliderCenterInLocalSpace.y * transform.localScale.y - hit.distance;

            if(distanceToFloatingPoint == 0f)
            {
                return;
            }

            float currentVerticalVelocity = rb.velocity.y;
            float amountToLift = distanceToFloatingPoint * colliderUtility.slopeData.StepReachForce - currentVerticalVelocity;
        
            Vector3 liftForce = new Vector3(0f, amountToLift, 0f);
            rb.AddForce(liftForce, ForceMode.VelocityChange);

        }

    }

    private void HandleMovement()
    {
        if (movementInput.magnitude > 0 && speedModifier != 0f)
        {
            moveDirection = CalculateMoveDirection();
            
            // Determine target speed based on input magnitude
            float targetSpeed = GetMovementSpeed();
            
            // Calculate target velocity
            Vector3 targetVelocity = moveDirection * targetSpeed;

            Vector3 currentHorizontalVelocity = rb.velocity;
            currentHorizontalVelocity.y = 0f;

            rb.AddForce(targetVelocity - currentHorizontalVelocity, ForceMode.VelocityChange);

            // Handle Rotation
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 
                rotationSpeed * Time.fixedDeltaTime);
        }

        else
        {
            Vector3 currentVelocity = rb.velocity;
            currentVelocity.y = 0f;
            // currentVelocity.x = 0f;
            // currentVelocity.z = 0f;
            // rb.velocity = currentVelocity;
            rb.AddForce(Vector3.zero - currentVelocity, ForceMode.Force);
        }
    }

    private void Jump()
    {
        // If running (speedModifier == 2)
        if (speedModifier == 2f)
        {
            // Calculate horizontal velocity components
            Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            Vector3 jumpForce = horizontalVelocity * 0.5f; // Half of the horizontal force
            jumpForce.y = 5f; // Set vertical component to 5
            
            rb.AddForce(jumpForce, ForceMode.VelocityChange);
        }
        // If walking (speedModifier == 1)
        else
        {
            rb.AddForce(new Vector3(0f, 5f, 0f), ForceMode.VelocityChange);
        }
        
        //Debug.Log("Jumped with speed modifier: " + speedModifier);
    }

    private float SetSlopeSpeedModifierOnAngle(float angle)
    {
        return slopeSpeedModifierCurve.Evaluate(angle);
    }

    private Vector3 CalculateMoveDirection()
    {
        Vector3 direction = new Vector3(movementInput.x, 0, movementInput.y);
        return Quaternion.AngleAxis(cameraTransform.rotation.eulerAngles.y, Vector3.up) * direction;
    }

    private float GetMovementSpeed()
    {
        if(movementInput.magnitude > 0.5f)
        {
            speedModifier = 2f;
        }
        else
        {
            speedModifier = 1f;
            slopeSpeedModifier = 1f;
        }
        
        return baseSpeed * speedModifier * slopeSpeedModifier;
    }

    private void UpdateAnimator()
    {
        // Calculate current horizontal speed
        float horizontalSpeed = new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude;
        
        animator.SetBool("isMoving", horizontalSpeed > 0.1f);
        animator.SetBool("isRunning", horizontalSpeed > baseSpeed);
        animator.SetFloat("speed", horizontalSpeed / baseSpeed);
    }
}