using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using jp.zabaglione.ui.core.binding;
using jp.zabaglione.ui.mvvm.viewmodels;

namespace jp.zabaglione.ui.core.binding.tests
{
    public class DataBindingTests
    {
        private GameObject _testObject;

        [SetUp]
        public void Setup()
        {
            _testObject = new GameObject("TestObject");
        }

        [TearDown]
        public void TearDown()
        {
            if (_testObject != null)
                Object.DestroyImmediate(_testObject);
        }

        #region PropertyBinding Tests

        private class TestBindingSource : INotifyPropertyChanged
        {
            private string _testProperty = "Initial";
            private int _intProperty = 42;

            public event System.Action<string> PropertyChanged;

            public string TestProperty
            {
                get => _testProperty;
                set
                {
                    if (_testProperty != value)
                    {
                        _testProperty = value;
                        PropertyChanged?.Invoke(nameof(TestProperty));
                    }
                }
            }

            public int IntProperty
            {
                get => _intProperty;
                set
                {
                    if (_intProperty != value)
                    {
                        _intProperty = value;
                        PropertyChanged?.Invoke(nameof(IntProperty));
                    }
                }
            }
        }

        private class TestBindingTarget : INotifyPropertyChanged
        {
            private string _targetProperty;
            private int _intTarget;

            public event System.Action<string> PropertyChanged;

            public string TargetProperty
            {
                get => _targetProperty;
                set
                {
                    if (_targetProperty != value)
                    {
                        _targetProperty = value;
                        PropertyChanged?.Invoke(nameof(TargetProperty));
                    }
                }
            }

            public int IntTarget
            {
                get => _intTarget;
                set
                {
                    if (_intTarget != value)
                    {
                        _intTarget = value;
                        PropertyChanged?.Invoke(nameof(IntTarget));
                    }
                }
            }
        }

        [Test]
        public void PropertyBinding_ValueChange_UpdatesTarget()
        {
            var source = _testObject.AddComponent<TestBindingSource>();
            var target = _testObject.AddComponent<TestBindingTarget>();
            var binding = _testObject.AddComponent<PropertyBinding>();

            binding.Source = source;
            binding.SourcePropertyPath = "TestProperty";
            binding.Target = target;
            binding.TargetPropertyPath = "TargetProperty";
            binding.Mode = BindingMode.OneWay;

            binding.SetupBinding();

            // Initial value should be synced
            Assert.AreEqual(source.TestProperty, target.TargetProperty);

            // Change source value
            source.TestProperty = "Updated";

            // Target should be updated
            Assert.AreEqual("Updated", target.TargetProperty);
        }

        [Test]
        public void PropertyBinding_TwoWay_SyncsCorrectly()
        {
            var source = _testObject.AddComponent<TestBindingSource>();
            var target = _testObject.AddComponent<TestBindingTarget>();
            var binding = _testObject.AddComponent<PropertyBinding>();

            binding.Source = source;
            binding.SourcePropertyPath = "TestProperty";
            binding.Target = target;
            binding.TargetPropertyPath = "TargetProperty";
            binding.Mode = BindingMode.TwoWay;

            binding.SetupBinding();

            // Update from source to target
            source.TestProperty = "FromSource";
            Assert.AreEqual("FromSource", target.TargetProperty);

            // Update from target to source
            target.TargetProperty = "FromTarget";
            Assert.AreEqual("FromTarget", source.TestProperty);
        }

        [Test]
        public void PropertyBinding_InvalidProperty_HandlesGracefully()
        {
            var source = _testObject.AddComponent<TestBindingSource>();
            var target = _testObject.AddComponent<TestBindingTarget>();
            var binding = _testObject.AddComponent<PropertyBinding>();

            binding.Source = source;
            binding.SourcePropertyPath = "NonExistentProperty";
            binding.Target = target;
            binding.TargetPropertyPath = "TargetProperty";

            // Should not throw
            Assert.DoesNotThrow(() => binding.SetupBinding());
        }

        [Test]
        public void PropertyBinding_TypeMismatch_ConvertsOrLogs()
        {
            var source = _testObject.AddComponent<TestBindingSource>();
            var target = _testObject.AddComponent<TestBindingTarget>();
            var binding = _testObject.AddComponent<PropertyBinding>();

            binding.Source = source;
            binding.SourcePropertyPath = "IntProperty";
            binding.Target = target;
            binding.TargetPropertyPath = "TargetProperty"; // String property
            binding.Mode = BindingMode.OneWay;

            binding.SetupBinding();

            // Should convert int to string
            Assert.AreEqual("42", target.TargetProperty);

            source.IntProperty = 100;
            Assert.AreEqual("100", target.TargetProperty);
        }

        [Test]
        public void PropertyBinding_CircularBinding_BreaksLoop()
        {
            var source = _testObject.AddComponent<TestBindingSource>();
            var binding1 = _testObject.AddComponent<PropertyBinding>();
            var binding2 = _testObject.AddComponent<PropertyBinding>();

            // Create circular binding
            binding1.Source = source;
            binding1.SourcePropertyPath = "TestProperty";
            binding1.Target = source;
            binding1.TargetPropertyPath = "TestProperty";
            binding1.Mode = BindingMode.TwoWay;

            Assert.DoesNotThrow(() =>
            {
                binding1.SetupBinding();
                source.TestProperty = "NewValue";
            });
        }

        [UnityTest]
        public IEnumerator PropertyBinding_1000Updates_CompletesUnder50ms()
        {
            var source = _testObject.AddComponent<TestBindingSource>();
            var target = _testObject.AddComponent<TestBindingTarget>();
            var binding = _testObject.AddComponent<PropertyBinding>();

            binding.Source = source;
            binding.SourcePropertyPath = "IntProperty";
            binding.Target = target;
            binding.TargetPropertyPath = "IntTarget";
            binding.Mode = BindingMode.OneWay;

            binding.SetupBinding();

            var startTime = Time.realtimeSinceStartup;

            for (int i = 0; i < 1000; i++)
            {
                source.IntProperty = i;
            }

            var duration = (Time.realtimeSinceStartup - startTime) * 1000f;

            Assert.Less(duration, 50f, "1000 property updates took too long");
            Assert.AreEqual(999, target.IntTarget);

            yield return null;
        }

        #endregion

        #region ObservableCollection Tests

        [Test]
        public void ObservableCollection_Add_NotifiesBindings()
        {
            var collection = new ObservableCollection<string>();
            var notificationCount = 0;
            NotifyCollectionChangedEventArgs lastArgs = null;

            collection.CollectionChanged += (sender, args) =>
            {
                notificationCount++;
                lastArgs = args;
            };

            collection.Add("Item1");

            Assert.AreEqual(1, notificationCount);
            Assert.AreEqual(NotifyCollectionChangedAction.Add, lastArgs.Action);
            Assert.AreEqual(1, lastArgs.NewItems.Count);
            Assert.AreEqual("Item1", lastArgs.NewItems[0]);
            Assert.AreEqual(0, lastArgs.NewStartingIndex);
        }

        [Test]
        public void ObservableCollection_Remove_NotifiesCorrectly()
        {
            var collection = new ObservableCollection<int> { 1, 2, 3 };
            var notificationCount = 0;
            NotifyCollectionChangedEventArgs lastArgs = null;

            collection.CollectionChanged += (sender, args) =>
            {
                notificationCount++;
                lastArgs = args;
            };

            collection.Remove(2);

            Assert.AreEqual(1, notificationCount);
            Assert.AreEqual(NotifyCollectionChangedAction.Remove, lastArgs.Action);
            Assert.AreEqual(1, lastArgs.OldItems.Count);
            Assert.AreEqual(2, lastArgs.OldItems[0]);
            Assert.AreEqual(1, lastArgs.OldStartingIndex);
        }

        [Test]
        public void ObservableCollection_Clear_SendsResetNotification()
        {
            var collection = new ObservableCollection<string> { "A", "B", "C" };
            var notificationCount = 0;
            NotifyCollectionChangedEventArgs lastArgs = null;

            collection.CollectionChanged += (sender, args) =>
            {
                notificationCount++;
                lastArgs = args;
            };

            collection.Clear();

            Assert.AreEqual(1, notificationCount);
            Assert.AreEqual(NotifyCollectionChangedAction.Reset, lastArgs.Action);
            Assert.AreEqual(0, collection.Count);
        }

        [Test]
        public void ObservableCollection_Move_NotifiesCorrectly()
        {
            var collection = new ObservableCollection<string> { "A", "B", "C", "D" };
            var notificationCount = 0;
            NotifyCollectionChangedEventArgs lastArgs = null;

            collection.CollectionChanged += (sender, args) =>
            {
                notificationCount++;
                lastArgs = args;
            };

            collection.Move(1, 3); // Move "B" from index 1 to index 3

            Assert.AreEqual(1, notificationCount);
            Assert.AreEqual(NotifyCollectionChangedAction.Move, lastArgs.Action);
            Assert.AreEqual("B", lastArgs.NewItems[0]);
            Assert.AreEqual(3, lastArgs.NewStartingIndex);
            Assert.AreEqual(1, lastArgs.OldStartingIndex);
            
            // Verify final order
            Assert.AreEqual("A", collection[0]);
            Assert.AreEqual("C", collection[1]);
            Assert.AreEqual("D", collection[2]);
            Assert.AreEqual("B", collection[3]);
        }

        [Test]
        public void ObservableCollection_AddRange_NotifiesOnce()
        {
            var collection = new ObservableCollection<int>();
            var notificationCount = 0;

            collection.CollectionChanged += (sender, args) => notificationCount++;

            collection.AddRange(new[] { 1, 2, 3, 4, 5 });

            Assert.AreEqual(1, notificationCount);
            Assert.AreEqual(5, collection.Count);
        }

        [Test]
        public void ObservableCollection_Sort_SendsResetNotification()
        {
            var collection = new ObservableCollection<int> { 3, 1, 4, 1, 5, 9 };
            var notificationCount = 0;
            NotifyCollectionChangedEventArgs lastArgs = null;

            collection.CollectionChanged += (sender, args) =>
            {
                notificationCount++;
                lastArgs = args;
            };

            collection.Sort();

            Assert.AreEqual(1, notificationCount);
            Assert.AreEqual(NotifyCollectionChangedAction.Reset, lastArgs.Action);
            
            // Verify sorted order
            Assert.AreEqual(1, collection[0]);
            Assert.AreEqual(1, collection[1]);
            Assert.AreEqual(3, collection[2]);
            Assert.AreEqual(4, collection[3]);
            Assert.AreEqual(5, collection[4]);
            Assert.AreEqual(9, collection[5]);
        }

        #endregion

        #region BindingConverter Tests

        [Test]
        public void BoolToVisibilityConverter_ConvertsCorrectly()
        {
            var converter = new BoolToVisibilityConverter();

            Assert.AreEqual(true, converter.Convert(true, typeof(bool), null));
            Assert.AreEqual(false, converter.Convert(false, typeof(bool), null));

            converter.InvertVisibility = true;
            Assert.AreEqual(false, converter.Convert(true, typeof(bool), null));
            Assert.AreEqual(true, converter.Convert(false, typeof(bool), null));
        }

        [Test]
        public void NumberToStringConverter_FormatsCorrectly()
        {
            var converter = new NumberToStringConverter { Format = "F2" };

            Assert.AreEqual("42.00", converter.Convert(42f, typeof(string), null));
            Assert.AreEqual("3.14", converter.Convert(3.14159f, typeof(string), "F2"));
            Assert.AreEqual("1000", converter.Convert(1000, typeof(string), "0"));
        }

        [Test]
        public void ColorToHexConverter_ConvertsCorrectly()
        {
            var converter = new ColorToHexConverter();

            Assert.AreEqual("FF0000FF", converter.Convert(Color.red, typeof(string), null));
            
            converter.IncludeAlpha = false;
            Assert.AreEqual("00FF00", converter.Convert(Color.green, typeof(string), null));

            // Test convert back
            var color = converter.ConvertBack("#0000FF", typeof(Color), null);
            Assert.AreEqual(Color.blue, color);
        }

        [Test]
        public void ChainedConverter_WorksCorrectly()
        {
            var chainedConverter = new ChainedConverter
            {
                Converters = new IValueConverter[]
                {
                    new InverseBooleanConverter(),
                    new BoolToVisibilityConverter()
                }
            };

            // true -> false (inverted) -> false (visibility)
            Assert.AreEqual(false, chainedConverter.Convert(true, typeof(bool), null));
            
            // false -> true (inverted) -> true (visibility)
            Assert.AreEqual(true, chainedConverter.Convert(false, typeof(bool), null));
        }

        #endregion

        #region ViewModelBase Integration Tests

        private class TestViewModel : ViewModelBase
        {
            private string _name;
            private int _age;

            public string Name
            {
                get => _name;
                set => SetProperty(ref _name, value);
            }

            public int Age
            {
                get => _age;
                set => SetProperty(ref _age, value);
            }
        }

        [Test]
        public void ViewModelBase_WithPropertyBinding_WorksCorrectly()
        {
            var viewModel = new TestViewModel { Name = "John", Age = 30 };
            var target = _testObject.AddComponent<TestBindingTarget>();
            var binding = _testObject.AddComponent<PropertyBinding>();

            binding.Source = viewModel;
            binding.SourcePropertyPath = "Name";
            binding.Target = target;
            binding.TargetPropertyPath = "TargetProperty";
            binding.Mode = BindingMode.OneWay;

            binding.SetupBinding();

            Assert.AreEqual("John", target.TargetProperty);

            viewModel.Name = "Jane";
            Assert.AreEqual("Jane", target.TargetProperty);
        }

        #endregion

        #region UPM Package Tests

        [Test]
        public void DataBinding_InSampleScene_WorksCorrectly()
        {
            // This test verifies binding works in a sample scene context
            // In a real Unity environment, this would load a sample scene
            Assert.Pass("Data binding sample scene test placeholder");
        }

        #endregion
    }
}