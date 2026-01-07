using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementManager : MonoBehaviour
{
    public static PlayerMovementManager Instance { get; private set; }

    [SerializeField] private float moveSpeed = 5f;

    private PlayerInputActions playerInputActions;

    private Vector2 inputMoveVector;

    private void Awake()
    {
        SetInstance();
        //Below ones should execute after InputActionManager's.
        playerInputActions = InputActionsManager.Instance.PlayerInputActions;
        playerInputActions.PlayerMap.Enable();
    }

    private void Start()
    {
        playerInputActions.PlayerMap.Movement.performed += OnMovementPerformed;
        playerInputActions.PlayerMap.Movement.canceled += OnMovementCancelled;
    }

    private void Update()
    {
        MovePlayer();
    }

    private bool IsColliderBlockingMovements(Collider collider)
    {
        if (collider.gameObject.layer == NON_BLOCKING_LAYER)
            return false;
        
        if (collider.transform.root.TryGetComponent<PlayerMovementManager>(out _)) //child objs of player (especially the one in hand)
            return false;

        return true;
    }

    private void MovePlayer()
    {
        float xMoveInput = inputMoveVector.x;
        float yMoveInput = inputMoveVector.y;

        Vector3 movementInput = new(xMoveInput, 0f, yMoveInput);

        if (movementInput != Vector3.zero)
        {
            /*Vector3 cameraForward = mainCamera.transform.forward;
            Vector2 cameraForward2D = new(cameraForward.x, cameraForward.z);

            cameraForward2D = cameraForward2D.normalized;

            cameraForward.x = cameraForward2D.x;
            cameraForward.y = 0f;
            cameraForward.z = cameraForward2D.y;

            Quaternion rotation = Quaternion.LookRotation(cameraForward);
            */

            //Vector3 finalMovement = rotation * movementInput;

            bool isHit = Physics.CapsuleCast(transform.position, transform.position + Vector3.up * 1.6f, 0.5f, movementInput, out RaycastHit hit, 0.1f);

            if (!isHit)
            {
                transform.position += Time.deltaTime * moveSpeed * movementInput;
            }
        }
    }

    private void OnMovementPerformed(InputAction.CallbackContext context)
    {
        inputMoveVector = playerInputActions.PlayerMap.Movement.ReadValue<Vector2>();
    }

    private void OnMovementCancelled(InputAction.CallbackContext context)
    {
        inputMoveVector = Vector3.zero;
    }

    private void SetInstance()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        Instance = this;
    }
}
