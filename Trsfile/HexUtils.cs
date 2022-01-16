namespace com.riscure.trs
{

    public class HexUtils
    {
        private static readonly byte[] HEX_ARRAY = StringHelper.GetBytes("0123456789ABCDEF", System.Text.Encoding.UTF8);

        public static string ToHexString(byte[] bytes)
        {
            byte[] hexChars = new byte[bytes.Length * 2];
            for (int j = 0; j < bytes.Length; j++)
            {
                int v = bytes[j] & 0xFF;
                hexChars[j * 2] = HEX_ARRAY[(int)((uint)v >> 4)];
                hexChars[j * 2 + 1] = HEX_ARRAY[v & 0x0F];
            }
            return StringHelper.NewString(hexChars, System.Text.Encoding.UTF8);
        }
    }

}