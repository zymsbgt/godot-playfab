using Godot;
using System;

public class colourwheel : Area2D
{
    private AnimatedSprite _animatedSprite;
    private bool queuePlay = false, mouseDown = false, joystickMoved = false;
    [Signal] delegate void disable_player_movement(bool state);

    public override void _Ready()
    {
        _animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        _animatedSprite.Play("passive");
        this.Visible = false;
    }

    public void _on_area_entered(Area2D area)
    {
        queuePlay = true;
    }

    public void _on_area_exited(Area2D area)
    {
        _animatedSprite.Play("passive");
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton eventMouseButton)
        {
            if (eventMouseButton.IsPressed())
            {
                mouseDown = true;
                SetVisibility(true);
            }
            else
            {
                mouseDown = false;
                if (!joystickMoved)
                    SetVisibility(false);
            }    
        }
    }

    private void SetVisibility(bool visibility)
    {
        if (visibility == false)
            _animatedSprite.Play("passive");
        this.Visible = visibility;
    }

    public override void _Process(float delta)
    {
        // queueplay
        if (queuePlay)
        {
            GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D").Play();
		    _animatedSprite.Play("active");
            
            if (GetTree().CurrentScene.Name == "LevelTemplate")
                EmitSignal("disable_player_movement", false);
        }
	    queuePlay = false;

        // controller
        float doNotTriggerBelow = 0.01f;
        float x = Input.GetActionStrength("sing_right_controller") - Input.GetActionStrength("sing_left_controller");
        float y = Input.GetActionStrength("sing_down_controller") - Input.GetActionStrength("sing_up_controller");
        if (x > doNotTriggerBelow || y > doNotTriggerBelow || x < -doNotTriggerBelow || y < -doNotTriggerBelow)
        {
            joystickMoved = true;
            SetVisibility(true);
        }
        else
        {
            joystickMoved = false;
            if (mouseDown == false)
                SetVisibility(false);
        }
    }
}
