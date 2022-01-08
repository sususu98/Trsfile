namespace com.riscure.trs
{

	public class HexUtils
	{
		private static readonly sbyte[] HEX_ARRAY = "0123456789ABCDEF".getBytes(StandardCharsets.UTF_8);

		public static string toHexString(sbyte[] bytes)
		{
			sbyte[] hexChars = new sbyte[bytes.Length * 2];
			for (int j = 0; j < bytes.Length; j++)
			{
				int v = bytes[j] & 0xFF;
				hexChars[j * 2] = HEX_ARRAY[(int)((uint)v >> 4)];
				hexChars[j * 2 + 1] = HEX_ARRAY[v & 0x0F];
			}
			return StringHelper.NewString(hexChars, StandardCharsets.UTF_8);
		}
	}

}