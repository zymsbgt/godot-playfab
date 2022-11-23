extends Area2D

onready var _animated_sprite = $AnimatedSprite

func _ready() -> void:
	self.visible = false
	_animated_sprite.play("passive")

func _on_mouse_entered() -> void:
	$AudioStreamPlayer2D.play()
	_animated_sprite.play("active")

func _on_mouse_exited() -> void:
	_animated_sprite.play("passive")

func _input(event):
	if event is InputEventMouseButton:
		if event.button_index == BUTTON_LEFT and event.pressed:
			self.visible = true
		elif event.button_index == BUTTON_LEFT:
			_animated_sprite.play("passive")
			self.visible = false
		elif event.button_index == BUTTON_RIGHT and event.pressed:
			self.visible = true
		elif event.button_index == BUTTON_RIGHT:
			_animated_sprite.play("passive")
			self.visible = false
