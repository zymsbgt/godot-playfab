using Godot;
using System;

public class Mochi : KinematicBody2D
{
    // Movement related variables
    [Export] private Vector2 speed = new Vector2(800.0f, 1000.0f);
    [Export] private float gravity = 1500.0f;
    private Vector2 velocity = Vector2.Zero;
    private Vector2 FLOOR_NORMAL = Vector2.Up;

    // Better jump
    private float fallMultiplier = 2.5f, lowJumpMultiplier = 2.0f;
    private bool canJump;
    private int coyoteTimer, jumpBuffer;
    private int maxJumpBuffer = 10;
    private int maxCoyoteTimer = 10;

    // Mouse related variables
    private Vector2 centerOfScreen, centerOfWheel, mouseOffsetFromCenterOfWheel; //centerOfWheel is Mochi's position relative to the screen

    // Mouse cursor node
    private Area2D mouseCursor;

    public override void _Ready()
    {
        centerOfScreen = GetViewportRect().Size * 0.5f; // Get the coordinates of the center of the screen
        OS.WindowFullscreen = true;
        mouseCursor = GetNode<Area2D>("MouseCursor");
        // Set the command below to toggle visibility instead of destroying the mouseCursor on spawn
        mouseCursor.Hide();
        // Captures the mouse. This should execute last because of the return keyword
        #if GODOT_HTML5
        return;
        #else
        Input.MouseMode = Input.MouseModeEnum.Captured;
        #endif
    }

    public override void _Input(InputEvent @event)
    {
        // Mouse in viewport coordinates.
        if (@event is InputEventMouseButton eventMouseButton)
        {
            if (eventMouseButton.IsPressed())
            {
                centerOfWheel = GetGlobalTransformWithCanvas().origin;
                mouseCursor.Position = Vector2.Zero;
                // if (eventMouseButton.ButtonIndex == 1)
                //     GD.Print("Left Mouse click at: ", lastKnownMousePosition);
                // else if (eventMouseButton.ButtonIndex == 2)
                //     GD.Print("Right Mouse click at: ", lastKnownMousePosition);
                // else
                //     GD.Print("Mochi.cs Input function: This shouldn't be happening!");
                mouseCursor.Show();
                #if GODOT_HTML5
                if (Input.MouseMode != Input.MouseModeEnum.Captured)
                    Input.MouseMode = Input.MouseModeEnum.Captured;
                #endif
            }
            else
            {
                mouseOffsetFromCenterOfWheel = Vector2.Zero;
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

    private Vector2 getDirection() {
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

    private Vector2 calculateMoveVelocity(Vector2 linearVelocity, Vector2 direction, Vector2 speed, bool isJumpInterrupted)
    {
        Vector2 output = linearVelocity;
        if (Input.IsActionPressed("move_down"))
            speed.x *= 0.5f;
        output.x = speed.x * direction.x;

        // Note: Complete jump process should ideally take between 650-750ms. Current time at 1500 gravity is 550-610ms.
        if (isJumpInterrupted) // Triggered on the frame when Jump button is released
        {
            output.y = 0.0f;
            return output;
        }
        if (direction.y == -1.0) // Player is moving upwards
        {
            output.y = speed.y * direction.y;
            output.y += gravity * (lowJumpMultiplier - 1) * GetPhysicsProcessDeltaTime();
        }
        else // player is not moving upwards
            output.y += gravity * (fallMultiplier - 1) * GetPhysicsProcessDeltaTime();
        return output;
    }

    public override void _PhysicsProcess(float delta) 
    {
        // Movement code
        velocity.y += gravity * delta;
        // velocity.y = Math.Max(velocity.y, maxSpeed.y);
        
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
        velocity = calculateMoveVelocity(velocity, direction, speed, isJumpInterrupted);

        // Final velocity
        velocity = MoveAndSlide(velocity, FLOOR_NORMAL);
    }
}
