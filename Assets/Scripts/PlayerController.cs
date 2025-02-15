using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerInputActions playerInput;  // Reference to the input actions
    private Vector2 moveInput;               // Stores input values from WASD/Arrow keys
    [SerializeField] private float moveSpeed = 5f;             // Speed of the player movement
    private Rigidbody rb;                    // Rigidbody for physics-based movement
    [SerializeField] private Transform playerIndicator;    // Reference to the indicator object
    [SerializeField] private float rotationSpeed = 10f;        // Speed of rotation (adjust for smoothness)
    [SerializeField] private InteractionSystem interactionSystem;  // Reference to the InteractionSystem
    [SerializeField] private GameObject conveyorBeltPrefab;  // Conveyor belt prefab to spawn
    private GameObject selectedSpawnPrefab; //memory a prefab that lastly selected to spawn
    private bool spawnToggleMode = false; //record Spawn is Toggle ON/OFF


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

    // Handles all interactions (Pick up, Drop, Spawn)
    private void Interact(InputAction.CallbackContext context)
    {
        // Check which key was pressed and call the appropriate method
        if (Keyboard.current.eKey.isPressed)
        {
            // Picking up a machine
            interactionSystem.TryInteract(playerIndicator.position);
        }
        else if (Keyboard.current.digit1Key.isPressed)
        {
            // Spawning a conveyor belt
            if (conveyorBeltPrefab != null)
            {
                if (!spawnToggleMode){
                    selectedSpawnPrefab = conveyorBeltPrefab;
                    interactionSystem.TrySpawn(selectedSpawnPrefab);
                } else {
                    interactionSystem.DeSpawn();
                }
                spawnToggleMode = !spawnToggleMode;
            }
            else
            {
                Debug.LogError("Conveyor Belt Prefab is not assigned in the Inspector!");
            }
        }
        else if (Keyboard.current.rKey.isPressed){
            interactionSystem.RotateHeldMachine90();
        }
    }

    public void SpawnAgain(){
        if (spawnToggleMode){
            interactionSystem.TrySpawn(selectedSpawnPrefab);
        }
    }
}
