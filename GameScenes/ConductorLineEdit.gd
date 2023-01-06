extends LineEdit

signal text_updated(text_value)


func _ready():
  GlobalSignal.add_emitter('text_updated', self)
  connect('text_changed', self, '_on_Text_changed')


func _on_Text_changed(_value):
  emit_signal('text_updated', text)
