extends Control
class_name LoggedIn

var login_result: LoginResult
const color_green = Color(0, 1, 0, 0.6)
const color_red = Color(1, 0, 0, 0.8)
signal logout

func _ready():
	var _error = PlayFabManager.client.connect("api_error", self, "_on_PlayFab_api_error")
	$VBoxContainer/StartButton.self_modulate = color_green
	$VBoxContainer/LogoutButton.self_modulate = color_red
	#OS.window_fullscreen = true

# Called when the node enters the scene tree for the first time.
func update():
	if login_result != null:
		$VBoxContainer/LoginResultContainer/AccountPlayerId/Edit.text = login_result.PlayFabId
		$VBoxContainer/LoginResultContainer/TitlePlayerId/Edit.text = login_result.InfoResultPayload.AccountInfo.TitleInfo.TitlePlayerAccount.Id
		$VBoxContainer/LoginResultContainer/TitlePlayerName/Edit.text = login_result.InfoResultPayload.AccountInfo.TitleInfo.DisplayName
		$VBoxContainer/LoginResultContainer/SessionTicket/Edit.text = login_result.SessionTicket
		$VBoxContainer/LoginResultContainer/EntityToken/Edit.text = login_result.EntityToken.EntityToken
		$VBoxContainer/LoginResultContainer/EntityType/Edit.text = login_result.EntityToken.Entity.Type
		$VBoxContainer/LoginResultContainer/EntityId/Edit.text = login_result.EntityToken.Entity.Id

		$VBoxContainer/LoginResultContainer.show()


func _on_GetTitleDataButton_pressed():
	var request_data = GetTitleDataRequest.new()
#	request_data.Keys.append("BarKey")	# Would only get the key "BarKey"
	PlayFabManager.client.get_title_data(request_data, funcref(self, "_on_get_title_data"))


func _on_get_title_data(response):
	$VBoxContainer/Response.text = JSON.print(response.data, "\t")


func _on_PlayFab_api_error(error: ApiErrorWrapper):
	print_debug(error.errorMessage)
#	if error.errorMessage == "User not found": # Email does not exist on PLayfab
#		# Add code here to return to main menu and show error message
#		pass
#	elif error.errorMessage == "Invalid email address or password": # Wrong password
#		# Add code here to return to main menu and show error message
#		pass
#	elif error.errorMessage == "Invalid input parameters": # Something's wrong? but idk what
#		# Add code here to return to main menu and show error message
#		pass


func _on_EventsPlayStream_pressed():
	SceneManager.goto_scene("res://Scenes/Events.tscn")

func _on_RequestBuilder_pressed():
	SceneManager.goto_scene("res://Scenes/RequestBuilder.tscn")

func _on_StartButton_pressed() -> void:
	OS.window_fullscreen = true
	SceneManager.goto_scene("res://GameScenes/Conductor.tscn")

func _on_LogoutButton_pressed():
	emit_signal("logout")
