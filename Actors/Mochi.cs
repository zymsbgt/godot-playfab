using Godot;
using System;

public class Mochi : KinematicBody2D
{
    // Movement related variables
    [Export] private Vector2 speed = new Vector2(800.0f, 1000.0f);
    [Export] private float gravity = 1500.0f;
    private Vector2 velocity = Vector2.Zero;
    private Vector2 FLOOR_NORMAL = Vector2.Up;

    // BetterJump
    private float fallMultiplier = 2.5f;
    private float lowJumpMultiplier = 2.0f;
    private bool canJump;
    private int coyoteTimer, jumpBuffer;
    private int maxJumpBuffer = 12;
    private int maxCoyoteTimer = 12;

    // Mouse related variables
    private Vector2 lastKnownMousePosition, centerOfScreen, centerOfWheel, mouseOffsetFromCenterOfWheel; //centerOfWheel is Mochi's position relative to the screen

    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
        centerOfScreen = GetViewportRect().Size / 2; // Get the coordinates of the center of the screen
    }

    public override void _Input(InputEvent @event)
    {
        // Mouse in viewport coordinates.
        if (@event is InputEventMouseButton eventMouseButton)
        {
            if (eventMouseButton.IsPressed())
            {
                Input.MouseMode = Input.MouseModeEnum.Confined;
                centerOfWheel = GetGlobalTransformWithCanvas().origin;
                Input.WarpMousePosition(lastKnownMousePosition = centerOfWheel); // set mouse position to be the same as Mochi
                // if (eventMouseButton.ButtonIndex == 1)
                //     GD.Print("Left Mouse Click when Mochi is at: ", lastKnownMousePosition);
                // else if (eventMouseButton.ButtonIndex == 2)
                //     GD.Print("Right Mouse Click when Mochi is at: ", lastKnownMousePosition);
                // else
                //     GD.Print("Mochi.cs Input function: This shouldn't be happening!");
            }
            else
            {
                Input.MouseMode = Input.MouseModeEnum.Captured;
                lastKnownMousePosition = eventMouseButton.Position;
                mouseOffsetFromCenterOfWheel = Vector2.Zero;
                // if (eventMouseButton.ButtonIndex == 1)
                //     GD.Print("Left Mouse Unclick at: ", eventMouseButton.Position);
                // else if (eventMouseButton.ButtonIndex == 2)
                //     GD.Print("Right Mouse Unclick at: ", eventMouseButton.Position);
                // else
                //     GD.Print("Mochi.cs Input function: This shouldn't be happening!");
            }
        }
        else if (@event is InputEventMouseMotion eventMouseMotion) {
            mouseOffsetFromCenterOfWheel = eventMouseMotion.Position - centerOfWheel;
            lastKnownMousePosition = eventMouseMotion.Position;
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
                y = -1.0f;
                jumpBuffer = 0;
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
        Vector2 i = linearVelocity;
        i.x = speed.x * direction.x;
        // Note: Complete jump process should ideally take between 650-750ms. Current time at 1500 gravity is 550-610ms.
        if (isJumpInterrupted) // Triggered on the frame when Jump button is released
        {
            i.y = 0.0f;
            return i;
        }
        if (direction.y == -1.0) // Player is moving upwards
        {
            i.y = speed.y * direction.y;
            i.y += gravity * (lowJumpMultiplier - 1) * GetPhysicsProcessDeltaTime();
        }
        else // player is not moving upwards
            i.y += gravity * (fallMultiplier - 1) * GetPhysicsProcessDeltaTime();
        return i;
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

        // Position mouse
        if (Input.MouseMode == Input.MouseModeEnum.Confined) {
            // Set the center of the wheel where Mochi is at
            centerOfWheel = GetGlobalTransformWithCanvas().origin;
            if (mouseOffsetFromCenterOfWheel.y < -0.9f)
                mouseOffsetFromCenterOfWheel.y += 0.9972534f; // Floating precision rounding adjustment
            if (mouseOffsetFromCenterOfWheel.x < -0.9f)
                mouseOffsetFromCenterOfWheel.x += 0.9972534f; // Floating precision rounding adjustment
            Vector2 newMousePosition = centerOfWheel + mouseOffsetFromCenterOfWheel;
            Input.WarpMousePosition(newMousePosition);
        }
    }
}
