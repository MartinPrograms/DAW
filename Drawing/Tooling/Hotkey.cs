using OpenTK.Platform;

namespace Ion.Drawing.Tooling;

public class Hotkey
{
    public string Name { get; }
    public string Description { get; }
    public Keys[] Key { get; }
    public Action Action { get; }
    
    public Hotkey(string name, string description, Keys []key, Action action)
    {
        this.Name = name;
        this.Description = description;
        this.Key = key;
        this.Action = action;
    }
    
    private bool _debounce = false;
    public bool IsPressed(KeyboardState state)
    {
        if (_debounce)
        {
            // Check if one of the keys is unpressed
            foreach (Keys k in Key)
            {
                if (!state.IsKeyDown(k))
                {
                    _debounce = false;
                    return false;
                }
            }
        }
        
        // Check if all keys are pressed
        foreach (Keys k in Key)
        {
            if (!state.IsKeyDown(k))
            {
                return false;
            }
        }
        
        if (!_debounce)
        {
            _debounce = true;
            return true;
        }

        return false;
    }
}