using System;
using System.Linq;

namespace com.riscure.trs.parameter.trace
{
	using ByteArrayParameter = com.riscure.trs.parameter.primitive.ByteArrayParameter;
	using IntegerArrayParameter = com.riscure.trs.parameter.primitive.IntegerArrayParameter;
	using Assertions = org.junit.jupiter.api.Assertions;
	using BeforeEach = org.junit.jupiter.api.BeforeEach;
	using Test = org.junit.jupiter.api.Test;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.jupiter.api.Assertions.assertThrows;


	public class UnmodifiableTraceParameterMapTest
	{
		private TraceParameterMap immutable;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @BeforeEach public void setup()
		public virtual void setup()
		{
			TraceParameterMap mutable = new TraceParameterMap();
			mutable.Add("FOO", 1);

			immutable = UnmodifiableTraceParameterMap.Of(mutable);
		}

		/// <summary>
		/// This test ensure that the underlying map of an unmodifiable map cannot change the map itself
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUnmodifiable()
		public virtual void testUnmodifiable()
		{
			sbyte[] ba = new sbyte[] {1, 2, 3, 4, 5};
			ByteArrayParameter bap = new ByteArrayParameter(ba);
			TraceParameterMap tpm = new TraceParameterMap();
			tpm.put("BA", bap);
			TraceParameterMap copy = UnmodifiableTraceParameterMap.Of(tpm);
			ba[1] = 6;
			sbyte[] baCopy = (sbyte[]) copy.get("BA").getValue();
			Assertions.assertFalse(baCopy.SequenceEqual(ba), "Arrays should not be equal, but they are");
		}


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void put()
		public virtual void put()
		{
			Exception e = assertThrows(typeof(System.NotSupportedException), () => immutable.Add("BLA", 2));

			string expectedMessage = "Unable to set parameter `BLA` to `[2]`: This trace set is in read mode and cannot be modified.";
			string actualMessage = e.Message;

			Assertions.assertTrue(actualMessage.Contains(expectedMessage));
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void remove()
		public virtual void remove()
		{
			Exception e = assertThrows(typeof(System.NotSupportedException), () => immutable.remove("FOO"));

			string expectedMessage = "Unable to remove parameter `FOO`: This trace set is in read mode and cannot be modified.";
			string actualMessage = e.Message;

			Assertions.assertTrue(actualMessage.Contains(expectedMessage));
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void putAll()
		public virtual void putAll()
		{
			TraceParameterMap source = new TraceParameterMap();
			source.Add("BEEP", 5);
			source.Add("BOOP", 7);

			Exception e = assertThrows(typeof(System.NotSupportedException), () => immutable.putAll(source));

			string expectedMessage = "Unable to add all of `{BEEP=[5], BOOP=[7]}` : This trace set is in read mode and cannot be modified.";
			string actualMessage = e.Message;

			Assertions.assertTrue(actualMessage.Contains(expectedMessage));
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void clear()
		public virtual void clear()
		{
			Exception e = assertThrows(typeof(System.NotSupportedException), () => immutable.clear());

			string expectedMessage = "Unable to modify: This trace set is in read mode and cannot be modified.";
			string actualMessage = e.Message;

			Assertions.assertTrue(actualMessage.Contains(expectedMessage));
		}


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void replaceAll()
		public virtual void replaceAll()
		{
			Exception e = assertThrows(typeof(System.NotSupportedException), () => immutable.replaceAll((key, oldValue) => new IntegerArrayParameter(new int[]{1})));

			string expectedMessage = "Unable to modify: This trace set is in read mode and cannot be modified.";
			string actualMessage = e.Message;

			Assertions.assertTrue(actualMessage.Contains(expectedMessage));
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void putIfAbsent()
		public virtual void putIfAbsent()
		{
			Exception e = assertThrows(typeof(System.NotSupportedException), () => immutable.putIfAbsent("BLA", new IntegerArrayParameter(new int[]{-1})));

			string expectedMessage = "Unable to set parameter `BLA` to `[-1]`: This trace set is in read mode and cannot be modified.";
			string actualMessage = e.Message;

			Assertions.assertTrue(actualMessage.Contains(expectedMessage));
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRemove()
		public virtual void testRemove()
		{
			Exception e = assertThrows(typeof(System.NotSupportedException), () => immutable.remove("BLA", "MEH"));

			string expectedMessage = "Unable to remove parameter `BLA`: This trace set is in read mode and cannot be modified.";
			string actualMessage = e.Message;

			Assertions.assertTrue(actualMessage.Contains(expectedMessage));
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void replace()
		public virtual void replace()
		{
			Exception e = assertThrows(typeof(System.NotSupportedException), () => immutable.replace("FOO", new IntegerArrayParameter(new int[]{1}), new IntegerArrayParameter(new int[]{-1})));

			string expectedMessage = "Unable to set parameter `FOO` to `[-1]`: This trace set is in read mode and cannot be modified.";
			string actualMessage = e.Message;

			Assertions.assertTrue(actualMessage.Contains(expectedMessage));
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReplace()
		public virtual void testReplace()
		{
			Exception e = assertThrows(typeof(System.NotSupportedException), () => immutable.replace("FOO", new IntegerArrayParameter(new int[]{-1})));

			string expectedMessage = "Unable to set parameter `FOO` to `[-1]`: This trace set is in read mode and cannot be modified.";
			string actualMessage = e.Message;

			Assertions.assertTrue(actualMessage.Contains(expectedMessage));
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void merge()
		public virtual void merge()
		{


			Exception e = assertThrows(typeof(System.NotSupportedException), () => immutable.merge("BLA", new IntegerArrayParameter(new int[]{77}), (traceParameter, traceParameter2) => new IntegerArrayParameter(new int[]{55})));

			string expectedMessage = "Unable to modify: This trace set is in read mode and cannot be modified.";
			string actualMessage = e.Message;

			Assertions.assertTrue(actualMessage.Contains(expectedMessage));
		}
	}
}