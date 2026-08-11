using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementScript : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private bool shouldFaceMoveDirection = false;
    [SerializeField] private float rychlostOtacania = 10f;

    public Rigidbody playerBody;
    public Vector3 moveDirection;

    [SerializeField] private float moveSpeed = 5f;

    public InputActionReference move;

    // Update is called once per frame
    void Update()
    {
        moveDirection = move.action.ReadValue<Vector3>();


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
    }
}
