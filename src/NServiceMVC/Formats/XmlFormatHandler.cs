using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

namespace NServiceMVC.Formats
{
    class XmlFormatHandler : IFormatHandler     
    {
       
        /// <summary>
        /// Serialize an object of typeof(model) into a string 
        /// Throws an exception if there is a problem.
        /// </summary>
        /// <param name="model">The data to serialize</param>
        /// <returns>A string containing the serialized object</returns>
        public string Serialize(object model, bool humanReadable = false)
        {
            return ObjectToXml(model, true, humanReadable);
        }

        /// <summary>
        /// Deserialize a string into an object of type T. 
        /// Throws an exception if there is a problem.
        /// </summary>
        /// <param name="serializedText">The string containing the serialized object</param>
        /// <returns>The deserialized object of type T</returns>
        object IFormatHandler.Deserialize(string input, Type modelType)
        {
            return XmlToObject(input, modelType);
        }


        public static string ObjectToXml(object toBeSerialised, bool includeXmlDeclaration, bool humanReadable)
        {
            string result = "";

            if (toBeSerialised != null)
            {
                // Make the output string pretty
                var xmlStringBuilder = new StringBuilder();
                var xmlWriterSettings = new XmlWriterSettings
                {
                    OmitXmlDeclaration = !includeXmlDeclaration,
                    Indent = humanReadable,
                    NewLineOnAttributes = humanReadable,
                };
                XmlWriter xmlWriter = XmlWriter.Create(xmlStringBuilder, xmlWriterSettings);

                // Try to find the root attribute so we can set the default namespace to use
                var objectType = toBeSerialised.GetType();
                var defaultNamespace = string.Empty;
                var xmlRootAttributes = toBeSerialised.GetType().GetCustomAttributes(typeof(XmlRootAttribute), true) as XmlRootAttribute[];
                if (xmlRootAttributes != null && xmlRootAttributes.Length > 0)
                {
                    defaultNamespace = xmlRootAttributes[0].Namespace;
                }

                // Use this object to prevent the serilaizer from adding extra "Ambient" namespaces
                var xmlSerializerNamespaces = new XmlSerializerNamespaces();
                xmlSerializerNamespaces.Add(String.Empty, defaultNamespace);

                // Using this constructor should mean the serializer is cached... beware of using any other
                // constructor it may cause memory leaks!!!!
                var xmlSerializer = new XmlSerializer(objectType);

                xmlSerializer.Serialize(xmlWriter, toBeSerialised, xmlSerializerNamespaces);
                xmlWriter.Flush();
                xmlWriter.Close();
                result = xmlStringBuilder.ToString();
            }

            return result;
        }

        public static T XmlToObject<T>(string xml) where T : class
        {
            // Using this constructor should mean the serializer is cached... beware of using any other
            // constructor it may cause memory leaks!!!!
            var xmlSerializer = new XmlSerializer(typeof(T));
            return xmlSerializer.Deserialize(new StringReader(xml)) as T;
        }

        public static object XmlToObject(string xml, System.Type type)
        {
            // Using this constructor should mean the serializer is cached... beware of using any other
            // constructor it may cause memory leaks!!!!
            var xmlSerializer = new XmlSerializer(type);
            return xmlSerializer.Deserialize(new StringReader(xml));
        }
    }
}
