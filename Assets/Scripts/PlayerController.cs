using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerInputActions playerInput;  // Reference to the input actions
    private Vector2 moveInput;               // Stores input values from WASD/Arrow keys
    public float moveSpeed = 5f;             // Speed of the player movement
    private Rigidbody rb;                    // Rigidbody for physics-based movement

    void Awake()
    {
        playerInput = new PlayerInputActions();  // Initialize the input actions
        rb = GetComponent<Rigidbody>();          // Get the Rigidbody component
    }

    void OnEnable()
    {
        playerInput.Player.Move.performed += Move;   // Subscribe to the Move input action
        playerInput.Player.Move.canceled += Move;    // Handle input stop
        playerInput.Enable();                        // Enable input listening
    }

    void OnDisable()
    {
        playerInput.Player.Move.performed -= Move;  // Unsubscribe to prevent memory leaks
        playerInput.Player.Move.canceled -= Move;
        playerInput.Disable();                      // Disable input listening
    }

    void Update()
    {
        // Can handle non-physics related updates here if needed
    }

    void FixedUpdate()
    {
        // Convert 2D input to 3D movement (X and Z axes)
        Vector3 movement = new Vector3(moveInput.x, 0, moveInput.y) * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);  // Apply movement using Rigidbody
    }

    // This method is called when Move input is detected
    private void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();  // Read input from the action
    }
}
