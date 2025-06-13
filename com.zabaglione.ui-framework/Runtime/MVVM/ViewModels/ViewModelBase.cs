using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace jp.zabaglione.ui.mvvm.viewmodels
{
    /// <summary>
    /// Base class for all ViewModels in the MVVM pattern
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        private readonly Dictionary<string, object> _properties = new Dictionary<string, object>();
        private bool _isBusy;
        private string _busyMessage;

        /// <summary>
        /// Occurs when a property value changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets whether the ViewModel is busy
        /// </summary>
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        /// <summary>
        /// Gets or sets the busy message
        /// </summary>
        public string BusyMessage
        {
            get => _busyMessage;
            set => SetProperty(ref _busyMessage, value);
        }

        /// <summary>
        /// Sets a property value and raises PropertyChanged if the value changed
        /// </summary>
        /// <typeparam name="T">The property type</typeparam>
        /// <param name="storage">Reference to the backing field</param>
        /// <param name="value">The new value</param>
        /// <param name="propertyName">The property name (auto-filled)</param>
        /// <returns>True if the value changed</returns>
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Sets a property value using a dictionary store
        /// </summary>
        /// <typeparam name="T">The property type</typeparam>
        /// <param name="value">The new value</param>
        /// <param name="propertyName">The property name (auto-filled)</param>
        /// <returns>True if the value changed</returns>
        protected bool SetProperty<T>(T value, [CallerMemberName] string propertyName = null)
        {
            if (propertyName == null) return false;

            if (_properties.TryGetValue(propertyName, out var currentValue))
            {
                if (EqualityComparer<T>.Default.Equals((T)currentValue, value))
                    return false;
            }

            _properties[propertyName] = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Gets a property value from the dictionary store
        /// </summary>
        /// <typeparam name="T">The property type</typeparam>
        /// <param name="defaultValue">Default value if property doesn't exist</param>
        /// <param name="propertyName">The property name (auto-filled)</param>
        /// <returns>The property value</returns>
        protected T GetProperty<T>(T defaultValue = default, [CallerMemberName] string propertyName = null)
        {
            if (propertyName == null) return defaultValue;

            if (_properties.TryGetValue(propertyName, out var value))
                return (T)value;

            return defaultValue;
        }

        /// <summary>
        /// Raises the PropertyChanged event
        /// </summary>
        /// <param name="propertyName">The property name</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Raises PropertyChanged for multiple properties
        /// </summary>
        /// <param name="propertyNames">The property names</param>
        protected void OnPropertiesChanged(params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
            {
                OnPropertyChanged(propertyName);
            }
        }

        /// <summary>
        /// Raises PropertyChanged for all properties
        /// </summary>
        protected void OnAllPropertiesChanged()
        {
            OnPropertyChanged(string.Empty);
        }

        /// <summary>
        /// Sets the busy state with an optional message
        /// </summary>
        /// <param name="isBusy">Whether the ViewModel is busy</param>
        /// <param name="message">Optional busy message</param>
        protected void SetBusy(bool isBusy, string message = null)
        {
            IsBusy = isBusy;
            BusyMessage = message;
        }

        /// <summary>
        /// Executes an action while showing busy state
        /// </summary>
        /// <param name="action">The action to execute</param>
        /// <param name="busyMessage">Optional busy message</param>
        protected void ExecuteBusy(Action action, string busyMessage = null)
        {
            try
            {
                SetBusy(true, busyMessage);
                action();
            }
            finally
            {
                SetBusy(false);
            }
        }

        /// <summary>
        /// Executes an async action while showing busy state
        /// </summary>
        /// <param name="action">The async action to execute</param>
        /// <param name="busyMessage">Optional busy message</param>
        protected async System.Threading.Tasks.Task ExecuteBusyAsync(Func<System.Threading.Tasks.Task> action, string busyMessage = null)
        {
            try
            {
                SetBusy(true, busyMessage);
                await action();
            }
            finally
            {
                SetBusy(false);
            }
        }

        /// <summary>
        /// Called when the ViewModel is being initialized
        /// </summary>
        public virtual void Initialize()
        {
            // Override in derived classes
        }

        /// <summary>
        /// Called when the ViewModel is being cleaned up
        /// </summary>
        public virtual void Cleanup()
        {
            // Override in derived classes
            _properties.Clear();
        }

        /// <summary>
        /// Validates the ViewModel state
        /// </summary>
        /// <returns>True if valid</returns>
        public virtual bool Validate()
        {
            return true;
        }

#if UNITY_EDITOR
        /// <summary>
        /// Debug helper to log all property values
        /// </summary>
        [ContextMenu("Log Properties")]
        public void LogProperties()
        {
            Debug.Log($"[{GetType().Name}] Properties:");
            foreach (var kvp in _properties)
            {
                Debug.Log($"  {kvp.Key}: {kvp.Value}");
            }
        }
#endif
    }
}