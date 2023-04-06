using Godot;
using System;

public class HUD : CanvasLayer
{
    public bool showDebugInfo = false;

    public override void _Ready()
    {
        GetNode<Label>("BeatDetection").Visible = false;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if (Input.IsActionJustPressed("debug_menu"))
        {
            showDebugInfo = !showDebugInfo;
            GetNode<Label>("DebugLabel").Visible = showDebugInfo;
            GetNode<Label>("AudioHardwareLatencyLabel").Visible = showDebugInfo;
            GetNode<Label>("SongPositionInBeatsLabel").Visible = showDebugInfo;
            GetNode<Label>("LevelLabel").Visible = showDebugInfo;
            GetNode<Label>("FPS").Visible = showDebugInfo;
            GetNode<Label>("BeatDetection").Visible = showDebugInfo;
        }
    }
}
