using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ChemeleonMovement : MonoBehaviour
{
    public static ChemeleonMovement Instance;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] CharacterController characterController;
    [SerializeField] Animator animator;
    Rigidbody rb;
    SpringJoint joint;
    [Header("Input")]
    [SerializeField] InputActionReference moveAction;
    [SerializeField] InputActionReference lookAction;
    [SerializeField] InputActionReference shootAction;
    public float sensitivity = 1f;
    [Header("Tounge")]
    [SerializeField] GameObject toungePrefab;
    [SerializeField] Transform head;
    public Transform toungeStart;
    [SerializeField] float toungeSpeed;
    [SerializeField] float springForce = 16f;
    [SerializeField] float dampining = 7f;
    public Tounge grappling;
    bool grapleActive;
    public bool airborne;
    [Header("Camera")]
    [SerializeField] Transform followTarget;
    [SerializeField] Transform cameraTransform;
    [SerializeField] PhysicsMaterial physicMaterial;
    float stillTime;

    private Vector2 moveInput;
    private Vector2 lookInput;

    private void Awake()
    {
        Instance = this;
    }
    private void OnEnable()
    {
        shootAction.action.performed += OnShootPressed;
        shootAction.action.canceled += OnShootCanceled;
    }

    private void OnDisable()
    {
        shootAction.action.performed -= OnShootPressed;
        shootAction.action.canceled -= OnShootCanceled;
    }

    void Move()
    {
        if (airborne) GrappleMove();
        else NormalMove();
    }
    void NormalMove()
    {
        moveInput = moveAction.action.ReadValue<Vector2>();

        Vector3 moveDirection =
            cameraTransform.forward.XZ().XZ() * moveInput.y +
            cameraTransform.right.XZ().XZ() * moveInput.x;

        if (moveDirection.sqrMagnitude > 1f)
            moveDirection.Normalize();

        RaycastHit groundHit;
        if (Physics.Raycast(transform.position, Vector3.down, out groundHit, 0.65f))
        {
            moveDirection = moveDirection.ProjectOntoPlane(groundHit.normal);
        }


        moveDirection.y = -1f;

        characterController.Move(moveDirection * moveSpeed * Time.deltaTime);

        if (moveDirection.XZ().magnitude > 0.3f) animator.SetAnimation("ChemeleonRun");
        else animator.SetAnimation("ChemeleonIdle");

        moveDirection.y = 0f;
        // Face the direction the player is moving.
        if (moveDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(moveDirection);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }
    void GrappleMove()
    {
        if (grappling != null)
        {
            if (joint != null) joint.connectedAnchor = grappling.transform.position;
            Vector2 moveDirection = (grappling.transform.position - transform.position).XZ().XZ();
            // Face the direction the player is moving.
            if (moveDirection.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation =
                    Quaternion.LookRotation(moveDirection);

                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );
            }
            //head.rotation = Quaternion.LookRotation((grappling.transform.position - head.position).normalized, head.up);
        }
        animator.SetAnimation("ChemeleonSwing");
        if (rb.linearVelocity.magnitude < 0.1f) stillTime += Time.deltaTime;
        else stillTime = 0f;
        if ((Physics.Raycast(transform.position, Vector3.down, 0.6f, ~LayerMask.GetMask("Player")) && !grapleActive) || stillTime > 8f)
        {
            SwitchToGroundMovement();
        }
    }

    #region Start End Grapple
    public void StartGrapple()
    {
        if (grapleActive) return;
        if (!airborne) SwitchToAirMovement();

        grapleActive = true;

        joint = gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = grappling.transform.position;
        float distanceFromGrapple = Vector3.Distance(transform.position, grappling.transform.position);

        RaycastHit hit;
        Physics.Raycast(grappling.transform.position, Vector3.down, out hit, distanceFromGrapple, ~(LayerMask.GetMask("Player") + LayerMask.GetMask("Tounge")));

        float distOffGround = Mathf.Max(hit.distance + -6f, 1f);
        joint.maxDistance = Mathf.Min(0.45f * distanceFromGrapple, distOffGround);
        joint.minDistance = 0.2f * distanceFromGrapple;

        joint.spring = springForce;
        joint.damper = dampining;
        joint.massScale = 4.5f;
    }
    void SwitchToGroundMovement()
    {
        if (TryGetComponent(out SpringJoint joint)) EndGrapple();
        if (TryGetComponent(out Rigidbody rb)) Destroy(rb);
        if (TryGetComponent(out CapsuleCollider coll)) Destroy(coll);
        CharacterController c = gameObject.AddComponent<CharacterController>();
        ApplyCharacterControllerSettings(c);
        airborne = false;
        stillTime = 0f;
    }
    void SwitchToAirMovement()
    {
        if (TryGetComponent(out CharacterController controller)) Destroy(controller);
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        CapsuleCollider coll = gameObject.AddComponent<CapsuleCollider>();
        ApplyRigidbodyAndColliderSettings(rb, coll);
        airborne = true;
    }
    public void EndGrapple()
    {
        if (TryGetComponent(out SpringJoint joint)) Destroy(joint);
        grapleActive = false;
    }
    void ApplyCharacterControllerSettings(CharacterController c)
    {
        if (c == null) { Debug.LogWarning("Invalid Switch To Grounded, Character Controller is null"); return; }
        c.slopeLimit = 45;
        c.stepOffset = 0.3f;
        c.skinWidth = 0.08f;
        c.minMoveDistance = 0.001f;
        c.radius = 0.5f;
        c.height = 1.03f;
        characterController = c;
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
    }
    void ApplyRigidbodyAndColliderSettings(Rigidbody rb, CapsuleCollider coll)
    {
        if (rb == null || coll == null) { Debug.LogWarning("Invalid Switch To Airborne, rb or coll is null"); return; }
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
        coll.radius = 0.5f;
        coll.height = 1.03f;
        coll.material = physicMaterial;
        rb.mass = 0.4f;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        this.rb = rb;
    }
    #endregion

    void Look()
    {
        if (Time.timeScale == 0f) return;
        followTarget.position = transform.position;
        lookInput = lookAction.action.ReadValue<Vector2>();
        Vector3 rotationEuler = followTarget.rotation.eulerAngles;
        rotationEuler.x -= lookInput.y * sensitivity;
        rotationEuler.y += lookInput.x * sensitivity;

        if (rotationEuler.x > 180f)
            rotationEuler.x -= 360f;

        //Snap rotation back to good values.
        rotationEuler.x = Mathf.Clamp(rotationEuler.x, -84f, 84f);

        followTarget.rotation = Quaternion.Euler(rotationEuler);

    }
    private void Update()
    {
        Move();
        Look();
    }

    private void OnShootPressed(InputAction.CallbackContext context)
    {
        // Reset the tounge if there is already one in the air.
        if (grappling != null) Destroy(grappling.gameObject);

        JSAM.AudioManager.PlaySound(SoundsSounds.Lick);
        Rigidbody toungeRB = Instantiate(toungePrefab, toungeStart.position, toungeStart.rotation).GetComponent<Rigidbody>();
        RaycastHit hit;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, 340f))
        {
            toungeRB.linearVelocity = toungeSpeed * (hit.point - toungeStart.position).normalized;
        }
        else toungeRB.linearVelocity = cameraTransform.forward * toungeSpeed;
    }

    private void OnShootCanceled(InputAction.CallbackContext obj)
    {
        if (grappling != null) Destroy(grappling.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > 3f) JSAM.AudioManager.PlaySound(SoundsSounds.Thud);

        if (collision.gameObject.layer == 8)
        {
            SceneManager.LoadScene(1);
        }
    }
}
