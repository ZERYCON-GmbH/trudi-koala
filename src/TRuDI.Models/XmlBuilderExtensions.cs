namespace TRuDI.Models
{
    using System.Text;

    public static class XmlBuilderExtensions
    {
        public static string WithNameExtension(this string id)
        {
            return id ?? (id.EndsWith(".sm") ? id : $"{id}.sm");
        }

        public static string ToHexBinary(this byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                return string.Empty;
            }

            var sb = new StringBuilder(data.Length * 2);

            for (int i = 0; i < data.Length; i++)
            {
                sb.AppendFormat("{0:x2}", data[i]);
            }

            return sb.ToString();
        }
    }
}