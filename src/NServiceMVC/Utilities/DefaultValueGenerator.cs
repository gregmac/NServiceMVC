using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Utilities.Reflection.ExtensionMethods;

namespace NServiceMVC.Utilities
{
    /// <summary>
    /// Class to create a default value for a specified type.
    /// </summary>
    public class DefaultValueGenerator
    {
        private DefaultValueGenerator() { }

        /// <summary>
        /// Gets a "sample" instance of type T. This tries to populate all public properties/fields
        /// of the type using sample values, including other sub-objects and collections.
        /// For collections, a single entry of the collection type is added.
        /// </summary>
        /// <param name="T"></param>
        /// <returns></returns>
        public static object GetSampleInstance(Type T)
        {
            var sample = GetBasicTypeValue(T); // try to create using "known" types
            if (sample == null)
            {
                if (T.IsArray)
                {
                    var elementType = T.GetElementType();

                    sample = Array.CreateInstance(elementType, 1);
                    var instance = GetSampleInstance(elementType);
                    ((Array)sample).SetValue(instance, 0);
                }
                else if (T.IsIEnumerable())
                {

                    sample = TrySampleGenericTypes(T);

                }
                else
                {
                    var actualType = Nullable.GetUnderlyingType(T);
                    if (actualType != null)
                    {
                        // T is Nullable<actualType>
                        sample = GetSampleInstance(actualType);
                    }
                    else
                    {
                        // create new instance of class
                        sample = T.CreateInstance();

                        foreach (var item in GetFieldsAndProperties(T))
                        {
                            var type = item.ActualType;
                            object value;
                            try
                            {
                                value = item.GetValue(sample); // get currently assigned value    
                            }
                            catch (TargetException e)
                            {
                                value = null; // suppress this error -- happens with nullable<T> types
                            }


                            if (value == null || (IsBasicType(type) && CheckIsDefaultValue(type, value)))
                            { // if null or default for that value type,
                                value = GetSampleInstance(type);
                            }

                            if (value != null)
                            {
                                item.SetValue(sample, value);
                            }

                        }
                    }
                    
                }
            }

            return sample;
        }


        /// <summary>
        /// Returns true if the type T is one of the basic types supported in System.* namespace
        /// </summary>
        /// <param name="T"></param>
        /// <returns></returns>
        public static bool IsBasicType(Type T)
        {
            switch (T.Name)
            {
                case "Int16": 
                case "Int32": 
                case "Int64": 
                case "UInt16": 
                case "UInt32": 
                case "UInt64": 
                case "String": 
                case "Boolean": 
                case "Single":
                case "Double":
                case "Decimal": 
                case "Char": 
                case "DateTime": 
                case "TimeSpan": 
                case "Byte": 
                case "SByte":
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the value for one of the basic types (defined in System.* namespace), if T is a basic type.
        /// If not, returns null.
        /// </summary>
        /// <param name="T"></param>
        /// <returns></returns>
        public static object GetBasicTypeValue(Type T)
        {
            switch (T.Name)
            {
                case "Int16": return (Int16)(-1600);
                case "Int32": return (Int32)(-3200);
                case "Int64": return (Int64)(-6400);
                case "UInt16": return (UInt16)(1600);
                case "UInt32": return (UInt32)(3200);
                case "UInt64": return (UInt64)(6400);
                case "String": return (String)"sample";
                case "Boolean": return (Boolean)true;
                case "Single": return (Single)100.123;
                case "Double": return (Double)200.123;
                case "Decimal": return (Decimal)12345.6789;
                case "Char": return (Char)'x';
                case "DateTime": return new DateTime(2012,01,01,00,00,00);
                case "TimeSpan": return (TimeSpan)TimeSpan.FromDays(1);
                case "Byte": return (Byte)8;
                case "SByte": return (SByte)(-8);
            }
            return null;
        }

        private static object TrySampleGenericTypes(Type T)
        {
            object sample = null;

            if (T.IsGenericType)
            {
                var containerType = T.GetGenericTypeDefinition();
                var innerTypes = T.GetGenericArguments();

                var innerSamples = new List<Object>();

                foreach (var innerType in innerTypes)
                {
                    innerSamples.Add(GetSampleInstance(innerType));
                }

                if (T.IsInterface)
                {
                    // substitute a type known to implement this interface

                    string newTypeName = null;

                    switch (containerType.FullName)
                    {
                        case "System.Collections.Generic.IList`1":
                        case "System.Collections.Generic.IEnumerable`1":
                        case "System.Collections.Generic.ICollection`1":
                            newTypeName = "System.Collections.Generic.List`1";
                            break;
                        case "System.Collections.Generic.IDictionary`2":
                            newTypeName = "System.Collections.Generic.Dictionary`2";
                            break;
                    }
                    if (newTypeName != null)
                    {
                        try
                        {
                            // thanks http://bartdesmet.net/blogs/bart/archive/2006/09/11/4410.aspx
                            var newType = Type.GetType(newTypeName).MakeGenericType(innerTypes);
                            sample = Activator.CreateInstance(newType);

                            var addMethod = newType.GetMethod("Add", innerTypes);
                            
                            if (addMethod != null)
                                addMethod.Invoke(sample, innerSamples.ToArray());

                        }
                        catch (Exception)
                        {
                            // ignore
                        }
                    }
                }
                else
                {
                    // directly create new instace
                    sample = Activator.CreateInstance(T);
                        
                    try
                    {
                        var addMethod = T.GetMethod("Add", innerTypes);
                        if (addMethod == null)
                            addMethod = T.GetMethod("Enqueue", innerTypes);
                        if (addMethod == null)
                            addMethod = T.GetMethod("Push", innerTypes);
                        if (addMethod == null)
                            addMethod = T.GetMethod("AddFirst", innerTypes);
                        
                        if (addMethod != null)
                            addMethod.Invoke(sample, innerSamples.ToArray());
                    }
                    catch (Exception)
                    {
                        // ignore
                    }
                }


            }
            return sample;
        }

        private static bool CheckIsDefaultValue(Type T, object val)
        {
            switch (T.Name)
            {
                case "Int16": return (Int16)val == default(Int16);
                case "Int32": return (Int32)val == default(Int32);
                case "Int64": return (Int64)val == default(Int64);
                case "UInt16": return (UInt16)val == default(UInt16);
                case "UInt32": return (UInt32)val == default(UInt32);
                case "UInt64": return (UInt64)val == default(UInt64);
                case "String": return (String)val == default(String);
                case "Boolean": return (Boolean)val == default(Boolean);
                case "Single": return (Single)val == default(Single);
                case "Double": return (Double)val == default(Double);
                case "Decimal": return (Decimal)val == default(Decimal);
                case "Char": return (Char)val == default(Char);
                case "DateTime": return (DateTime)val == default(DateTime);
                case "TimeSpan": return (TimeSpan)val == default(TimeSpan);
                case "Byte": return (Byte)val == default(Byte);
                case "SByte": return (SByte)val == default(SByte);
            }
            return false;
        }

        /// <summary>
        /// Creates a new default instance of type T.
        /// Requires that T has a parameter-less constructor, or can be created using <code>default(T)</code>
        /// </summary>
        /// <param name="T"></param>
        /// <returns></returns>
        public static object GetDefaultValue(Type T)
        {
            

            try
            {
                return System.Activator.CreateInstance(T, true);
                //TODO: if array type, also create a single item of that type
            }
            catch (Exception activatorException)
            {
                try
                {
                    // from http://stackoverflow.com/a/2490267/7913
                    var defaultGeneratorType = typeof(DefaultGenerator<>).MakeGenericType(T);

                    return defaultGeneratorType.InvokeMember(
                      "GetDefault",
                      BindingFlags.Static |
                      BindingFlags.Public |
                      BindingFlags.InvokeMethod,
                      null, null, new object[0]);
                }
                catch //(Exception defaultGeneratorException)
                {
                    throw new MissingMethodException(string.Format("No parameterless constructor defined for model {0}", T.Name), activatorException);
                }

            }

        }

        // from http://stackoverflow.com/a/2490267/7913
        private class DefaultGenerator<T>
        {
            public static T GetDefault()
            {
                return default(T);
            }
        }

        #region GetFieldsAndProperties helper


        public class FieldOrProperty
        {
            public FieldOrProperty(FieldInfo field)
            {
                ActualType = field.FieldType;
                _getValueMethod = (obj) => field.GetValue(obj);
                _setValueMethod = (obj, value) => field.SetValue(obj, value);
            }

            public FieldOrProperty(PropertyInfo prop)
            {
                ActualType = prop.PropertyType;
                _getValueMethod = (obj) => prop.GetValue(obj, null);
                _setValueMethod = (obj, value) => prop.SetValue(obj, value, null);
            }

            public Type ActualType { get; set; }

            private Func<object, object> _getValueMethod;

            public object GetValue(object obj)
            {
                return _getValueMethod.Invoke(obj);
            }

            private Action<object, object> _setValueMethod;

            public void SetValue(object obj, object value)
            {
                _setValueMethod.Invoke(obj, value);
            }
        }

        public static IEnumerable<FieldOrProperty> GetFieldsAndProperties(Type type)
        {
            return (
                        from f in type.GetFields() select new FieldOrProperty(f)
                   )
                   .Union(
                        from p in type.GetProperties() select new FieldOrProperty(p)
                   );
        }

        #endregion
    }
}
