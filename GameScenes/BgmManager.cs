using Godot;
using System;

public class BgmManager : Node
{
    private Conductor conductor;
    private AudioStreamPlayer bgmIntro, bgmActive, bgmPassive;
    private float maxVolume = -9.0f, muteVolume = -120.0f;
    private Level.Playlist nowPlaying;
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
    }

    public float GetPlaybackPosition()
    {
        if (bgmIntro.Playing == true)
        {
            return bgmIntro.GetPlaybackPosition();
        }
        else
        {
            return bgmPassive.GetPlaybackPosition();
        }
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

    public void _on_ColourWheel_area_entered(int note)
    {
        bgmActive.VolumeDb = maxVolume;
        mochiState = MochiState.active;
    }

    public void _on_changeScene()
    {
        conductor = GetNode<Conductor>("/root/Conductor");
        GD.Print(conductor.currentScene);

        // Set the soundtrack
        if (conductor == null)
            return;
        if (nowPlaying == conductor.currentScene.soundtrack)
            return;
        switch (conductor.currentScene.soundtrack)
        {
            // case Level.Playlist.none:
            //     break;
            case Level.Playlist.dream:
                bgmIntro.VolumeDb = maxVolume;
                bgmIntro.Play();
                break;
            default:
                break;
        }
        nowPlaying = conductor.currentScene.soundtrack;
    }
    #endregion

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton eventMouseButton && !eventMouseButton.IsPressed())
            mochiState = MochiState.passive;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if (mochiState == MochiState.passive && bgmActive.VolumeDb > muteVolume)
            bgmActive.VolumeDb -= Math.Min(Math.Abs(muteVolume - bgmActive.VolumeDb), delta * 100);
    }
}
