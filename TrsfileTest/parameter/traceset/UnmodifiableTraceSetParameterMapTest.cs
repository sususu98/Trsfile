using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace com.riscure.trs.parameter.traceset
{
    using IntegerArrayParameter = com.riscure.trs.parameter.primitive.IntegerArrayParameter;

    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static org.junit.jupiter.api.Assertions.*;

    internal class UnmodifiableTraceSetParameterMapTest
    {
        private TraceSetParameterMap immutable;

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @BeforeEach public void setup()
        public virtual void Setup()
        {
            TraceSetParameterMap mutable = new();
            mutable.Add("FOO", 1);

            immutable = UnmodifiableTraceSetParameterMap.Of(mutable);
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
        //ORIGINAL LINE: @Test public void testRemove()
        public virtual void TestRemove()
        {
            Exception e = Assert.ThrowsException<NotSupportedException>(() => immutable.Remove("BLA", out _));

            string expectedMessage = "Unable to remove parameter `BLA`: This trace set is in read mode and cannot be modified.";
            string actualMessage = e.Message;

            Assert.IsTrue(actualMessage.Contains(expectedMessage));

        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void replace()
        public virtual void Replace()
        {
            Exception e = Assert.ThrowsException<NotSupportedException>(() => immutable["FOO"] = new TraceSetParameter(new IntegerArrayParameter(new int[] { -1 })));

            string expectedMessage = "Unable to set parameter `FOO` to `[-1]`: This trace set is in read mode and cannot be modified.";
            string actualMessage = e.Message;

            Assert.IsTrue(actualMessage.Contains(expectedMessage));

        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testReplace()
        public virtual void TestReplace()
        {
            Exception e = Assert.ThrowsException<NotSupportedException>(() => immutable["FOO"] = new TraceSetParameter(new IntegerArrayParameter(new int[] { -1 })));

            string expectedMessage = "Unable to set parameter `FOO` to `[-1]`: This trace set is in read mode and cannot be modified.";
            string actualMessage = e.Message;

            Assert.IsTrue(actualMessage.Contains(expectedMessage));

        }
    }
}