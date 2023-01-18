using Godot;
using System;

public class ColourWheelNew : ControllerWheel
{
    private AnimatedSprite animatedSprite;
    private bool queuePlay = false, active;
    [Export] private int note;
    private int numberOfMochisVoices;
    [Export] private AudioStream[] MochisVoices;
    private AudioStreamPlayer2D audioStreamPlayer2D;
    [Signal] delegate void disable_player_movement(bool state);
    [Signal] delegate void _on_ColourWheel_area_entered(int note);
    [Signal] delegate void _on_ColourWheel_area_exited(int note);
    [Signal] delegate void mochiActive(bool state);

    public override void _Ready()
    {
        audioStreamPlayer2D = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
        audioStreamPlayer2D.VolumeDb = -7.5f;
        animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        animatedSprite.Play("passive");
        SetVisibility(false);
        numberOfMochisVoices = MochisVoices.Length;
    }

    public void _on_area_entered(Area2D area)
    {
        queuePlay = true;
        // pass down the note type to Mochi
        EmitSignal("_on_ColourWheel_area_entered", note);
        active = true;
    }

    public void _on_area_exited(Area2D area)
    {
        EmitSignal("_on_ColourWheel_area_exited", note);
        animatedSprite.Play("passive");
        audioStreamPlayer2D.Stop();
    }

    public override void SetVisibility(bool visibility)
    {
        if (!visibility)
            animatedSprite.Play("passive");
        base.SetVisibility(visibility);
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
        //GD.Print(active); // there are 8 segments in total, only one of them will return true at any given time

        if (queuePlay)
        {
            audioStreamPlayer2D.Stream = MochisVoices[Math.Abs((int)GD.Randi() % numberOfMochisVoices)];
            audioStreamPlayer2D.Play();
		    animatedSprite.Play("active");
            
            if (GetNode<Node2D>("../../").Name == "LevelTemplate")
                EmitSignal("disable_player_movement", false);
        }
	    queuePlay = false;
    }
}
