using Godot;
using System;

public class MouseCursor : ControllerWheel
{
    private Sprite sprite;
    private Vector2 lastControllerPosition; 

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        sprite = GetNode<Sprite>("Sprite");

        // Capture the mouse if on PC
        #if GODOT_WEB // Capture mouse on mouse click event instead
        #elif GODOT // for devices with mouse(s)
        Input.MouseMode = Input.MouseModeEnum.Captured;
        #endif
    }

    private void DrawMouseCursor(float alpha)
    {
        sprite.Modulate = Color.ColorN("white", alpha);
    }

    private Vector2 DrawMouseCursor(Vector2 position)
    {
        float multiplier = 0.015f;
        float x = Math.Abs(position.x * multiplier);
        float y = Math.Abs(position.y * multiplier);
        float alpha = Math.Min(Math.Max(x, y), 1.0f);
        DrawMouseCursor(alpha);
        return new Vector2(position);
    }

    private Vector2 DrawMouseCursor(Vector2? position)
    {
        Vector2 output = (Vector2)position;
        DrawMouseCursor(output);
        return output;
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (@event is InputEventMouseButton eventMouseButton)
        {
            if (eventMouseButton.IsPressed())
            {
                //if (GetLastKnownMousePosition() == null)
                    DrawMouseCursor(Position = Vector2.Zero);
                //else // joystick is active
                //    DrawMouseCursor(this.Position = GetLastKnownMousePosition());

                // If exported to web browser, capture the mouse
                #if GODOT_WEB
                if (Input.MouseMode != Input.MouseModeEnum.Captured)
                    Input.MouseMode = Input.MouseModeEnum.Captured;
                #endif
            }
            else
                Position = Vector2.Zero; 
        }
        else if (@event is InputEventMouseMotion eventMouseMotion) {
            if (Visible)
            {
                mouseOffset += eventMouseMotion.Relative;
                Position += eventMouseMotion.Relative;
                DrawMouseCursor(Position); // Adjust transparancy based on how far the mouseCursor is
            }
            else if (!joystickMoved)
                Position = Vector2.Zero;
        }
    }

    public override void MoveCursorWithJoystick(float x = 0.0f, float y = 0.0f)
    {
        base.MoveCursorWithJoystick();
        float multiplier = 290.0f;
        Vector2 currentControllerPosition = new Vector2(x *= multiplier, y *= multiplier);
        if (mouseOffset == null)
            Position += currentControllerPosition - lastControllerPosition + lastKnownMousePosition;
        else
            Position += currentControllerPosition - lastControllerPosition + (Vector2)mouseOffset;
        DrawMouseCursor(Position);
    }

    public override void JoystickReleased(bool joystickMoved, Vector2? mouseOffset)
    {
        joystickMoved = false;
        if (mouseOffset == null) // if mouse is released
        {
            Position = DrawMouseCursor(Vector2.Zero);
            SetVisibility(false);
        }
    }

    //controller support, if mouse is moving make sure to override this!
    public override void _Process(float delta)
    {
        lastControllerPosition = Position;
        base._Process(delta);
    }
}
