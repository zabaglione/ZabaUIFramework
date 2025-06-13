using System;
using UnityEngine;

namespace jp.zabaglione.ui.core.theme
{
    /// <summary>
    /// Defines layout settings for UI theming
    /// </summary>
    [Serializable]
    public class LayoutSettings
    {
        [Header("Spacing")]
        [SerializeField] private float _spacingXS = 4f;
        [SerializeField] private float _spacingSM = 8f;
        [SerializeField] private float _spacingMD = 16f;
        [SerializeField] private float _spacingLG = 24f;
        [SerializeField] private float _spacingXL = 32f;
        [SerializeField] private float _spacingXXL = 48f;

        [Header("Padding")]
        [SerializeField] private RectOffset _buttonPadding = new RectOffset(16, 16, 8, 8);
        [SerializeField] private RectOffset _panelPadding = new RectOffset(16, 16, 16, 16);
        [SerializeField] private RectOffset _cardPadding = new RectOffset(20, 20, 20, 20);
        [SerializeField] private RectOffset _inputFieldPadding = new RectOffset(12, 12, 8, 8);

        [Header("Border Radius")]
        [SerializeField] private float _borderRadiusSM = 4f;
        [SerializeField] private float _borderRadiusMD = 8f;
        [SerializeField] private float _borderRadiusLG = 12f;
        [SerializeField] private float _borderRadiusFull = 9999f;

        [Header("Border Width")]
        [SerializeField] private float _borderWidthThin = 1f;
        [SerializeField] private float _borderWidthMedium = 2f;
        [SerializeField] private float _borderWidthThick = 4f;

        [Header("Shadow")]
        [SerializeField] private Vector2 _shadowOffsetSM = new Vector2(0, 2);
        [SerializeField] private Vector2 _shadowOffsetMD = new Vector2(0, 4);
        [SerializeField] private Vector2 _shadowOffsetLG = new Vector2(0, 8);
        [SerializeField] private float _shadowBlurSM = 4f;
        [SerializeField] private float _shadowBlurMD = 8f;
        [SerializeField] private float _shadowBlurLG = 16f;

        [Header("Component Sizes")]
        [SerializeField] private Vector2 _buttonSizeSM = new Vector2(80, 32);
        [SerializeField] private Vector2 _buttonSizeMD = new Vector2(120, 40);
        [SerializeField] private Vector2 _buttonSizeLG = new Vector2(160, 48);
        [SerializeField] private float _inputFieldHeight = 40f;
        [SerializeField] private float _iconSizeSM = 16f;
        [SerializeField] private float _iconSizeMD = 24f;
        [SerializeField] private float _iconSizeLG = 32f;

        [Header("Grid & Alignment")]
        [SerializeField] private float _gridUnit = 8f;
        [SerializeField] private int _columnCount = 12;
        [SerializeField] private float _gutterWidth = 16f;

        // Spacing
        public float SpacingXS => _spacingXS;
        public float SpacingSM => _spacingSM;
        public float SpacingMD => _spacingMD;
        public float SpacingLG => _spacingLG;
        public float SpacingXL => _spacingXL;
        public float SpacingXXL => _spacingXXL;

        // Padding
        public RectOffset ButtonPadding => _buttonPadding;
        public RectOffset PanelPadding => _panelPadding;
        public RectOffset CardPadding => _cardPadding;
        public RectOffset InputFieldPadding => _inputFieldPadding;

        // Border Radius
        public float BorderRadiusSM => _borderRadiusSM;
        public float BorderRadiusMD => _borderRadiusMD;
        public float BorderRadiusLG => _borderRadiusLG;
        public float BorderRadiusFull => _borderRadiusFull;

        // Border Width
        public float BorderWidthThin => _borderWidthThin;
        public float BorderWidthMedium => _borderWidthMedium;
        public float BorderWidthThick => _borderWidthThick;

        // Shadow
        public Vector2 ShadowOffsetSM => _shadowOffsetSM;
        public Vector2 ShadowOffsetMD => _shadowOffsetMD;
        public Vector2 ShadowOffsetLG => _shadowOffsetLG;
        public float ShadowBlurSM => _shadowBlurSM;
        public float ShadowBlurMD => _shadowBlurMD;
        public float ShadowBlurLG => _shadowBlurLG;

        // Component Sizes
        public Vector2 ButtonSizeSM => _buttonSizeSM;
        public Vector2 ButtonSizeMD => _buttonSizeMD;
        public Vector2 ButtonSizeLG => _buttonSizeLG;
        public float InputFieldHeight => _inputFieldHeight;
        public float IconSizeSM => _iconSizeSM;
        public float IconSizeMD => _iconSizeMD;
        public float IconSizeLG => _iconSizeLG;

        // Grid & Alignment
        public float GridUnit => _gridUnit;
        public int ColumnCount => _columnCount;
        public float GutterWidth => _gutterWidth;

        /// <summary>
        /// Gets spacing by size
        /// </summary>
        public float GetSpacing(SpacingSize size)
        {
            return size switch
            {
                SpacingSize.XS => SpacingXS,
                SpacingSize.SM => SpacingSM,
                SpacingSize.MD => SpacingMD,
                SpacingSize.LG => SpacingLG,
                SpacingSize.XL => SpacingXL,
                SpacingSize.XXL => SpacingXXL,
                _ => SpacingMD
            };
        }

        /// <summary>
        /// Gets border radius by size
        /// </summary>
        public float GetBorderRadius(BorderRadiusSize size)
        {
            return size switch
            {
                BorderRadiusSize.None => 0f,
                BorderRadiusSize.SM => BorderRadiusSM,
                BorderRadiusSize.MD => BorderRadiusMD,
                BorderRadiusSize.LG => BorderRadiusLG,
                BorderRadiusSize.Full => BorderRadiusFull,
                _ => BorderRadiusMD
            };
        }

        /// <summary>
        /// Gets shadow settings by size
        /// </summary>
        public (Vector2 offset, float blur) GetShadow(ShadowSize size)
        {
            return size switch
            {
                ShadowSize.None => (Vector2.zero, 0f),
                ShadowSize.SM => (ShadowOffsetSM, ShadowBlurSM),
                ShadowSize.MD => (ShadowOffsetMD, ShadowBlurMD),
                ShadowSize.LG => (ShadowOffsetLG, ShadowBlurLG),
                _ => (ShadowOffsetMD, ShadowBlurMD)
            };
        }

        /// <summary>
        /// Snaps a value to the grid
        /// </summary>
        public float SnapToGrid(float value)
        {
            return Mathf.Round(value / _gridUnit) * _gridUnit;
        }

        /// <summary>
        /// Creates a copy of this layout settings
        /// </summary>
        public LayoutSettings Clone()
        {
            return new LayoutSettings
            {
                _spacingXS = _spacingXS,
                _spacingSM = _spacingSM,
                _spacingMD = _spacingMD,
                _spacingLG = _spacingLG,
                _spacingXL = _spacingXL,
                _spacingXXL = _spacingXXL,
                _buttonPadding = new RectOffset(_buttonPadding.left, _buttonPadding.right, _buttonPadding.top, _buttonPadding.bottom),
                _panelPadding = new RectOffset(_panelPadding.left, _panelPadding.right, _panelPadding.top, _panelPadding.bottom),
                _cardPadding = new RectOffset(_cardPadding.left, _cardPadding.right, _cardPadding.top, _cardPadding.bottom),
                _inputFieldPadding = new RectOffset(_inputFieldPadding.left, _inputFieldPadding.right, _inputFieldPadding.top, _inputFieldPadding.bottom),
                _borderRadiusSM = _borderRadiusSM,
                _borderRadiusMD = _borderRadiusMD,
                _borderRadiusLG = _borderRadiusLG,
                _borderRadiusFull = _borderRadiusFull,
                _borderWidthThin = _borderWidthThin,
                _borderWidthMedium = _borderWidthMedium,
                _borderWidthThick = _borderWidthThick,
                _shadowOffsetSM = _shadowOffsetSM,
                _shadowOffsetMD = _shadowOffsetMD,
                _shadowOffsetLG = _shadowOffsetLG,
                _shadowBlurSM = _shadowBlurSM,
                _shadowBlurMD = _shadowBlurMD,
                _shadowBlurLG = _shadowBlurLG,
                _buttonSizeSM = _buttonSizeSM,
                _buttonSizeMD = _buttonSizeMD,
                _buttonSizeLG = _buttonSizeLG,
                _inputFieldHeight = _inputFieldHeight,
                _iconSizeSM = _iconSizeSM,
                _iconSizeMD = _iconSizeMD,
                _iconSizeLG = _iconSizeLG,
                _gridUnit = _gridUnit,
                _columnCount = _columnCount,
                _gutterWidth = _gutterWidth
            };
        }
    }

    /// <summary>
    /// Spacing size options
    /// </summary>
    public enum SpacingSize
    {
        XS,
        SM,
        MD,
        LG,
        XL,
        XXL
    }

    /// <summary>
    /// Border radius size options
    /// </summary>
    public enum BorderRadiusSize
    {
        None,
        SM,
        MD,
        LG,
        Full
    }

    /// <summary>
    /// Shadow size options
    /// </summary>
    public enum ShadowSize
    {
        None,
        SM,
        MD,
        LG
    }
}