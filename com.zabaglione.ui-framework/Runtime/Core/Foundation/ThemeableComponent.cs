using System;
using UnityEngine;
using jp.zabaglione.ui.core.theme;

namespace jp.zabaglione.ui.core.foundation
{
    /// <summary>
    /// Base component for UI elements that support theming
    /// </summary>
    public abstract class ThemeableComponent : MonoBehaviour, IThemeable
    {
        [SerializeField]
        private bool _autoApplyTheme = true;

        private bool _isThemeApplied = false;
        private UIThemeData _currentTheme = null;

        /// <summary>
        /// Whether to automatically apply theme changes
        /// </summary>
        public bool AutoApplyTheme
        {
            get => _autoApplyTheme;
            set => _autoApplyTheme = value;
        }

        /// <summary>
        /// Applies the current theme to this component
        /// </summary>
        public void ApplyTheme()
        {
            if (_currentTheme == null)
            {
                _currentTheme = GetCurrentTheme();
            }

            if (_currentTheme != null)
            {
                try
                {
                    OnApplyTheme(_currentTheme);
                    _isThemeApplied = true;
                    OnThemeApplied();
                }
                catch (Exception e)
                {
                    Debug.LogError($"[ThemeableComponent] Failed to apply theme: {e.Message}", this);
                }
            }
        }

        /// <summary>
        /// Forces a theme refresh
        /// </summary>
        public void RefreshTheme()
        {
            _currentTheme = GetCurrentTheme();
            ApplyTheme();
        }

        /// <summary>
        /// Called when the theme needs to be applied
        /// </summary>
        /// <param name="theme">The theme data to apply</param>
        protected abstract void OnApplyTheme(UIThemeData theme);

        /// <summary>
        /// Gets the current theme from the theme manager
        /// </summary>
        /// <returns>The current theme data</returns>
        protected virtual UIThemeData GetCurrentTheme()
        {
            var themeManager = UIThemeManager.Instance;
            return themeManager != null ? themeManager.CurrentTheme : null;
        }

        /// <summary>
        /// IThemeable implementation
        /// </summary>
        public void ApplyTheme(UIThemeData theme)
        {
            if (theme == null) return;
            
            _currentTheme = theme;
            _isThemeApplied = false;
            ApplyTheme();
        }

        /// <summary>
        /// Called after the theme has been successfully applied
        /// </summary>
        protected virtual void OnThemeApplied()
        {
            // Override in derived classes if needed
        }

        /// <summary>
        /// Marks the theme as dirty and needs reapplication
        /// </summary>
        protected void InvalidateTheme()
        {
            _isThemeApplied = false;
            if (_autoApplyTheme && isActiveAndEnabled)
            {
                ApplyTheme();
            }
        }

        protected virtual void OnEnable()
        {
            // Register with theme manager
            UIThemeManager.Instance?.RegisterThemeableComponent(this);
            
            if (_autoApplyTheme && !_isThemeApplied)
            {
                ApplyTheme();
            }
        }

        protected virtual void OnDisable()
        {
            // Unregister from theme manager
            UIThemeManager.Instance?.UnregisterThemeableComponent(this);
        }

        protected virtual void Start()
        {
            if (_autoApplyTheme && !_isThemeApplied)
            {
                ApplyTheme();
            }
        }

        protected virtual void OnValidate()
        {
            if (_autoApplyTheme && Application.isPlaying && isActiveAndEnabled)
            {
                RefreshTheme();
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// Editor-only method to preview theme in edit mode
        /// </summary>
        [ContextMenu("Preview Theme")]
        private void PreviewTheme()
        {
            RefreshTheme();
        }

        /// <summary>
        /// Editor-only method to reset theme
        /// </summary>
        [ContextMenu("Reset Theme")]
        private void ResetTheme()
        {
            _isThemeApplied = false;
            _currentTheme = null;
        }
#endif
    }
}