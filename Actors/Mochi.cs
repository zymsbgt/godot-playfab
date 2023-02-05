using Godot;
using System;

//[Tool]
public class Mochi : KinematicBody2D
{
    // Initialising variables
    private bool isInitialising = false;

    // Movement related variables
    private float gravity, tempGravity, lowGravityTimer = 0.0f, maxGravityTimer = 0.75f;
    private bool lowerGravityOnJump, disableMovement = false, isOnIce = false;
    private float acceleration, deceleration, targetVelocity, iceAcceleration, iceDeceleration;
    private Vector2 maxSpeed = new Vector2(800.0f, 1000.0f), maxIceSpeed = new Vector2(1000.0f, 1000.0f);
    private Vector2 velocity = Vector2.Zero, FLOOR_NORMAL = Vector2.Up;

    // Better jump
    private float fallMultiplier = 2.5f, lowJumpMultiplier = 2.0f;
    private bool canJump;
    private int coyoteTimer, jumpBuffer, maxJumpBuffer = 10, maxCoyoteTimer = 10;

    // Note detection
    private int[] last10notes = new int[10];

    // Reference nodes
    //Godot.Collections.Dictionary<int, PackedScene> mochiHints;
    private Conductor conductor;
    private Node2D currentScene;
    private Area2D mouseCursor;
    private AnimationPlayer animationPlayer;
    private Camera2D camera;
    [Export] private int cameraLimitRight;
    [Export] private int theVoid; // How far below Mochi can go before the game determines that Mochi has failed the level
    private bool voidTriggered = false;

    // For MochiHint (Sing along part)
    [Signal] delegate void showHint();
    private bool dropFirstBeatSignal;
    private int storeBeatForMochiHint;
    public int score;

    // Signals
    [Signal] delegate void destroy_left_mouse_click_hint();
    [Signal] delegate void ColourWheel_area_entered();
    [Signal] delegate void ColourWheel_area_exited();
    [Signal] delegate void changeScene();
    [Signal] delegate void BirdJumpBoostActivated();

    public override void _Ready()
    {
        // Initialising variables
        acceleration = maxSpeed.x * 10.0f;
        deceleration = maxSpeed.x * 20.0f;
        iceAcceleration = maxIceSpeed.x * 0.75f;
        iceDeceleration = maxIceSpeed.x * 1.25f;
        SetGravity();

        mouseCursor = GetNode<Area2D>("MouseCursor");
        mouseCursor.Hide();
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        camera = GetNode<Camera2D>("Camera2D");
        // Add code to set hard limit for camera right based on level
        if (cameraLimitRight > 540)
            camera.LimitRight = cameraLimitRight;
        if (theVoid > 960)
            camera.LimitBottom = theVoid;

        //isInitialising = true; // enable this and comment line below to delay initialising by 1 frame
        Initialise();
    }

    private void Initialise() // called on the first frame of the scene
    {
        conductor = GetNode<Conductor>("../../");
        currentScene = GetNode<Node2D>("../");
        Connect("changeScene", currentScene, "_on_changeScene");

        isInitialising = false;
    }

    // // Perhaps this code could be moved to a class inherited by Mochi. (maybe MochiEditor?)
    // public override string _GetConfigurationWarning()
    // {
    //     if (cameraLimitRight <= 0)
    //         return "Please set a camera right limit!";
    //     else
    //         return "";
    // }

    public int GetStoreBeatForMochiHint()
    {
        return storeBeatForMochiHint;
    }

    public int[] GetNote()
    {
        return last10notes;
    }

    public int GetNote(int i)
    {
        return last10notes[i];
    }

    private async void RestartLevel()
    {
        voidTriggered = true; // prevent this function from being called again
        conductor.GetNode<AudioStreamPlayer>("death").Play();
        animationPlayer.Play("fade_to_black");
        await ToSignal(animationPlayer, "animation_finished");
        CallDeferred(nameof(DeferredRestartScene), currentScene);
    }

    private void DeferredRestartScene(Node currentScene)
    {
        // Broadcast a signal to show that scene has changed
        EmitSignal("changeScene");

        // Queue current scene for deletion
        currentScene.QueueFree();

        // Instance the new scene.
        PackedScene restartScene = (PackedScene)ResourceLoader.Load(currentScene.Filename);
        currentScene = restartScene.Instance();

        // Add it to the active scene, as child of root.
        conductor.AddChild(currentScene);
    }

    #region buffs
    public void SetGravity(float i = 1500.0f, bool isTemporary = false)
    {
        if (isTemporary)
        {
            lowerGravityOnJump = true;
            lowGravityTimer = maxGravityTimer;
            tempGravity = i;
        }
        else
            gravity = i;
    }
    #endregion

    #region signals
    public void StartRhythmSequence()
    {
        // Connect beatSignal from Conductor. Todo: Add code to disconnect the signal when the song ends
        if (!conductor.IsConnected("beatSignal", this, "_on_beatSignal"))
            conductor.Connect("beatSignal", this, "_on_beatSignal");
        // mochiHints = new Godot.Collections.Dictionary<int, PackedScene>();
        dropFirstBeatSignal = true;
    }

    public void _on_beatSignal(int song_position_in_beats)
    {
        if (dropFirstBeatSignal)
        {
            dropFirstBeatSignal = false;
            return;
        }
        // Generate new hints
        CallDeferred(nameof(InstanciateNewHint), song_position_in_beats);

        // Store the current beat for MochiHint
        storeBeatForMochiHint = song_position_in_beats;
    }

    private void InstanciateNewHint(int song_position_in_beats)
    {
        // Instance the new hint.
        PackedScene mochiHintPackedScene = (PackedScene)ResourceLoader.Load("res://Actors/MochiHint.tscn");
        
        // Add to dictionary
        // mochiHints.Add(song_position_in_beats, mochiHintPackedScene);

        // Add it to the active scene, as child of root.
        AddChild(mochiHintPackedScene.Instance());
    }

    public void _on_disable_player_movement(bool state = true)
    {
        // This is an incoming signal from mouseCursor or Mochi's colour wheel segments
        disableMovement = state;

        // Fire a signal to mouse hint to disappear
        if (!state && GetNode<Node2D>("../").Name == "LevelTutorial")
            EmitSignal("destroy_left_mouse_click_hint");
    }

    public void _on_ColourWheel_area_entered(int note)
    {
        // This signal is fired from the coloured wheels
        for (int i = last10notes.Length - 1; i > 0; i--)
        {
            last10notes[i] = last10notes[i - 1];
        }
        last10notes[0] = note;
        // Create a gateway to share this information with birds
        EmitSignal("ColourWheel_area_entered", note);
    }

    public void _on_ColourWheel_area_exited(int note)
    {
        EmitSignal("ColourWheel_area_exited", note);
    }
    #endregion

    private Vector2 GetMaxSpeed()
    {
        if (isOnIce)
            return maxIceSpeed;
        else
            return maxSpeed;
    }

    private float GetAcceleration()
    {
        if (isOnIce)
            return iceAcceleration;
        else
            return acceleration;
    }

    private float GetDeceleration()
    {
        if (isOnIce)
            return iceDeceleration;
        else
            return deceleration;
    }

    private Vector2 getDirection()
    {
        if (disableMovement)
            return Vector2.Down;
        
        float x = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
        float y;
        if (Input.IsActionJustPressed("jump"))
            jumpBuffer = maxJumpBuffer;
        if (jumpBuffer > 0)
        {
            if (canJump)
            {
                jumpBuffer = 0;
                canJump = false;
                if (Input.IsActionPressed("jump"))
                {
                    if (lowerGravityOnJump)
                    {
                        gravity = tempGravity;
                        lowerGravityOnJump = false;
                        // Tell bird to start countdown
                        EmitSignal("BirdJumpBoostActivated");
                    }  
                    y = -1.0f;
                }
                else
                {
                    y = 0.0f;
                }
                return new Vector2(x,y);
            }
            else
                jumpBuffer--;
        }

        y = 1.0f;
        return new Vector2(x,y);
    }

    private Vector2 calculateMoveVelocity(
        Vector2 currentVelocity,
        Vector2 direction, 
        bool isJumpInterrupted, 
        Vector2 maxSpeed,
        float delta
        )
    {
        //Firstly, calculate target speed
        targetVelocity = maxSpeed.x * direction.x;
        if (IsOnFloor() && Input.IsActionPressed("move_down"))
            targetVelocity *= 0.5f;

        //Acceleration should reach from 0 to top speed in 6 frames and decelerate from top speed to 0 in 3 frames
        // Calculate x
        float x = currentVelocity.x;
        if (direction.x > 0.0f)
        {
            if (targetVelocity > currentVelocity.x)
            {
                x += GetAcceleration() * delta;
                x = Math.Min(x, targetVelocity);
            }
            else if (targetVelocity < currentVelocity.x)
            {
                x -= GetDeceleration() * delta;
                x = Math.Min(x, targetVelocity);
            }
        }
        else if (direction.x < 0.0f)
        {
            if (targetVelocity < currentVelocity.x)
            {
                x -= GetAcceleration() * delta;
                x = Math.Max(x, targetVelocity);
            }
            else if (targetVelocity > currentVelocity.x)
            {
                x -= GetDeceleration() * delta;
                x = Math.Max(x, targetVelocity);
            }
        }
        else // player isn't holding down any directional keys, targetVelocity = 0.0f
        {
            if (currentVelocity.x < 0.0f) // player is moving left
            {
                x += GetDeceleration() * delta;
                x = Math.Min(x, 0.0f);
            } 
            else if (currentVelocity.x > 0.0f) //player is moving right
                x -= Math.Min(GetDeceleration() * delta, x);
            else
                x = 0.0f;
        }

        // Calculate y
        // Note: Complete jump process should ideally take between 650-750ms. Current time at 1500 gravity is 550-610ms.
        float y = currentVelocity.y;
        if (isJumpInterrupted) // Triggered on the frame when Jump button is released
        {
            y = 0.0f;
            SetGravity();
        }
            

        if (direction.y == -1.0) // Player is moving upwards
        {
            y = maxSpeed.y * direction.y;
            y += gravity * (lowJumpMultiplier - 1.0f) * delta;
        }
        else // player is not moving upwards
            y += gravity * (fallMultiplier - 1.0f) * delta;
        return new Vector2(x, y);
    }

    public override void _PhysicsProcess(float delta) 
    {
        if (isInitialising) 
            Initialise();

        if (Position.y > theVoid && theVoid != 0 && !voidTriggered)
            RestartLevel();

        // Set velocity.y
        velocity.y += gravity * delta;
        
        // Coyote time! (exclusive to Mochi)
        if (IsOnFloor())
        {
            canJump = true;
            coyoteTimer = maxCoyoteTimer;
            SetGravity();
        }
        else if (coyoteTimer == 0)
            canJump = false;
        else
            coyoteTimer--;

        if (!lowerGravityOnJump)
            lowGravityTimer -= Math.Min(delta, lowGravityTimer);

        //Keyboard controls (exclusive to Mochi)
        bool isJumpInterrupted = (Input.IsActionJustReleased("jump") && velocity.y < 0.0f);
        Vector2 direction = getDirection();
        velocity = calculateMoveVelocity(velocity, direction, isJumpInterrupted, GetMaxSpeed(), delta);
        velocity = MoveAndSlide(velocity, FLOOR_NORMAL); // FLOOR_NORMAL = Vector2.Up

        // Determine if Mochi is on ice or not
        int sc = GetSlideCount();
        if (sc > 0)
        {
            for (int i = 0; i < sc; i++)
            {
                Node body = (Node)GetSlideCollision(i).Collider;
                if (body.Name == "IceTileMap")
                    isOnIce = true;
                else
                    isOnIce = false;
            }
        }
    }
}
