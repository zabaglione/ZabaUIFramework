using UnityEngine;

namespace jp.zabaglione.ui.core.theme
{
    /// <summary>
    /// ScriptableObject that holds all theme data for the UI
    /// </summary>
    [CreateAssetMenu(fileName = "UITheme", menuName = "ZabaUI/Theme Data", order = 1)]
    public class UIThemeData : ScriptableObject
    {
        [Header("Theme Information")]
        [SerializeField] private string _themeName = "Default Theme";
        [SerializeField] private string _description = "";
        [SerializeField] private bool _isDarkTheme = false;

        [Header("Theme Components")]
        [SerializeField] private ColorPalette _colors = new ColorPalette();
        [SerializeField] private Typography _typography = new Typography();
        [SerializeField] private LayoutSettings _layout = new LayoutSettings();

        [Header("Animation Settings")]
        [SerializeField] private float _defaultAnimationDuration = 0.3f;
        [SerializeField] private AnimationCurve _defaultEaseCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        /// <summary>
        /// Gets the theme name
        /// </summary>
        public string ThemeName => _themeName;

        /// <summary>
        /// Gets the theme description
        /// </summary>
        public string Description => _description;

        /// <summary>
        /// Gets whether this is a dark theme
        /// </summary>
        public bool IsDarkTheme => _isDarkTheme;

        /// <summary>
        /// Gets the color palette
        /// </summary>
        public ColorPalette Colors => _colors;

        /// <summary>
        /// Gets the typography settings
        /// </summary>
        public Typography Typography => _typography;

        /// <summary>
        /// Gets the layout settings
        /// </summary>
        public LayoutSettings Layout => _layout;

        /// <summary>
        /// Gets the default animation duration
        /// </summary>
        public float DefaultAnimationDuration => _defaultAnimationDuration;

        /// <summary>
        /// Gets the default animation curve
        /// </summary>
        public AnimationCurve DefaultEaseCurve => _defaultEaseCurve;

        /// <summary>
        /// Validates the theme data
        /// </summary>
        public bool Validate()
        {
            if (string.IsNullOrEmpty(_themeName))
            {
                Debug.LogError($"[UIThemeData] Theme name is empty in {name}");
                return false;
            }

            if (_colors == null)
            {
                Debug.LogError($"[UIThemeData] Color palette is null in {name}");
                return false;
            }

            if (_typography == null)
            {
                Debug.LogError($"[UIThemeData] Typography is null in {name}");
                return false;
            }

            if (_layout == null)
            {
                Debug.LogError($"[UIThemeData] Layout settings is null in {name}");
                return false;
            }

            if (!_typography.Validate())
            {
                Debug.LogError($"[UIThemeData] Typography validation failed in {name}");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Creates a deep copy of this theme
        /// </summary>
        public UIThemeData Clone()
        {
            var clone = CreateInstance<UIThemeData>();
            clone._themeName = _themeName + " (Copy)";
            clone._description = _description;
            clone._isDarkTheme = _isDarkTheme;
            clone._colors = _colors?.Clone();
            clone._typography = _typography?.Clone();
            clone._layout = _layout?.Clone();
            clone._defaultAnimationDuration = _defaultAnimationDuration;
            clone._defaultEaseCurve = new AnimationCurve(_defaultEaseCurve.keys);
            return clone;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            // Ensure animation duration is positive
            _defaultAnimationDuration = Mathf.Max(0f, _defaultAnimationDuration);

            // Ensure we have at least 2 keyframes for the animation curve
            if (_defaultEaseCurve == null || _defaultEaseCurve.keys.Length < 2)
            {
                _defaultEaseCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
            }
        }

        [ContextMenu("Validate Theme")]
        private void ValidateTheme()
        {
            if (Validate())
            {
                Debug.Log($"[UIThemeData] Theme '{_themeName}' is valid!");
            }
        }

        [ContextMenu("Apply as Default Theme")]
        private void ApplyAsDefault()
        {
            var themeManager = UIThemeManager.Instance;
            if (themeManager != null)
            {
                themeManager.SetTheme(this);
                Debug.Log($"[UIThemeData] Applied '{_themeName}' as the default theme");
            }
            else
            {
                Debug.LogWarning("[UIThemeData] UIThemeManager not found in scene");
            }
        }

        [ContextMenu("Create Light Theme Preset")]
        private void CreateLightThemePreset()
        {
            _themeName = "Light Theme";
            _isDarkTheme = false;
            _colors = new ColorPalette();
            UnityEditor.EditorUtility.SetDirty(this);
        }

        [ContextMenu("Create Dark Theme Preset")]
        private void CreateDarkThemePreset()
        {
            _themeName = "Dark Theme";
            _isDarkTheme = true;
            
            // Set dark theme colors
            _colors = new ColorPalette();
            // Note: In a real implementation, we'd set dark theme appropriate colors here
            
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}