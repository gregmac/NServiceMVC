﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NServiceMVC.Metadata.Models
{
    /// <summary>
    /// Details about a model class (POCO)
    /// </summary>
    [DotLiquid.LiquidType("Name", "Description", "ModelType", "HasMetadata", "SampleJson", "SampleXml", "SampleCSharp", "IsBasicType", "IsArray", "ArrayDetail")]
    public class ModelDetail
    {
        /// <summary>
        /// The full name of the type
        /// </summary>
        public string Name { get; set; }
        public string Description { get; set; }

        /// <summary>
        /// The actual model type
        /// </summary>
        public System.Type ModelType;

        /// <summary>
        /// If this is a known type we can serve metadata for
        /// </summary>
        public bool HasMetadata { get; set; }

        public string SampleJson { get; set; }
        public string SampleXml { get; set; }
        public string SampleCSharp { get; set; }

        /// <summary>
        /// If this is a basic type (eg, string, int -- part of System.* namespace)
        /// </summary>
        public bool IsBasicType { get; set; }

        /// <summary>
        /// If the metadata is actually an array of this type
        /// </summary>
        public bool IsArray { get; set; }

        /// <summary>
        /// Details of the inner type, if this is an array
        /// </summary>
        public ModelDetail ArrayDetail { get; set; }
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
