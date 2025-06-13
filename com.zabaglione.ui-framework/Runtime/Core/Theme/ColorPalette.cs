using System;
using UnityEngine;

namespace jp.zabaglione.ui.core.theme
{
    /// <summary>
    /// Defines a color palette for UI theming
    /// </summary>
    [Serializable]
    public class ColorPalette
    {
        [Header("Primary Colors")]
        [SerializeField] private Color _primary = new Color(0.2f, 0.5f, 1f, 1f);
        [SerializeField] private Color _primaryLight = new Color(0.4f, 0.7f, 1f, 1f);
        [SerializeField] private Color _primaryDark = new Color(0.1f, 0.3f, 0.8f, 1f);

        [Header("Secondary Colors")]
        [SerializeField] private Color _secondary = new Color(1f, 0.5f, 0.2f, 1f);
        [SerializeField] private Color _secondaryLight = new Color(1f, 0.7f, 0.4f, 1f);
        [SerializeField] private Color _secondaryDark = new Color(0.8f, 0.3f, 0.1f, 1f);

        [Header("Semantic Colors")]
        [SerializeField] private Color _success = new Color(0.2f, 0.8f, 0.2f, 1f);
        [SerializeField] private Color _warning = new Color(1f, 0.8f, 0.2f, 1f);
        [SerializeField] private Color _danger = new Color(0.9f, 0.2f, 0.2f, 1f);
        [SerializeField] private Color _info = new Color(0.2f, 0.7f, 0.9f, 1f);

        [Header("Neutral Colors")]
        [SerializeField] private Color _background = new Color(0.95f, 0.95f, 0.95f, 1f);
        [SerializeField] private Color _surface = Color.white;
        [SerializeField] private Color _text = new Color(0.1f, 0.1f, 0.1f, 1f);
        [SerializeField] private Color _textLight = new Color(0.4f, 0.4f, 0.4f, 1f);
        [SerializeField] private Color _textOnPrimary = Color.white;
        [SerializeField] private Color _textOnSecondary = Color.white;

        [Header("UI Element Colors")]
        [SerializeField] private Color _border = new Color(0.8f, 0.8f, 0.8f, 1f);
        [SerializeField] private Color _divider = new Color(0.9f, 0.9f, 0.9f, 1f);
        [SerializeField] private Color _shadow = new Color(0f, 0f, 0f, 0.2f);
        [SerializeField] private Color _overlay = new Color(0f, 0f, 0f, 0.5f);

        [Header("State Colors")]
        [SerializeField] private Color _hover = new Color(0f, 0f, 0f, 0.05f);
        [SerializeField] private Color _pressed = new Color(0f, 0f, 0f, 0.1f);
        [SerializeField] private Color _disabled = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        [SerializeField] private Color _selected = new Color(0.2f, 0.5f, 1f, 0.2f);

        // Primary Colors
        public Color Primary => _primary;
        public Color PrimaryLight => _primaryLight;
        public Color PrimaryDark => _primaryDark;

        // Secondary Colors
        public Color Secondary => _secondary;
        public Color SecondaryLight => _secondaryLight;
        public Color SecondaryDark => _secondaryDark;

        // Semantic Colors
        public Color Success => _success;
        public Color Warning => _warning;
        public Color Danger => _danger;
        public Color Info => _info;

        // Neutral Colors
        public Color Background => _background;
        public Color Surface => _surface;
        public Color Text => _text;
        public Color TextLight => _textLight;
        public Color TextOnPrimary => _textOnPrimary;
        public Color TextOnSecondary => _textOnSecondary;

        // UI Element Colors
        public Color Border => _border;
        public Color Divider => _divider;
        public Color Shadow => _shadow;
        public Color Overlay => _overlay;

        // State Colors
        public Color Hover => _hover;
        public Color Pressed => _pressed;
        public Color Disabled => _disabled;
        public Color Selected => _selected;

        /// <summary>
        /// Creates a copy of this color palette
        /// </summary>
        public ColorPalette Clone()
        {
            return new ColorPalette
            {
                _primary = _primary,
                _primaryLight = _primaryLight,
                _primaryDark = _primaryDark,
                _secondary = _secondary,
                _secondaryLight = _secondaryLight,
                _secondaryDark = _secondaryDark,
                _success = _success,
                _warning = _warning,
                _danger = _danger,
                _info = _info,
                _background = _background,
                _surface = _surface,
                _text = _text,
                _textLight = _textLight,
                _textOnPrimary = _textOnPrimary,
                _textOnSecondary = _textOnSecondary,
                _border = _border,
                _divider = _divider,
                _shadow = _shadow,
                _overlay = _overlay,
                _hover = _hover,
                _pressed = _pressed,
                _disabled = _disabled,
                _selected = _selected
            };
        }

        /// <summary>
        /// Gets a color by variant type
        /// </summary>
        public Color GetVariantColor(ColorVariant variant)
        {
            return variant switch
            {
                ColorVariant.Primary => Primary,
                ColorVariant.PrimaryLight => PrimaryLight,
                ColorVariant.PrimaryDark => PrimaryDark,
                ColorVariant.Secondary => Secondary,
                ColorVariant.SecondaryLight => SecondaryLight,
                ColorVariant.SecondaryDark => SecondaryDark,
                ColorVariant.Success => Success,
                ColorVariant.Warning => Warning,
                ColorVariant.Danger => Danger,
                ColorVariant.Info => Info,
                _ => Primary
            };
        }

        /// <summary>
        /// Gets the appropriate text color for a given background color
        /// </summary>
        public Color GetTextColorFor(Color backgroundColor)
        {
            // Calculate perceived brightness
            float brightness = backgroundColor.r * 0.299f + backgroundColor.g * 0.587f + backgroundColor.b * 0.114f;
            return brightness > 0.5f ? _text : _textOnPrimary;
        }
    }

    /// <summary>
    /// Color variant types
    /// </summary>
    public enum ColorVariant
    {
        Primary,
        PrimaryLight,
        PrimaryDark,
        Secondary,
        SecondaryLight,
        SecondaryDark,
        Success,
        Warning,
        Danger,
        Info
    }
}