using Godot;
using System;

public class BirdCue : Node2D
{
    private Conductor conductor;
    private Bird bird;
    private Sprite cueSprite;
    private AudioStreamPlayer2D audioStreamPlayer2D;

    public override void _Ready()
    {
        conductor = GetNode<Conductor>("../../../");
        //conductor.Connect("beatSignal", this, "_on_BeatSignal");
        //conductor.Connect("measureSignal", this, "_on_measureSignal");
        bird = GetNode<Bird>("../");
        cueSprite = GetNode<Sprite>("Sprite");
        audioStreamPlayer2D = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
    }

    public void Play(bool showVisualHint = true)
    {
        cueSprite.Visible = showVisualHint;

        #if GODOT_WEB
        //if (!audioStreamPlayer2D.Playing)
            audioStreamPlayer2D.Play();
        #else
        audioStreamPlayer2D.Play();
        #endif
    }

    public void HideCue()
    {
        cueSprite.Visible = false;
    }
}
