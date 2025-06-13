using System;
using System.Collections.Generic;
using UnityEngine;

namespace jp.zabaglione.ui.core.theme
{
    /// <summary>
    /// Singleton manager for UI themes
    /// </summary>
    public class UIThemeManager : MonoBehaviour
    {
        private static UIThemeManager _instance;
        private UIThemeData _currentTheme;
        private UIThemeData _defaultTheme;
        private readonly List<WeakReference> _themeableComponents = new List<WeakReference>();

        [Header("Theme Settings")]
        [SerializeField] private UIThemeData _startingTheme;
        [SerializeField] private bool _persistThemeSelection = true;
        [SerializeField] private string _themePreferenceKey = "ZabaUI.SelectedTheme";

        /// <summary>
        /// Gets the singleton instance
        /// </summary>
        public static UIThemeManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<UIThemeManager>();
                    if (_instance == null)
                    {
                        var go = new GameObject("UIThemeManager");
                        _instance = go.AddComponent<UIThemeManager>();
                        DontDestroyOnLoad(go);
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Gets the current active theme
        /// </summary>
        public UIThemeData CurrentTheme => _currentTheme ?? _defaultTheme;

        /// <summary>
        /// Event triggered when the theme changes
        /// </summary>
        public event Action<UIThemeData> OnThemeChanged;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeTheme();
        }

        private void InitializeTheme()
        {
            // Set default theme
            _defaultTheme = _startingTheme;

            // Load persisted theme if enabled
            if (_persistThemeSelection && PlayerPrefs.HasKey(_themePreferenceKey))
            {
                string themePath = PlayerPrefs.GetString(_themePreferenceKey);
                var loadedTheme = Resources.Load<UIThemeData>(themePath);
                if (loadedTheme != null)
                {
                    _currentTheme = loadedTheme;
                }
            }

            // Use starting theme if no theme is set
            if (_currentTheme == null)
            {
                _currentTheme = _startingTheme;
            }

            // Validate theme
            if (_currentTheme != null && !_currentTheme.Validate())
            {
                Debug.LogWarning($"[UIThemeManager] Current theme '{_currentTheme.ThemeName}' validation failed");
            }
        }

        /// <summary>
        /// Sets the current theme
        /// </summary>
        public void SetTheme(UIThemeData theme)
        {
            if (theme == null)
            {
                Debug.LogError("[UIThemeManager] Cannot set null theme");
                return;
            }

            if (!theme.Validate())
            {
                Debug.LogError($"[UIThemeManager] Theme '{theme.ThemeName}' validation failed");
                return;
            }

            _currentTheme = theme;

            // Persist theme selection
            if (_persistThemeSelection)
            {
                string themePath = theme.name;
                PlayerPrefs.SetString(_themePreferenceKey, themePath);
                PlayerPrefs.Save();
            }

            // Apply theme to all registered components
            ApplyThemeToAll();

            // Trigger event
            OnThemeChanged?.Invoke(_currentTheme);

            Debug.Log($"[UIThemeManager] Theme changed to '{theme.ThemeName}'");
        }

        /// <summary>
        /// Registers a themeable component
        /// </summary>
        internal void RegisterThemeableComponent(IThemeable component)
        {
            if (component == null) return;

            // Clean up dead references
            CleanupDeadReferences();

            // Add new component
            _themeableComponents.Add(new WeakReference(component));

            // Apply current theme
            if (_currentTheme != null)
            {
                component.ApplyTheme(_currentTheme);
            }
        }

        /// <summary>
        /// Unregisters a themeable component
        /// </summary>
        internal void UnregisterThemeableComponent(IThemeable component)
        {
            if (component == null) return;

            _themeableComponents.RemoveAll(wr => !wr.IsAlive || wr.Target == component);
        }

        /// <summary>
        /// Applies the current theme to all registered components
        /// </summary>
        public void ApplyThemeToAll()
        {
            if (_currentTheme == null) return;

            CleanupDeadReferences();

            foreach (var weakRef in _themeableComponents)
            {
                if (weakRef.IsAlive && weakRef.Target is IThemeable themeable)
                {
                    try
                    {
                        themeable.ApplyTheme(_currentTheme);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"[UIThemeManager] Failed to apply theme to component: {e.Message}");
                    }
                }
            }
        }

        /// <summary>
        /// Resets to the default theme
        /// </summary>
        public void ResetToDefaultTheme()
        {
            if (_defaultTheme != null)
            {
                SetTheme(_defaultTheme);
            }
        }

        /// <summary>
        /// Toggles between light and dark themes
        /// </summary>
        public void ToggleDarkMode()
        {
            // This is a simplified implementation
            // In a real scenario, you'd have separate light and dark themes
            if (_currentTheme != null && _currentTheme.IsDarkTheme)
            {
                // Find and apply light theme
                var themes = Resources.LoadAll<UIThemeData>("");
                foreach (var theme in themes)
                {
                    if (!theme.IsDarkTheme)
                    {
                        SetTheme(theme);
                        break;
                    }
                }
            }
            else
            {
                // Find and apply dark theme
                var themes = Resources.LoadAll<UIThemeData>("");
                foreach (var theme in themes)
                {
                    if (theme.IsDarkTheme)
                    {
                        SetTheme(theme);
                        break;
                    }
                }
            }
        }

        private void CleanupDeadReferences()
        {
            _themeableComponents.RemoveAll(wr => !wr.IsAlive);
        }

#if UNITY_EDITOR
        [ContextMenu("Log Registered Components")]
        private void LogRegisteredComponents()
        {
            CleanupDeadReferences();
            Debug.Log($"[UIThemeManager] {_themeableComponents.Count} components registered");
            foreach (var weakRef in _themeableComponents)
            {
                if (weakRef.IsAlive)
                {
                    Debug.Log($"  - {weakRef.Target.GetType().Name}");
                }
            }
        }

        [ContextMenu("Force Apply Theme")]
        private void ForceApplyTheme()
        {
            ApplyThemeToAll();
        }
#endif
    }

    /// <summary>
    /// Interface for themeable components
    /// </summary>
    public interface IThemeable
    {
        /// <summary>
        /// Applies the given theme to this component
        /// </summary>
        void ApplyTheme(UIThemeData theme);
    }
}