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


        public static object GetSampleInstance(Type T)
        {
            var sample = T.CreateInstance();
            
            foreach (var field in T.GetFields())
            {
                var type = field.FieldType;
                var value = field.GetValue(sample); // get currently assigned value 

                value = FindDefaultValue(type, value);

                if (value != null)
                {
                    field.SetValue(sample, value);
                }

            }

            foreach (var prop in T.GetProperties())
            {
                var type = prop.PropertyType;
                var value = prop.GetValue(sample, null); // get currently assigned value 

                value = FindDefaultValue(type, value);

                if (value != null)
                {
                    prop.SetValue(sample, value, null);
                }

            }

            return sample;

            //if (type.IsIEnumerable())
            //{

            //}
            //else if (type.)
        }

        private static object FindDefaultValue(Type type, object value)
        {
            if (value == null || CheckIsDefaultValue(type, value))
            { // if null or default for that value type,
                value = TrySampleBasicTypes(type); // try to create using "known" types
            }

            if (value == null)
            {

                if (type.IsArray)
                {
                    // make new array with one memeber
                }
                else if (type.IsClass)
                {
                    // call recursive
                    //TODO: protect from self-referential
                    value = GetSampleInstance(type);
                }
            }

            return value;
        }

        private static object TrySampleBasicTypes(Type T)
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
                case "Char": return (Char)'x';
                case "DateTime": return new DateTime(2012,01,01,00,00,00);
                case "TimeSpan": return (TimeSpan)TimeSpan.FromDays(1);
                case "Byte": return (Byte)8;
                case "SByte": return (SByte)(-8);
            }
            return null;
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


    }
}
