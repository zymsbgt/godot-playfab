using Godot;
using System;

[Tool]
public class LevelSelectButton : Button
{
    [Export] private PackedScene startingScene;
    public Node Conductor { get; set; }
    public Node CurrentScene { get; set; }
    [Signal] public delegate void changeScene();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Conductor = GetNode<Node>("../../../../../");
        CurrentScene = GetNode<Node2D>("../../../../");
        Connect("changeScene", CurrentScene, "_on_changeScene");
    }

    public override string _GetConfigurationWarning()
    {
        if (startingScene == null)
            return "The next scene property can't be empty!";
        else
            return "";
    }

    #region signals
    public void _on_Level1_pressed()
    {
        CallDeferred(nameof(DeferredGotoScene), startingScene);
    }
    #endregion

    private void DeferredGotoScene(PackedScene startingScene)
    {
        // Broadcast a signal to show that scene has changed
        EmitSignal("changeScene");

        // Queue current scene for deletion
        CurrentScene.QueueFree();

        // Instance the new scene.
        CurrentScene = startingScene.Instance();

        // Add it to the active scene, as child of root.
        Conductor.AddChild(CurrentScene);
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
