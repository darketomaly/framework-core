using System;

public enum DarkButtonMode
{
    AlwaysEnabled,
    EnabledInPlayMode,
    DisabledInPlayMode
}

[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public class DarkButtonAttribute : Attribute
{
    public string ButtonName { get; }
    public DarkButtonMode Mode { get; set; } = DarkButtonMode.AlwaysEnabled;

    public DarkButtonAttribute(string buttonName = null)
    {
        ButtonName = buttonName;
    }
}