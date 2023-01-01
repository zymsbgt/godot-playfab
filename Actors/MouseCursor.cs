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
    }

    private void DrawMouseCursor(float alpha)
    {
        try 
        {
            sprite.Modulate = Color.ColorN("white", alpha);
        }
        catch (Exception e)
        {
            GD.PushError("Error on MouseCursor.cs: check if alpha value is not between 0.0f and 1.0f!");
            GD.Print("Here's more details about the error: ", e.Message);
        }
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
        //if (position == null)
        //    position = Vector2.Zero;
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
                    DrawMouseCursor(this.Position = Vector2.Zero);
                //else // joystick is active
                //    DrawMouseCursor(this.Position = GetLastKnownMousePosition());
            }
            else
            {
                this.Position = Vector2.Zero;
            }
                
        }
        else if (@event is InputEventMouseMotion eventMouseMotion) {
            if (this.Visible)
            {
                SetMouseOffset(GetMouseOffset() + eventMouseMotion.Relative);
                this.Position += eventMouseMotion.Relative;
                DrawMouseCursor(this.Position); // Adjust transparancy based on how far the mouseCursor is
            }
            else
            {
                if (!GetJoystickMoved())
                    this.Position = Vector2.Zero;
            }
        }
    }

    public override void MoveCursorWithJoystick(float x = 0.0f, float y = 0.0f)
    {
        base.MoveCursorWithJoystick();
        float multiplier = 100.0f;
        Vector2 currentControllerPosition = new Vector2(x *= multiplier, y *= multiplier);
        if (GetMouseOffset() == null)
            this.Position += currentControllerPosition - lastControllerPosition + GetLastKnownMousePosition();
        else
            this.Position += currentControllerPosition - lastControllerPosition + (Vector2)GetMouseOffset();
        DrawMouseCursor(this.Position);
    }

    public override void JoystickReleased(bool joystickMoved, Vector2? mouseOffset)
    {
        joystickMoved = false;
        if (mouseOffset == null) // if mouse is released
        {
            this.Position = DrawMouseCursor(Vector2.Zero);
            SetVisibility(false);
        }
    }

    //controller support, if mouse is moving make sure to override this!
    public override void _Process(float delta)
    {
        lastControllerPosition = this.Position;
        base._Process(delta);
    }
}
