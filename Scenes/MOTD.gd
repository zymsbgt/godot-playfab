extends Label

func _ready() -> void:
	visible = false
	#if OS.get_name() == "HTML5":
	#	visible = true
	if OS.get_model_name() != "GenericDevice":
		visible = true
		text = "Warning: " + OS.get_model_name() + " device detected.\nThis game is designed for desktops and laptops only."
