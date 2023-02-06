using Godot;
using System;

public class Conductor : Node
{
    #if GODOT_PC
    // Quitting the game
    private int holdEscapeToQuitBeats = 4;
    private float holdEscapeToQuitSecs = 3.0f;
    #endif
    // Fill in data about the song
    private int bpm;
    private int measures;
    private float offset; 
    public float maxVolume, muteVolume;
    private bool loopable;
    private Level.Playlist nowPlaying;

    // Tracking the beat and song position
    private double song_position = 0.0;
    private int song_position_in_beats = 1;
    public double sec_per_beat; // initialise in Ready() function
    private int last_reported_beat;
    private int beats_before_start = 0;
    private int measure = 1;
    private double last_frame_song_position;
    // Determining how close to the beat an event is
    private int closest = 0;
    private double time_off_beat = 0.0;
    // Attach to nodes
    private BgmManager bgmManager;
    private PackedScene packedScene;
    private Level currentScene;
    private AudioStreamPlayer exitBeep;

    [Signal] public delegate void beatSignal();
    [Signal] public delegate void measureSignal();
    [Signal] public delegate void changeScene();
    public override void _Ready()
    {
        exitBeep = GetNode<AudioStreamPlayer>("ExitBeep");
        // Connect to BgmManager
        bgmManager = GetNode<BgmManager>("/root/BgmManager");
        Connect("changeScene", bgmManager, "_on_changeScene");
        _on_changeScene();

        //OS.WindowMaximized = true;
        #if GODOT_PC
        // #if GODOT_WINDOWS
        // OS.WindowFullscreen = true;
        // OS.WindowMaximized = true;
        // OS.WindowBorderless = true;
        #endif

        //GD.Print("Viewport resolution is: ", GetViewport().Size);
        // Connect Signals
        Connect("beatSignal", this, "_on_BeatSignal");
    }

    public bool IsSongLoopable()
    {
        if (loopable)
            return true;
        else
            return false;
    }

    private void GetMusicData()
    {
        if (nowPlaying == currentScene.soundtrack)
            return;
        switch (currentScene.soundtrack)
        {
            case Level.Playlist.levelselect:
                bpm = 106;
                measures = 4;
                offset = 0.2208f;
                // Remember to change volume in BgmManager._Ready() as well!
                muteVolume = 0.0f;
                maxVolume = 0.0f;
                loopable = true;
                break;
            case Level.Playlist.dream:
                bpm = 110;
                measures = 4;
                offset = 0.2292f;
                muteVolume = -120.0f;
                maxVolume = -9.0f;
                loopable = true;
                break;
            case Level.Playlist.dreamcastle:
                bpm = 110;
                measures = 4;
                offset = 0.2292f;
                muteVolume = -120.0f;
                maxVolume = -9.0f;
                loopable = false;
                break;
            case Level.Playlist.dreamsingalong:
                bpm = 70;
                measures = 3;
                offset = 0.35f;
                muteVolume = -120.0f;
                maxVolume = -6.0f;
                loopable = false;
                break;
            case Level.Playlist.dreamboss:
                bpm = 106;
                measures = 4;
                offset = 0.215f;
                muteVolume = 3.0f;
                maxVolume = 3.0f;
                loopable = false;
                break;
            default:
                break;
        }
        sec_per_beat = 60.0 / bpm;
        nowPlaying = currentScene.soundtrack;
        last_reported_beat = 0;
    }

    public Level GetCurrentScene()
    {
        return currentScene;
    }

    #region signals
    public void _on_BeatSignal(int i) // i = song_position_in_beats (which is not used)
    {
        if (Input.IsActionPressed("escape") && bgmManager.IsPlaying())
        {
            if (holdEscapeToQuitBeats <= 0)
                GetTree().Quit(0);
            else if (holdEscapeToQuitBeats == 1)
            {
                holdEscapeToQuitBeats--;
                exitBeep.PitchScale = 2;
                exitBeep.Play();
            }
            else
            {
                holdEscapeToQuitBeats--;
                exitBeep.Play();
            }
        }
        else
        {
            exitBeep.PitchScale = 1;
            holdEscapeToQuitBeats = 4;
            holdEscapeToQuitSecs = 3.0f;
        }
    }

    // Not a signal, but this function is quite similar to _on_changeScene
    // To be used when a song is to be changed in the middle of a scene, such as singalongs
    public void RequestSoundtrackChange(Level.Playlist song)
    {
        currentScene.soundtrack = song;
        GetMusicData();
        bgmManager.RequestSoundtrackChange(song);
    }

    // Keep this function the last one in the Signals region
    public void _on_changeScene()
    {
        GD.Print("Change of scene detected by conductor!");
        CallDeferred(nameof(DeferredChangeScene));
    }
    #endregion

    private void DeferredChangeScene()
    {
        // Get the current Node2D scene in the remote scene list
        int j = 0;
        foreach(Node i in GetChildren())
        {
            // Zym if you're here looking for a bug, chances are you forgot to attach the Level script to the Level node.
            if (i is Level) // && !i.IsQueuedForDeletion())
            {
                currentScene = (Level)GetChild(j);
            }
            j++;
        }

        GetMusicData();

        // Tell BgmManager that the scene has changed. Only call this signal after the scene and music data has been determined.
        EmitSignal("changeScene"); 

        // Connect signals
        Connect("beatSignal", currentScene, "_on_BeatSignal");
        Connect("measureSignal", currentScene, "_on_measureSignal");
    }

    public override void _PhysicsProcess(float _delta)
    {
        if (Input.IsActionJustPressed("fullscreen"))
            OS.WindowFullscreen = !OS.WindowFullscreen;
        if (Input.IsActionJustPressed("debug_window"))
        {
            OS.WindowFullscreen = false;
            OS.WindowSize = new Vector2(960, 540);
        }
        
        song_position = bgmManager.GetPlaybackPosition() + AudioServer.GetTimeSinceLastMix();
        // Compensate for output latency. This is not available on web.
        #if GODOT_WINDOWS || GODOT_X11
        song_position -= AudioServer.GetOutputLatency();
        #endif
        song_position -= offset;
        song_position_in_beats = (int)Math.Round(song_position / sec_per_beat) + beats_before_start;

        string debugLabel = "Debug Info: ";
        // #if GODOT_WINDOWS
        //     #if GODOT_64
        //     GetNode<Label>("HUD/DebugLabel").Text = "Debug Info: Windows 64-bit";
        //     #elif GODOT_32
        //     GetNode<Label>("HUD/DebugLabel").Text = "Debug Info: Windows 32-bit";
        //     #else
        //     GetNode<Label>("HUD/DebugLabel").Text = "Debug Info: Windows (unknown architecture)";
        //     #endif
        // #elif GODOT_X11
        // if (OS.HasFeature("64"))
        //     GetNode<Label>("HUD/DebugLabel").Text = "Debug Info: GNU/Linux Generic Distro / X11 / SteamOS";
        // #endif
        #if GODOT_PC
            #if GODOT_WINDOWS
            debugLabel += "Windows ";
            #elif GODOT_X11
            debugLabel += "GNU/Linux Generic Distro / X11 / SteamOS ";
            #else
            debugLabel += "Other PC device (such as macOS) ";
            #endif
            if (OS.HasFeature("64"))
                debugLabel += "64-bit ";
            else if (OS.HasFeature("32"))
                debugLabel += "32-bit ";
            if (OS.GetPowerPercentLeft() == -1)
                debugLabel += "Desktop ";
            else
                debugLabel += "Laptop (" + OS.GetPowerPercentLeft() + "% power remaining)";
        #elif GODOT_WEB
            debugLabel += "Browser type: " + JavaScript.Eval("navigator.userAgent");
        #else
            debugLabel += "Unsupported device";
        #endif

        GetNode<Label>("HUD/DebugLabel").Text = debugLabel;
        #if GODOT_WINDOWS || GODOT_X11
        int SpeakerLatencyLabel = (int)Math.Round((AudioServer.GetTimeSinceLastMix() + AudioServer.GetOutputLatency()) * 1000);
        GetNode<Label>("HUD/AudioHardwareLatencyLabel").Text = "Speaker Output Latency: " + SpeakerLatencyLabel.ToString() + "ms";
        #else
        GetNode<Label>("HUD/AudioHardwareLatencyLabel").Text = "Speaker Output Latency: " + "Not Detected";
        #endif
        GetNode<Label>("HUD/SongPositionInBeatsLabel").Text = "Beat " + song_position_in_beats.ToString();
        if (currentScene != null)
            GetNode<Label>("HUD/LevelLabel").Text = "Current " + currentScene.Name;
        GetNode<Label>("HUD/FPS").Text = "FPS: " + Engine.GetFramesPerSecond();
        #if GODOT_PC
        if (Input.IsActionPressed("escape"))
        {
            if (bgmManager.IsPlaying())
                GetNode<Label>("HUD/HoldEscapeToQuit").Text = "Quitting in " + holdEscapeToQuitBeats;
            else
                GetNode<Label>("HUD/HoldEscapeToQuit").Text = "Quitting in " + (int)Math.Floor(holdEscapeToQuitSecs);
        } 
        else
            GetNode<Label>("HUD/HoldEscapeToQuit").Text = "";
        #endif
        
        ReportBeat();
    }

    // For manual beat syncronising
    // public override void _Input(InputEvent @event)
    // {
    //     if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.IsPressed())
    //     {
    //         GD.Print("Mouse click: ", song_position);
    //     }
    // }

    private void ReportBeat()
    {
        if (last_reported_beat < song_position_in_beats)
        {
            // For manual beat syncronising
            //GD.Print("beatSignal: ", song_position);

            if (measure > measures)
			    measure = 1;
            
            EmitSignal("beatSignal", song_position_in_beats);
            EmitSignal("measureSignal", measure);
		    last_reported_beat = song_position_in_beats;
            
		    measure += 1;
        }
        
        if (song_position < last_frame_song_position)
            last_reported_beat = 0;
        last_frame_song_position = bgmManager.GetPlaybackPosition() - offset;
    }

    public override void _Process(float delta)
    {
        if (Input.IsActionPressed("escape") && !bgmManager.IsPlaying())
        {
            if (holdEscapeToQuitSecs <= 0)
                GetTree().Quit(0);
            else
                holdEscapeToQuitSecs -= delta;
        }
        else
            holdEscapeToQuitSecs = 3.0f;
    }
}
