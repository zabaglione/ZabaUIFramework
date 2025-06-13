using System;
using UnityEngine;

namespace jp.zabaglione.ui.core.theme
{
    /// <summary>
    /// Defines typography settings for UI theming
    /// </summary>
    [Serializable]
    public class Typography
    {
        [Header("Font Assets")]
        [SerializeField] private Font _defaultFont;
        [SerializeField] private Font _headingFont;
        [SerializeField] private Font _monospacedFont;

        [Header("Font Sizes")]
        [SerializeField] private int _h1Size = 32;
        [SerializeField] private int _h2Size = 28;
        [SerializeField] private int _h3Size = 24;
        [SerializeField] private int _h4Size = 20;
        [SerializeField] private int _h5Size = 18;
        [SerializeField] private int _h6Size = 16;
        [SerializeField] private int _bodySize = 14;
        [SerializeField] private int _captionSize = 12;
        [SerializeField] private int _buttonSize = 14;

        [Header("Font Weights")]
        [SerializeField] private FontStyle _headingStyle = FontStyle.Bold;
        [SerializeField] private FontStyle _bodyStyle = FontStyle.Normal;
        [SerializeField] private FontStyle _buttonStyle = FontStyle.Bold;

        [Header("Line Heights")]
        [SerializeField] private float _headingLineHeight = 1.2f;
        [SerializeField] private float _bodyLineHeight = 1.5f;

        [Header("Letter Spacing")]
        [SerializeField] private float _headingLetterSpacing = 0f;
        [SerializeField] private float _bodyLetterSpacing = 0f;
        [SerializeField] private float _buttonLetterSpacing = 0.5f;

        // Font Assets
        public Font DefaultFont => _defaultFont;
        public Font HeadingFont => _headingFont ?? _defaultFont;
        public Font MonospacedFont => _monospacedFont ?? _defaultFont;

        // Font Sizes
        public int H1Size => _h1Size;
        public int H2Size => _h2Size;
        public int H3Size => _h3Size;
        public int H4Size => _h4Size;
        public int H5Size => _h5Size;
        public int H6Size => _h6Size;
        public int BodySize => _bodySize;
        public int CaptionSize => _captionSize;
        public int ButtonSize => _buttonSize;

        // Font Styles
        public FontStyle HeadingStyle => _headingStyle;
        public FontStyle BodyStyle => _bodyStyle;
        public FontStyle ButtonStyle => _buttonStyle;

        // Line Heights
        public float HeadingLineHeight => _headingLineHeight;
        public float BodyLineHeight => _bodyLineHeight;

        // Letter Spacing
        public float HeadingLetterSpacing => _headingLetterSpacing;
        public float BodyLetterSpacing => _bodyLetterSpacing;
        public float ButtonLetterSpacing => _buttonLetterSpacing;

        /// <summary>
        /// Gets font size by text variant
        /// </summary>
        public int GetFontSize(TextVariant variant)
        {
            return variant switch
            {
                TextVariant.H1 => H1Size,
                TextVariant.H2 => H2Size,
                TextVariant.H3 => H3Size,
                TextVariant.H4 => H4Size,
                TextVariant.H5 => H5Size,
                TextVariant.H6 => H6Size,
                TextVariant.Body => BodySize,
                TextVariant.Caption => CaptionSize,
                TextVariant.Button => ButtonSize,
                _ => BodySize
            };
        }

        /// <summary>
        /// Gets font style by text variant
        /// </summary>
        public FontStyle GetFontStyle(TextVariant variant)
        {
            return variant switch
            {
                TextVariant.H1 => HeadingStyle,
                TextVariant.H2 => HeadingStyle,
                TextVariant.H3 => HeadingStyle,
                TextVariant.H4 => HeadingStyle,
                TextVariant.H5 => HeadingStyle,
                TextVariant.H6 => HeadingStyle,
                TextVariant.Body => BodyStyle,
                TextVariant.Caption => BodyStyle,
                TextVariant.Button => ButtonStyle,
                _ => BodyStyle
            };
        }

        /// <summary>
        /// Gets font by text variant
        /// </summary>
        public Font GetFont(TextVariant variant)
        {
            return variant switch
            {
                TextVariant.H1 => HeadingFont,
                TextVariant.H2 => HeadingFont,
                TextVariant.H3 => HeadingFont,
                TextVariant.H4 => HeadingFont,
                TextVariant.H5 => HeadingFont,
                TextVariant.H6 => HeadingFont,
                _ => DefaultFont
            };
        }

        /// <summary>
        /// Gets line height by text variant
        /// </summary>
        public float GetLineHeight(TextVariant variant)
        {
            return variant switch
            {
                TextVariant.H1 => HeadingLineHeight,
                TextVariant.H2 => HeadingLineHeight,
                TextVariant.H3 => HeadingLineHeight,
                TextVariant.H4 => HeadingLineHeight,
                TextVariant.H5 => HeadingLineHeight,
                TextVariant.H6 => HeadingLineHeight,
                _ => BodyLineHeight
            };
        }

        /// <summary>
        /// Gets letter spacing by text variant
        /// </summary>
        public float GetLetterSpacing(TextVariant variant)
        {
            return variant switch
            {
                TextVariant.H1 => HeadingLetterSpacing,
                TextVariant.H2 => HeadingLetterSpacing,
                TextVariant.H3 => HeadingLetterSpacing,
                TextVariant.H4 => HeadingLetterSpacing,
                TextVariant.H5 => HeadingLetterSpacing,
                TextVariant.H6 => HeadingLetterSpacing,
                TextVariant.Button => ButtonLetterSpacing,
                _ => BodyLetterSpacing
            };
        }

        /// <summary>
        /// Creates a copy of this typography settings
        /// </summary>
        public Typography Clone()
        {
            return new Typography
            {
                _defaultFont = _defaultFont,
                _headingFont = _headingFont,
                _monospacedFont = _monospacedFont,
                _h1Size = _h1Size,
                _h2Size = _h2Size,
                _h3Size = _h3Size,
                _h4Size = _h4Size,
                _h5Size = _h5Size,
                _h6Size = _h6Size,
                _bodySize = _bodySize,
                _captionSize = _captionSize,
                _buttonSize = _buttonSize,
                _headingStyle = _headingStyle,
                _bodyStyle = _bodyStyle,
                _buttonStyle = _buttonStyle,
                _headingLineHeight = _headingLineHeight,
                _bodyLineHeight = _bodyLineHeight,
                _headingLetterSpacing = _headingLetterSpacing,
                _bodyLetterSpacing = _bodyLetterSpacing,
                _buttonLetterSpacing = _buttonLetterSpacing
            };
        }

        /// <summary>
        /// Validates the typography settings
        /// </summary>
        public bool Validate()
        {
            if (_defaultFont == null)
            {
                Debug.LogError("[Typography] Default font is not set!");
                return false;
            }

            if (_h1Size <= 0 || _h2Size <= 0 || _h3Size <= 0 || 
                _h4Size <= 0 || _h5Size <= 0 || _h6Size <= 0 || 
                _bodySize <= 0 || _captionSize <= 0 || _buttonSize <= 0)
            {
                Debug.LogError("[Typography] Font sizes must be greater than 0!");
                return false;
            }

            return true;
        }
    }

    /// <summary>
    /// Text variant types
    /// </summary>
    public enum TextVariant
    {
        H1,
        H2,
        H3,
        H4,
        H5,
        H6,
        Body,
        Caption,
        Button
    }
}