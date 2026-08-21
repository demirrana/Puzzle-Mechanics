using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementManager : MonoBehaviour
{
    public static PlayerMovementManager Instance { get; private set; }

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;

    private readonly int NON_BLOCKING_LAYER = 9;

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

    private void OnEnable()
    {
        playerInputActions.PlayerMap.Enable();
    }

    private void OnDisable()
    {
        playerInputActions.PlayerMap.Disable();
    }

    private List<RaycastHit> GetHitRaycasts(Vector3 movementInput)
    {
        RaycastHit[] raycastsHit = Physics.CapsuleCastAll(
            transform.position,
            transform.position + Vector3.up * 1.6f,
            0.5f,
            movementInput,
            0.1f
        );

        List<RaycastHit> filteredRaycastsHit = new();

        foreach (RaycastHit raycastHit in raycastsHit)
        {
            if (IsColliderBlockingMovements(raycastHit.collider))
                filteredRaycastsHit.Add(raycastHit);
        }

        return filteredRaycastsHit;
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

        //Vector3 movementInput = new(xMoveInput, 0f, yMoveInput);

        if (xMoveInput != 0f || yMoveInput != 0f)
        {
            CinemachineVirtualCameraBase activeCamera = CameraManager.Instance.GetCameraUnderTheName(CameraManager.Instance.GetActiveCameraName());
            Vector3 camForward = activeCamera.transform.forward;
            Vector3 camRight = activeCamera.transform.right;

            camForward.y = 0f;
            camRight.y = 0f;

            camForward.Normalize();
            camRight.Normalize();

            Vector3 movementInput = (camForward * yMoveInput) + (camRight * xMoveInput);

            if (movementInput != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(movementInput);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            List<RaycastHit> hits = GetHitRaycasts(movementInput);
            bool isHit = hits.Count > 0;

            if (!isHit)
            {
                transform.position += moveSpeed * Time.deltaTime * movementInput;
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
