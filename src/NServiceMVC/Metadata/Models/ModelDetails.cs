using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NServiceMVC.Metadata.Models
{
    /// <summary>
    /// Details about a model class (POCO)
    /// </summary>
    public class ModelDetail
    {
        /// <summary>
        /// The full name of the type
        /// </summary>
        public string Name { get; set; }
        public string Description { get; set; }

        /// <summary>
        /// If this is a known type we can serve metadata for
        /// </summary>
        public bool HasMetadata { get; set; }

        public string SampleJson { get; set; }
        public string SampleXml { get; set; }
        public string SampleCSharp { get; set; }
    }

    public class ModelDetailCollection : System.Collections.ObjectModel.KeyedCollection<string, ModelDetail>
    {
        public ModelDetailCollection() : base() { }
        public ModelDetailCollection(IEnumerable<ModelDetail> models)
            : base()
        {
            if (models != null)
            {
                foreach (var item in models) Add(item);
            }
        }

        protected override string GetKeyForItem(ModelDetail item)
        {
            return item.Name;
        }
    }

}
