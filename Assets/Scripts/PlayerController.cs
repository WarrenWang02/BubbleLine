using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerInputActions playerInput;  // Reference to the input actions
    private Vector2 moveInput;               // Stores input values from WASD/Arrow keys
    public float moveSpeed = 5f;             // Speed of the player movement
    private Rigidbody rb;                    // Rigidbody for physics-based movement
    public Transform playerIndicator;    // Reference to the indicator object
    public float rotationSpeed = 10f;        // Speed of rotation (adjust for smoothness)
    public InteractionSystem interactionSystem;  // Reference to the InteractionSystem

    void Awake()
    {
        playerInput = new PlayerInputActions();  // Initialize the input actions
        rb = GetComponent<Rigidbody>();          // Get the Rigidbody component
    }

    void OnEnable()
    {
        playerInput.Player.Move.performed += Move;   // Subscribe to the Move input action
        playerInput.Player.Move.canceled += Move;    // Handle input stop
        playerInput.Player.Interact.performed += Interact;  // Trigger interaction
        playerInput.Enable();                        // Enable input listening
    }

    void OnDisable()
    {
        playerInput.Player.Move.performed -= Move;  // Unsubscribe to prevent memory leaks
        playerInput.Player.Move.canceled -= Move;
        playerInput.Player.Interact.performed -= Interact;
        playerInput.Disable();                      // Disable input listening
    }

    void FixedUpdate()
    {
        // Convert 2D input to 3D movement (X and Z axes)
        Vector3 movement = new Vector3(moveInput.x, 0, moveInput.y) * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);  // Apply movement using Rigidbody

        // Rotate player to face movement direction
        if (moveInput != Vector2.zero)
        {
            Vector3 targetDirection = new Vector3(moveInput.x, 0, moveInput.y);
            Quaternion targetRotation = Quaternion.LookRotation(-targetDirection);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    // This method is called when Move input is detected
    private void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();  // Read input from the action
    }
    private void Interact(InputAction.CallbackContext context)
    {
        // Trigger interaction through the InteractionSystem
        interactionSystem.TryInteract(playerIndicator.position);
    }
}
