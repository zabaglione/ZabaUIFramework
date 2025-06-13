using System;

namespace jp.zabaglione.ui.core.binding
{
    /// <summary>
    /// Interface for objects that notify when their properties change
    /// (Unity-compatible version of System.ComponentModel.INotifyPropertyChanged)
    /// </summary>
    public interface INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when a property value changes
        /// </summary>
        event Action<string> PropertyChanged;
    }
}