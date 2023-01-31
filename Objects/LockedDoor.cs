using Godot;
using System;

[Tool]
public class LockedDoor : Portal
{
    private Mochi mochi;

    public override void _Ready()
    {
        base._Ready();
        mochi = GetNode<Mochi>("../Mochi");
    }

    #region signals
    public override void _on_body_entered(Area2D _area)
    {
        // Start a rhythm sequence for the player
        mochi._on_disable_player_movement(true);
        mochi.StartRhythmSequence();
        conductor.RequestSoundtrackChange(Level.Playlist.dreamsingalong);
        GetNode<BgmManager>("/root/BgmManager").GetNode<AudioStreamPlayer>("BgmPassive").Connect("finished", this, "WhenSongFinished");
    }
    #endregion

    private async void WhenSongFinished()
    {
        GD.Print("Song finished!");
        animationPlayer.Play("fade_to_black");
        await ToSignal(animationPlayer, "animation_finished");
        CallDeferred(nameof(DeferredGotoScene), nextScene);
        mochi._on_disable_player_movement(false);
    }

    public override void DeferredGotoScene(PackedScene nextScene)
    {
        // Broadcast a signal to show that scene has changed
        EmitSignal("changeScene");

        // Queue current scene for deletion
        CurrentScene.QueueFree();

        // Instance the new scene.
        CurrentScene = nextScene.Instance();

        // Add it to the active scene, as child of root.
        conductor.AddChild(CurrentScene);
    }
}
