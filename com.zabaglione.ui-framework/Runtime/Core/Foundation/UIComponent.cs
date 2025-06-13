using System;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using jp.zabaglione.ui.core.theme;

namespace jp.zabaglione.ui.core.foundation
{
    /// <summary>
    /// Base class for all UI components combining animation and theme support
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public abstract class UIComponent : TweenManagerComponent, IThemeable
    {
        [Header("UI Component Settings")]
        [SerializeField]
        private bool _autoApplyTheme = true;
        
        [SerializeField]
        private bool _animateOnEnable = false;
        
        [SerializeField]
        private float _animationDuration = 0.3f;

        private RectTransform _rectTransform;
        private CanvasGroup _canvasGroup;
        private bool _isThemeApplied = false;
        private UIThemeData _currentTheme = null;
        private bool _isInitialized = false;

        /// <summary>
        /// Gets the RectTransform component
        /// </summary>
        public RectTransform RectTransform
        {
            get
            {
                if (_rectTransform == null)
                    _rectTransform = GetComponent<RectTransform>();
                return _rectTransform;
            }
        }

        /// <summary>
        /// Gets the CanvasGroup component (creates one if needed)
        /// </summary>
        public CanvasGroup CanvasGroup
        {
            get
            {
                if (_canvasGroup == null)
                {
                    _canvasGroup = GetComponent<CanvasGroup>();
                    if (_canvasGroup == null)
                        _canvasGroup = gameObject.AddComponent<CanvasGroup>();
                }
                return _canvasGroup;
            }
        }

        /// <summary>
        /// Whether to automatically apply theme changes
        /// </summary>
        public bool AutoApplyTheme
        {
            get => _autoApplyTheme;
            set => _autoApplyTheme = value;
        }

        /// <summary>
        /// Default animation duration for this component
        /// </summary>
        public float AnimationDuration
        {
            get => _animationDuration;
            set => _animationDuration = Mathf.Max(0f, value);
        }

        /// <summary>
        /// Initializes the component
        /// </summary>
        protected virtual void Initialize()
        {
            if (_isInitialized) return;
            
            OnInitialize();
            _isInitialized = true;
        }

        /// <summary>
        /// Override to add custom initialization logic
        /// </summary>
        protected virtual void OnInitialize()
        {
            // Override in derived classes
        }

        /// <summary>
        /// Shows the component with animation
        /// </summary>
        public virtual Tween Show(float? duration = null)
        {
            KillAllTweens();
            gameObject.SetActive(true);
            
            float actualDuration = duration ?? _animationDuration;
            return RegisterTween(
                CanvasGroup.DOFade(1f, actualDuration)
                    .SetEase(Ease.OutQuad)
                    .OnStart(() => OnShowStart())
                    .OnComplete(() => OnShowComplete())
            );
        }

        /// <summary>
        /// Hides the component with animation
        /// </summary>
        public virtual Tween Hide(float? duration = null)
        {
            KillAllTweens();
            
            float actualDuration = duration ?? _animationDuration;
            return RegisterTween(
                CanvasGroup.DOFade(0f, actualDuration)
                    .SetEase(Ease.InQuad)
                    .OnStart(() => OnHideStart())
                    .OnComplete(() =>
                    {
                        gameObject.SetActive(false);
                        OnHideComplete();
                    })
            );
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
                    Debug.LogError($"[UIComponent] Failed to apply theme: {e.Message}", this);
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
        protected abstract void OnApplyTheme(UIThemeData theme);

        /// <summary>
        /// Gets the current theme from the theme manager
        /// </summary>
        protected virtual UIThemeData GetCurrentTheme()
        {
            var themeManager = UIThemeManager.Instance;
            return themeManager != null ? themeManager.CurrentTheme : null;
        }

        /// <summary>
        /// IThemeable implementation
        /// </summary>
        void IThemeable.ApplyTheme(UIThemeData theme)
        {
            if (theme == null) return;
            
            _currentTheme = theme;
            _isThemeApplied = false;
            ApplyTheme();
        }

        /// <summary>
        /// Called after the theme has been successfully applied
        /// </summary>
        protected virtual void OnThemeApplied() { }

        /// <summary>
        /// Called when show animation starts
        /// </summary>
        protected virtual void OnShowStart() { }

        /// <summary>
        /// Called when show animation completes
        /// </summary>
        protected virtual void OnShowComplete() { }

        /// <summary>
        /// Called when hide animation starts
        /// </summary>
        protected virtual void OnHideStart() { }

        /// <summary>
        /// Called when hide animation completes
        /// </summary>
        protected virtual void OnHideComplete() { }

        protected virtual void Awake()
        {
            Initialize();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            // Register with theme manager
            UIThemeManager.Instance?.RegisterThemeableComponent(this);
            
            if (_autoApplyTheme && !_isThemeApplied)
            {
                ApplyTheme();
            }

            if (_animateOnEnable)
            {
                CanvasGroup.alpha = 0f;
                Show();
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
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
        [ContextMenu("Preview Theme")]
        private void PreviewTheme()
        {
            RefreshTheme();
        }

        [ContextMenu("Test Show Animation")]
        private void TestShow()
        {
            if (Application.isPlaying)
            {
                Hide(0f);
                Show();
            }
        }

        [ContextMenu("Test Hide Animation")]
        private void TestHide()
        {
            if (Application.isPlaying)
            {
                Show(0f);
                Hide();
            }
        }
#endif
    }
}