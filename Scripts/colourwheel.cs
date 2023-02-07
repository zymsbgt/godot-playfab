using Godot;
using System;

public class ColourWheel : ControllerWheel
{
    private BgmManager bgmManager;
    private Mochi mochi;
    private AnimatedSprite animatedSprite;
    private bool queuePlay = false, active = false;
    [Export] public int note;
    private int numberOfMochisVoices;
    [Export] private AudioStream[] MochisVoices;
    private AudioStreamPlayer2D audioStreamPlayer2D;
    [Signal] delegate void disable_player_movement(bool state);
    [Signal] delegate void _on_ColourWheel_area_entered(int note);
    [Signal] delegate void _on_ColourWheel_area_exited(int note);

    public override void _Ready()
    {
        bgmManager = GetNode<BgmManager>("/root/BgmManager");
        Connect("_on_ColourWheel_area_entered", bgmManager, "_on_ColourWheel_area_entered");
        mochi = GetNode<Mochi>("../");
        Connect("disable_player_movement", mochi, "_on_disable_player_movement");
        Connect("_on_ColourWheel_area_entered", mochi, "_on_ColourWheel_area_entered");
        Connect("_on_ColourWheel_area_exited", mochi, "_on_ColourWheel_area_exited");
        audioStreamPlayer2D = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
        audioStreamPlayer2D.VolumeDb = -7.5f;
        animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        animatedSprite.Play("passive");
        SetVisibility(false);
        numberOfMochisVoices = MochisVoices.Length;
    }

    #region signals
    public void _on_Mochi_showHint(int hint)
    {
        if (hint == note) {} // do stuff, else don't do anything
    }

    public void _on_area_entered(Area2D area)
    {
        queuePlay = true;
        // pass down the note type to Mochi
        EmitSignal("_on_ColourWheel_area_entered", note);
    }

    public void _on_area_exited(Area2D area)
    {
        active = false;
        EmitSignal("_on_ColourWheel_area_exited", note);
        animatedSprite.Play("passive");
        audioStreamPlayer2D.Stop();
    }
    #endregion

    public override void SetVisibility(bool visibility)
    {
        if (!visibility)
            animatedSprite.Play("passive");
        base.SetVisibility(visibility);
    }

    public bool GetActive()
    {
        return active;
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
            active = true;
            
            if (GetNode<Node2D>("../../").Name == "LevelTutorial")
                EmitSignal("disable_player_movement", false);
        }
	    queuePlay = false;
    }
}
