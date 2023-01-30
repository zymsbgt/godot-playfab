using Godot;
using System;

[Tool]
public class LockedDoor : Portal
{
    private Mochi mochi;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        mochi = GetNode<Mochi>("../Mochi");
    }

    #region signals
    public override void _on_body_entered(Area2D _area)
    {
        // Start a rhythm sequence for the player

    }
    #endregion

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
