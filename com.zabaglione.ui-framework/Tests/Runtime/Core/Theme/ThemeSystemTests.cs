using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using jp.zabaglione.ui.core.theme;
using jp.zabaglione.ui.core.foundation;

namespace jp.zabaglione.ui.core.theme.tests
{
    public class ThemeSystemTests
    {
        private UIThemeData _testTheme;
        private GameObject _testObject;

        [SetUp]
        public void Setup()
        {
            _testTheme = ScriptableObject.CreateInstance<UIThemeData>();
            _testObject = new GameObject("TestObject");
        }

        [TearDown]
        public void TearDown()
        {
            if (_testTheme != null)
                Object.DestroyImmediate(_testTheme);
            if (_testObject != null)
                Object.DestroyImmediate(_testObject);
        }

        #region UIThemeData Tests

        [Test]
        public void ThemeData_ValidateColors_AllColorsSet()
        {
            Assert.IsNotNull(_testTheme.Colors);
            Assert.IsNotNull(_testTheme.Typography);
            Assert.IsNotNull(_testTheme.Layout);
            
            // Test some color properties
            Assert.IsNotNull(_testTheme.Colors.Primary);
            Assert.IsNotNull(_testTheme.Colors.Secondary);
            Assert.IsNotNull(_testTheme.Colors.Background);
        }

        [Test]
        public void ThemeData_Clone_CreatesDeepCopy()
        {
            var clone = _testTheme.Clone();
            
            Assert.IsNotNull(clone);
            Assert.AreNotSame(_testTheme, clone);
            Assert.AreNotSame(_testTheme.Colors, clone.Colors);
            Assert.AreNotSame(_testTheme.Typography, clone.Typography);
            Assert.AreNotSame(_testTheme.Layout, clone.Layout);
        }

        [Test]
        public void ThemeData_Validate_RequiresTypography()
        {
            // Create a theme without default font
            var invalidTheme = ScriptableObject.CreateInstance<UIThemeData>();
            
            // Typography validation should fail without font
            Assert.IsFalse(invalidTheme.Typography.Validate());
            
            Object.DestroyImmediate(invalidTheme);
        }

        #endregion

        #region ColorPalette Tests

        [Test]
        public void ColorPalette_GetVariantColor_ReturnsCorrectColors()
        {
            var palette = new ColorPalette();
            
            Assert.AreEqual(palette.Primary, palette.GetVariantColor(ColorVariant.Primary));
            Assert.AreEqual(palette.Secondary, palette.GetVariantColor(ColorVariant.Secondary));
            Assert.AreEqual(palette.Success, palette.GetVariantColor(ColorVariant.Success));
            Assert.AreEqual(palette.Danger, palette.GetVariantColor(ColorVariant.Danger));
        }

        [Test]
        public void ColorPalette_GetTextColorFor_ReturnsDarkForLight()
        {
            var palette = new ColorPalette();
            
            var lightBackground = Color.white;
            var textColor = palette.GetTextColorFor(lightBackground);
            
            // Should return dark text for light background
            Assert.AreEqual(palette.Text, textColor);
        }

        [Test]
        public void ColorPalette_GetTextColorFor_ReturnsLightForDark()
        {
            var palette = new ColorPalette();
            
            var darkBackground = Color.black;
            var textColor = palette.GetTextColorFor(darkBackground);
            
            // Should return light text for dark background
            Assert.AreEqual(palette.TextOnPrimary, textColor);
        }

        #endregion

        #region Typography Tests

        [Test]
        public void Typography_GetFontSize_ReturnsCorrectSizes()
        {
            var typography = new Typography();
            
            Assert.AreEqual(typography.H1Size, typography.GetFontSize(TextVariant.H1));
            Assert.AreEqual(typography.BodySize, typography.GetFontSize(TextVariant.Body));
            Assert.AreEqual(typography.ButtonSize, typography.GetFontSize(TextVariant.Button));
        }

        [Test]
        public void Typography_GetFontStyle_ReturnsCorrectStyles()
        {
            var typography = new Typography();
            
            Assert.AreEqual(typography.HeadingStyle, typography.GetFontStyle(TextVariant.H1));
            Assert.AreEqual(typography.BodyStyle, typography.GetFontStyle(TextVariant.Body));
            Assert.AreEqual(typography.ButtonStyle, typography.GetFontStyle(TextVariant.Button));
        }

        #endregion

        #region LayoutSettings Tests

        [Test]
        public void LayoutSettings_GetSpacing_ReturnsCorrectValues()
        {
            var layout = new LayoutSettings();
            
            Assert.AreEqual(layout.SpacingXS, layout.GetSpacing(SpacingSize.XS));
            Assert.AreEqual(layout.SpacingMD, layout.GetSpacing(SpacingSize.MD));
            Assert.AreEqual(layout.SpacingXL, layout.GetSpacing(SpacingSize.XL));
        }

        [Test]
        public void LayoutSettings_GetBorderRadius_ReturnsCorrectValues()
        {
            var layout = new LayoutSettings();
            
            Assert.AreEqual(0f, layout.GetBorderRadius(BorderRadiusSize.None));
            Assert.AreEqual(layout.BorderRadiusSM, layout.GetBorderRadius(BorderRadiusSize.SM));
            Assert.AreEqual(layout.BorderRadiusFull, layout.GetBorderRadius(BorderRadiusSize.Full));
        }

        [Test]
        public void LayoutSettings_SnapToGrid_SnapsCorrectly()
        {
            var layout = new LayoutSettings();
            
            Assert.AreEqual(8f, layout.SnapToGrid(7f));
            Assert.AreEqual(8f, layout.SnapToGrid(9f));
            Assert.AreEqual(16f, layout.SnapToGrid(15f));
        }

        #endregion

        #region UIThemeManager Tests

        private class TestThemeable : MonoBehaviour, IThemeable
        {
            public UIThemeData AppliedTheme { get; private set; }
            public int ApplyCount { get; private set; }

            public void ApplyTheme(UIThemeData theme)
            {
                AppliedTheme = theme;
                ApplyCount++;
            }
        }

        [UnityTest]
        public IEnumerator ThemeManager_LoadTheme_AppliesGlobally()
        {
            var themeManager = _testObject.AddComponent<UIThemeManager>();
            yield return null; // Wait for Awake

            var testComponent = new GameObject().AddComponent<TestThemeable>();
            themeManager.RegisterThemeableComponent(testComponent);

            themeManager.SetTheme(_testTheme);

            Assert.AreEqual(_testTheme, testComponent.AppliedTheme);
            Assert.AreEqual(1, testComponent.ApplyCount);

            Object.DestroyImmediate(testComponent.gameObject);
        }

        [UnityTest]
        public IEnumerator ThemeManager_SwitchTheme_UpdatesComponents()
        {
            var themeManager = _testObject.AddComponent<UIThemeManager>();
            yield return null;

            var testComponent = new GameObject().AddComponent<TestThemeable>();
            themeManager.RegisterThemeableComponent(testComponent);

            var theme1 = ScriptableObject.CreateInstance<UIThemeData>();
            var theme2 = ScriptableObject.CreateInstance<UIThemeData>();

            themeManager.SetTheme(theme1);
            Assert.AreEqual(theme1, testComponent.AppliedTheme);

            themeManager.SetTheme(theme2);
            Assert.AreEqual(theme2, testComponent.AppliedTheme);
            Assert.AreEqual(2, testComponent.ApplyCount);

            Object.DestroyImmediate(testComponent.gameObject);
            Object.DestroyImmediate(theme1);
            Object.DestroyImmediate(theme2);
        }

        [Test]
        public void ThemeManager_LoadNullTheme_UsesDefault()
        {
            var themeManager = _testObject.AddComponent<UIThemeManager>();
            
            LogAssert.Expect(LogType.Error, "[UIThemeManager] Cannot set null theme");
            themeManager.SetTheme(null);
        }

        [Test]
        public void ThemeManager_InvalidThemeData_LogsError()
        {
            var themeManager = _testObject.AddComponent<UIThemeManager>();
            var invalidTheme = ScriptableObject.CreateInstance<UIThemeData>();
            
            // This should log validation errors
            themeManager.SetTheme(invalidTheme);
            
            Object.DestroyImmediate(invalidTheme);
        }

        [UnityTest]
        public IEnumerator ThemeManager_Switch100Components_CompletesUnder100ms()
        {
            var themeManager = _testObject.AddComponent<UIThemeManager>();
            yield return null;

            // Create 100 themeable components
            var components = new TestThemeable[100];
            for (int i = 0; i < 100; i++)
            {
                components[i] = new GameObject($"TestComponent{i}").AddComponent<TestThemeable>();
                themeManager.RegisterThemeableComponent(components[i]);
            }

            var startTime = Time.realtimeSinceStartup;
            themeManager.SetTheme(_testTheme);
            var duration = (Time.realtimeSinceStartup - startTime) * 1000f; // Convert to ms

            Assert.Less(duration, 100f, "Theme switch took too long");

            // Cleanup
            foreach (var component in components)
            {
                Object.DestroyImmediate(component.gameObject);
            }
        }

        #endregion

        #region Integration Tests

        private class TestUIComponent : UIComponent
        {
            public UIThemeData LastAppliedTheme { get; private set; }

            protected override void OnApplyTheme(UIThemeData theme)
            {
                LastAppliedTheme = theme;
            }
        }

        [UnityTest]
        public IEnumerator ThemeSystem_UIComponent_IntegratesCorrectly()
        {
            var themeManager = _testObject.AddComponent<UIThemeManager>();
            yield return null;

            var uiComponent = new GameObject().AddComponent<TestUIComponent>();
            yield return null; // Wait for OnEnable

            themeManager.SetTheme(_testTheme);

            Assert.AreEqual(_testTheme, uiComponent.LastAppliedTheme);

            Object.DestroyImmediate(uiComponent.gameObject);
        }

        [Test]
        public void ThemeAssets_CreateFromMenu_GeneratesCorrectly()
        {
            // This test verifies the CreateAssetMenu attribute works
            var menuItems = typeof(UIThemeData).GetCustomAttributes(typeof(CreateAssetMenuAttribute), false);
            Assert.AreEqual(1, menuItems.Length);
            
            var menuItem = menuItems[0] as CreateAssetMenuAttribute;
            Assert.AreEqual("ZabaUI/Theme Data", menuItem.menuName);
        }

        #endregion
    }
}