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
        public string Name { get; set; }
        public string SampleJson { get; set; }
        public string Description { get; set; }
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
