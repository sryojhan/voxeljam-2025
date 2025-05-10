using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    protected override bool DestroyOnLoad => false;

    InputMap input;

    private void Awake()
    {
        input = new InputMap();
    }

    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }

    public InputMap GetInput()
    {
        return input;
    }

    public float GetHorizontal()
    {
        return input.Player.Move.ReadValue<Vector2>().x;
    }

    public float GetVertical()
    {
        return input.Player.Move.ReadValue<Vector2>().y;
    }

    public bool GetJump()
    {
        return input.Player.Jump.ReadValue<bool>();
    }

    public bool JumpPressedThisFrame()
    {
        return input.Player.Jump.WasPressedThisFrame();
    }

    public bool JumpReleasedThisFrame()
    {
        return input.Player.Jump.WasReleasedThisFrame();
    }
}
