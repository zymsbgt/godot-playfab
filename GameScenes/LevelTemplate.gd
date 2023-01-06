extends Node2D

func _process(_delta: float) -> void:
	OS.window_fullscreen
	get_tree().get_root().set_transparent_background(true)
	pass
