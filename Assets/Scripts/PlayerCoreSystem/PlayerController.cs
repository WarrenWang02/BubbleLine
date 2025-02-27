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
    [SerializeField] private DialogueUIManager dialogManager;
    [SerializeField] private GameObject conveyorBeltPrefab;
    private GameObject selectedSpawnPrefab;
    private bool spawnToggleMode = false;

    private PlayerInput playerInput;

    private GroundManager groundManager;
    private int gridWidth;                     // Width of the gird system
    private int gridHeight;                    // Height of the gird system

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        interactionSystem = GetComponent<InteractionSystem>();

        //Find ground in the scene and import width and height
        GameObject groundObj = GameObject.Find("Ground");
        if (groundObj != null)
        {
            groundManager = groundObj.GetComponent<GroundManager>();
            //Debug.Log("[DEBUG] Ground's manager assigned to groundManager.");
            gridWidth = groundManager.GetGridWidth();
            gridHeight = groundManager.GetGridHeight();
        }
        else
        {
            Debug.LogError("[ERROR] Ground not found in the scene!");
        }

        //Find ground in the scene and import width and height
        GameObject UIManager = GameObject.Find("DialogUIManager");
        if (UIManager != null)
        {
            dialogManager = UIManager.GetComponent<DialogueUIManager>();
        }
        else
        {
            Debug.LogError("[ERROR] DialogUIManager not found in the scene!");
        }
    }

    void OnEnable()
    {
        // Bind input actions to their respective functions
        playerInput.actions["Move"].performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        playerInput.actions["Move"].canceled += ctx => moveInput = Vector2.zero;

        playerInput.actions["Interact"].performed += ctx => interactionSystem.TryInteract(GetClampedIndicatorPosition());
        playerInput.actions["Spawn"].performed += ctx => ToggleSpawn();
        playerInput.actions["Rotate"].performed += ctx => interactionSystem.RotateHeldMachine90();
        playerInput.actions["Delete"].performed += ctx => interactionSystem.TryDelete(GetClampedIndicatorPosition());

        playerInput.actions["MoveNext"].performed += ctx => dialogManager.MoveNextDialogue();
    }

    void OnDisable()
    {
        // Unbind input actions
        playerInput.actions["Move"].performed -= ctx => moveInput = ctx.ReadValue<Vector2>();
        playerInput.actions["Move"].canceled -= ctx => moveInput = Vector2.zero;

        playerInput.actions["Interact"].performed -= ctx => interactionSystem.TryInteract(GetClampedIndicatorPosition());
        playerInput.actions["Spawn"].performed -= ctx => ToggleSpawn();
        playerInput.actions["Rotate"].performed -= ctx => interactionSystem.RotateHeldMachine90();
        playerInput.actions["Delete"].performed -= ctx => interactionSystem.TryDelete(GetClampedIndicatorPosition());

        playerInput.actions["MoveNext"].performed -= ctx => dialogManager.MoveNextDialogue();
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

    private Vector3 GetClampedIndicatorPosition()
    {
        Vector3 clampedPosition = playerIndicator.position;

        // Clamp the world position directly
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, -gridWidth / 2, gridWidth / 2 - 1);
        clampedPosition.z = Mathf.Clamp(clampedPosition.z, -gridHeight / 2, gridHeight / 2 - 1);

        return clampedPosition;
    }

}
