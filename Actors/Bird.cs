using Godot;
using System;

public class Bird : KinematicBody2D
{
    private AnimatedSprite animatedSprite;
    private Sprite cueSprite;
    private AudioStreamPlayer audioStreamPlayer;
    private float timer;
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

    public void _on_AudioStreamPlayer_finished()
    {
        GD.Print("My callback!");
    }

    public override void _Process(float delta)
    {
        animatedSprite.Play("flying");
        switch (cueAnimationState)
        {
            case CueAnimationState.off:
                if (timer > 3.5f)
                {
                    audioStreamPlayer.Play();
                    cueAnimationState = CueAnimationState.one;
                    timer = 0.0f;
                }
                cueSprite.Visible = false;
                timer += delta;
                return;
            case CueAnimationState.one:
                if (timer > 0.5f)
                {
                    cueAnimationState = CueAnimationState.off;
                    timer = 0.0f;
                }
                cueSprite.Visible = true;
                timer += delta;
                return;
        }
    }
}