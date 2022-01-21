using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ByteArrayParameter = com.riscure.trs.parameter.primitive.ByteArrayParameter;
using IntegerArrayParameter = com.riscure.trs.parameter.primitive.IntegerArrayParameter;

namespace com.riscure.trs.parameter.trace
{
    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static org.junit.jupiter.api.Assertions.assertThrows;


    public class UnmodifiableTraceParameterMapTest
    {
        private TraceParameterMap immutable;

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @BeforeEach public void setup()
        public virtual void Setup()
        {
            TraceParameterMap mutable = new();
            mutable.Add("FOO", 1);

            immutable = UnmodifiableTraceParameterMap.Of(mutable);
        }

        /// <summary>
        /// This test ensure that the underlying map of an unmodifiable map cannot change the map itself
        /// </summary>
        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testUnmodifiable()
        public virtual void TestUnmodifiable()
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


        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void put()
        public virtual void Add()
        {
            Exception e = Assert.ThrowsException<NotSupportedException>(() => immutable.Add("BLA", 2));

            string expectedMessage = "Unable to set parameter `BLA` to `[2]`: This trace set is in read mode and cannot be modified.";
            string actualMessage = e.Message;
            Assert.IsTrue(actualMessage.Contains(expectedMessage));
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void remove()
        public virtual void Remove()
        {
            Exception e = Assert.ThrowsException<NotSupportedException>(() => immutable.Remove("FOO"));

            string expectedMessage = "Unable to remove parameter `FOO`: This trace set is in read mode and cannot be modified.";
            string actualMessage = e.Message;

            Assert.IsTrue(actualMessage.Contains(expectedMessage));
        }


        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void clear()
        public virtual void Clear()
        {
            Exception e = Assert.ThrowsException<NotSupportedException>(() => immutable.Clear());

            string expectedMessage = "Unable to modify: This trace set is in read mode and cannot be modified.";
            string actualMessage = e.Message;

            Assert.IsTrue(actualMessage.Contains(expectedMessage));
        }


        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void replaceAll()
        public virtual void ReplaceAll()
        {
            Exception e = Assert.ThrowsException<NotSupportedException>(() => new IntegerArrayParameter(new int[] { 1 }));

            string expectedMessage = "Unable to modify: This trace set is in read mode and cannot be modified.";
            string actualMessage = e.Message;

            Assert.IsTrue(actualMessage.Contains(expectedMessage));
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testRemove()
        public virtual void TestRemove()
        {
            Exception e = Assert.ThrowsException<NotSupportedException>(() => immutable.Remove("BLA", out _));

            string expectedMessage = "Unable to remove parameter `BLA`: This trace set is in read mode and cannot be modified.";
            string actualMessage = e.Message;

            Assert.IsTrue(actualMessage.Contains(expectedMessage));
        }

    }
}