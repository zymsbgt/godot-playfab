using Godot;
using System;

public class colourwheel : Area2D
{
    private AnimatedSprite _animatedSprite;
    private bool doNotPlayOnThisFrame = false, queuePlay = false;

    public override void _Ready()
    {
        _animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        _animatedSprite.Play("passive");
        this.Visible = false;
    }

    public void _on_mouse_entered()
    {
        queuePlay = true;
    }

    public void _on_mouse_exited()
    {
        _animatedSprite.Play("passive");
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton eventMouseButton)
            if (eventMouseButton.IsPressed())
            {
                this.Visible = true;
                doNotPlayOnThisFrame = true;
            }
            else if (!eventMouseButton.IsPressed())
            {
                _animatedSprite.Play("passive");
                this.Visible = false;
            }
    }

    public override void _Process(float delta)
    {
        if (queuePlay && !doNotPlayOnThisFrame)
        {
            GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D").Play();
		    _animatedSprite.Play("active");
        }
	    doNotPlayOnThisFrame = false;
	    queuePlay = false;
    }
	
}
