using Godot;
using System;

public class Mochi : KinematicBody2D
{
    // Movement related variables
    private Vector2 speed;
    [Export] private Vector2 maxSpeed = new Vector2(800.0f, 1000.0f);
    [Export] private float gravity = 1500.0f;
    private Vector2 velocity = Vector2.Zero;
    private Vector2 FLOOR_NORMAL = Vector2.Up;

    // Better jump
    private float fallMultiplier = 2.5f, lowJumpMultiplier = 2.0f;
    private bool canJump;
    private int coyoteTimer, jumpBuffer;
    private int maxJumpBuffer = 10;
    private int maxCoyoteTimer = 10;

    // Mouse cursor node
    private Area2D mouseCursor;

    public override void _Ready()
    {
        //OS.WindowFullscreen = true;
        mouseCursor = GetNode<Area2D>("MouseCursor");
        // Set the command below to toggle visibility instead of destroying the mouseCursor on spawn
        mouseCursor.Hide();

        // Captures the mouse if on PC
        #if GODOT_PC
        Input.MouseMode = Input.MouseModeEnum.Captured;
        #elif GODOT_WEB // Capture mouse on mouse click event instead
        #elif GODOT // In case someone compiles this code to run on an unintended OS
        Input.MouseMode = Input.MouseModeEnum.Captured;
        #elif UNITY_5_3_OR_NEWER
        GD.Print("Mochi.cs: Seems like you found a way to run this script on Unity. Cool!")
        #endif
    }

    public override void _Input(InputEvent @event)
    {
        // Mouse in viewport coordinates.
        if (@event is InputEventMouseButton eventMouseButton)
        {
            if (eventMouseButton.IsPressed())
            {
                mouseCursor.Position = Vector2.Zero;
                // if (eventMouseButton.ButtonIndex == 1)
                //     GD.Print("Left Mouse click at: ", lastKnownMousePosition);
                // else if (eventMouseButton.ButtonIndex == 2)
                //     GD.Print("Right Mouse click at: ", lastKnownMousePosition);
                // else
                //     GD.Print("Mochi.cs Input function: This shouldn't be happening!");
                mouseCursor.Show();
                #if GODOT_WEB
                if (Input.MouseMode != Input.MouseModeEnum.Captured)
                    Input.MouseMode = Input.MouseModeEnum.Captured;
                #endif
            }
            else
            {
                mouseCursor.Position = Vector2.Zero;
                // if (eventMouseButton.ButtonIndex == 1)
                //     GD.Print("Left Mouse Unclick at: ", eventMouseButton.Position);
                // else if (eventMouseButton.ButtonIndex == 2)
                //     GD.Print("Right Mouse Unclick at: ", eventMouseButton.Position);
                // else
                //     GD.Print("Mochi.cs Input function: This shouldn't be happening!");
                mouseCursor.Hide();
            }
        }
    }

    private Vector2 getDirection() 
    {
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
                    y = -1.0f;
                else
                    y = 0.0f;
                return new Vector2(x,y);
            }
            else
                jumpBuffer--;
        }
        y = 1.0f;
        return new Vector2(x,y);
    }

    private Vector2 calculateMoveVelocity(
        Vector2 linearVelocity, 
        Vector2 direction, 
        bool isJumpInterrupted, 
        Vector2 maxSpeed, 
        Vector2? speed = null
        )
    {
        if (speed == null)
            speed = Vector2.Zero;

        // Calculate x
        float x = linearVelocity.x;
        if (Input.IsActionPressed("move_down"))
            maxSpeed.x *= 0.5f;
        x = maxSpeed.x * direction.x;

        // Calculate y
        // Note: Complete jump process should ideally take between 650-750ms. Current time at 1500 gravity is 550-610ms.
        if (isJumpInterrupted) // Triggered on the frame when Jump button is released
        {
            return new Vector2(x, 0.0f);
        }

        float y = linearVelocity.y;
        if (direction.y == -1.0) // Player is moving upwards
        {
            y = maxSpeed.y * direction.y;
            y += gravity * (lowJumpMultiplier - 1.0f) * GetPhysicsProcessDeltaTime();
        }
        else // player is not moving upwards
        {
            y += gravity * (fallMultiplier - 1.0f) * GetPhysicsProcessDeltaTime();
        }
        return new Vector2(x, y);
    }

    public override void _PhysicsProcess(float delta) 
    {
        // Set velocity.y
        velocity.y += gravity * delta;
        
        // Coyote time! (exclusive to Mochi)
        if (IsOnFloor())
        {
            canJump = true;
            coyoteTimer = maxCoyoteTimer;
        }
        else if (coyoteTimer == 0)
            canJump = false;
        else
            coyoteTimer--;

        //Keyboard controls (exclusive to Mochi)
        bool isJumpInterrupted = (Input.IsActionJustReleased("jump") && velocity.y < 0.0f);
        Vector2 direction = getDirection();
        velocity = calculateMoveVelocity(velocity, direction, isJumpInterrupted, maxSpeed);

        // Final velocity
        velocity = MoveAndSlide(velocity, FLOOR_NORMAL);
    }
}
