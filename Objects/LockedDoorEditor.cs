using Godot;
using System;

[Tool]
public class LockedDoorEditor : LockedDoor
{
    public override string _GetConfigurationWarning()
    {
        if (nextScene == null)
            return "The next scene property can't be empty!";
        else
            return "";
    }
}
