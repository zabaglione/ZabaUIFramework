using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace jp.zabaglione.ui.core.binding
{
    /// <summary>
    /// Component that binds a source property to a target property
    /// </summary>
    public class PropertyBinding : MonoBehaviour
    {
        [Header("Source")]
        [SerializeField] private UnityEngine.Object _source;
        [SerializeField] private string _sourcePropertyPath;

        [Header("Target")]
        [SerializeField] private UnityEngine.Object _target;
        [SerializeField] private string _targetPropertyPath;

        [Header("Binding Settings")]
        [SerializeField] private BindingMode _bindingMode = BindingMode.OneWay;
        [SerializeField] private bool _bindOnEnable = true;
        [SerializeField] private float _updateInterval = 0f; // 0 = immediate updates

        [Header("Type Conversion")]
        [SerializeField] private bool _enableTypeConversion = true;

        [Header("Events")]
        [SerializeField] private UnityEvent _onValueChanged = new UnityEvent();
        [SerializeField] private UnityEvent<string> _onBindingError = new UnityEvent<string>();

        private PropertyInfo _sourceProperty;
        private PropertyInfo _targetProperty;
        private object _lastSourceValue;
        private object _lastTargetValue;
        private float _lastUpdateTime;
        private bool _isUpdating;
        private Action<string> _sourcePropertyChangedHandler;
        private Action<string> _targetPropertyChangedHandler;

        /// <summary>
        /// Gets or sets the source object
        /// </summary>
        public UnityEngine.Object Source
        {
            get => _source;
            set
            {
                if (_source != value)
                {
                    UnsubscribeFromSource();
                    _source = value;
                    _sourceProperty = null;
                    if (isActiveAndEnabled)
                    {
                        SetupBinding();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the source property path
        /// </summary>
        public string SourcePropertyPath
        {
            get => _sourcePropertyPath;
            set
            {
                if (_sourcePropertyPath != value)
                {
                    _sourcePropertyPath = value;
                    _sourceProperty = null;
                    if (isActiveAndEnabled)
                    {
                        SetupBinding();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the binding mode
        /// </summary>
        public BindingMode Mode
        {
            get => _bindingMode;
            set => _bindingMode = value;
        }

        private void OnEnable()
        {
            if (_bindOnEnable)
            {
                SetupBinding();
            }
        }

        private void OnDisable()
        {
            UnsubscribeFromSource();
            UnsubscribeFromTarget();
        }

        private void Update()
        {
            if (_updateInterval > 0 && Time.time - _lastUpdateTime >= _updateInterval)
            {
                UpdateBinding();
                _lastUpdateTime = Time.time;
            }
        }

        /// <summary>
        /// Manually triggers binding setup
        /// </summary>
        public void SetupBinding()
        {
            UnsubscribeFromSource();
            UnsubscribeFromTarget();

            if (!ValidateBinding())
            {
                return;
            }

            // Get property info
            _sourceProperty = GetProperty(_source, _sourcePropertyPath);
            _targetProperty = GetProperty(_target, _targetPropertyPath);

            if (_sourceProperty == null || _targetProperty == null)
            {
                LogError($"Failed to find properties: Source={_sourcePropertyPath}, Target={_targetPropertyPath}");
                return;
            }

            // Subscribe to property changes
            SubscribeToSource();
            if (_bindingMode == BindingMode.TwoWay || _bindingMode == BindingMode.OneWayToSource)
            {
                SubscribeToTarget();
            }

            // Initial update
            if (_bindingMode != BindingMode.OneWayToSource)
            {
                UpdateTarget();
            }
            else
            {
                UpdateSource();
            }
        }

        /// <summary>
        /// Manually updates the binding
        /// </summary>
        public void UpdateBinding()
        {
            if (_bindingMode == BindingMode.OneWay || _bindingMode == BindingMode.TwoWay)
            {
                UpdateTarget();
            }
            
            if (_bindingMode == BindingMode.TwoWay || _bindingMode == BindingMode.OneWayToSource)
            {
                UpdateSource();
            }
        }

        private bool ValidateBinding()
        {
            if (_source == null)
            {
                LogError("Source object is null");
                return false;
            }

            if (_target == null)
            {
                LogError("Target object is null");
                return false;
            }

            if (string.IsNullOrEmpty(_sourcePropertyPath))
            {
                LogError("Source property path is empty");
                return false;
            }

            if (string.IsNullOrEmpty(_targetPropertyPath))
            {
                LogError("Target property path is empty");
                return false;
            }

            return true;
        }

        private PropertyInfo GetProperty(object obj, string propertyPath)
        {
            if (obj == null || string.IsNullOrEmpty(propertyPath))
                return null;

            var type = obj.GetType();
            var parts = propertyPath.Split('.');
            PropertyInfo property = null;

            foreach (var part in parts)
            {
                property = type.GetProperty(part, BindingFlags.Public | BindingFlags.Instance);
                if (property == null)
                {
                    // Try field as fallback
                    var field = type.GetField(part, BindingFlags.Public | BindingFlags.Instance);
                    if (field != null)
                    {
                        // Create a wrapper property for the field
                        // This is a simplified approach - in production, you'd want proper field support
                        LogError($"Field binding not fully supported yet: {part}");
                        return null;
                    }
                    return null;
                }

                if (property.PropertyType.IsValueType || property.PropertyType == typeof(string))
                {
                    // This should be the last part
                    break;
                }

                type = property.PropertyType;
            }

            return property;
        }

        private void UpdateTarget()
        {
            if (_isUpdating || _sourceProperty == null || _targetProperty == null)
                return;

            try
            {
                _isUpdating = true;
                var sourceValue = _sourceProperty.GetValue(_source);

                if (!Equals(sourceValue, _lastSourceValue))
                {
                    _lastSourceValue = sourceValue;
                    
                    var convertedValue = ConvertValue(sourceValue, _targetProperty.PropertyType);
                    _targetProperty.SetValue(_target, convertedValue);
                    
                    _onValueChanged?.Invoke();
                }
            }
            catch (Exception e)
            {
                LogError($"Failed to update target: {e.Message}");
            }
            finally
            {
                _isUpdating = false;
            }
        }

        private void UpdateSource()
        {
            if (_isUpdating || _sourceProperty == null || _targetProperty == null)
                return;

            try
            {
                _isUpdating = true;
                var targetValue = _targetProperty.GetValue(_target);

                if (!Equals(targetValue, _lastTargetValue))
                {
                    _lastTargetValue = targetValue;
                    
                    var convertedValue = ConvertValue(targetValue, _sourceProperty.PropertyType);
                    _sourceProperty.SetValue(_source, convertedValue);
                    
                    _onValueChanged?.Invoke();
                }
            }
            catch (Exception e)
            {
                LogError($"Failed to update source: {e.Message}");
            }
            finally
            {
                _isUpdating = false;
            }
        }

        private object ConvertValue(object value, Type targetType)
        {
            if (value == null)
                return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;

            var sourceType = value.GetType();
            if (targetType.IsAssignableFrom(sourceType))
                return value;

            if (!_enableTypeConversion)
            {
                LogError($"Type conversion disabled. Cannot convert {sourceType} to {targetType}");
                return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
            }

            try
            {
                // Handle common Unity type conversions
                if (targetType == typeof(string))
                {
                    return value.ToString();
                }

                if (sourceType == typeof(string))
                {
                    if (targetType == typeof(int))
                        return int.Parse((string)value);
                    if (targetType == typeof(float))
                        return float.Parse((string)value);
                    if (targetType == typeof(bool))
                        return bool.Parse((string)value);
                }

                // Try standard conversion
                return Convert.ChangeType(value, targetType);
            }
            catch (Exception e)
            {
                LogError($"Failed to convert {sourceType} to {targetType}: {e.Message}");
                return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
            }
        }

        private void SubscribeToSource()
        {
            if (_source is INotifyPropertyChanged notifySource)
            {
                _sourcePropertyChangedHandler = (propertyName) =>
                {
                    if (propertyName == _sourcePropertyPath || string.IsNullOrEmpty(propertyName))
                    {
                        if (_updateInterval <= 0)
                        {
                            UpdateTarget();
                        }
                    }
                };
                notifySource.PropertyChanged += _sourcePropertyChangedHandler;
            }
        }

        private void UnsubscribeFromSource()
        {
            if (_source is INotifyPropertyChanged notifySource && _sourcePropertyChangedHandler != null)
            {
                notifySource.PropertyChanged -= _sourcePropertyChangedHandler;
                _sourcePropertyChangedHandler = null;
            }
        }

        private void SubscribeToTarget()
        {
            if (_target is INotifyPropertyChanged notifyTarget)
            {
                _targetPropertyChangedHandler = (propertyName) =>
                {
                    if (propertyName == _targetPropertyPath || string.IsNullOrEmpty(propertyName))
                    {
                        if (_updateInterval <= 0)
                        {
                            UpdateSource();
                        }
                    }
                };
                notifyTarget.PropertyChanged += _targetPropertyChangedHandler;
            }
        }

        private void UnsubscribeFromTarget()
        {
            if (_target is INotifyPropertyChanged notifyTarget && _targetPropertyChangedHandler != null)
            {
                notifyTarget.PropertyChanged -= _targetPropertyChangedHandler;
                _targetPropertyChangedHandler = null;
            }
        }

        private void LogError(string message)
        {
            Debug.LogError($"[PropertyBinding] {message}", this);
            _onBindingError?.Invoke(message);
        }

#if UNITY_EDITOR
        [ContextMenu("Force Update Binding")]
        private void ForceUpdate()
        {
            UpdateBinding();
        }

        [ContextMenu("Log Binding Info")]
        private void LogBindingInfo()
        {
            Debug.Log($"[PropertyBinding] Source: {_source?.GetType().Name}.{_sourcePropertyPath} -> Target: {_target?.GetType().Name}.{_targetPropertyPath} (Mode: {_bindingMode})");
            
            if (_sourceProperty != null)
            {
                var value = _sourceProperty.GetValue(_source);
                Debug.Log($"  Source Value: {value} ({_sourceProperty.PropertyType})");
            }
            
            if (_targetProperty != null)
            {
                var value = _targetProperty.GetValue(_target);
                Debug.Log($"  Target Value: {value} ({_targetProperty.PropertyType})");
            }
        }
#endif
    }
}