using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementScript : MonoBehaviour
{
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
        // 1. vodorovny pohyb, zvisla rychlost sa nechava tak
        playerBody.linearVelocity = new Vector3(moveDirection.x * moveSpeed, playerBody.linearVelocity.y, moveDirection.z * moveSpeed);
    }
}
