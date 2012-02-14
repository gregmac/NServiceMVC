using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;

namespace NServiceMVC.Tests.MetadataReflectorTestModels
{
    [System.ComponentModel.Description("Description for SimpleModel")]
    public class SimpleModel
    {
        public string Name;
        public int Value;
    }
}

namespace NServiceMVC.Tests
{
    [TestFixture]
    class MetadataReflectorTests
    {

        [Test]
        public void BasicModelsFound()
        {
            var configMock = new Mock<NServiceMVC.NsConfiguration>();
            configMock.Setup(m => m.ModelAssemblies).Returns(
                new List<NServiceMVC.NsConfiguration.ModelAssembly>() { 
                    new NServiceMVC.NsConfiguration.ModelAssembly() { 
                        Assembly = System.Reflection.Assembly.GetExecutingAssembly(), 
                        Namespace = "NServiceMVC.Tests.MetadataReflectorTestModels" 
                    }
                }
            );
            configMock.Setup(m => m.ControllerAssemblies).Returns(
                new List<System.Reflection.Assembly>() 
            );
            var config = configMock.Object;

            var formatterMock = new Mock<Formats.FormatManager>(config);
            var formatter = formatterMock.Object;

            var reflector = new Metadata.MetadataReflector(config, formatter);

            Assert.That(reflector.ModelTypes.Contains("SimpleModel"), Is.True);
            Assert.That(reflector.ModelTypes["SimpleModel"].IsBasicType, Is.False);
            Assert.That(reflector.ModelTypes["SimpleModel"].HasMetadata, Is.True);
            Assert.That(reflector.ModelTypes["SimpleModel"].Description, Is.EqualTo("Description for SimpleModel"));
        }
    }
}
