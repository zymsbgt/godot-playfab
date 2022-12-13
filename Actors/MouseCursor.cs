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

    private byte ReturnModulateAlphaValue(Vector2 position)
    {
        int multiplier = 3;
        float x = Math.Abs(position.x * multiplier);
        float y = Math.Abs(position.y * multiplier);
        float largerNumber = Math.Min(Math.Max(x, y), 255);
        return (byte)largerNumber;
    }

    public override void _Input(InputEvent @event)
    {
        // Mouse in viewport coordinates.
        if (@event is InputEventMouseButton eventMouseButton)
        {
            if (eventMouseButton.IsPressed())
            {
                sprite.Modulate = Color.Color8(255,255,255,0);
            }
            else
            {
                // if (eventMouseButton.ButtonIndex == 1)
                //     GD.Print("Left Mouse Unclick at: ", eventMouseButton.Position);
                // else if (eventMouseButton.ButtonIndex == 2)
                //     GD.Print("Right Mouse Unclick at: ", eventMouseButton.Position);
                // else
                //     GD.Print("Mochi.cs Input function: This shouldn't be happening!");
            }
        }
        else if (@event is InputEventMouseMotion eventMouseMotion) {
            if (this.Visible == true)
            {
                this.Position += eventMouseMotion.Relative;
                // Adjust transparancy based on how far the mouseCursor is
                sprite.Modulate = Color.Color8(255,255,255,ReturnModulateAlphaValue(this.Position));
            }
            else
            {
                this.Position = Vector2.Zero;
            }
            
        }
    }
}
