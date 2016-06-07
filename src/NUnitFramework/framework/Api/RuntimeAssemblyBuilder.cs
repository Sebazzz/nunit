namespace NUnit.Framework.Api {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Attributes;
    using Interfaces;
    using Internal;

    /// <summary>
    /// Represents an <see cref="ITestAssemblyBuilder"/> which will instantiate the correct <see cref="ITestAssemblyBuilder"/> at runtime
    /// </summary>
    public sealed class RuntimeAssemblyBuilder : ITestAssemblyBuilder {
        static Logger log = InternalTrace.GetLogger(typeof(RuntimeAssemblyBuilder));

        /// <summary>
        /// Build a suite of tests from a provided assembly
        /// </summary>
        /// <param name="assembly">The assembly from which tests are to be built</param>
        /// <param name="options">A dictionary of options to use in building the suite</param>
        /// <returns>A TestSuite containing the tests found in the assembly</returns>
        public ITest Build(Assembly assembly, IDictionary<string, object> options) {
#if PORTABLE
            log.Debug("Loading {0}", assembly.FullName);
#else
            log.Debug("Loading {0} in AppDomain {1}", assembly.FullName, AppDomain.CurrentDomain.FriendlyName);
#endif

#if SILVERLIGHT
            string assemblyPath = AssemblyHelper.GetAssemblyName(assembly).Name;
#elif PORTABLE
            string assemblyPath = AssemblyHelper.GetAssemblyName(assembly).FullName;
#else
            string assemblyPath = AssemblyHelper.GetAssemblyPath(assembly);
#endif

            return BuildUsingInnerBuilder(assembly, assemblyPath, options);
        }

        /// <summary>
        /// Build a suite of tests given the filename of an assembly
        /// </summary>
        /// <param name="assemblyName">The filename of the assembly from which tests are to be built</param>
        /// <param name="options">A dictionary of options to use in building the suite</param>
        /// <returns>A TestSuite containing the tests found in the assembly</returns>
        public ITest Build(string assemblyName, IDictionary<string, object> options) {
#if PORTABLE
            log.Debug("Loading {0}", assemblyName);
#else
            log.Debug("Loading {0} in AppDomain {1}", assemblyName, AppDomain.CurrentDomain.FriendlyName);
#endif

            ITest test = null;

            try {
                var assembly = AssemblyHelper.Load(assemblyName);
                test = BuildUsingInnerBuilder(assembly, assemblyName, options);
            }
            catch (Exception ex) {
                var testAssembly = new TestAssembly(assemblyName);
                testAssembly.RunState = RunState.NotRunnable;
                testAssembly.Properties.Set(PropertyNames.SkipReason, ex.Message);

                test = testAssembly;
            }

            return test;
        }

        private static ITest BuildUsingInnerBuilder(Assembly assembly, string assemblyName, IDictionary<string, object> options) {
            ITestAssemblyBuilder testAssemblyBuilder;

            try {
                testAssemblyBuilder = ConstructTestAssemblyBuilder(assembly);
            }
            catch (Exception ex) {
                var testAssembly = new TestAssembly(assembly, assemblyName);
                testAssembly.RunState = RunState.NotRunnable;
                testAssembly.Properties.Set(PropertyNames.SkipReason, ex.Message);

                return testAssembly;
            }

            return testAssemblyBuilder.Build(assembly, options);
        }

        private static ITestAssemblyBuilder ConstructTestAssemblyBuilder(Assembly assembly) {
            log.Debug("Looking up ITestAssemblyBuilder for assembly");

#if PORTABLE
            var attributes = assembly.GetCustomAttributes<TestAssemblyBuilderAttribute>().ToArray();
#else
            var attributes = assembly.GetCustomAttributes(typeof(TestAssemblyBuilderAttribute), false /*unused*/);
#endif

            TestAssemblyBuilderAttribute testAssemblyBuilderAttribute;
            if (attributes.Length == 1 && (testAssemblyBuilderAttribute = attributes[0] as TestAssemblyBuilderAttribute) != null) {
                log.Debug("Constructing ITestAssemblyBuilder {0}", testAssemblyBuilderAttribute.AssemblyBuilderType);

                return (ITestAssemblyBuilder) Reflect.Construct(testAssemblyBuilderAttribute.AssemblyBuilderType);
            }

            // Fallback to default implementation
            return new DefaultTestAssemblyBuilder();
        }
    }
}