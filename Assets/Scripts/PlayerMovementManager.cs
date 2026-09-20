using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementManager : MonoBehaviour
{
    public static PlayerMovementManager Instance { get; private set; }

    [Header("Gravity & Ground Settings")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private float gravity = -9.81f;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;

    private PlayerInputActions playerInputActions;
    private Vector2 inputMoveVector;

    private float verticalVelocity;

    private void Awake()
    {
        SetInstance();
        // Ensure characterController is assigned if not set from Inspector
        if (characterController == null)
        {
            characterController = GetComponent<CharacterController>();
        }

        //Below ones should execute after InputActionManager's.
        playerInputActions = InputActionsManager.Instance.PlayerInputActions;
    }

    private void Start()
    {
        playerInputActions.PlayerMap.Movement.performed += OnMovementPerformed;
        playerInputActions.PlayerMap.Movement.canceled += OnMovementCancelled;
    }

    private void Update()
    {
        Vector3 verticalMove = CalculateGravity();
        Vector3 horizontalMove = CalculateMovement();

        //Combine horizontal move and gravity into a single CharacterController.Move call
        Vector3 finalVelocity = horizontalMove + verticalMove;
        characterController.Move(finalVelocity * Time.deltaTime);
    }

    private void OnEnable()
    {
        if (playerInputActions != null)
            playerInputActions.PlayerMap.Enable();
    }

    private void OnDisable()
    {
        if (playerInputActions != null)
            playerInputActions.PlayerMap.Disable();
    }

    private void OnDestroy()
    {
        if (playerInputActions != null)
        {
            playerInputActions.PlayerMap.Movement.performed -= OnMovementPerformed;
            playerInputActions.PlayerMap.Movement.canceled -= OnMovementCancelled;
        }
    }

    private Vector3 CalculateMovement()
    {
        float xMoveInput = inputMoveVector.x;
        float yMoveInput = inputMoveVector.y;

        if (xMoveInput == 0f && yMoveInput == 0f)
            return Vector3.zero;

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

            return movementInput * moveSpeed;
        }

        return Vector3.zero;
    }

    private Vector3 CalculateGravity()
    {
        if (characterController.isGrounded)
        {
            if (verticalVelocity < 0f)
            {
                //Small negative force to keep the player anchored to slopes/stairs
                verticalVelocity = -2f;
            }
        }
        else
        {
            //Continuous acceleration downwards when no floor detected
            verticalVelocity += gravity * Time.deltaTime;
        }

        return new Vector3(0f, verticalVelocity, 0f);
    }

    private void OnMovementPerformed(InputAction.CallbackContext context)
    {
        inputMoveVector = playerInputActions.PlayerMap.Movement.ReadValue<Vector2>();
    }

    private void OnMovementCancelled(InputAction.CallbackContext context)
    {
        inputMoveVector = Vector2.zero;
    }

    private void SetInstance()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
}