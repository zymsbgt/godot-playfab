using Godot;
using System;

public class MouseCursor : Area2D
{
    private Sprite sprite;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        sprite = GetNode<Sprite>("Sprite");
    }

    private void DrawMouseCursor(Vector2 position)
    {
        float multiplier = 0.015f;
        float x = Math.Abs(position.x * multiplier);
        float y = Math.Abs(position.y * multiplier);
        float alpha = Math.Min(Math.Max(x, y), 1.0f);
        DrawMouseCursor(alpha);
    }

    private void DrawMouseCursor(float alpha)
    {
        try 
        {
            sprite.Modulate = Color.ColorN("white", alpha);
        }
        catch (Exception e)
        {
            GD.Print("Error on MouseCursor.cs: check if alpha value is not between 0.0f and 1.0f!");
            GD.Print("Here's more details about the error: ", e.Message);
        }
    }

    public override void _Input(InputEvent @event)
    {
        // Mouse in viewport coordinates.
        if (@event is InputEventMouseButton eventMouseButton)
        {
            if (eventMouseButton.IsPressed())
            {
                DrawMouseCursor(0.0f);
            }
            else
            {
                // if (eventMouseButton.ButtonIndex == 1)
                //     GD.Print("Left Mouse Unclick at: ", eventMouseButton.Position);
                // else if (eventMouseButton.ButtonIndex == 2)
                //     GD.Print("Right Mouse Unclick at: ", eventMouseButton.Position);
                // else
                //     GD.Print("MouseCursor.cs Input function: This shouldn't be happening!");
            }
        }
        else if (@event is InputEventMouseMotion eventMouseMotion) {
            if (this.Visible == true)
            {
                this.Position += eventMouseMotion.Relative;
                DrawMouseCursor(this.Position); // Adjust transparancy based on how far the mouseCursor is
            }
            else
            {
                this.Position = Vector2.Zero;
            }
        }
    }
}
