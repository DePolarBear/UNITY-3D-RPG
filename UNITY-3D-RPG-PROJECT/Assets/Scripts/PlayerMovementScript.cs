using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementScript : MonoBehaviour
{
    [Header("Pohyb")]
    public InputActionReference move;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private bool shouldFaceMoveDirection = false;
    [SerializeField] private float rychlostOtacania = 10f;

    public Rigidbody playerBody;
    public Vector3 moveDirection;

    public bool zablokovany = false;

    [SerializeField] private float moveSpeed = 5f;

    [Header("Skok")]
    public InputActionReference jump;
    public float jumpStrength = 10f;
    public float gravityScale = 3f;
    public float fallMultiplier = 1.5f;
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.3f;

    private bool jumpQueued;

    // Update is called once per frame
    void Update()
    {

        if (zablokovany)
        {
            moveDirection = Vector3.zero;
            return;
        }

        moveDirection = move.action.ReadValue<Vector3>();

        if (jump.action.WasPressedThisFrame())
        {
            jumpQueued = true;
        }
    }

    private void FixedUpdate()
    {
        // vodorovny smer, kam sa kamera pozera
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        // vstup prepocitany do smeru kamery
        Vector3 smer = forward * moveDirection.z + right * moveDirection.x;

        // vodorovny pohyb, zvisla rychlost sa nechava tak
        playerBody.linearVelocity = new Vector3(smer.x * moveSpeed, playerBody.linearVelocity.y, smer.z * moveSpeed);

        if (shouldFaceMoveDirection && smer.sqrMagnitude > 0.01f)
        {
            Quaternion cielova = Quaternion.LookRotation(smer);
            playerBody.rotation = Quaternion.Slerp(playerBody.rotation, cielova, rychlostOtacania * Time.fixedDeltaTime);
        }

        if (jumpQueued && IsGrounded())
        {
            Vector3 v = playerBody.linearVelocity;
            v.y = jumpStrength;
            playerBody.linearVelocity = v;
        }
        jumpQueued = false;

        float scale = (playerBody.linearVelocity.y < 0f) ? gravityScale * fallMultiplier : gravityScale;
        playerBody.AddForce(Physics.gravity * scale, ForceMode.Acceleration);
    }

    private void Awake()
    {
        playerBody.useGravity = false;
    }

    private bool IsGrounded()
    {
        float dlzka = 0.5f + groundCheckDistance;
        return Physics.Raycast(transform.position, Vector3.down, dlzka, groundLayer);
    }
}
