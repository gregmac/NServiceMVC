using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;


namespace NServiceMVC.Utilities
{
    /// <summary>
    /// Class to create a default value for a specified type.
    /// </summary>
    class DefaultValueGenerator
    {
        private DefaultValueGenerator() { }

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
