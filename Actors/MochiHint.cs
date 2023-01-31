using Godot;
using System;

public class MochiHint : Area2D
{
    private Conductor conductor;
    private Mochi mochi;
    private int id;
    private double four_beats_duration; 
    private int time_to_live_in_beats = 5;

    // tempo related stuff
    private double sec_per_beat;

    private void DisplayHint(int i)
    {
        float x, y;
        x = 0; y = 0;
        if (i >= 1 && i <= 3)
            x = -200.0f;
        else if (i >= 5 && i <= 7)
            x = 200.0f;
        else
            x = 0.0f;
        if (i >= 3 && i <= 5)
            y = 200.0f;
        else if (i == 2 || i == 6)
            y = 0.0f;
        else
            y = -200.0f;
        Position = new Vector2(x, y);
        GetNode<Sprite>(i.ToString()).Visible = true;
        GetNode<Sprite>(i.ToString()).Modulate = Color.ColorN("white", 0.7f);
    }

    public override void _Ready()
    {
        conductor = GetNode<Conductor>("../../../");
        conductor.Connect("beatSignal", this, "_on_beatSignal");
        sec_per_beat = conductor.sec_per_beat;
        //GD.Print(sec_per_beat);

        // Calculate the duration of 4 beats, then divide equally among the distance
        four_beats_duration = sec_per_beat * 4;

        mochi = GetNode<Mochi>("../");
        // Get current beat
        id = mochi.storeBeatForMochiHint;
        
        // Based on the beat, get the note that it should play as
        // First note is 6, so should first appear at beat 2 as a C4 note
        switch(id)
        {
            // Unfortunately, Godot-C# is based on C# 7.3, where putting two or more numbers on a single case switch is not allowed.
            // I very much wish that I could shorten this code as well.
            case 2:
                DisplayHint(1);
                break;
            case 5:
                DisplayHint(5);
                break;
            case 6:
                DisplayHint(3);
                break;
            case 7:
                DisplayHint(1);
                break;
            case 8:
                DisplayHint(7);
                break;
            case 11:
                DisplayHint(8);
                break;
            case 14:
                DisplayHint(1);
                break;
            case 17:
                DisplayHint(5);
                break;
            case 18:
                DisplayHint(3);
                break;
            case 19:
                DisplayHint(1);
                break;
            case 20:
                DisplayHint(7);
                break;
            case 23:
                DisplayHint(6);
                break;
            case 25:
                DisplayHint(7);
                break;
            case 26:
                DisplayHint(8);
                break;
        }
    }

    public void _on_beatSignal(int song_position_in_beats)
    {
        time_to_live_in_beats--;
    }

    public override void _Process(float delta)
    {
        if (time_to_live_in_beats == 0)
            QueueFree();
        float distanceToTravel = 200.0f / (float)four_beats_duration * delta;
        if (GetNode<Sprite>("1").Visible)
            if (Position.x < 0 && Position.y < 0)
                Position = new Vector2(Position.x + distanceToTravel, Position.y + distanceToTravel);
            else
                NoteReachedPositionElseStatement(1, delta);
        if (GetNode<Sprite>("2").Visible)
            if (Position.x < 0)
                Position = new Vector2(Position.x + distanceToTravel, Position.y);
            else
                NoteReachedPositionElseStatement(2, delta);
        if (GetNode<Sprite>("3").Visible)
            if (Position.x < 0 && Position.y > 0)
                Position = new Vector2(Position.x + distanceToTravel, Position.y - distanceToTravel);
            else
                NoteReachedPositionElseStatement(3, delta);
        if (GetNode<Sprite>("4").Visible)
            if (Position.y > 0)
                Position = new Vector2(Position.x, Position.y - distanceToTravel);
            else
                NoteReachedPositionElseStatement(4, delta);
        if (GetNode<Sprite>("5").Visible)
            if (Position.x > 0 && Position.y > 0)
                Position = new Vector2(Position.x - distanceToTravel, Position.y - distanceToTravel);
            else
                NoteReachedPositionElseStatement(5, delta);
        if (GetNode<Sprite>("6").Visible)
            if (Position.x > 0)
                Position = new Vector2(Position.x - distanceToTravel, Position.y);
            else
                NoteReachedPositionElseStatement(6, delta);
        if (GetNode<Sprite>("7").Visible)
            if (Position.x > 0 && Position.y < 0)
                Position = new Vector2(Position.x - distanceToTravel, Position.y + distanceToTravel);
            else
                NoteReachedPositionElseStatement(7, delta);
        if (GetNode<Sprite>("8").Visible)
            if (Position.y < 0)
                Position = new Vector2(Position.x, Position.y + distanceToTravel);
            else
                NoteReachedPositionElseStatement(8, delta);
    }

    private void NoteReachedPositionElseStatement(int i, float delta)
    {
        Scale = new Vector2(Scale.x + (delta / 2), Scale.y + (delta / 2));
        GetNode<Sprite>(i.ToString()).Modulate = Color.ColorN("white", GetNode<Sprite>(i.ToString()).Modulate.a - delta);
    }
}
