// Stub classes for legacy WinForms menu types removed in .NET 8
// These allow CADability.Forms to compile but the menu functionality is disabled

using System.ComponentModel;
using System.Collections;

namespace System.Windows.Forms
{
    // Legacy menu types removed in .NET 8 - providing minimal stubs
    public class MenuItem : Component
    {
        public ICollection MenuItems { get; } = new ArrayList();
        public string Text { get; set; } = string.Empty;
        public int Index { get; set; }
        public bool Checked { get; set; }
        public bool Enabled { get; set; } = true;
        public object? Tag { get; set; }
        public bool IsParent => MenuItems.Count > 0;

        public event EventHandler? Click;
        public event EventHandler? Popup;
        public event EventHandler? Select;

        protected virtual void OnClick(EventArgs e) => Click?.Invoke(this, e);
    }

    public class MainMenu : Menu
    {
        public ICollection MenuItems { get; } = new ArrayList();
        public virtual MenuItem[] GetMenuItems() => Array.Empty<MenuItem>();
    }

    public class ContextMenu : Menu
    {
        public ICollection MenuItems { get; } = new ArrayList();
        public Control? SourceControl { get; set; }
        public void Show(Control control, System.Drawing.Point pos) { }
    }

    public abstract class Menu : Component
    {
        public object? Tag { get; set; }
    }

    // Additional stub types referenced by CADability.Forms
    public class MenuItemWithHandler : MenuItem
    {
        public MenuItemWithHandler(object? target, string methodName, string text)
        {
            Text = text;
        }
    }

    public class ContextMenuWithHandler : ContextMenu
    {
        public ContextMenuWithHandler(object? target)
        {
        }

        public void UpdateCommand() { }
    }
}

// Stub for MenuManager namespace
namespace CADability.UserInterface
{
    public static class MenuManager
    {
        public static System.Windows.Forms.ContextMenuWithHandler MakeContextMenu(object? target)
        {
            return new System.Windows.Forms.ContextMenuWithHandler(target);
        }

        public static System.Windows.Forms.MainMenu MakeMainMenu(object? target)
        {
            return new System.Windows.Forms.MainMenu();
        }
    }
}
