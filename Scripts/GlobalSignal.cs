using Godot;
using System;

public class GlobalSignal : Node
{
    private Godot.Collections.Dictionary _emitters = new Godot.Collections.Dictionary{};
    private Godot.Collections.Array _emit_queue;
    private Godot.Collections.Dictionary _listeners = new Godot.Collections.Dictionary{};
    private bool _gs_ready = false;

    // func _process(_delta):
    // if not _gs_ready:
    // 	_make_ready()
    // 	set_process(false)
    // 	set_physics_process(false)

    public override void _Process(float delta)
    {
        if (!_gs_ready)
        {
            //_make_ready();
            SetProcess(false);
            SetPhysicsProcess(false);
        }
    }

    private void _connect_emitter_to_listeners(String signal_name, Godot.Object emitter)
    {
        var listeners = _listeners[signal_name];
    }

// # Connect an emitter to existing listeners of its signal.
// func _connect_emitter_to_listeners(signal_name: String, emitter: Object) -> void:
// 	var listeners = _listeners[signal_name]
// 	for listener in listeners.values():
// 		if _process_purge(listener, listeners):
// 			continue
// 		emitter.connect(signal_name, listener.object, listener.method)


// # Connect a listener to emitters who emit the signal it's listening for.
// func _connect_listener_to_emitters(signal_name: String, listener: Object, method: String) -> void:
// 	var emitters = _emitters[signal_name]
// 	for emitter in emitters.values():
// 		if _process_purge(emitter, emitters):
// 			continue
// 		emitter.object.connect(signal_name, listener, method)


// # Execute the ready process and initiate processing the emit queue.
// func _make_ready() -> void:
// 	_gs_ready = true
// 	_process_emit_queue()
}
