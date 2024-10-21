using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace ApplicationFramework
{
    public abstract class VMBase : ObservableObject
    {
        public abstract string? Title { get; }
    }
}
