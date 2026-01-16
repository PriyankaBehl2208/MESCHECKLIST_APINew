using System.Text;

namespace MESCHECKLIST.Common
{
    public static class  Encryption
    {
        static string EncryptionSecretKey;
        static Encryption()
        {
            var _configuration = new ConfigurationBuilder()
                                                  .AddJsonFile("appSettings.Development.json")
                                                  .Build();
            IConfigurationSection appSettings = _configuration.GetSection("AppSettings");
            EncryptionSecretKey = appSettings["EncryptionSecretKey"];
        }
        public static string EncodePasswordToBase64(string password)
        {
            try
            {
                byte[] inputBytes=Encoding.UTF8.GetBytes(password);
                byte[] keyBytes=Encoding.UTF8.GetBytes(EncryptionSecretKey);
                byte[] result= new byte[inputBytes.Length];

                for(int i = 0;i< inputBytes.Length; i++)
                {
                    result[i] = (byte)(inputBytes[i]^ keyBytes[i % keyBytes.Length]);
                }
                return Encoding.UTF8.GetString(result);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in base64Encode" + ex.Message);
            }
        }
        public static string DecodePasswordToBase64(string encodedData)
        {
            System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
            System.Text.Decoder utf8Decode = encoder.GetDecoder();
            byte[] todecode_byte = Convert.FromBase64String(encodedData);
            int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            char[] decoded_char = new char[charCount];
            utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            string result = new String(decoded_char);
            return result;
        }
    }
}
