using Godot;
using System;

public class BgmManager : Node
{
    private Conductor conductor;
    private AudioStreamPlayer bgmIntro, bgmActive, bgmPassive;
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
        // For introduction music, meant to be transitioned to a loopable track once finished playing
        bgmIntro = GetNode<AudioStreamPlayer>("BgmIntro");
        // Bonus track. Can be loopable or unloopable.
        bgmActive = GetNode<AudioStreamPlayer>("BgmActive");
        // The main track. Can be loopable or unloopable.
        bgmPassive = GetNode<AudioStreamPlayer>("BgmPassive");

        conductor = GetNodeOrNull<Conductor>("/root/Conductor");
        
        if (conductor == null) // we're on the login screen
        {
            bgmPassive.Stream = (AudioStream)ResourceLoader.Load("res://Music/levelselect_106bpm.ogg", "AudioStream", false);
            bgmPassive.VolumeDb = 6.0f;
            bgmPassive.Play();
        }
    }

    private void ChangeSoundtrack(Level.Playlist song)
    {
        switch (song)
        {
            case Level.Playlist.levelselect:
                if (bgmPassive.Stream == (AudioStream)ResourceLoader.Load("res://Music/levelselect_106bpm.ogg", "AudioStream", false) && bgmPassive.Playing)
                    break;
                bgmPassive.Stream = (AudioStream)ResourceLoader.Load("res://Music/levelselect_106bpm.ogg", "AudioStream", false);
                bgmPassive.VolumeDb = conductor.maxVolume;
                bgmPassive.Play();
                break;
            case Level.Playlist.dream:
                bgmIntro.Stream = (AudioStream)ResourceLoader.Load("res://Music/level1_intro_110bpm.wav", "AudioStream", false);
                bgmPassive.Stream = (AudioStream)ResourceLoader.Load("res://Music/level1_passive_110bpm.ogg", "AudioStream", false);
                bgmActive.Stream = (AudioStream)ResourceLoader.Load("res://Music/level1_active_110bpm.ogg", "AudioStream", false);
                bgmIntro.VolumeDb = conductor.maxVolume;
                bgmIntro.Play();
                break;
            case Level.Playlist.dreamcastle:
                bgmPassive.Stop();
                bgmActive.Stop();
                bgmIntro.Stream = (AudioStream)ResourceLoader.Load("res://Music/level1_castledoor_110bpm.wav", "AudioStream", false);
                bgmIntro.Play();
                break;
            case Level.Playlist.dreamsingalong:
                bgmIntro.Stop();
                bgmPassive.Stream = (AudioStream)ResourceLoader.Load("res://Music/level1_singalong_70bpm.wav", "AudioStream", false);
                bgmPassive.VolumeDb = conductor.maxVolume;
                bgmPassive.Play();
                break;
            case Level.Playlist.dreamboss:
                bgmPassive.Stream = (AudioStream)ResourceLoader.Load("res://Music/level1_castleboss_106bpm.wav", "AudioStream", false);
                bgmPassive.Play();
                break;
            default:
                break;
        }
    }

    #region calling_functions
    // Not a signal, but this function is quite similar to _on_changeScene
    // To be used when a song is to be changed in the middle of a scene, such as singalongs
    public void RequestSoundtrackChange(Level.Playlist song)
    {
        ChangeSoundtrack(song);
    }

    public float GetPlaybackPosition()
    {
        if (bgmIntro.Playing == true)
            return bgmIntro.GetPlaybackPosition();
        else
            return bgmPassive.GetPlaybackPosition();
    }

    public bool IsPlaying()
    {
        if (bgmIntro.Playing)
            return true;
        if (bgmPassive.Playing)
            return true;
        if (bgmActive.Playing)
            return true;
        return false;
    }
    #endregion

    #region signals
    public void _on_BgmIntro_finished()
    {
        if (conductor.IsSongLoopable())
        {
            bgmPassive.VolumeDb = conductor.maxVolume;
            bgmPassive.Play();
            bgmActive.VolumeDb = conductor.muteVolume;
            bgmActive.Play();
        }
    }

    public void _on_ColourWheel_area_entered(int note)
    {
        bgmActive.VolumeDb = conductor.maxVolume;
        mochiState = MochiState.active;
    }

    public void _on_mochiPassive()
    {
        mochiState = MochiState.passive;
    }

    public void _on_changeScene()
    {
        conductor = GetNode<Conductor>("/root/Conductor");

        // Set the soundtrack
        if (conductor == null)
            return;
        if (nowPlaying == conductor.currentScene.soundtrack)
            return;
        ChangeSoundtrack(conductor.currentScene.soundtrack);
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
        if (conductor != null)
            if (mochiState == MochiState.passive && bgmActive.VolumeDb > conductor.muteVolume)
                bgmActive.VolumeDb -= Math.Min(Math.Abs(conductor.muteVolume - bgmActive.VolumeDb), delta * 100);
    }
}
