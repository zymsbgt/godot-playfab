using Godot;
using System;

public class Level1 : Node2D
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
    // Determining how close to the beat an event is
    private int closest = 0;
    private double time_off_beat = 0.0;
    // Attach to nodes
    private AudioStreamPlayer backgroundMusic;
    private Node globalSignal;

    [Signal] public delegate void beatSignal();
    [Signal] public delegate void measureSignal();
    public override void _Ready()
    {
        sec_per_beat = 60.0 / bpm;
        globalSignal = GetNode<Node>("/root/GlobalSignal");
        backgroundMusic = GetNode<AudioStreamPlayer>("/root/Conductor/BackgroundMusic");
        backgroundMusic.Play();
        backgroundMusic.VolumeDb = -3;
    }

    // Signals
    public void _on_Conductor_finished()
    {
        backgroundMusic.StreamPaused = true; // does this even work?
    }

    public override void _PhysicsProcess(float _delta)
    {
        song_position = backgroundMusic.GetPlaybackPosition() + AudioServer.GetTimeSinceLastMix();
        // Compensate for output latency.
        song_position -= AudioServer.GetOutputLatency();
        song_position_in_beats = (int)Math.Round(song_position / sec_per_beat) + beats_before_start;

        // GetNode<LineEdit>("VBoxContainer/AudioOutputLatencyContainer/Edit").Text = AudioServer.GetOutputLatency().ToString();
        // GetNode<LineEdit>("VBoxContainer/SongPositionContainer/Edit").Text = song_position.ToString();
        // GetNode<LineEdit>("VBoxContainer/SongPositionInBeatsContainer/Edit").Text = song_position_in_beats.ToString();
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
    }
}
