namespace NUnit.Framework.Attributes {
    using System;

    /// <summary>
    /// When declared on an assembly, specified which type should be used for discovery of tests within the assembly
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class TestAssemblyBuilderAttribute : Attribute {
        /// <summary>
        /// Gets the type that is used for discovery of tests in the assembly
        /// </summary>
        public Type AssemblyBuilderType { get; }

        /// <summary>
        /// Constructs a TestAssemblyBuilderAttribute
        /// </summary>
        /// <param name="assemblyBuilderType"></param>
        public TestAssemblyBuilderAttribute(Type assemblyBuilderType) {
            this.AssemblyBuilderType = assemblyBuilderType;
        }
    }
}