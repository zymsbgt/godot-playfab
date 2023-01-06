using Godot;
using System;

public class ColourWheel : ControllerWheel
{
    private AnimatedSprite _animatedSprite;
    private AudioStreamPlayer2D _audioStreamPlayer2D;
    private bool queuePlay = false;
    [Signal] delegate void disable_player_movement(bool state);

    public override void _Ready()
    {
        _audioStreamPlayer2D = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
        _audioStreamPlayer2D.VolumeDb = 15.0f;
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
        _audioStreamPlayer2D.Stop();
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
            _audioStreamPlayer2D.Play();
		    _animatedSprite.Play("active");
            
            if (GetNode<Node2D>("../../").Name == "LevelTemplate")
                EmitSignal("disable_player_movement", false);
        }
	    queuePlay = false;
    }
}
