using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using jp.zabaglione.ui.core.foundation;
using jp.zabaglione.ui.mvvm.viewmodels;
using DG.Tweening;

namespace jp.zabaglione.ui.core.foundation.tests
{
    public class FoundationTests
    {
        private GameObject _testObject;

        [SetUp]
        public void Setup()
        {
            _testObject = new GameObject("TestObject");
            DOTween.Init();
        }

        [TearDown]
        public void TearDown()
        {
            if (_testObject != null)
                Object.DestroyImmediate(_testObject);
            DOTween.Clear();
        }

        #region TweenManagerComponent Tests

        private class TestTweenManager : TweenManagerComponent
        {
            public Tween CreateTestTween()
            {
                return RegisterTween(transform.DOScale(Vector3.one * 2f, 0.1f));
            }

            public void TestKillAll(bool complete = false)
            {
                KillAllTweens(complete);
            }

            public void TestPause()
            {
                PauseAllTweens();
            }

            public void TestResume()
            {
                ResumeAllTweens();
            }

            public int GetActiveCount()
            {
                return ActiveTweenCount;
            }

            public bool HasActive()
            {
                return HasActiveTweens();
            }
        }

        [Test]
        public void TweenManager_RegisterTween_AddsToActiveList()
        {
            var manager = _testObject.AddComponent<TestTweenManager>();
            var tween = manager.CreateTestTween();
            
            Assert.IsNotNull(tween);
            Assert.AreEqual(1, manager.GetActiveCount());
        }

        [UnityTest]
        public IEnumerator TweenManager_OnDestroy_KillsAllTweens()
        {
            var manager = _testObject.AddComponent<TestTweenManager>();
            var tween = manager.CreateTestTween();
            
            Assert.IsTrue(tween.IsActive());
            
            Object.DestroyImmediate(_testObject);
            yield return null;
            
            Assert.IsFalse(tween.IsActive());
        }

        [Test]
        public void TweenManager_RegisterNullTween_DoesNotThrow()
        {
            var manager = _testObject.AddComponent<TestTweenManager>();
            
            Assert.DoesNotThrow(() =>
            {
                manager.RegisterTween<Tween>(null);
            });
            
            Assert.AreEqual(0, manager.GetActiveCount());
        }

        [Test]
        public void TweenManager_DoubleDestroy_DoesNotThrow()
        {
            var manager = _testObject.AddComponent<TestTweenManager>();
            manager.CreateTestTween();
            
            Assert.DoesNotThrow(() =>
            {
                manager.TestKillAll();
                manager.TestKillAll();
            });
        }

        [UnityTest]
        public IEnumerator TweenManager_DisableEnable_PauseResume()
        {
            var manager = _testObject.AddComponent<TestTweenManager>();
            var tween = manager.CreateTestTween();
            
            Assert.IsTrue(tween.IsPlaying());
            
            _testObject.SetActive(false);
            yield return null;
            Assert.IsFalse(tween.IsPlaying());
            
            _testObject.SetActive(true);
            yield return null;
            Assert.IsTrue(tween.IsPlaying());
        }

        #endregion

        #region ThemeableComponent Tests

        private class TestThemeable : ThemeableComponent
        {
            public bool ThemeApplied { get; private set; }
            public object LastAppliedTheme { get; private set; }

            protected override void OnApplyTheme(object theme)
            {
                ThemeApplied = true;
                LastAppliedTheme = theme;
            }

            protected override object GetCurrentTheme()
            {
                return "TestTheme";
            }
        }

        [Test]
        public void ThemeableComponent_ApplyTheme_UpdatesVisuals()
        {
            var themeable = _testObject.AddComponent<TestThemeable>();
            
            themeable.ApplyTheme();
            
            Assert.IsTrue(themeable.ThemeApplied);
            Assert.AreEqual("TestTheme", themeable.LastAppliedTheme);
        }

        [Test]
        public void ThemeableComponent_NullTheme_DoesNotThrow()
        {
            var themeable = new GameObject().AddComponent<TestThemeableNull>();
            
            Assert.DoesNotThrow(() =>
            {
                themeable.ApplyTheme();
            });
            
            Object.DestroyImmediate(themeable.gameObject);
        }

        private class TestThemeableNull : ThemeableComponent
        {
            protected override void OnApplyTheme(object theme) { }
            protected override object GetCurrentTheme() => null;
        }

        #endregion

        #region ViewModelBase Tests

        private class TestViewModel : ViewModelBase
        {
            private string _testProperty;
            public string TestProperty
            {
                get => _testProperty;
                set => SetProperty(ref _testProperty, value);
            }

            public string DictionaryProperty
            {
                get => GetProperty<string>("DefaultValue");
                set => SetProperty(value);
            }
        }

        [Test]
        public void ViewModelBase_SetProperty_RaisesPropertyChanged()
        {
            var viewModel = new TestViewModel();
            bool propertyChangedRaised = false;
            string changedPropertyName = null;
            
            viewModel.PropertyChanged += (s, e) =>
            {
                propertyChangedRaised = true;
                changedPropertyName = e.PropertyName;
            };
            
            viewModel.TestProperty = "NewValue";
            
            Assert.IsTrue(propertyChangedRaised);
            Assert.AreEqual("TestProperty", changedPropertyName);
            Assert.AreEqual("NewValue", viewModel.TestProperty);
        }

        [Test]
        public void ViewModelBase_SetSameValue_DoesNotRaisePropertyChanged()
        {
            var viewModel = new TestViewModel();
            viewModel.TestProperty = "Value";
            
            bool propertyChangedRaised = false;
            viewModel.PropertyChanged += (s, e) => propertyChangedRaised = true;
            
            viewModel.TestProperty = "Value";
            
            Assert.IsFalse(propertyChangedRaised);
        }

        [Test]
        public void ViewModelBase_DictionaryProperty_WorksCorrectly()
        {
            var viewModel = new TestViewModel();
            
            Assert.AreEqual("DefaultValue", viewModel.DictionaryProperty);
            
            viewModel.DictionaryProperty = "NewValue";
            Assert.AreEqual("NewValue", viewModel.DictionaryProperty);
        }

        [Test]
        public void ViewModelBase_BusyState_WorksCorrectly()
        {
            var viewModel = new TestViewModel();
            
            Assert.IsFalse(viewModel.IsBusy);
            
            viewModel.ExecuteBusy(() =>
            {
                Assert.IsTrue(viewModel.IsBusy);
            }, "Testing");
            
            Assert.IsFalse(viewModel.IsBusy);
        }

        #endregion

        #region UIComponent Tests

        private class TestUIComponent : UIComponent
        {
            public bool ThemeApplied { get; private set; }
            
            protected override void OnApplyTheme(object theme)
            {
                ThemeApplied = true;
            }
        }

        [UnityTest]
        public IEnumerator UIComponent_ShowHide_AnimatesCorrectly()
        {
            var component = _testObject.AddComponent<TestUIComponent>();
            component.AnimationDuration = 0.1f;
            
            // Test Show
            var showTween = component.Show();
            Assert.IsNotNull(showTween);
            Assert.IsTrue(showTween.IsActive());
            
            yield return new WaitForSeconds(0.15f);
            Assert.AreEqual(1f, component.CanvasGroup.alpha);
            
            // Test Hide
            var hideTween = component.Hide();
            Assert.IsNotNull(hideTween);
            Assert.IsTrue(hideTween.IsActive());
            
            yield return new WaitForSeconds(0.15f);
            Assert.AreEqual(0f, component.CanvasGroup.alpha);
            Assert.IsFalse(_testObject.activeSelf);
        }

        [Test]
        public void UIComponent_CreatesCanvasGroup_WhenNeeded()
        {
            var component = _testObject.AddComponent<TestUIComponent>();
            
            Assert.IsNull(_testObject.GetComponent<CanvasGroup>());
            
            var canvasGroup = component.CanvasGroup;
            
            Assert.IsNotNull(canvasGroup);
            Assert.IsNotNull(_testObject.GetComponent<CanvasGroup>());
        }

        #endregion

        #region Integration Tests

        [Test]
        public void Foundation_AsPartOfPackage_LoadsCorrectly()
        {
            // Test that all foundation classes can be instantiated
            Assert.DoesNotThrow(() =>
            {
                var tweenManager = _testObject.AddComponent<TestTweenManager>();
                var themeable = _testObject.AddComponent<TestThemeable>();
                var uiComponent = _testObject.AddComponent<TestUIComponent>();
                var viewModel = new TestViewModel();
            });
        }

        #endregion
    }
}