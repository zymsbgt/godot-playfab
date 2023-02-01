using Godot;
using System;

//[Tool]
public class LockedDoor : Portal
{
    private Mochi mochi;
    private AnimatedSprite animatedSprite;
    private Label scoreLabel;
    private bool isSongPlaying = false;

    public override void _Ready()
    {
        base._Ready();
        scoreLabel = GetNode<Label>("ScoreLabel");
        scoreLabel.Visible = false;
        mochi = GetNode<Mochi>("../Mochi");
        animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        animatedSprite.Play("locked");
    }

    #region signals
    public override void _on_body_entered(Area2D _area)
    {
        // Start a rhythm sequence for the player
        mochi._on_disable_player_movement(true);
        mochi.StartRhythmSequence();
        conductor.RequestSoundtrackChange(Level.Playlist.dreamsingalong);
        GetNode<BgmManager>("/root/BgmManager").GetNode<AudioStreamPlayer>("BgmPassive").Connect("finished", this, "WhenSongFinished");
        scoreLabel.Visible = true;
        isSongPlaying = true;
    }
    #endregion

    private async void WhenSongFinished()
    {
        GD.Print("Song finished!");
        // Get score
        if (mochi.score > 150)
        {
            animatedSprite.Play("open");
            animationPlayer.Play("fade_to_black");
            await ToSignal(animationPlayer, "animation_finished");
            CallDeferred(nameof(DeferredGotoScene), nextScene);
            scoreLabel.Visible = false;
        }
        else
        {
            scoreLabel.Text = "Try singing again!";
            mochi.score = 0;
        }
        isSongPlaying = false;
        mochi._on_disable_player_movement(false);
        GetNode<BgmManager>("/root/BgmManager").GetNode<AudioStreamPlayer>("BgmPassive").Disconnect("finished", this, "WhenSongFinished");
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

    public override void _Process(float delta)
    {
        base._Process(delta);
        if (isSongPlaying)
            scoreLabel.Text = "Score: " + mochi.score.ToString();
    }
}
