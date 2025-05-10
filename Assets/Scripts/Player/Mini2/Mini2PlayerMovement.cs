using System;
using System.Collections;
using UnityEngine;

public class Mini2PlayerMovement : MonoBehaviour
{
    public Transform foot;
    [SerializeField] private Transform playerVisual;
    [SerializeField] private float visualTiltAngle = 30f;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform cameraTransform;
    // [SerializeField] private Transform cameraArmTransform;F
    [SerializeField] private TouchInputMini2 touchInputManager;

    [SerializeField] private LayerMask footLayerMask;

    [SerializeField] private bool realisticAcceleration;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private AnimationCurve animCurve;
    [SerializeField] private AnimationCurve inputCurve;

    [SerializeField] private float moveSpeed = 20f;
    //min 15, max 30
    [SerializeField] private float gravity = -800f;

    [SerializeField] private Mini2MeshTrail meshTrail;
    [SerializeField] private GameObject[] bubbles;

    public bool isFalling {get; private set;}
    private float rayCastLength = float.MaxValue;
    private RaycastHit[] hits;
    private Vector3 footDir;
    private Vector3 normalVector;

    public bool isOnFootSurface = false;

    private Vector2 movementInput;

    private Rigidbody rb;

    private Mini2Manager mini2Manager;
    private Mini2Manager.Mini2State currentMini2State;

    // private float counter;
    // Add variables for spring movement

    // private float initialLateralVelocity = 0.5f;
    private float currentLateralVelocity = 0f;
    private float targetLateralVelocity = 0f;

    [Header("Wall Climbing")]
    [SerializeField] private float wallCheckHeight = -.75f; // Height above pivot to check for walls
    [SerializeField] private float wallCheckDistance = 2f; // How far forward to check for walls
    [SerializeField] private float wallClimbRotationSpeed = 45f; // Degrees per second to rotate upward
    [SerializeField] private float maxWallClimbAngle = 180f; // Maximum angle we can rotate up
    
    [SerializeField] private bool isWallClimbing = false;
    private RaycastHit wallHit;


    [Header("Debug Visualization")]
    [SerializeField] private bool showDebugGizmos = true;
    [SerializeField] private Color rayColorNoHit = Color.yellow;
    [SerializeField] private Color rayColorHit = Color.red;
    [SerializeField] private float gizmoSphereRadius = 0.1f;
    private Vector3 lastRayStart;
    private bool lastRayHit;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        isFalling = true;
        rb.useGravity = false;

        // counter = 0f;

        mini2Manager = FindObjectOfType<Mini2Manager>();
    }
    private void Start()
    {
        normalVector = (transform.position - foot.position).normalized;

        mini2Manager.onMini2StateChanged.AddListener(HandleMini2StateChanged);

        animator.speed = 0;

        meshTrail.enabled = false;
        foreach (GameObject bubble in bubbles){
            bubble.SetActive(false);
        }
    }

    private void HandleMini2StateChanged(Mini2Manager.Mini2State state)
    {
        currentMini2State = state;

        if(currentMini2State == Mini2Manager.Mini2State.GamePlay)
        {
            rb.isKinematic = false;
            animator.speed = 1;
            if(isFalling)
            {
                StartCoroutine(ShrinkPlayerVisual());
            }
            
        }

        else if(currentMini2State == Mini2Manager.Mini2State.GameOver || currentMini2State == Mini2Manager.Mini2State.Instruction)
        {
            rb.isKinematic = true;
        }
    }

    private void Update()
    {
        // Debug.Log(counter);
        if(currentMini2State == Mini2Manager.Mini2State.GamePlay)
        {
            movementInput = touchInputManager.movementAmount;
            targetLateralVelocity = movementInput.x;
            // counter += Time.deltaTime;
        }

        // UpdateAnimator();

        switch(mini2Manager.timeRemaining){
            case >= 90f:
                moveSpeed = 19f;
                gravity = -700f;
                break;
            case >= 60f:
                moveSpeed = 21f;
                break;
            case >= 30f:
                moveSpeed = 23f;
                break;
            case > 0f:
                gravity = -750f;
                moveSpeed = 25f;
                break;
            default:
                moveSpeed = 15f;
                break;
        }
    }

    private void FixedUpdate()
    {
        if(currentMini2State == Mini2Manager.Mini2State.GamePlay)
        {
            
            rb.useGravity = isFalling;
            if(isFalling == false)
            {   
                Movement();
                ApplyGravity();
                CheckForWallClimb();
            }
            
        }
    }

    private void Movement()
    {
        // Interpolate lateral velocity using inputCurve
        float springStrength = inputCurve.Evaluate(Mathf.Abs(targetLateralVelocity - currentLateralVelocity));
        currentLateralVelocity = Mathf.Lerp(currentLateralVelocity, targetLateralVelocity, springStrength * Time.fixedDeltaTime);
        // currentLateralVelocity = movementInput.x == 0 ? movementInput.x : Mathf.Lerp(currentLateralVelocity, targetLateralVelocity, springStrength * Time.fixedDeltaTime);

        // Tilt visual transform along z axis wrt currentLateralvelocity * -30f
        playerVisual.localRotation = Quaternion.Euler(0, 0, currentLateralVelocity * -visualTiltAngle);

        // Vector3 movement = new Vector3(movementInput.x, 0f, 1f);
        Vector3 movement = new Vector3(realisticAcceleration ? currentLateralVelocity : movementInput.x, 0f, 1f);
        // Debug.Log(movement);
        Vector3 cameraRotation = new Vector3(0, cameraTransform.localEulerAngles.y, 0);
        Vector3 direction = Quaternion.Euler(cameraRotation) * movement;
        Vector3 movement_direction = transform.forward * direction.z + transform.right * direction.x;
        movement_direction.Normalize();


        Quaternion movementRotation = Quaternion.LookRotation(movement_direction, normalVector);  // Calculate target rotation
        Quaternion footRotation = Quaternion.FromToRotation(transform.up, normalVector) * transform.rotation;
        
        Vector3 combineRotation = (movementRotation * Vector3.forward + footRotation * Vector3.forward).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(combineRotation, normalVector);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime * animCurve.Evaluate(Time.time));


        Vector3 currentNormalVelocity = Vector3.Project(rb.velocity, normalVector.normalized);
        rb.velocity = currentNormalVelocity + (movement_direction * moveSpeed);

    }

    private void ApplyGravity()
    {
        if(foot == null) return;

        hits = Physics.RaycastAll(transform.position, -transform.up, rayCastLength, footLayerMask);

        if(hits.Length == 0)
        {
            footDir = foot.position - transform.position;
            //Debug.Log("Hits.Length = 0: I am called!");
            hits = Physics.RaycastAll(transform.position, footDir, rayCastLength, footLayerMask);
        }

        GetFootNormal();
        rb.AddForce(normalVector.normalized * gravity, ForceMode.Acceleration);
        hits = new RaycastHit[0];
    }

    private void CheckForWallClimb()
    {
        // Calculate the ray start position
        lastRayStart = transform.position + transform.up * wallCheckHeight;
        
        // Perform the raycast and store hit info for gizmos
        lastRayHit = Physics.Raycast(lastRayStart, transform.forward, out wallHit, wallCheckDistance, footLayerMask);

        if (lastRayHit)
        {
            isWallClimbing = true;
            
            float currentAngle = Vector3.Angle(transform.up, wallHit.normal);
            
            if (currentAngle < maxWallClimbAngle)
            {
                float rotationAmount = wallClimbRotationSpeed * Time.fixedDeltaTime;
                Quaternion targetRotation = transform.rotation * Quaternion.Euler(rotationAmount, 0, 0);
                normalVector = wallHit.normal;
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
                rb.AddForce((transform.up + transform.forward) * moveSpeed * 0.5f, ForceMode.Acceleration);
            }
        }
        else
        {
            isWallClimbing = false;
        }
    }

    private void GetFootNormal()
    {
        if(foot == null) return;
        // normalVector = (transform.position - foot.position).normalized;

        for(int i = 0; i < hits.Length; i++)
        {
            if(hits[i].transform == foot)
            {
                normalVector = hits[i].normal.normalized;
                break;
            }
        }

        return;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform == foot)
        {
            isOnFootSurface = true;
            isFalling = false;
            meshTrail.enabled = false;
            rb.useGravity = false;

            foreach (GameObject bubble in bubbles)
            {
                bubble.SetActive(true);
            }

            if(!mini2Manager.instructionPlayed)
            {
                StartCoroutine(TriggerInstruction());
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.transform == foot)
        {
            isOnFootSurface = false;
        }
    }

    private IEnumerator TriggerInstruction()
    {
        yield return new WaitForSeconds(2f);
        mini2Manager.instructionTrigger = true;
    }



    private void OnDrawGizmos()
    {
        if (!showDebugGizmos) return;
        
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.5f);
        Gizmos.DrawLine(transform.position, transform.position + normalVector);
        
        // If in editor and not playing, calculate ray start
        if (!Application.isPlaying)
        {
            lastRayStart = transform.position + transform.up * wallCheckHeight;
            Gizmos.color = rayColorNoHit;
            Gizmos.DrawLine(lastRayStart, lastRayStart + transform.forward * wallCheckDistance);
            Gizmos.DrawWireSphere(lastRayStart, gizmoSphereRadius);
            return;
        }

        // Draw the ray
        Gizmos.color = lastRayHit ? rayColorHit : rayColorNoHit;
        
        // Draw ray origin sphere
        Gizmos.DrawWireSphere(lastRayStart, gizmoSphereRadius);
        
        if (lastRayHit)
        {
            // Draw line up to hit point
            Gizmos.DrawLine(lastRayStart, wallHit.point);
            
            // Draw hit point sphere
            Gizmos.DrawWireSphere(wallHit.point, gizmoSphereRadius);
            
            // Draw normal vector at hit point
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(wallHit.point, wallHit.point + wallHit.normal);
            
            // Draw climb angle visualization
            if (isWallClimbing)
            {
                Gizmos.color = Color.green;
                float angle = Vector3.Angle(transform.up, wallHit.normal);
                Gizmos.DrawWireSphere(wallHit.point, gizmoSphereRadius * 1.5f);
                
                // Draw max climb angle indicator
                Gizmos.color = angle < maxWallClimbAngle ? Color.green : Color.red;
                Vector3 maxClimbDirection = Quaternion.AngleAxis(maxWallClimbAngle, transform.right) * transform.up;
                Gizmos.DrawLine(wallHit.point, wallHit.point + maxClimbDirection);
            }
        }
        else
        {
            // Draw full ray when no hit
            Gizmos.DrawLine(lastRayStart, lastRayStart + transform.forward * wallCheckDistance);
        }
    }

    private IEnumerator ShrinkPlayerVisual()
    {
        meshTrail.enabled = true;
        float timeElapsed = 0f;
        Vector3 originalScale = playerVisual.localScale;
        while (timeElapsed < 5f)
        {
            float scaleMultiplier = Mathf.Lerp(5f, 1f, timeElapsed / 5f);
            playerVisual.localScale = originalScale * scaleMultiplier;
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }

    // private void UpdateAnimator()
    // {
    //     animator.SetBool("isTurning", targetLateralVelocity != 0);
    // }
}
