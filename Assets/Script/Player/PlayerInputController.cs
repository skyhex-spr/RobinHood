using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{

    public float MoveData;
    public bool Isjumping;
    public bool IsCrouching;

    private PlayerInput playerInput;

    private void Awake()
    {
        playerInput = new PlayerInput();
    }

    private void OnEnable()
    {
        playerInput.Player.Enable();

        playerInput.Player.Movement.performed += OnMove;
        playerInput.Player.Movement.canceled += OnMove;

        playerInput.Player.Jump.performed += OnJump;
        playerInput.Player.Jump.canceled += OnJump;

        playerInput.Player.Crouch.started += OnCrouchStart;
        playerInput.Player.Crouch.canceled += OnCrouchEnd;

    }


    private void OnDisable()
    {
        playerInput.Player.Movement.performed -= OnMove;
        playerInput.Player.Movement.canceled -= OnMove;

        playerInput.Player.Jump.performed -= OnJump;
        playerInput.Player.Jump.canceled -= OnJump;

        playerInput.Player.Crouch.started -= OnCrouchStart;
        playerInput.Player.Crouch.canceled -= OnCrouchEnd;

        playerInput.Player.Disable();
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        MoveData = ctx.ReadValue<float>();
    }

    private void OnJump(InputAction.CallbackContext ctx)
    {
       if(ctx.performed)
            Isjumping = true;
       else 
            Isjumping = false;
    }

    private void OnCrouchStart(InputAction.CallbackContext ctx)
    { 
        IsCrouching = true;
    }

    private void OnCrouchEnd(InputAction.CallbackContext ctx) 
    {
        IsCrouching = false;
    }

}
