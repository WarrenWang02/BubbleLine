using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Vector2 moveInput;
    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody rb;
    [SerializeField] private Transform playerIndicator;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private InteractionSystem interactionSystem;
    [SerializeField] private GameObject conveyorBeltPrefab;
    private GameObject selectedSpawnPrefab;
    private bool spawnToggleMode = false;

    private PlayerInput playerInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        interactionSystem = GetComponent<InteractionSystem>();
    }

    void OnEnable()
    {
        // Bind input actions to their respective functions
        playerInput.actions["Move"].performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        playerInput.actions["Move"].canceled += ctx => moveInput = Vector2.zero;

        playerInput.actions["Interact"].performed += ctx => interactionSystem.TryInteract(playerIndicator.position);
        playerInput.actions["Spawn"].performed += ctx => ToggleSpawn();
        playerInput.actions["Rotate"].performed += ctx => interactionSystem.RotateHeldMachine90();
        playerInput.actions["Delete"].performed += ctx => interactionSystem.TryDelete(playerIndicator.position);
    }

    void OnDisable()
    {
        // Unbind input actions
        playerInput.actions["Move"].performed -= ctx => moveInput = ctx.ReadValue<Vector2>();
        playerInput.actions["Move"].canceled -= ctx => moveInput = Vector2.zero;

        playerInput.actions["Interact"].performed -= ctx => interactionSystem.TryInteract(playerIndicator.position);
        playerInput.actions["Spawn"].performed -= ctx => ToggleSpawn();
        playerInput.actions["Rotate"].performed -= ctx => interactionSystem.RotateHeldMachine90();
        playerInput.actions["Delete"].performed -= ctx => interactionSystem.TryDelete(playerIndicator.position);
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
