using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Utilities.Reflection.ExtensionMethods;

namespace NServiceMVC.Tests
{
    [TestFixture]
    public class DefaultValueGeneratorTests
    {

        #region Basic types

        [Test]
        public void BasicTypesCreated()
        {
            Assert.That(Utilities.DefaultValueGenerator.GetSampleInstance(typeof(Int16)), Is.EqualTo((Int16)(-1600)));
            Assert.That(Utilities.DefaultValueGenerator.GetSampleInstance(typeof(Int32)), Is.EqualTo((Int32)(-3200)));
            Assert.That(Utilities.DefaultValueGenerator.GetSampleInstance(typeof(Int64)), Is.EqualTo((Int64)(-6400)));
            Assert.That(Utilities.DefaultValueGenerator.GetSampleInstance(typeof(UInt16)), Is.EqualTo((UInt16)(1600)));
            Assert.That(Utilities.DefaultValueGenerator.GetSampleInstance(typeof(UInt32)), Is.EqualTo((UInt32)(3200)));
            Assert.That(Utilities.DefaultValueGenerator.GetSampleInstance(typeof(UInt64)), Is.EqualTo((UInt64)(6400)));
            Assert.That(Utilities.DefaultValueGenerator.GetSampleInstance(typeof(String)), Is.EqualTo("sample"));
            Assert.That(Utilities.DefaultValueGenerator.GetSampleInstance(typeof(Boolean)), Is.EqualTo((bool)true));
            Assert.That(Utilities.DefaultValueGenerator.GetSampleInstance(typeof(Single)), Is.EqualTo((Single)100.123));
            Assert.That(Utilities.DefaultValueGenerator.GetSampleInstance(typeof(Double)), Is.EqualTo((Double)200.123));
            Assert.That(Utilities.DefaultValueGenerator.GetSampleInstance(typeof(Decimal)), Is.EqualTo((Decimal)12345.6789));
            Assert.That(Utilities.DefaultValueGenerator.GetSampleInstance(typeof(Char)), Is.EqualTo((char)'x'));
            Assert.That(Utilities.DefaultValueGenerator.GetSampleInstance(typeof(DateTime)), Is.EqualTo(new DateTime(2012, 01, 01, 00, 00, 00)));
            Assert.That(Utilities.DefaultValueGenerator.GetSampleInstance(typeof(TimeSpan)), Is.EqualTo(TimeSpan.FromDays(1)));
            Assert.That(Utilities.DefaultValueGenerator.GetSampleInstance(typeof(Byte)), Is.EqualTo((Byte)8));
            Assert.That(Utilities.DefaultValueGenerator.GetSampleInstance(typeof(SByte)), Is.EqualTo((sbyte)(-8)));
        }

        #endregion

        #region Object with Basic types (also checks values)
        public class TestBasicTypes
        {
            public Int16 Int16Field;
            public Int32 Int32Field;
            public Int64 Int64Field;
            public UInt16 UInt16Field;
            public UInt32 UInt32Field;
            public UInt64 UInt64Field;
            public String StringField;
            public Boolean BooleanField;
            public Single SingleField;
            public Double DoubleField;
            public Decimal DecimalField;
            public Char CharField;
            public DateTime DateTimeField;
            public TimeSpan TimeSpanField;
            public Byte ByteField;
            public SByte SByteField;

            public Int16 Int16Prop { get; set; }
            public Int32 Int32Prop { get; set; }
            public Int64 Int64Prop { get; set; }
            public UInt16 UInt16Prop { get; set; }
            public UInt32 UInt32Prop { get; set; }
            public UInt64 UInt64Prop { get; set; }
            public String StringProp { get; set; }
            public Boolean BooleanProp { get; set; }
            public Single SingleProp { get; set; }
            public Double DoubleProp { get; set; }
            public Decimal DecimalProp { get; set; }
            public Char CharProp { get; set; }
            public DateTime DateTimeProp { get; set; }
            public TimeSpan TimeSpanProp { get; set; }
            public Byte ByteProp { get; set; }
            public SByte SByteProp { get; set; }
        }

        [Test]
        public void BasicTypesInitializedCorrectly()
        {
            var sample = Utilities.DefaultValueGenerator.GetSampleInstance(typeof(TestBasicTypes));

            Assert.That(sample, Is.TypeOf<TestBasicTypes>());

            Assert.That(((TestBasicTypes)sample).Int16Field, Is.EqualTo((Int16)(-1600)));
            Assert.That(((TestBasicTypes)sample).Int32Field, Is.EqualTo((Int32)(-3200)));
            Assert.That(((TestBasicTypes)sample).Int64Field, Is.EqualTo((Int64)(-6400)));
            Assert.That(((TestBasicTypes)sample).UInt16Field, Is.EqualTo((UInt16)(1600)));
            Assert.That(((TestBasicTypes)sample).UInt32Field, Is.EqualTo((UInt32)(3200)));
            Assert.That(((TestBasicTypes)sample).UInt64Field, Is.EqualTo((UInt64)(6400)));
            Assert.That(((TestBasicTypes)sample).StringField, Is.EqualTo("sample"));
            Assert.That(((TestBasicTypes)sample).BooleanField, Is.EqualTo((bool)true));
            Assert.That(((TestBasicTypes)sample).SingleField, Is.EqualTo((Single)100.123));
            Assert.That(((TestBasicTypes)sample).DoubleField, Is.EqualTo((Double)200.123));
            Assert.That(((TestBasicTypes)sample).DecimalField, Is.EqualTo((Decimal)12345.6789));
            Assert.That(((TestBasicTypes)sample).CharField, Is.EqualTo((char)'x'));
            Assert.That(((TestBasicTypes)sample).DateTimeField, Is.EqualTo(new DateTime(2012, 01, 01, 00, 00, 00)));
            Assert.That(((TestBasicTypes)sample).TimeSpanField, Is.EqualTo(TimeSpan.FromDays(1)));
            Assert.That(((TestBasicTypes)sample).ByteField, Is.EqualTo((Byte)8));
            Assert.That(((TestBasicTypes)sample).SByteField, Is.EqualTo((sbyte)(-8)));

            Assert.That(((TestBasicTypes)sample).Int16Prop, Is.EqualTo((Int16)(-1600)));
            Assert.That(((TestBasicTypes)sample).Int32Prop, Is.EqualTo((Int32)(-3200)));
            Assert.That(((TestBasicTypes)sample).Int64Prop, Is.EqualTo((Int64)(-6400)));
            Assert.That(((TestBasicTypes)sample).UInt16Prop, Is.EqualTo((UInt16)(1600)));
            Assert.That(((TestBasicTypes)sample).UInt32Prop, Is.EqualTo((UInt32)(3200)));
            Assert.That(((TestBasicTypes)sample).UInt64Prop, Is.EqualTo((UInt64)(6400)));
            Assert.That(((TestBasicTypes)sample).StringProp, Is.EqualTo("sample"));
            Assert.That(((TestBasicTypes)sample).BooleanProp, Is.EqualTo((bool)true));
            Assert.That(((TestBasicTypes)sample).SingleProp, Is.EqualTo((Single)100.123));
            Assert.That(((TestBasicTypes)sample).DoubleProp, Is.EqualTo((Double)200.123));
            Assert.That(((TestBasicTypes)sample).DecimalProp, Is.EqualTo((Decimal)12345.6789));
            Assert.That(((TestBasicTypes)sample).CharProp, Is.EqualTo((char)'x'));
            Assert.That(((TestBasicTypes)sample).DateTimeProp, Is.EqualTo(new DateTime(2012, 01, 01, 00, 00, 00)));
            Assert.That(((TestBasicTypes)sample).TimeSpanProp, Is.EqualTo(TimeSpan.FromDays(1)));
            Assert.That(((TestBasicTypes)sample).ByteProp, Is.EqualTo((Byte)8));
            Assert.That(((TestBasicTypes)sample).SByteProp, Is.EqualTo((sbyte)(-8)));
        }


        public class TestBasicTypesInitialized : TestBasicTypes 
        {
            public TestBasicTypesInitialized()
            {
                Int16Field = 4;
                Int32Field = 5;
                Int64Field = 6;
                UInt16Field = 7;
                UInt32Field = 8;
                UInt64Field = 9;
                StringField = "blah";
                BooleanField = true;
                SingleField = 42.5F;
                DoubleField = 66.5F;
                DecimalField = 33.44M;
                CharField = 'y';
                DateTimeField = new DateTime(2001,01,01,12,34,56);
                TimeSpanField = TimeSpan.FromMilliseconds(42);
                ByteField = 2;
                SByteField = 3;

                Int16Prop = 4;
                Int32Prop = 5;
                Int64Prop = 6;
                UInt16Prop = 7;
                UInt32Prop = 8;
                UInt64Prop = 9;
                StringProp = "blah";
                BooleanProp = true;
                SingleProp = 42.5F;
                DoubleProp = 66.5F;
                DecimalProp = 33.44M;
                CharProp = 'y';
                DateTimeProp = new DateTime(2001, 01, 01, 12, 34, 56);
                TimeSpanProp = TimeSpan.FromMilliseconds(42);
                ByteProp = 2;
                SByteProp = 3;
            }
        }

        /// <summary>
        /// Check that types that already have a value assigned (by the cosntructor of the object) are not changed
        /// </summary>
        [Test]
        public void BasicTypesNotReInitialized()
        {
            var sample = Utilities.DefaultValueGenerator.GetSampleInstance(typeof(TestBasicTypesInitialized));

            Assert.That(sample, Is.TypeOf<TestBasicTypesInitialized>());

            Assert.That(((TestBasicTypesInitialized)sample).Int16Field, Is.EqualTo((Int16)(4)));
            Assert.That(((TestBasicTypesInitialized)sample).Int32Field, Is.EqualTo((Int32)(5)));
            Assert.That(((TestBasicTypesInitialized)sample).Int64Field, Is.EqualTo((Int64)(6)));
            Assert.That(((TestBasicTypesInitialized)sample).UInt16Field, Is.EqualTo((UInt16)(7)));
            Assert.That(((TestBasicTypesInitialized)sample).UInt32Field, Is.EqualTo((UInt32)(8)));
            Assert.That(((TestBasicTypesInitialized)sample).UInt64Field, Is.EqualTo((UInt64)(9)));
            Assert.That(((TestBasicTypesInitialized)sample).StringField, Is.EqualTo("blah"));
            Assert.That(((TestBasicTypesInitialized)sample).BooleanField, Is.EqualTo((bool)true));
            Assert.That(((TestBasicTypesInitialized)sample).SingleField, Is.EqualTo((Single)42.5));
            Assert.That(((TestBasicTypesInitialized)sample).DoubleField, Is.EqualTo((Double)66.5));
            Assert.That(((TestBasicTypesInitialized)sample).DecimalField, Is.EqualTo((Decimal)33.44));
            Assert.That(((TestBasicTypesInitialized)sample).CharField, Is.EqualTo((char)'y'));
            Assert.That(((TestBasicTypesInitialized)sample).DateTimeField, Is.EqualTo(new DateTime(2001, 01, 01, 12, 34, 56)));
            Assert.That(((TestBasicTypesInitialized)sample).TimeSpanField, Is.EqualTo(TimeSpan.FromMilliseconds(42)));
            Assert.That(((TestBasicTypesInitialized)sample).ByteField, Is.EqualTo((Byte)2));
            Assert.That(((TestBasicTypesInitialized)sample).SByteField, Is.EqualTo((sbyte)(3)));

            Assert.That(((TestBasicTypesInitialized)sample).Int16Prop, Is.EqualTo((Int16)(4)));
            Assert.That(((TestBasicTypesInitialized)sample).Int32Prop, Is.EqualTo((Int32)(5)));
            Assert.That(((TestBasicTypesInitialized)sample).Int64Prop, Is.EqualTo((Int64)(6)));
            Assert.That(((TestBasicTypesInitialized)sample).UInt16Prop, Is.EqualTo((UInt16)(7)));
            Assert.That(((TestBasicTypesInitialized)sample).UInt32Prop, Is.EqualTo((UInt32)(8)));
            Assert.That(((TestBasicTypesInitialized)sample).UInt64Prop, Is.EqualTo((UInt64)(9)));
            Assert.That(((TestBasicTypesInitialized)sample).StringProp, Is.EqualTo("blah"));
            Assert.That(((TestBasicTypesInitialized)sample).BooleanProp, Is.EqualTo((bool)true));
            Assert.That(((TestBasicTypesInitialized)sample).SingleProp, Is.EqualTo((Single)42.5));
            Assert.That(((TestBasicTypesInitialized)sample).DoubleProp, Is.EqualTo((Double)66.5));
            Assert.That(((TestBasicTypesInitialized)sample).DecimalProp, Is.EqualTo((Decimal)33.44));
            Assert.That(((TestBasicTypesInitialized)sample).CharProp, Is.EqualTo((char)'y'));
            Assert.That(((TestBasicTypesInitialized)sample).DateTimeProp, Is.EqualTo(new DateTime(2001, 01, 01, 12, 34, 56)));
            Assert.That(((TestBasicTypesInitialized)sample).TimeSpanProp, Is.EqualTo(TimeSpan.FromMilliseconds(42)));
            Assert.That(((TestBasicTypesInitialized)sample).ByteProp, Is.EqualTo((Byte)2));
            Assert.That(((TestBasicTypesInitialized)sample).SByteProp, Is.EqualTo((sbyte)(3)));
        }

        #endregion

        #region Complex types
        public class ComplexOne
        {
            public string Name { get; set; }
            public ComplexTwo Two { get; set; }
        }

        public class ComplexTwo
        {
            public int TwoVal { get; set; }
            public ComplexThree Three { get; set; }
        }

        public class ComplexThree
        {
            public string ThirdName { get; set; }
        }

        [Test]
        public void ComplexTypeInitialized()
        {
            var sample = Utilities.DefaultValueGenerator.GetSampleInstance(typeof(ComplexOne));

            Assert.That(sample, Is.TypeOf<ComplexOne>());

            ComplexOne s = (ComplexOne)sample;

            Assert.That(s.Name, Is.Not.Null.Or.Empty);
            Assert.That(s.Two, Is.TypeOf(typeof(ComplexTwo)));
            Assert.That(s.Two.TwoVal, Is.Not.EqualTo(0)); // has some value 
            Assert.That(s.Two.Three, Is.TypeOf(typeof(ComplexThree)));
            Assert.That(s.Two.Three.ThirdName, Is.Not.Null.Or.Empty);

        }

        #endregion

        #region Arrays/enumerables
        [Test]
        public void BasicArrayTypesInitialized()
        {
            var intArray = Utilities.DefaultValueGenerator.GetSampleInstance(typeof(int[]));
            Assert.That(intArray, Is.TypeOf<int[]>());
            Assert.That(((int[])intArray).Length, Is.EqualTo(1));

            // multi-dimensional array
            var int3Array = Utilities.DefaultValueGenerator.GetSampleInstance(typeof(int[][][]));
            Assert.That(int3Array, Is.TypeOf<int[][][]>());
            Assert.That(((int[][][])int3Array).Length, Is.EqualTo(1));
            Assert.That(((int[][][])int3Array)[0].Length, Is.EqualTo(1));
            Assert.That(((int[][][])int3Array)[0][0].Length, Is.EqualTo(1));
            Assert.That(((int[][][])int3Array)[0][0][0], Is.Not.EqualTo(0)); // was initialized
        }

        [Test]
        public void CollectionTypesInitialized()
        {
            TestCollectionTypeInitialized<List<string>>();
            TestCollectionTypeInitialized<Queue<string>>();
            TestCollectionTypeInitialized<SortedSet<string>>();
            TestCollectionTypeInitialized<LinkedList<string>>();
            TestCollectionTypeInitialized<Stack<string>>();
            TestCollectionTypeInitialized<Dictionary<string, int>>();
            TestCollectionTypeInitialized<SortedDictionary<string, int>>();
            TestCollectionTypeInitialized<SortedList<string, int>>();
        }

        private void TestCollectionTypeInitialized<T>()
        {
            var type = typeof(T);
            var obj = Utilities.DefaultValueGenerator.GetSampleInstance(type);
            Assert.That(obj, Is.TypeOf<T>());

            int count = 0;
            try
            {
                count = (int)type.GetProperty("Count").GetValue(obj, null);
            }
            catch (Exception)
            {
                try
                {
                    count = (int)type.GetProperty("Length").GetValue(obj, null);
                }
                catch (Exception)
                {
                }
            }

            Assert.That(count, Is.EqualTo(1));
        }

        [Test]
        public void CollectionInterfaceTypes()
        {
            Assert.That(Utilities.DefaultValueGenerator.GetSampleInstance(typeof(IList<string>)), Is.TypeOf<List<string>>());
            Assert.That(Utilities.DefaultValueGenerator.GetSampleInstance(typeof(ICollection<string>)), Is.TypeOf<List<string>>());
            Assert.That(Utilities.DefaultValueGenerator.GetSampleInstance(typeof(IEnumerable<string>)), Is.TypeOf<List<string>>());
            Assert.That(Utilities.DefaultValueGenerator.GetSampleInstance(typeof(IDictionary<string, int>)), Is.TypeOf<Dictionary<string, int>>());
            
        }
        #endregion

        #region Objects with Nullables
        
        public class NullableTestType
        {
            public Nullable<Int16> Int16Prop { get; set; }
            public Nullable<Int32> Int32Prop { get; set; }
            public Nullable<Int64> Int64Prop { get; set; }
            public Nullable<UInt16> UInt16Prop { get; set; }
            public Nullable<UInt32> UInt32Prop { get; set; }
            public Nullable<UInt64> UInt64Prop { get; set; }
            public Nullable<Boolean> BooleanProp { get; set; }
            public Nullable<Single> SingleProp { get; set; }
            public Nullable<Double> DoubleProp { get; set; }
            public Nullable<Decimal> DecimalProp { get; set; }
            public Nullable<Char> CharProp { get; set; }
            public Nullable<DateTime> DateTimeProp { get; set; }
            public Nullable<TimeSpan> TimeSpanProp { get; set; }
            public Nullable<Byte> ByteProp { get; set; }
            public Nullable<SByte> SByteProp { get; set; }
        }

        [Test]
        public void NullablePropertiesInitialized()
        {
            var sample = Utilities.DefaultValueGenerator.GetSampleInstance(typeof(NullableTestType));

            Assert.That(sample, Is.TypeOf<NullableTestType>());

            NullableTestType s = (NullableTestType)sample;


            Assert.That(s.Int16Prop, Is.EqualTo((Int16)(-1600)));
            Assert.That(s.Int32Prop, Is.EqualTo((Int32)(-3200)));
            Assert.That(s.Int64Prop, Is.EqualTo((Int64)(-6400)));
            Assert.That(s.UInt16Prop, Is.EqualTo((UInt16)(1600)));
            Assert.That(s.UInt32Prop, Is.EqualTo((UInt32)(3200)));
            Assert.That(s.UInt64Prop, Is.EqualTo((UInt64)(6400)));
            Assert.That(s.BooleanProp, Is.EqualTo((bool)true));
            Assert.That(s.SingleProp, Is.EqualTo((Single)100.123));
            Assert.That(s.DoubleProp, Is.EqualTo((Double)200.123));
            Assert.That(s.DecimalProp, Is.EqualTo((Decimal)12345.6789));
            Assert.That(s.CharProp, Is.EqualTo((char)'x'));
            Assert.That(s.DateTimeProp, Is.EqualTo(new DateTime(2012, 01, 01, 00, 00, 00)));
            Assert.That(s.TimeSpanProp, Is.EqualTo(TimeSpan.FromDays(1)));
            Assert.That(s.ByteProp, Is.EqualTo((Byte)8));
            Assert.That(s.SByteProp, Is.EqualTo((sbyte)(-8)));

            // ---- 
            // note: below are the tests I'd prefer, but this does not currently return nullable<T>, not sure why. 
            // perhaps related: http://bradwilson.typepad.com/blog/2008/07/index.html

            //Assert.That(s.Int16Prop, Is.TypeOf<Nullable<Int16>>());

            //Assert.That(s.Int16Prop.HasValue, Is.True);
            //Assert.That(s.Int32Prop.HasValue, Is.True);  
            //Assert.That(s.Int64Prop.HasValue, Is.True);  
            //Assert.That(s.UInt16Prop.HasValue, Is.True);
            //Assert.That(s.UInt32Prop.HasValue, Is.True);
            //Assert.That(s.UInt64Prop.HasValue, Is.True);
            //Assert.That(s.BooleanProp.HasValue, Is.True);
            //Assert.That(s.SingleProp.HasValue, Is.True);
            //Assert.That(s.DoubleProp.HasValue, Is.True);
            //Assert.That(s.DecimalProp.HasValue, Is.True);
            //Assert.That(s.CharProp.HasValue, Is.True);
            //Assert.That(s.DateTimeProp.HasValue, Is.True);
            //Assert.That(s.TimeSpanProp.HasValue, Is.True);
            //Assert.That(s.ByteProp.HasValue, Is.True);
            //Assert.That(s.SByteProp.HasValue, Is.True);
        }

        [Test]
        public void NullableTypesInitialized()
        {
            var int16sample = Utilities.DefaultValueGenerator.GetSampleInstance(typeof(Nullable<Int16>));

            Assert.That(int16sample, Is.TypeOf<Nullable<Int16>>());
            Assert.That(((Nullable<Int16>)int16sample).HasValue, Is.True);
        }


        #endregion

    }
}
