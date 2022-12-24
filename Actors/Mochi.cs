using Godot;
using System;

public class Mochi : KinematicBody2D
{
    // Movement related variables
    private float acceleration, deceleration, targetVelocity;
    [Export] private Vector2 maxSpeed = new Vector2(800.0f, 1000.0f);
    [Export] private float gravity = 1500.0f;
    private Vector2 velocity = Vector2.Zero;
    private Vector2 FLOOR_NORMAL = Vector2.Up;
    private bool disableMovement = false;

    // Better jump
    private float fallMultiplier = 2.5f, lowJumpMultiplier = 2.0f;
    private bool canJump;
    private int coyoteTimer, jumpBuffer;
    private int maxJumpBuffer = 10;
    private int maxCoyoteTimer = 10;

    // Mouse cursor node
    private Area2D mouseCursor; 
    private Sprite LeftMouseClickHint;

    // Signals
    [Signal] delegate void destroy_left_mouse_click_hint();

    public override void _Ready()
    {
        //OS.WindowFullscreen = true;
        mouseCursor = GetNode<Area2D>("MouseCursor");
        mouseCursor.Hide();

        if (GetTree().CurrentScene.Name == "LevelTemplate")
            LeftMouseClickHint = GetNode<Sprite>("../LeftMouseClickHint");

        acceleration = maxSpeed.x * 10.0f;
        deceleration = maxSpeed.x * 20.0f;

        // Captures the mouse if on PC
        #if GODOT_PC
        Input.MouseMode = Input.MouseModeEnum.Captured;
        #elif GODOT_WEB // Capture mouse on mouse click event instead
        #elif GODOT // In case someone compiles this code to run on an unintended OS
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

    // Incoming signal
    public void _on_disable_player_movement(bool state = true)
    {
        disableMovement = state;

        // Fire a signal to mouse hint to disappear
        if (!state && GetTree().CurrentScene.Name == "LevelTemplate")
            EmitSignal("destroy_left_mouse_click_hint");
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
        //float x = Math.Abs(currentVelocity.x);
        // x += acceleration * delta;
        // if (IsOnFloor() && Input.IsActionPressed("move_down"))
        //     x = Math.Min(x, maxSpeed.x * Math.Abs(direction.x) * 0.5f);
        // else
        //     x = Math.Min(x, maxSpeed.x);
        // //If direction.x = 0 but currentVelocity.x is not 0, decelerate
        // if (direction.x == 0.0f)
        // {
        //     if (currentVelocity.x < 0.0f) // player is moving left
        //     {
        //         x -= Math.Min(deceleration * delta, x);
        //         x *= -1;
        //     } 
        //     else if (currentVelocity.x > 0.0f) //player is moving right
        //         x -= Math.Min(deceleration * delta, x);
        //     else
        //         x = 0.0f;
        // }
        // else
        //     x *= direction.x;

        // Calculating x rewrite
        float x = currentVelocity.x;
        if (direction.x > 0.0f)
        {
            if (targetVelocity > currentVelocity.x)
            {
                x += acceleration * delta;
                x = Math.Min(x, targetVelocity);
            }
            else if (targetVelocity < currentVelocity.x)
            {
                x -= deceleration * delta;
                x = Math.Min(x, targetVelocity);
            }
        }
        else if (direction.x < 0.0f)
        {
            if (targetVelocity < currentVelocity.x)
            {
                x -= acceleration * delta;
                x = Math.Max(x, targetVelocity);
            }
            else if (targetVelocity > currentVelocity.x)
            {
                x -= deceleration * delta;
                x = Math.Max(x, targetVelocity);
            }
        }
        else // player isn't holding down any directional keys, targetVelocity = 0.0f
        {
            if (currentVelocity.x < 0.0f) // player is moving left
            {
                x += deceleration * delta;
                x = Math.Min(x, 0.0f);
            } 
            else if (currentVelocity.x > 0.0f) //player is moving right
                x -= Math.Min(deceleration * delta, x);
            else
                x = 0.0f;
        }

        // Calculate y
        // Note: Complete jump process should ideally take between 650-750ms. Current time at 1500 gravity is 550-610ms.
        if (isJumpInterrupted) // Triggered on the frame when Jump button is released
        {
            return new Vector2(x, 0.0f);
        }

        float y = currentVelocity.y;
        if (direction.y == -1.0) // Player is moving upwards
        {
            y = maxSpeed.y * direction.y;
            y += gravity * (lowJumpMultiplier - 1.0f) * delta;
        }
        else // player is not moving upwards
        {
            y += gravity * (fallMultiplier - 1.0f) * delta;
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
        velocity = calculateMoveVelocity(velocity, direction, isJumpInterrupted, maxSpeed, delta);

        if (!disableMovement)
            velocity = MoveAndSlide(velocity, FLOOR_NORMAL);
    }
}
