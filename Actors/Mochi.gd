extends KinematicBody2D

#Movement related variables
export var _speed = Vector2(800.0, 1000.0)
export var _gravity: float = 3000.0
var _velocity = Vector2.ZERO
const FLOOR_NORMAL = Vector2.UP

#Mouse related variables
var lastKnownMousePosition: Vector2;

func _ready() -> void:
	Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED)

func _input(event):
	if event is InputEventMouseButton:
		if event.pressed:
			Input.set_mouse_mode(Input.MOUSE_MODE_CONFINED)
			lastKnownMousePosition = self.position
			Input.warp_mouse_position(self.position) # set mouse position to be the same as Mochi
			#if event.button_index == BUTTON_LEFT:
				#print("Left mouse button was clicked at ", event.position)
			#elif event.button_index == BUTTON_RIGHT:
				#print("Right mouse button was clicked at ", event.position)
		else:
			Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED)
			lastKnownMousePosition = self.position
			#if event.button_index == BUTTON_LEFT:
				#print("Left mouse button was unclicked at ", event.position)
			#elif event.button_index == BUTTON_RIGHT:
				#print("Right mouse button was unclicked at ", event.position)
	elif event is InputEventMouseMotion:
		lastKnownMousePosition = event.position;

func getDirection() -> Vector2:
	return Vector2(
		Input.get_action_strength("move_right") - Input.get_action_strength("move_left"),
		-1.0 if Input.is_action_just_pressed("jump") and is_on_floor() else 1.0
	)

func calculate_move_velocity(
		linear_velocity: Vector2,
		direction: Vector2,
		speed: Vector2,
		is_jump_interrupted: bool
	) -> Vector2:
	var out = linear_velocity
	out.x = speed.x * direction.x
	out.y += _gravity * get_physics_process_delta_time()
	if direction.y == -1.0:
		out.y = speed.y * direction.y 
	if is_jump_interrupted:
		out.y = 0.0
	return out

func _physics_process(delta: float) -> void:
	# Save Mochi's position
	var _MochiPosition: Vector2 = self.position;

	# Movement code
	_velocity.y += _gravity * delta;
	#_velocity.y = max(_velocity.y, maxSpeed.y);
	
	#Keyboard controls (exclusive to Mochi)
	var is_jump_interrupted = Input.is_action_just_released("jump") and _velocity.y < 0.0
	var _direction = getDirection()
	_velocity = calculate_move_velocity(_velocity, _direction, _speed, is_jump_interrupted)
	_velocity = move_and_slide(_velocity, FLOOR_NORMAL)

	# Save Mochi's new position after movement calculations
	var _newMochiPosition: Vector2 = self.position;

	#Position mouse
	if Input.get_mouse_mode() == Input.MOUSE_MODE_CONFINED:
		# Calculate displacements
		var _MochiPositionDisplacement: Vector2 = _newMochiPosition - _MochiPosition; #how much Mochi has moved

		# Calculate the mouse's new position
		var _newMousePosition: Vector2 = lastKnownMousePosition + _MochiPositionDisplacement;

		# Set the mouse's new position
		Input.warp_mouse_position(_newMousePosition);
