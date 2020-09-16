using System;

public static class Events
{
    public delegate void OnPlayerHealthChange(int oldHealth, int newHealth, int maxHealth);
    public static OnPlayerHealthChange onPlayerHealthChange = delegate { };
    public static Action<string> onInfoAction = delegate { };
}