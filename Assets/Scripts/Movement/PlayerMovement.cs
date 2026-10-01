using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    private const float MoveForceMultiplier = 10f;

    private static readonly int SpeedParameter = Animator.StringToHash("speed");

    [Header("Movement")]
    public float speedDefault;
    public float drag;
    public Transform orientation;
    public Animator animator;
    public bool movementEnabled = true;

    private float horizontalInput;
    private float verticalInput;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.linearDamping = drag;
    }

    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        if (!movementEnabled)
        {
            StopHorizontalMovement();
            animator.SetFloat(SpeedParameter, 0f);
            return;
        }

        MovePlayer();
        LimitHorizontalSpeed();
        animator.SetFloat(SpeedParameter, rb.linearVelocity.magnitude);
    }

    private void MovePlayer()
    {
        Vector3 flatForward = Vector3.ProjectOnPlane(orientation.forward, Vector3.up).normalized;
        Vector3 flatRight = Vector3.ProjectOnPlane(orientation.right, Vector3.up).normalized;
        Vector3 moveDirection = (flatForward * verticalInput + flatRight * horizontalInput).normalized;

        rb.AddForce(moveDirection * speedDefault * MoveForceMultiplier, ForceMode.Force);
    }

    private void LimitHorizontalSpeed()
    {
        Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (flatVelocity.magnitude <= speedDefault)
        {
            return;
        }

        Vector3 limitedVelocity = flatVelocity.normalized * speedDefault;
        rb.linearVelocity = new Vector3(limitedVelocity.x, rb.linearVelocity.y, limitedVelocity.z);
    }

    private void StopHorizontalMovement()
    {
        rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
    }
}
