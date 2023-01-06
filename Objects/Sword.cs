using Godot;
using System;

public class Sword : Area2D
{
    private Sprite LeftMouseClickHint, ControllerJoyHint;
    private bool disablePlayerMovement; // only here to track the state of the player
    private bool controllerAttached;
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
        ControllerJoyHint = GetNode<Sprite>("../ControllerJoyHint");
    }

    private Sprite ReturnHintSprite()
    {
        if (controllerAttached)
            return ControllerJoyHint;
        else
            return LeftMouseClickHint;
    }

    public void _on_body_entered(Area2D _area) // triggered when player collects sword
    {
        if (this.Visible)
        {
            this.Visible = false;
            if (Input.GetConnectedJoypads().Count == 0)
                controllerAttached = false;
            else
                controllerAttached = true;
            ReturnHintSprite().Visible = true;
            ReturnHintSprite().Modulate = Color.ColorN("white", clickHintTransparancy = 0.0f);
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

    private void ProcessHintAnimation(float delta)
    {
        float multiplier = 2.0f;
        if (clickHintAnimationState == ClickHintAnimationState.fadeIn)
        {
            clickHintTransparancy += delta * multiplier;
            clickHintTransparancy = Math.Min(1.0f, clickHintTransparancy);
            ReturnHintSprite().Modulate = Color.ColorN("white", clickHintTransparancy);
        }
        else if (clickHintAnimationState == ClickHintAnimationState.fadeOut)
        {
            clickHintTransparancy -= delta * multiplier;
            if (clickHintTransparancy <= 0.0f)
            {
                LeftMouseClickHint.QueueFree();
                ControllerJoyHint.QueueFree();
                this.QueueFree();
            }
            else
                ReturnHintSprite().Modulate = Color.ColorN("white", clickHintTransparancy);
        }
    }

    // Used to process fade in/out animation
    public override void _Process(float delta)
    {
        ProcessHintAnimation(delta);
    }
}
