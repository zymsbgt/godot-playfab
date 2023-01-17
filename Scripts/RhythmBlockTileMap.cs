using Godot;
using System;

public class RhythmBlockTileMap : TileMap
{
    private Node2D currentLevel;
    private int waitTime = 2;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        currentLevel = GetNode<Node2D>("../");
        currentLevel.Connect("beatSignal", this, "_on_beatSignal");
    }

    private void ToggleVisibility()
    {
        if (Visible)
        {
            Visible = false;
            SetCollisionLayerBit(3, false);
        }
        else
        {
            Visible = true;
            SetCollisionLayerBit(3, true);
        }
    }

    #region signals
    public void _on_beatSignal(int song_position_in_beats)
    {
        if (song_position_in_beats % waitTime == 0)
            ToggleVisibility();
    }
    #endregion
}
