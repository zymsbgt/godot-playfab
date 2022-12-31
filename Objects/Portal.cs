using Godot;
using System;

[Tool]
public class Portal : Area2D
{
    [Export] private PackedScene nextScene;
    private AnimationPlayer animationPlayer;

    public override void _Ready()
    {
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }

    public override string _GetConfigurationWarning()
    {
        if (nextScene == null)
            return "The next scene property can't be empty!";
        else
            return "";
    }

    public void _on_body_entered(Area2D _area)
    {
        teleport();
        // maybe change this in future to show a speech bubble prompt to enter the portal?
    }

    private async void teleport()
    {
        animationPlayer.Play("fade_to_black");
        await ToSignal(animationPlayer, "animation_finished");
        GetTree().ChangeSceneTo(nextScene);
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
