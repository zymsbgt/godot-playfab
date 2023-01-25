extends Label

func _ready() -> void:
	JavaScript.eval("alert('Calling JavaScript per GDScript!');")
	visible = false
	if OS.get_name() == "HTML5":
		visible = true
		text = "Heads up! This web version of the game is provided for convenience and may contain bugs.\nIf you've encountered a bug, please check if it's present on the Windows/Linux versions before reporting!"
	if OS.get_model_name() != "GenericDevice":
		visible = true
		text = "Warning: " + OS.get_model_name() + " device detected.\nThis game is designed for desktops and laptops only."
