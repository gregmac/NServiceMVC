using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NServiceMVC.Tests
{
    [TestFixture]
    public class DefaultValueGeneratorTests
    {

        #region Basic types
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
                CharProp = 'y';
                DateTimeProp = new DateTime(2001, 01, 01, 12, 34, 56);
                TimeSpanProp = TimeSpan.FromMilliseconds(42);
                ByteProp = 2;
                SByteProp = 3;
            }
        }

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
        public void BasicTypesNotReInitialized()
        {
            var sample = Utilities.DefaultValueGenerator.GetSampleInstance(typeof(ComplexOne));

            Assert.That(sample, Is.TypeOf<ComplexOne>());

            ComplexOne s = (ComplexOne)sample;

            Assert.That(s.Name, Is.Not.Null.Or.Empty);
            Assert.That(s.Two, Is.TypeOf(typeof(ComplexTwo)));

        }

        #endregion

        #region Arrays/enumerables
        #endregion
    }
}
