extends Area2D

onready var _animated_sprite = $AnimatedSprite
var _queuePlay: bool = false
signal disable_player_movement(state)

func _ready() -> void:
	self.visible = false
	_animated_sprite.play("passive")

func _on_area_entered(_area: Area2D) -> void:
	_queuePlay = true

func _on_area_exited(_area: Area2D) -> void:
	_animated_sprite.play("passive")

func _input(event):
	if event is InputEventMouseButton:
		if event.pressed:
			self.visible = true
		else:
			_animated_sprite.play("passive")
			self.visible = false

func _process(_delta: float) -> void:
	if (_queuePlay):
		$AudioStreamPlayer2D.play()
		_animated_sprite.play("active")
		
		if get_tree().get_current_scene().get_name() == "LevelTemplate":
			emit_signal("disable_player_movement", false)
	_queuePlay = false
