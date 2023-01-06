using Godot;
using System;

public class BackgroundMusic : AudioStreamPlayer
{
    private Node globalSignal;
    [Export] private AudioStream audioStream;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        globalSignal = GetNode<Node>("/root/GlobalSignal");

        if (audioStream != null)
            this.Stream = audioStream;
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
