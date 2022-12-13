extends Area2D

onready var _animated_sprite = $AnimatedSprite
var _doNotPlayOnThisFrame: bool = false;
var _queuePlay: bool = false;

func _ready() -> void:
	self.visible = false
	_animated_sprite.play("passive")
	#self.monitoring = true

func _on_mouse_entered() -> void:
	_queuePlay = true

func _on_mouse_exited() -> void:
	_animated_sprite.play("passive")

func _on_area_entered(area: Area2D) -> void:
	_queuePlay = true

func _on_area_exited(area: Area2D) -> void:
	_animated_sprite.play("passive")

func _input(event):
	if event is InputEventMouseButton:
		if event.pressed:
			self.visible = true
			_doNotPlayOnThisFrame = true
		else:
			_animated_sprite.play("passive")
			self.visible = false

func _process(delta: float) -> void:
	if (_queuePlay && !_doNotPlayOnThisFrame):
		$AudioStreamPlayer2D.play()
		_animated_sprite.play("active")
	_doNotPlayOnThisFrame = false
	_queuePlay = false
