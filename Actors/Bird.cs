using Godot;
using System;

public class Bird : KinematicBody2D
{
    private Node2D currentLevel;
    private Mochi mochi;
    private AnimatedSprite animatedSprite;
    private Sprite cueSprite;
    private AudioStreamPlayer2D PlayC, PlayD, PlayE, PlayG;
    private float happyCountdownTimer;
    private int birdWaitTime = 8;
    [Export] private int[] birdPattern;
    private bool playCueOnThisBeat = false, canBeHappy = true;
    private Vector2 spawnPosition;
    private Vector2 positionOnCanvas, centerOfCanvas;
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
        PlayC = GetNode<AudioStreamPlayer2D>("PlayC");
        PlayD = GetNode<AudioStreamPlayer2D>("PlayD");
        PlayE = GetNode<AudioStreamPlayer2D>("PlayE");
        PlayG = GetNode<AudioStreamPlayer2D>("PlayG");

        // Connect signals
        currentLevel.Connect("beatSignal", this, "_on_beatSignal");
        mochi.Connect("ColourWheel_area_entered", this, "_on_ColourWheel_area_entered");

        // Set origin position (the position where the bird spawns at)
        spawnPosition = Position;
        //spawnPosition = animatedSprite.Position;

        // Code to make sure all hints are visible regardless of player viewport or resolution

        positionOnCanvas = GetGlobalTransformWithCanvas().origin;
        //GD.Print(positionOnCanvas);

        centerOfCanvas = GetViewport().Size / 2;
        //GD.Print(centerOfCanvas);
    }

    #region signals
    public void _on_beatSignal(int song_position_in_beats)
    {
        if (happyState == HappyState.unhappy)
        {
            if (song_position_in_beats % birdWaitTime == 1)
            {
                switch (currentLevel.Name)
                {
                    case "Level1-1":
                        playCueOnThisBeat = true;
                        PlayE.Play();
                        break;
                    case "Level1-2":
                        playCueOnThisBeat = false;
                        PlayD.Play();
                        break;
                }
            }
            else if (song_position_in_beats % birdWaitTime == 2)
            {
                if (currentLevel.Name == "Level1-2")
                    PlayG.Play();
                playCueOnThisBeat = false;
            }
            else if (song_position_in_beats % birdWaitTime == 3)
            {
                if (currentLevel.Name == "Level1-2")
                    PlayC.Play();
                playCueOnThisBeat = false;
            }
            else
                playCueOnThisBeat = false;
        }
        else if (happyState == HappyState.happy)
        {
            if (song_position_in_beats % birdWaitTime == 1)
            {
                switch (currentLevel.Name)
                {
                    case "Level1-1":
                        PlayE.Play();
                        break;
                    case "Level1-2":
                        PlayD.Play();
                        break;
                }
            }
            else if (song_position_in_beats % birdWaitTime == 2 && currentLevel.Name == "Level1-2")
                PlayG.Play();
            else if (song_position_in_beats % birdWaitTime == 3 && currentLevel.Name == "Level1-2")
                PlayC.Play();
            playCueOnThisBeat = false;
        }
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
            }
        }
        // if (mochi.GetLast10Notes()[0] == birdPattern[0]) {}
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