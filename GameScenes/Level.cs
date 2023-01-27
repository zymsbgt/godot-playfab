using Godot;
using System;

public class Level : Node2D
{
    private Node Conductor;
    public enum Playlist
    {
        none,
        dream,
        dreamcastle,
        dreamboss
    }
    [Export] public Playlist soundtrack;
    // Signals
    [Signal] public delegate void beatSignal();
    [Signal] public delegate void measureSignal();
    [Signal] public delegate void changeScene();

    public override void _Ready()
    {
        Conductor = GetNode<Node>("../");
        Connect("changeScene", Conductor, "_on_changeScene");
    }

    public void _on_BeatSignal(int song_position_in_beats)
    {
        EmitSignal("beatSignal", song_position_in_beats);
    }

    public void _on_measureSignal(int measure)
    {
        EmitSignal("measureSignal", measure);
    }

    public void _on_changeScene()
    {
        EmitSignal("changeScene");
    }
}
