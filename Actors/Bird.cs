using Godot;
using System;

public class Bird : KinematicBody2D
{
    private Node2D currentLevel;
    private Mochi mochi;
    private AnimatedSprite animatedSprite;
    private Sprite cueSprite;
    private AudioStreamPlayer2D audioStreamPlayer2D;
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
        // Attach to parent and sibling nodes in scene
        currentLevel = GetNode<Node2D>("../");
        mochi = GetNode<Mochi>("../Mochi");

        // Attach to child nodes
        animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        cueSprite = GetNode<Sprite>("Cue/Sprite");
        audioStreamPlayer2D = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");

        // Connect signals
        currentLevel.Connect("beatSignal", this, "_on_beatSignal");
        mochi.Connect("ColourWheel_area_entered", this, "_on_ColourWheel_area_entered");

        // Set origin position (the position where the bird spawns at)
        origin = Position;
        //origin = animatedSprite.Position;
    }

    #region signals
    public void _on_beatSignal(int song_position_in_beats)
    {
        if (happyState == HappyState.unhappy)
        {
            if (song_position_in_beats % birdWaitTime == 1)
            {
                audioStreamPlayer2D.Play();
                playCueOnThisBeat = true;
            }
            else
                playCueOnThisBeat = false;
        }
        else if (happyState == HappyState.happy)
        {
            if (song_position_in_beats % birdWaitTime == 1)
                audioStreamPlayer2D.Play();
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
    #endregion

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
                //animatedSprite.Position = origin;
            }
            else
                Position = mochi.Position + new Vector2(0.0f, -128.0f);
                //animatedSprite.Position = mochi.Position - Position + new Vector2(0.0f, -128.0f);
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