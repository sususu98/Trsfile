using System;

namespace com.riscure.trs
{
	public class TRSFormatException : Exception
	{
		public TRSFormatException(string message) : base(message)
		{
		}

		public TRSFormatException(Exception throwable) : base(throwable)
		{
		}

		public TRSFormatException(string message, Exception throwable) : base(message, throwable)
		{
		}
	}

}