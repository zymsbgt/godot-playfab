using Godot;
using System;

public class Sword : Area2D
{
    private Sprite LeftMouseClickHint;
    private bool disablePlayerMovement; // only here to track the state of the player
    private float clickHintTransparancy;
    [Signal] delegate void disable_player_movement(bool state);
    // animation state machine
    private enum ClickHintAnimationState
    {
        fadeIn,
        fadeOut
    }
    ClickHintAnimationState clickHintAnimationState;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        LeftMouseClickHint = GetNode<Sprite>("../LeftMouseClickHint");
    }

    public void _on_body_entered(Area2D _area)
    {
        if (this.Visible)
        {
            this.Visible = false;
            LeftMouseClickHint.Visible = true;
            LeftMouseClickHint.Modulate = Color.ColorN("white", clickHintTransparancy = 0.0f);
            clickHintAnimationState = ClickHintAnimationState.fadeIn;
            // fire a signal to Mochi to DisablePlayerMovement
            EmitSignal("disable_player_movement", true);
            disablePlayerMovement = true;
        }
    }

    public void _on_destroy_left_mouse_click_hint()
    {
        if (disablePlayerMovement)
            clickHintAnimationState = ClickHintAnimationState.fadeOut;
    }

    private void ProcessLeftMouseClickHintAnimation(float delta)
    {
        float multiplier = 2.0f;
        if (clickHintAnimationState == ClickHintAnimationState.fadeIn)
        {
            clickHintTransparancy += delta * multiplier;
            clickHintTransparancy = Math.Min(1.0f, clickHintTransparancy);
            LeftMouseClickHint.Modulate = Color.ColorN("white", clickHintTransparancy);
        }
        else if (clickHintAnimationState == ClickHintAnimationState.fadeOut)
        {
            clickHintTransparancy -= delta * multiplier;
            if (clickHintTransparancy <= 0.0f)
            {
                LeftMouseClickHint.QueueFree();
                this.QueueFree();
            }
            else
                LeftMouseClickHint.Modulate = Color.ColorN("white", clickHintTransparancy);
        }
    }

    // Used to process LeftMouseClickHint fade in/out animation
    public override void _Process(float delta)
    {
        ProcessLeftMouseClickHintAnimation(delta);
    }
}
