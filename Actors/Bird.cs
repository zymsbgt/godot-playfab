using Godot;
using System;

public class Bird : KinematicBody2D
{
    private Mochi mochi;
    private Node2D birdSpawner;
    private AnimatedSprite animatedSprite;
    private Sprite cueSprite;
    private AudioStreamPlayer audioStreamPlayer;
    private VisibilityEnabler2D visibilityEnabler2D;
    private float happyCountdownTimer;
    private int birdWaitTime = 8;
    private bool playCueOnThisBeat = false, canBeHappy = false;
    private Vector2 origin;
    private enum CueAnimationState
    {
        off,
        one
    }
    private CueAnimationState cueAnimationState = CueAnimationState.off;
    private enum HappyState
    {
        unhappy,
        happy
    }
    private HappyState happyState = HappyState.unhappy;

    public override void _Ready()
    {
        //birdSpawner = GetNode<Node2D>("../");
        mochi = GetNode<Mochi>("../Mochi");
        mochi.Connect("ColourWheel_area_entered", this, "_on_ColourWheel_area_entered");
        animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        cueSprite = GetNode<Sprite>("Cue/Sprite");
        audioStreamPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
        origin = Position;
    }

    // Signals

    public void _on_beatSignal(int song_position_in_beats)
    {
        if (happyState == HappyState.unhappy)
        {
            if (song_position_in_beats % birdWaitTime == 1)
            {
                audioStreamPlayer.Play();
                playCueOnThisBeat = true;
            }
            else
                playCueOnThisBeat = false;
        }
        else if (happyState == HappyState.happy)
        {
            if (song_position_in_beats % birdWaitTime == 1)
                audioStreamPlayer.Play();
            playCueOnThisBeat = false;
        }
        
    }

    public void _on_body_entered(Node node) 
    {
        if (node is Mochi)
            canBeHappy = true;
    }

    public void _on_body_exited(Node node) 
    {
        if (node is Mochi)
            canBeHappy = false;
    }

    public void _on_ColourWheel_area_entered(int note)
    {
        if (note == 3 && canBeHappy)
        //if (mochi.GetLast10Notes(0) == 3 && canBeHappy)
        {
            happyState = HappyState.happy;
            happyCountdownTimer = 0.5f;
            mochi.SetGravity(500.0f, true);
        }
    }

    // End of Signals

    public override void _Process(float delta)
    {
        animatedSprite.Play("flying");
        cueSprite.Visible = playCueOnThisBeat;
        if (happyState == HappyState.happy)
        {
            if (happyCountdownTimer == 0.0f)
            {
                happyState = HappyState.unhappy;
                Position = origin;
            }
            else
                Position = mochi.Position + new Vector2(0.0f, -128.0f);
        }
            
    }

    public override void _PhysicsProcess(float delta)
    {
        if (Input.IsActionPressed("jump") && happyState == HappyState.happy)
        {
            happyCountdownTimer -= Math.Min(happyCountdownTimer, delta);
        }
    }
}