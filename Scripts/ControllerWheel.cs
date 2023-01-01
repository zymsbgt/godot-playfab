using Godot;
using System;

public class ControllerWheel : Area2D
{
    private bool joystickMoved = false;
    private Vector2? mouseOffset = null;

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton eventMouseButton)
        {
            if (eventMouseButton.IsPressed())
            {
                mouseOffset = Vector2.Zero;
                //SetVisibility(true);
            }
            else
            {
                mouseOffset = null;
                if (!joystickMoved) {}
                    //SetVisibility(false);
            }
                
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        float doNotTriggerBelow = 0.01f;
        float x = Input.GetActionStrength("sing_right_controller") - Input.GetActionStrength("sing_left_controller");
        float y = Input.GetActionStrength("sing_down_controller") - Input.GetActionStrength("sing_up_controller");
        if (x > doNotTriggerBelow || y > doNotTriggerBelow || x < -doNotTriggerBelow || y < -doNotTriggerBelow)
        {
            joystickMoved = true;
            //SetVisibility(true);
        }
        else
        {
            joystickMoved = false;
            if (mouseOffset == null) {}
                //SetVisibility(false);
        }
    }
}
