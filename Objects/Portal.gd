tool
extends Area2D

onready var anim_player: AnimationPlayer = $AnimationPlayer

#export var next_scene: PackedScene
export var next_scene_string: String

func _on_body_entered(_body: PhysicsBody2D):
	teleport()

func _get_configuration_warning() -> String:
	return "The property Next Level can't be empty" if not next_scene_string else ""

func teleport() -> void:
	anim_player.play("fade_to_black")
	yield(anim_player, "animation_finished")
	#get_tree().change_scene_to(next_scene)
	SceneManager.goto_scene(next_scene_string)
