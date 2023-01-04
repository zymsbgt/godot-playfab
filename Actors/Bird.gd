extends KinematicBody2D


# Declare member variables here. Examples:


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	$AudioStreamPlayer.play()
	$AudioStreamPlayer
	#pass


# Called every frame. 'delta' is the elapsed time since the previous frame.
#func _process(delta: float) -> void:
#	pass


func _on_AudioStreamPlayer_finished() -> void:
	print("finished!")
