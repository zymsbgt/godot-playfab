using Godot;
using System;

public class colourwheel : Area2D
{
    private AnimatedSprite _animatedSprite;

    public override void _Ready()
    {
        _animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        _animatedSprite.Play("passive");
        this.Visible = false;
    }

    public void _on_mouse_entered()
    {
        GD.Print("Mouse entered segment");
        GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D").Play();
        _animatedSprite.Play("active");
    }

    public void _on_mouse_exited()
    {
        GD.Print("Mouse exited segment");
        _animatedSprite.Play("passive");
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton eventMouseButton)
            if (eventMouseButton.IsPressed())
            {
                this.Visible = true;
            }
            else if (!eventMouseButton.IsPressed())
            {
                _animatedSprite.Play("passive");
                this.Visible = false;
            }
    }
}
