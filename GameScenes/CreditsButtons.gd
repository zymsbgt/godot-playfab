extends Button

func _ready() -> void:
	Input.set_mouse_mode(Input.MOUSE_MODE_VISIBLE)

func _on_Button1_pressed() -> void:
	OS.shell_open("https://ma9ici4n.itch.io/pixel-art-bird-16x16")

func _on_Button2_pressed() -> void:
	OS.shell_open("https://www.deviantart.com/kspriter95/art/kirby-FC-sword-sprite-read-description-421667074")

func _on_Button3_pressed() -> void:
	OS.shell_open("https://lena.fyi")

func _on_Button4_pressed() -> void:
	OS.shell_open("https://www.ashellinthepit.com/")
