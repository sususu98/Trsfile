using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Trsfile.Parameter.Primitive;
using Trsfile.Parameter.Trace;
using Trsfile.Parameter;

namespace Trsfile.Test
{
    [TestClass]
    public class UnmodifiableTraceParameterMapTest
    {
        private TraceParameterMap immutable;

        [TestInitialize]
        public void Setup()
        {
            TraceParameterMap mutable = new();
            mutable.Add("FOO", 1);

            immutable = UnmodifiableTraceParameterMap.Of(mutable);
        }

        /// <summary>
        /// This test ensure that the underlying map of an unmodifiable map cannot change the map itself
        /// </summary>
        [TestMethod]
        public void TestUnmodifiable()
        {
            byte[] ba = new byte[] { 1, 2, 3, 4, 5 };
            ByteArrayParameter bap = new(ba);
            TraceParameterMap tpm = new();
            tpm.Add("BA", bap);
            TraceParameterMap copy = UnmodifiableTraceParameterMap.Of(tpm);
            ba[1] = 6;
            var t = copy["BA"];
            if (t is TraceParameter<byte> Tparameter)
                Assert.IsFalse(Tparameter.Value.Equals(ba));

        }

        [TestMethod]
        public void Add()
        {
            Exception e = Assert.ThrowsException<NotSupportedException>(() => immutable.Add("BLA", 2));

            string expectedMessage = "Unable to set parameter `BLA` to `[2]`: This trace set is in read mode and cannot be modified.";
            string actualMessage = e.Message;
            Assert.IsTrue(actualMessage.Contains(expectedMessage));
        }

        [TestMethod]
        public void Remove()
        {
            Exception e = Assert.ThrowsException<NotSupportedException>(() => immutable.Remove("FOO"));

            string expectedMessage = "Unable to remove parameter `FOO`: This trace set is in read mode and cannot be modified.";
            string actualMessage = e.Message;

            Assert.IsTrue(actualMessage.Contains(expectedMessage));
        }


        [TestMethod]
        public void Clear()
        {
            Exception e = Assert.ThrowsException<NotSupportedException>(() => immutable.Clear());

            string expectedMessage = "Unable to modify: This trace set is in read mode and cannot be modified.";
            string actualMessage = e.Message;

            Assert.IsTrue(actualMessage.Contains(expectedMessage));
        }


        [TestMethod]
        public void TestReplace()
        {
            Exception e = Assert.ThrowsException<NotSupportedException>(() => immutable["BLA"] = new IntegerArrayParameter(new int[] { 1 }));

            string expectedMessage = "Unable to set parameter `BLA` to `[1]`: This trace set is in read mode and cannot be modified.";
            string actualMessage = e.Message;

            Assert.IsTrue(actualMessage.Contains(expectedMessage));
        }

        [TestMethod]
        public void TestRemove()
        {
            Exception e = Assert.ThrowsException<NotSupportedException>(() => immutable.Remove("BLA", out _));

            string expectedMessage = "Unable to remove parameter `BLA`: This trace set is in read mode and cannot be modified.";
            string actualMessage = e.Message;

            Assert.IsTrue(actualMessage.Contains(expectedMessage));
        }

    }
}