extends Button

func _ready() -> void:
	if OS.get_name() == "HTML5":
		text = "Download game"
