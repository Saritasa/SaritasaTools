﻿namespace Saritasa.Tools.PropertyChangedGenerator;

/// <summary>
/// Solution constants.
/// </summary>
internal static class Constants
{
    /// <summary>
    /// Do not notify attribute name.
    /// </summary>
    public const string DoNotNotifyAttributeName = "DoNotNotifyAttribute";

    /// <summary>
    /// Also notify attribute name.
    /// </summary>
    public const string AlsoNotifyAttributeName = "AlsoNotifyAttribute";

    /// <summary>
    /// Accessibility attribute name.
    /// </summary>
    public const string AccessibilityAttributeName = "AccessibilityAttribute";

    /// <summary>
    /// Property attribute name.
    /// </summary>
    public const string PropertyAttributeName = "PropertyAttribute";

    /// <summary>
    /// Autogenerated attributes.
    /// </summary>
    public const string Attributes = """
        namespace Saritasa.Tools.PropertyChangedGenerator
        {
            /// <summary>
            /// Ignore property changed event firing.
            /// </summary>
            [global::System.AttributeUsage(global::System.AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
            [global::System.Diagnostics.Conditional("DEBUG")]
            internal class DoNotNotifyAttribute : global::System.Attribute
            {
            }

            /// <summary>
            /// Notify other properties.
            /// </summary>
            [global::System.AttributeUsage(global::System.AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
            [global::System.Diagnostics.Conditional("DEBUG")]
            internal class AlsoNotifyAttribute : global::System.Attribute
            {
                /// <summary>
                /// Also notify property names.
                /// </summary>
                public string[] PropertyNames { get; }

                /// <summary>
                /// Constructor.
                /// </summary>
                /// <param name="propertyNames">Also notify property names.</param>
                public AlsoNotifyAttribute(params string[] propertyNames)
                {
                    PropertyNames = propertyNames;
                }
            }

            /// <summary>
            /// Specifies field accessibility.
            /// </summary>
            [global::System.AttributeUsage(global::System.AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
            [global::System.Diagnostics.Conditional("DEBUG")]
            internal class AccessibilityAttribute : global::System.Attribute
            {
                /// <summary>
                /// Constructor.
                /// </summary>
                /// <param name="getter">Specifies getter accessibility.</param>
                public AccessibilityAttribute(Getter getter)
                {
                }

                /// <summary>
                /// Constructor.
                /// </summary>
                /// <param name="setter">Specifies setter accessibility.</param>
                public AccessibilityAttribute(Setter setter)
                {
                }
            }

            /// <summary>
            /// Specifies property attribute with any parameters. <para/>
            /// It allows to specify the attribute (with any parameters) to be generated by Source Generator to matching property.
            /// <example>
            /// <para>Attribute usage</para>
            /// <code>
            /// [Property&lt;RequiredAttribute&gt;]
            /// [Property&lt;RangeAttribute&gt;(0, 10)]
            /// private int number;
            /// </code>
            /// <para>Source generator output:</para>
            /// <code>
            /// [RequiredAttribute]
            /// [RangeAttribute(0, 10)]
            /// public int Number { ... }
            /// </code>
            /// </example>
            /// </summary>
            [global::System.AttributeUsage(global::System.AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
            [global::System.Diagnostics.Conditional("DEBUG")]
            internal class PropertyAttribute<TAttribute> : global::System.Attribute
                where TAttribute : global::System.Attribute
            {
                /// <summary>
                /// Custom attribute parameters.
                /// </summary>
                public object[] Parameters { get; }

                /// <summary>
                /// Constructor.
                /// </summary>
                /// <param name="parameters">Custom attribute parameters</param>
                public PropertyAttribute(params object[] parameters)
                {
                    Parameters = parameters;
                }
            }
        }
        """;

    /// <summary>
    /// Autogenerated accessors.
    /// </summary>
    public const string Accessors = """
        namespace Saritasa.Tools.PropertyChangedGenerator
        {
            /// <summary>
            /// Specifies the accessibility of a generated property getter.
            /// </summary>
            internal enum Getter
            {
                Public = 6,
                ProtectedInternal = 5,
                Internal = 4,
                Protected = 3,
                PrivateProtected = 2,
                Private = 1,
            }

            /// <summary>
            /// Specifies the accessibility of a generated property setter.
            /// </summary>
            internal enum Setter
            {
                Public = 6,
                ProtectedInternal = 5,
                Internal = 4,
                Protected = 3,
                PrivateProtected = 2,
                Private = 1,
            }
        }
        """;
}
