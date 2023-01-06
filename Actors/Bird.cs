using Godot;
using System;

public class Bird : KinematicBody2D
{
    private AnimatedSprite animatedSprite;
    private Sprite cueSprite;
    private AudioStreamPlayer audioStreamPlayer;
    private VisibilityEnabler2D visibilityEnabler2D;
    private float timer;
    private int birdWaitTime = 8;
    private bool playCueOnThisBeat = false;
    private enum CueAnimationState
    {
        off,
        one
    }
    CueAnimationState cueAnimationState = CueAnimationState.off;

    public override void _Ready()
    {
        animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        cueSprite = GetNode<Sprite>("Cue/Sprite");
        audioStreamPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
    }

    public void _on_beatSignal(int song_position_in_beats)
    {
        if (song_position_in_beats % birdWaitTime == 1)
        {
            audioStreamPlayer.Play();
            playCueOnThisBeat = true;
        }
        else
            playCueOnThisBeat = false;
    }

    public void _on_measureSignal(int measure)
    {
        
    }

    public override void _Process(float delta)
    {
        animatedSprite.Play("flying");
        cueSprite.Visible = playCueOnThisBeat;
    }
}