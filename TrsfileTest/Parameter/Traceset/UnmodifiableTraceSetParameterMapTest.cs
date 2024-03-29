﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Trsfile.Parameter.Primitive;
using Trsfile.Parameter.Traceset;

namespace Trsfile.Test
{
    [TestClass]
    public class UnmodifiableTraceSetParameterMapTest
    {
        private TraceSetParameterMap immutable;

        [TestInitialize]
        public void Setup()
        {
            TraceSetParameterMap mutable = new();
            mutable.Add("FOO", 1);

            immutable = UnmodifiableTraceSetParameterMap.Of(mutable);
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
        public void TestRemove()
        {
            Exception e = Assert.ThrowsException<NotSupportedException>(() => immutable.Remove("BLA", out _));

            string expectedMessage = "Unable to remove parameter `BLA`: This trace set is in read mode and cannot be modified.";
            string actualMessage = e.Message;

            Assert.IsTrue(actualMessage.Contains(expectedMessage));

        }

        [TestMethod]
        public void Replace()
        {
            Exception e = Assert.ThrowsException<NotSupportedException>(() => immutable["FOO"] = new TraceSetParameter(new IntegerArrayParameter(new int[] { -1 })));

            string expectedMessage = "Unable to set parameter `{0}` to `[{1}]`: This trace set is in read mode and cannot be modified.";
            string actualMessage = e.Message;
            Assert.IsTrue(actualMessage.Contains(expectedMessage));

        }

        [TestMethod]
        public void TestReplace()
        {
            Exception e = Assert.ThrowsException<NotSupportedException>(() => immutable["FOO"] = new TraceSetParameter(new IntegerArrayParameter(new int[] { -1 })));

            string expectedMessage = "Unable to set parameter `{0}` to `[{1}]`: This trace set is in read mode and cannot be modified.";
            string actualMessage = e.Message;

            Assert.IsTrue(actualMessage.Contains(expectedMessage));

        }
    }
}