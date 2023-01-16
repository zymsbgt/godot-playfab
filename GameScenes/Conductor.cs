using Godot;
using System;

public class Conductor : Node
{
    [Export] private int bpm = 110;
    [Export] private int measures = 4;

    // Tracking the beat and song position
    private double song_position = 0.0;
    private int song_position_in_beats = 1;
    private double sec_per_beat; // initialise in Ready() function
    private int last_reported_beat = 0;
    private int beats_before_start = 0;
    private int measure = 1;
    private double last_frame_song_position;
    // Determining how close to the beat an event is
    private int closest = 0;
    private double time_off_beat = 0.0;
    // Attach to nodes
    private AudioStreamPlayer backgroundMusic;
    private PackedScene packedScene;
    private Node2D currentScene;

    [Signal] public delegate void beatSignal();
    [Signal] public delegate void measureSignal();
    public override void _Ready()
    {
        sec_per_beat = 60.0 / bpm;
        backgroundMusic = GetNode<AudioStreamPlayer>("BackgroundMusic");
        backgroundMusic.Play();
        backgroundMusic.VolumeDb = -6;
        backgroundMusic.GetPlaybackPosition();
        _on_changeScene();
    }

    #region signals
    public void _on_changeScene()
    {
        GD.Print("Change of scene detected by conductor!");
        CallDeferred(nameof(DeferredGotoScene));
    }
    #endregion

    private void DeferredGotoScene()
    {
        // Get the current Node2D scene in the remote scene list
        int j = 0;
        foreach(Node i in GetChildren())
        {
            if (i is Node2D)
                currentScene = (Node2D)GetChild(j);
            j++;
        }

        // Connect signals
        Connect("beatSignal", currentScene, "_on_BeatSignal");
        Connect("measureSignal", currentScene, "_on_measureSignal");
    }

    public override void _PhysicsProcess(float _delta)
    {
        song_position = backgroundMusic.GetPlaybackPosition() + AudioServer.GetTimeSinceLastMix();
        // Compensate for output latency.
        song_position -= AudioServer.GetOutputLatency();
        song_position_in_beats = (int)Math.Round(song_position / sec_per_beat) + beats_before_start;

        // GetNode<LineEdit>("VBoxContainer/AudioOutputLatencyContainer/Edit").Text = AudioServer.GetOutputLatency().ToString();
        // GetNode<LineEdit>("VBoxContainer/SongPositionContainer/Edit").Text = song_position.ToString();
        GetNode<Label>("HUD/Label").Text = "Song position in beats: " + song_position_in_beats.ToString();
        ReportBeat();
    }

    private void ReportBeat()
    {
        if (last_reported_beat < song_position_in_beats)
        {
            if (measure > measures)
			    measure = 1;
            EmitSignal("beatSignal", song_position_in_beats);
            EmitSignal("measureSignal", measure);
		    last_reported_beat = song_position_in_beats;
            
		    measure += 1;
        }
        if (song_position < last_frame_song_position)
            last_reported_beat = 0;
        last_frame_song_position = backgroundMusic.GetPlaybackPosition();
    }
}
