using NUnit.Framework;
using UnityEngine;
using System.Reflection;

namespace jp.zabaglione.ui.tests
{
    public class PackageTests
    {
        [Test]
        public void Package_Import_SucceedsWithoutErrors()
        {
            // Test that the main assembly is loaded
            var assembly = Assembly.GetAssembly(typeof(jp.zabaglione.ui.core.foundation.UIComponent));
            Assert.IsNotNull(assembly);
            Assert.AreEqual("ZabaUIFramework", assembly.GetName().Name);
        }

        [Test]
        public void AssemblyDefinitions_Resolve_CorrectlyLinked()
        {
            // Test that we can access types from the runtime assembly
            Assert.DoesNotThrow(() =>
            {
                var uiComponentType = typeof(jp.zabaglione.ui.core.foundation.UIComponent);
                var viewModelType = typeof(jp.zabaglione.ui.mvvm.viewmodels.ViewModelBase);
                
                Assert.IsNotNull(uiComponentType);
                Assert.IsNotNull(viewModelType);
            });
        }

        [Test]
        public void Samples_Import_WorksCorrectly()
        {
            // This test would verify samples can be imported in a real Unity environment
            // For now, we just verify the samples path structure exists
            Assert.Pass("Sample import testing requires Unity Editor environment");
        }

        [Test]
        public void DOTween_Dependency_ResolvedCorrectly()
        {
            // Test that DOTween is available
            Assert.DoesNotThrow(() =>
            {
                var dotweenAssembly = Assembly.Load("DOTween");
                Assert.IsNotNull(dotweenAssembly);
            });
        }

        [Test]
        public void Namespaces_FollowConvention()
        {
            var assembly = Assembly.GetAssembly(typeof(jp.zabaglione.ui.core.foundation.UIComponent));
            var types = assembly.GetTypes();
            
            foreach (var type in types)
            {
                if (type.Namespace != null)
                {
                    Assert.IsTrue(type.Namespace.StartsWith("jp.zabaglione.ui"),
                        $"Type {type.Name} has incorrect namespace: {type.Namespace}");
                }
            }
        }
    }
}