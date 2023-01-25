using Godot;
using System;

public class BgmManager : Node
{
    private AudioStreamPlayer bgmIntro, bgmActive, bgmPassive;
    private float maxVolume = -12.0f, muteVolume = -120.0f;
    private enum MochiState
    {
        passive,
        active
    }
    private MochiState mochiState;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        bgmIntro = GetNode<AudioStreamPlayer>("BgmIntro");
        bgmActive = GetNode<AudioStreamPlayer>("BgmActive");
        bgmPassive = GetNode<AudioStreamPlayer>("BgmPassive");
        bgmIntro.VolumeDb = maxVolume;
        bgmIntro.Play();
    }

    #region signals
    public void _on_BgmIntro_finished()
    {
        // Play bgmPassive and bgmActive track (muted)
        bgmPassive.VolumeDb = maxVolume;
        bgmPassive.Play();
        bgmActive.VolumeDb = muteVolume;
        bgmActive.Play();
    }
    #endregion

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton eventMouseButton)
        {
            if (eventMouseButton.IsPressed())
            {
                bgmActive.VolumeDb = maxVolume;
                mochiState = MochiState.active;
            }
            else
            {
                bgmActive.VolumeDb = muteVolume;
                mochiState = MochiState.passive;
            }
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    // public override void _Process(float delta)
    // {
    //  
    // }
}
