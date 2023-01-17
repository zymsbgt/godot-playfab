using Godot;
using System;

public class Bird : KinematicBody2D
{
    private Node2D currentLevel;
    private Mochi mochi;
    private AnimatedSprite animatedSprite;
    private float happyCountdownTimer;
    public int birdWaitTime = 8;
    [Export] private int[] birdPattern;
    private bool canBeHappy = true;
    private Vector2 spawnPosition;
    private Vector2 positionOnCanvas, centerOfCanvas;
    private enum CueAnimationState
    {
        off,
        one
    }
    private CueAnimationState cueAnimationState = CueAnimationState.off;
    public enum HappyState
    {
        unhappy,
        happy
    }
    public HappyState happyState = HappyState.unhappy;

    public override void _Ready()
    {
        // Attach to parent and sibling nodes in scene
        currentLevel = GetNode<Node2D>("../");
        mochi = GetNode<Mochi>("../Mochi");

        // Attach to child nodes
        animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");

        // Connect signals
        currentLevel.Connect("beatSignal", this, "_on_beatSignal");
        mochi.Connect("ColourWheel_area_entered", this, "_on_ColourWheel_area_entered");

        // Set origin position (the position where the bird spawns at)
        spawnPosition = Position;
        //spawnPosition = animatedSprite.Position;

        // Todo: Code to make sure all hints are visible regardless of player viewport or resolution

        positionOnCanvas = GetGlobalTransformWithCanvas().origin;
        //GD.Print(positionOnCanvas);

        centerOfCanvas = GetViewport().Size / 2;
        //GD.Print(centerOfCanvas);
    }

    private void HideAllVisualCues()
    {
        for (int i = 0; i <= 7; i++)
            GetNode<BirdCue>("Cue" + i).HideCue();
    }

    #region signals
    public void _on_beatSignal(int song_position_in_beats)
    {
        HideAllVisualCues();

        bool showVisualHint;
        if (happyState == HappyState.unhappy)
            showVisualHint = true;
        else //if (happyState == HappyState.happy)
            showVisualHint = false;
        
        int cueToPlay = song_position_in_beats % birdWaitTime;
        if (cueToPlay >= 1 && cueToPlay <= 7)
            if (birdPattern.Length >= cueToPlay)
                GetNode<BirdCue>("Cue" + birdPattern[--cueToPlay]).Play(showVisualHint);
    }

    public void _on_screen_entered()
    {
        canBeHappy = true;
    }

    public void _on_screen_exited()
    {
        canBeHappy = false;
    }

    public void _on_ColourWheel_area_entered(int note)
    {
        if (canBeHappy)
        {
            int correctNotes = 0;
            int j = birdPattern.Length - 1;
            for (int i = 0; i < birdPattern.Length; i++)
            {
                if (mochi.GetNote(i) == birdPattern[j])
                    correctNotes++;
                j--;
            }
            if (correctNotes == birdPattern.Length) // pattern success
            {
                happyState = HappyState.happy;
                happyCountdownTimer = 0.5f;
                mochi.SetGravity(500.0f, true);
                HideAllVisualCues();
            }
        }
        // if (mochi.GetLast10Notes()[0] == birdPattern[0]) {}
    }
    #endregion

    public override void _Process(float delta)
    {
        animatedSprite.Play("flying");
        if (happyState == HappyState.happy)
        {
            if (happyCountdownTimer == 0.0f)
            {
                happyState = HappyState.unhappy;
                Position = spawnPosition;
                //animatedSprite.Position = spawnPosition;
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