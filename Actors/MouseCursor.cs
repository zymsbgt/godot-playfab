using Godot;
using System;

public class MouseCursor : Area2D
{
    private Sprite sprite;
    private float doNotTriggerBelow = 0.01f;

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
            GD.PushError("Error on MouseCursor.cs: check if alpha value is not between 0.0f and 1.0f!");
            GD.Print("Here's more details about the error: ", e.Message);
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton eventMouseButton)
        {
            this.Position = Vector2.Zero;
            if (eventMouseButton.IsPressed())
            {
                DrawMouseCursor(0.0f);
                this.Show();
            }
            else
                this.Hide();
        }
        else if (@event is InputEventMouseMotion eventMouseMotion) {
            if (this.Visible)
            {
                this.Position += eventMouseMotion.Relative;
                DrawMouseCursor(this.Position); // Adjust transparancy based on how far the mouseCursor is
            }
            else
                this.Position = Vector2.Zero;
        }
    }

    //controller support, if mouse is moving make sure to override this!
    public override void _Process(float delta)
    {
        float x = Input.GetActionStrength("sing_right_controller") - Input.GetActionStrength("sing_left_controller");
        float y = Input.GetActionStrength("sing_down_controller") - Input.GetActionStrength("sing_up_controller");
        if (x > doNotTriggerBelow || y > doNotTriggerBelow || x < -doNotTriggerBelow || y < -doNotTriggerBelow)
        {
            this.Visible = true;
            x *= 100;
            y *= 100;
            this.Position = new Vector2(x,y);
        }
        else
        {
            this.Position = Vector2.Zero;
        }
    }
}
