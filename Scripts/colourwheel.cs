using Godot;
using System;

public class ColourWheel : ControllerWheel
{
    private AnimatedSprite _animatedSprite;
    private bool queuePlay = false;
    [Signal] delegate void disable_player_movement(bool state);

    public override void _Ready()
    {
        _animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        _animatedSprite.Play("passive");
        SetVisibility(false);
    }

    public void _on_area_entered(Area2D area)
    {
        queuePlay = true;
    }

    public void _on_area_exited(Area2D area)
    {
        _animatedSprite.Play("passive");
    }

    public override void SetVisibility(bool visibility)
    {
        if (visibility == false)
            _animatedSprite.Play("passive");
        base.SetVisibility(visibility);
    }

    public override void _Process(float delta)
    {
        base._Process(delta);

        if (queuePlay)
        {
            GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D").Play();
		    _animatedSprite.Play("active");
            
            if (GetTree().CurrentScene.Name == "LevelTemplate")
                EmitSignal("disable_player_movement", false);
        }
	    queuePlay = false;
    }
}
