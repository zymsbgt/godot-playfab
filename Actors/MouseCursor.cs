using Godot;
using System;

public class MouseCursor : Area2D
{
    private Sprite sprite;
    private bool joystickMoved = false;
    private Vector2 lastControllerPosition; 
    private Vector2? mouseOffset = null;

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
        if (@event is InputEventMouseButton eventMouseButton)
        {
            this.Position = Vector2.Zero;
            if (eventMouseButton.IsPressed())
            {
                DrawMouseCursor(mouseOffset = Vector2.Zero);
                this.Show();
            }
            else
            {
                mouseOffset = null;
                if (!joystickMoved)
                    this.Hide();
            }
                
        }
        else if (@event is InputEventMouseMotion eventMouseMotion) {
            if (this.Visible)
            {
                mouseOffset += eventMouseMotion.Relative;
                this.Position += eventMouseMotion.Relative;
                DrawMouseCursor(this.Position); // Adjust transparancy based on how far the mouseCursor is
            }
            else
            {
                if (!joystickMoved)
                    this.Position = Vector2.Zero;
            }
        }
    }

    //controller support, if mouse is moving make sure to override this!
    public override void _Process(float delta)
    {
        // Get current position
        lastControllerPosition = this.Position;

        // calculate GetActionStrength difference between current frame and previous frame
        float doNotTriggerBelow = 0.01f;
        float multiplier = 100.0f;
        float x = Input.GetActionStrength("sing_right_controller") - Input.GetActionStrength("sing_left_controller");
        float y = Input.GetActionStrength("sing_down_controller") - Input.GetActionStrength("sing_up_controller");
        if (x > doNotTriggerBelow || y > doNotTriggerBelow || x < -doNotTriggerBelow || y < -doNotTriggerBelow)
        {
            joystickMoved = true;
            this.Visible = true;
            Vector2 currentControllerPosition = new Vector2(x *= multiplier, y *= multiplier);
            if (mouseOffset == null)
                this.Position += currentControllerPosition - lastControllerPosition;
            else
                this.Position += currentControllerPosition - lastControllerPosition + (Vector2)mouseOffset;
            DrawMouseCursor(this.Position);
        }
        else
        {
            joystickMoved = false;
            if (mouseOffset == null) // if mouse is released
            {
                this.Position = DrawMouseCursor(Vector2.Zero);
                this.Hide();
            }
        }
    }
}
