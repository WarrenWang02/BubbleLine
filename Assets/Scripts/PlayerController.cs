using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Vector2 moveInput;  // Stores input values from WASD/Controller Stick
    [SerializeField] private float moveSpeed = 5f;  
    private Rigidbody rb;  // Rigidbody for physics-based movement
    [SerializeField] private Transform playerIndicator; 
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private InteractionSystem interactionSystem; 
    [SerializeField] private GameObject conveyorBeltPrefab;
    private GameObject selectedSpawnPrefab; 
    private bool spawnToggleMode = false; 

    private PlayerInput playerInput; // Reference to Unity's Player Input component

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        //playerInput.actions.Enable(); // ✅ Force enable input actions
        playerInput = GetComponent<PlayerInput>(); // Get assigned PlayerInput component
    }

    // void OnEnable()
    // {
    //     playerInput.onActionTriggered += HandleInput; // Subscribe to input events
    // }

    void OnEnable()
    {
        playerInput.onActionTriggered += HandleInput;
        ////Debug onActionTriggered
        // playerInput.onActionTriggered += ctx => 
        // {
        //     Debug.Log($"[DEBUG] Action Triggered: {ctx.action.name} | Phase: {ctx.phase} | Value: {ctx.ReadValueAsObject()}");
        // };
    }

    void OnDisable()
    {
        playerInput.onActionTriggered -= HandleInput; // Unsubscribe to prevent memory leaks
    }

    void FixedUpdate()
    {
        Vector3 movement = new Vector3(moveInput.x, 0, moveInput.y) * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);

        if (moveInput != Vector2.zero)
        {
            Vector3 targetDirection = new Vector3(moveInput.x, 0, moveInput.y);
            Quaternion targetRotation = Quaternion.LookRotation(-targetDirection);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    private void HandleInput(InputAction.CallbackContext context)
    {
        Debug.Log($"Action Triggered: {context.action.name}");

        switch (context.action.name)
        {
            case "Move":
                moveInput = context.ReadValue<Vector2>();
                Debug.Log($"Move Input: {moveInput}");
                break;
            case "Interact":
                Debug.Log("Interact Pressed");
                HandleInteract();
                break;
        }
    }

    private void HandleInteract()
    {
        if (playerInput.currentControlScheme == "Keyboard & Mouse")
        {
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                interactionSystem.TryInteract(playerIndicator.position);
            }
            else if (Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                ToggleSpawn();
            }
            else if (Keyboard.current.rKey.wasPressedThisFrame)
            {
                interactionSystem.RotateHeldMachine90();
            }
            else if (Keyboard.current.cKey.wasPressedThisFrame)
            {
                interactionSystem.TryDelete(playerIndicator.position);
            }
        }
        else if (playerInput.currentControlScheme == "Gamepad")
        {
            Gamepad gamepad = (Gamepad)playerInput.devices[0];

            if (gamepad.buttonSouth.wasPressedThisFrame) // 'A' button on Xbox, 'X' on PlayStation
            {
                interactionSystem.TryInteract(playerIndicator.position);
            }
            else if (gamepad.buttonWest.wasPressedThisFrame) // 'X' button on Xbox, 'Square' on PlayStation
            {
                ToggleSpawn();
            }
            else if (gamepad.buttonNorth.wasPressedThisFrame) // 'Y' button on Xbox, 'Triangle' on PlayStation
            {
                interactionSystem.RotateHeldMachine90();
            }
            else if (gamepad.buttonEast.wasPressedThisFrame) // 'B' button on Xbox, 'Circle' on PlayStation
            {
                interactionSystem.TryDelete(playerIndicator.position);
            }
        }
    }

    private void ToggleSpawn()
    {
        if (conveyorBeltPrefab != null)
        {
            if (!spawnToggleMode)
            {
                selectedSpawnPrefab = conveyorBeltPrefab;
                interactionSystem.TrySpawn(selectedSpawnPrefab);
            }
            else
            {
                interactionSystem.DeSpawn();
            }
            spawnToggleMode = !spawnToggleMode;
        }
        else
        {
            Debug.LogError("Conveyor Belt Prefab is not assigned in the Inspector!");
        }
    }

    public void SpawnAgain()
    {
        if (spawnToggleMode)
        {
            interactionSystem.TrySpawn(selectedSpawnPrefab);
        }
    }
}
