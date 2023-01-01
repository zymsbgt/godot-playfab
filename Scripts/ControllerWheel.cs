using Godot;

public class ControllerWheel : Area2D
{
    private bool joystickMoved = false;
    private Vector2 lastKnownMousePosition = Vector2.Zero;
    private Vector2? mouseOffset = null;

    // Getting and Setting variables
    public Vector2? GetMouseOffset()
    {
        return mouseOffset;
    }

    public void SetMouseOffset(Vector2? value)
    {
        mouseOffset = value;
    }

    public Vector2 GetLastKnownMousePosition()
    {
        return lastKnownMousePosition;
    }

    // public void SetLastKnownMousePosition(Vector2 value)
    // {
    //     lastKnownMousePosition = value;
    // }

    public bool GetJoystickMoved()
    {
        return joystickMoved;
    }

    public virtual void SetVisibility(bool visibility)
    {
        this.Visible = visibility;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton eventMouseButton)
        {
            if (eventMouseButton.IsPressed())
            {
                mouseOffset = Vector2.Zero;
                SetVisibility(true);
            }
            else
            {
                if (mouseOffset == null)
                    lastKnownMousePosition = Vector2.Zero;
                else
                    lastKnownMousePosition = (Vector2)mouseOffset;
                
                mouseOffset = null;

                if (!joystickMoved)
                {
                    lastKnownMousePosition = Vector2.Zero;
                    SetVisibility(false);
                }   
            } 
        }
    }

    public virtual void MoveCursorWithJoystick(float x = 0.0f, float y = 0.0f)
    {
        joystickMoved = true;
        SetVisibility(true);
    }
    public virtual void JoystickReleased(bool joystickMoved, Vector2? mouseOffset)
    {
        joystickMoved = false;
        if (mouseOffset == null)
            SetVisibility(false);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        float doNotTriggerBelow = 0.01f;
        float x = Input.GetActionStrength("sing_right_controller") - Input.GetActionStrength("sing_left_controller");
        float y = Input.GetActionStrength("sing_down_controller") - Input.GetActionStrength("sing_up_controller");
        if (x > doNotTriggerBelow || y > doNotTriggerBelow || x < -doNotTriggerBelow || y < -doNotTriggerBelow)
            MoveCursorWithJoystick(x, y);
        else
            JoystickReleased(joystickMoved, mouseOffset);
    }
}
