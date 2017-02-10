using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VSCoverageViewer.Models;

namespace VSCoverageViewer
{
    /// <summary>
    /// Helper for reading metadata from an assembly.
    /// </summary>
    /// <devdoc>
    /// Nearly all of this is guesswork, unfortunately.  A lot of it is inferred from IL
    /// DASM and how it encodes/displays information.
    /// 
    /// This information does appear to exist in ECMA-335 (partition II).  However, I
    /// see some discrepancies between that and IL DASM - notably how it shows unsigned
    /// types (ECMA: "unsigned int16", ILDASM: "uint16").
    /// 
    /// For the bits that I *can* map directly to the ECMA standard, I have indicated as
    /// such in the comments preceding the relevant code.
    /// </devdoc>
    internal sealed class MetadataHelper
    {
        private const BindingFlags LoadFlags = BindingFlags.Instance | BindingFlags.Static |
                                               BindingFlags.Public | BindingFlags.NonPublic |
                                               BindingFlags.DeclaredOnly;

        // Good references (IL DASM): System.Convert, System.Runtime.InteropServices.Marshal
        // TODO (metadata): verify how unsigned types are named. ECMA-335 suggests differently.
        private static readonly IReadOnlyDictionary<Type, string> IlSpecialNames =
            new Dictionary<Type, string>
            {
                [typeof(string)]         = "string",
                [typeof(object)]         = "object",
                [typeof(bool)]           = "bool",
                [typeof(float)]          = "float32",
                [typeof(double)]         = "float64",
                [typeof(byte)]           = "uint8",
                [typeof(sbyte)]          = "int8",
                [typeof(ushort)]         = "uint16",
                [typeof(short)]          = "int16",
                [typeof(uint)]           = "uint32",
                [typeof(int)]            = "int32",
                [typeof(ulong)]          = "uint64",
                [typeof(long)]           = "int64",
                [typeof(IntPtr)]         = "native int",
                [typeof(UIntPtr)]        = "native uint",
                [typeof(TypedReference)] = "typedref",
            };

        // really just the keyword names from C#...
        private static readonly IReadOnlyDictionary<Type, string> FriendlyNames =
            new Dictionary<Type, string>
            {
                [typeof(string)]  = "string",
                [typeof(object)]  = "object",
                [typeof(bool)]    = "bool",
                [typeof(float)]   = "float",
                [typeof(double)]  = "double",
                [typeof(byte)]    = "byte",
                [typeof(sbyte)]   = "sbyte",
                [typeof(ushort)]  = "ushort",
                [typeof(short)]   = "short",
                [typeof(uint)]    = "uint",
                [typeof(int)]     = "int",
                [typeof(ulong)]   = "ulong",
                [typeof(long)]    = "long",
                [typeof(decimal)] = "decimal",
                [typeof(void)]    = "void",
            };


        private readonly Assembly _asm;


        /// <summary>
        /// Initializes a new instance of <see cref="MetadataHelper"/>.
        /// </summary>
        /// <param name="assemblyPath">Path to the assembly to read.</param>
        public MetadataHelper(string assemblyPath)
        {
            // TODO (metadata): is it worthwhile to load using CCI/Mono.Cecil to avoid needing to be able to run the assembly?
            _asm = Assembly.LoadFrom(assemblyPath);
        }



        /// <summary>
        /// Loads the metadata for the given model.
        /// </summary>
        /// <param name="model">The model to read into.</param>
        public void LoadMetadataFor(CoverageNodeModel model)
        {
            if (!model.HasReadMetadata)
            {
                switch (model.NodeType)
                {
                    case CoverageNodeType.CoverageFile:
                    case CoverageNodeType.Module:
                    case CoverageNodeType.Namespace:
                        // nothing to simplify
                        break;

                    case CoverageNodeType.Type:
                        LoadTypeMetadata(model);
                        break;

                    case CoverageNodeType.Function:
                        {
                            // read the metadata for the declaring type.
                            var cls = model.ClosestAncestor(CoverageNodeType.Type);

                            if (cls != null)
                            {
                                LoadMetadataFor(cls);
                            }
                            break;
                        }

                    default:
                        throw Utility.UnreachableCode("Unexpected coverage node type.");
                }

                foreach (var child in model.Children)
                {
                    LoadMetadataFor(child);
                }
            }
        }


        /// <summary>
        /// Loads a class's metadata.
        /// </summary>
        /// <param name="classNode">The class node to read into.</param>
        private void LoadTypeMetadata(CoverageNodeModel classNode)
        {
            Debug.Assert(classNode != null);
            Debug.Assert(classNode.NodeType == CoverageNodeType.Type);

            Module mod = GetModule(classNode);

            string lookupName = GetNamespaceQualifiedName(classNode);
            Type type = mod.GetType(lookupName);

            string className = GetCleanTypeName(type);
            classNode.Name = className;

            foreach (var ctor in type.GetConstructors(LoadFlags))
            {
                // ".cctor", ".ctor" - see: ECMA-335, II.10.5 Special Members
                string coverageName =
                    (ctor.IsStatic ? ".cctor" : ".ctor") +
                    GetCoverageParameterString(ctor);

                var node = classNode.Children.FirstOrDefault(m => m.Name == coverageName);

                if (node != null)
                {
                    LoadMethodMetadata(node, type, ctor);
                }
            }

            foreach (var method in type.GetMethods(LoadFlags))
            {
                string coverageName = method.Name + GetCoverageParameterString(method);

                var node = classNode.Children.FirstOrDefault(m => m.Name == coverageName);

                if (node != null)
                {
                    LoadMethodMetadata(node, type, method);
                }
            }

            if (type.IsValueType)
            {
                classNode.CodeType = CodeElementType.Struct;
            }

            classNode.HasReadMetadata = true;
        }

        /// <summary>
        /// Loads a method's metadata.
        /// </summary>
        /// <param name="methodNode">The method node to read into.</param>
        /// <param name="declaringType">The method's declaring type.</param>
        /// <param name="methodInfo">The method info.</param>
        private void LoadMethodMetadata(CoverageNodeModel methodNode, Type declaringType, MethodBase methodInfo)
        {
            Debug.Assert(methodNode != null);
            Debug.Assert(methodNode.NodeType == CoverageNodeType.Function);
            Debug.Assert(methodNode.Parent != null);
            Debug.Assert(methodNode.Parent.NodeType == CoverageNodeType.Type);

            Module mod = GetModule(methodNode);

            methodNode.Name = GetCleanMethodName(methodInfo);

            if (methodInfo.IsSpecialName)
            {
                foreach (var prop in declaringType.GetProperties(LoadFlags))
                {
                    if (methodInfo.Equals(prop.GetMethod) ||
                        methodInfo.Equals(prop.SetMethod))
                    {
                        methodNode.CodeType = CodeElementType.Property;
                        break;
                    }
                }
            }

            methodNode.HasReadMetadata = true;
        }

        /// <summary>
        /// Gets the module containing the code for the coverage node.
        /// </summary>
        /// <param name="node">The node to get the module for.</param>
        /// <returns>The module for the code, or null if the module is not found.</returns>
        private Module GetModule(CoverageNodeModel node)
        {
            node = node?.ClosestAncestor(CoverageNodeType.Module);

            if (node != null)
            {
                return _asm.GetModule(node.Name);
            }

            return null;
        }


        /// <summary>
        /// Gets the cleaned (user-friendly) name of the given type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The name of the type.</returns>
        private static string GetCleanTypeName(Type type)
        {
            string name;

            // do we have a friendly (or keyword) name?
            if (FriendlyNames.TryGetValue(type, out name))
            {
                return name;
            }

            var builder = new StringBuilder();


            if (type.DeclaringType != null &&
                !type.IsGenericParameter)
            {
                // The type is nested in another type
                builder.Append(GetCleanTypeName(type.DeclaringType));
                builder.Append("+");
            }

            if (type.IsGenericType)
            {
                // The type has generic arguments
                if (type.IsValueType &&
                    type.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    // type is a nullable struct, so "Type?".
                    builder.Append(GetCleanTypeName(Nullable.GetUnderlyingType(type)));
                    builder.Append("?");
                }
                else
                {
                    string baseName = type.Name;
                    builder.Append(baseName.Substring(0, baseName.IndexOf('`')));

                    builder.Append("<");

                    foreach (var t in type.GetGenericArguments())
                    {
                        builder.Append(GetCleanTypeName(t));
                        builder.Append(", ");
                    }

                    // remove the extra ", " from the end.
                    builder.Length -= 2;

                    builder.Append(">");
                }
            }
            else if (type.IsGenericParameter)
            {
                // the type _is_ a generic argument
                // This catches (for instance) the "TKey" and "TValue" from Dictionary<TKey, TValue>
                builder.Append(type.Name);
            }
            else
            {
                // just a plain type.
                builder.Append(type.Name);
            }

            Debug.Assert(name == null, "use the builder");
            return builder.ToString();
        }

        /// <summary>
        /// Gets the cleaned (user-friendly) name of the given method or constructor.
        /// </summary>
        /// <param name="method">The method or constructor to get the name of.</param>
        /// <returns>The name of the method or constructor.</returns>
        private static string GetCleanMethodName(MethodBase method)
        {
            StringBuilder sb = new StringBuilder();

            if (method is MethodInfo)
            {
                sb.Append(GetCleanTypeName((method as MethodInfo).ReturnType));
                sb.Append(' ');

                sb.Append(method.Name);
            }
            else if (method is ConstructorInfo)
            {
                sb.Append(GetCleanTypeName(method.DeclaringType));
            }

            if (method.IsGenericMethod)
            {
                var genArgs = method.GetGenericArguments();

                sb.Append('<');

                sb.Append(string.Join(", ", genArgs.Select(GetCleanTypeName)));

                sb.Append('>');
            }


            var parameters = method.GetParameters();
            sb.Append('(');

            sb.Append(string.Join(", ", parameters.Select(p =>
                string.Concat(GetCleanTypeName(p.ParameterType), " ", p.Name))));

            sb.Append(')');

            return sb.ToString();
        }


        /// <summary>
        /// Gets the parameter string for the given method as it would appear in the coverage file.
        /// </summary>
        /// <param name="method">The method to get the parameter string for.</param>
        /// <returns>
        /// A string containing the parameter declarations, including the surrounding parentheses.
        /// </returns>
        private static string GetCoverageParameterString(MethodBase method)
        {
            return "(" +
                string.Join(",",
                    method.GetParameters().Select(p => GetCoverageParameterTypeString(p.ParameterType))
                ) +
                ")";
        }

        /// <summary>
        /// Gets the name of a type as it would appear in the signature of a method in the coverage file.
        /// </summary>
        /// <param name="type">The type to get the coverage name for.</param>
        /// <returns>A string containing the parameter type.</returns>
        private static string GetCoverageParameterTypeString(Type type)
        {
            string name;

            if (type.IsGenericParameter)
            {
                // "!#", "!!#" - see: ECMA-335, II.9.4 Instantiating generic types
                //
                // note: DeclaringType seems to always be non-null, and gets the method's
                // declaring type when the generic arg is defined by the method.
                // As such, order of these checks matter!
                if (type.DeclaringMethod != null)
                {
                    Type[] genericArgs = type.DeclaringMethod.GetGenericArguments();
                    int index = Array.IndexOf(genericArgs, type);

                    name = "!!" + index;
                }
                else //if (type.DeclaringType != null)
                {
                    Type[] genericArgs = type.DeclaringType.GetGenericArguments();
                    int index = Array.IndexOf(genericArgs, type);

                    name = "!" + index;
                }
            }
            else if (IlSpecialNames.TryGetValue(type, out name))
            {
                // all good.
            }
            else if (type.IsClass || type.IsInterface)
            {
                // note: interfaces are also "class"es to IL
                name = "class " + type.FullName;
            }
            else if (type.IsValueType)
            {
                name = "valuetype " + type.FullName;
            }

            // TODO (metadata): verify ordering of these suffixes
            if (type.IsArray)
            {
                //TODO (metadata): verify array syntax.
                int rank = type.GetArrayRank();
                name += "[" + new string(',', rank) + "]";
            }

            // check if this is a ref or out parameter
            if (type.IsByRef)
            {
                name += '&';
            }
            else if (type.IsPointer)
            {
                name += '*';
            }

            return name;
        }


        /// <summary>
        /// Gets the namespace-qualified name of the given coverage node.
        /// </summary>
        /// <param name="model">The coverage node to get the qualified name of.</param>
        /// <returns>The name of the coverage node, up to and including namespaces.</returns>
        private static string GetNamespaceQualifiedName(CoverageNodeModel model)
        {
            var parts = new Stack<string>();

            while (model != null &&
                   model.NodeType != CoverageNodeType.CoverageFile &&
                   model.NodeType != CoverageNodeType.Module)
            {
                if (model.NodeType != CoverageNodeType.Type)
                {
                    parts.Push(model.Name);
                }
                else
                {
                    // replace '.' in nested class names with '+'s
                    parts.Push(model.Name.Replace('.', '+'));
                }

                model = model.Parent;
            }

            return string.Join(".", parts);
        }
    }
}
