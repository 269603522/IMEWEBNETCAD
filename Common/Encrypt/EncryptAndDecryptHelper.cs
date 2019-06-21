﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
namespace IMEWebCAD.Common.Encrypt
{
    /// <summary>
    /// Summary description for EncryptAndDecryptHelper
    /// </summary>
    public class EncryptAndDecryptHelper
    {
        public EncryptAndDecryptHelper()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        private string HowToUse()
        {
            string result = "";
            string data = EncryptAndDecryptHelper.Locker.Encrypt("待加密的数据信息", "自定义密码");
            result += "<br/>" + data;
            result += EncryptAndDecryptHelper.Locker.Decrypt(data, "自定义密码");
            return result;
        }

        /// <summary>
        /// 自定义可逆加解密算法，数据、密码，可以为任意字符串；
        /// 1、加密 Locker.Encrypt()
        /// 2、解密 Locker.Decrypt()
        /// </summary>
        public class Locker
        {
            public static void example()
            {
                string data = Locker.Encrypt("待加密的数据信息", "自定义密码");
                string result = Locker.Decrypt("9olrx3xiueqa9iegopfqskl2updirdfp922o0zi1tqxsltks", "自定义密码");

                //MessageBox.Show(data.Equals("9olrx3xiueqa9iegopfqskl2updirdfp922o0zi1tqxsltks") + "");  // true
                //MessageBox.Show(result.Equals("待加密的数据信息") + "");                                // true
            }

            /// <summary>
            /// 使用SecretKey对数据data进行加密
            /// </summary>
            /// <param name="data">待加密的数据</param>
            /// <param name="SecretKey">自定义密码串</param>
            /// <returns></returns>
            public static string Encrypt(string data, string SecretKey)
            {
                string key = MD5.Encrypt(SecretKey);
                data = Encoder.EncodeAlphabet(data);

                StringBuilder builder = new StringBuilder();

                int i = 0;
                foreach (char A in data)
                {
                    if (i >= key.Length)
                    {
                        i = i % key.Length;
                        key = MD5.Encrypt(key);
                    }
                    char B = key[i++];

                    char n = ToChar(ToInt(A) + ToInt(B) * 3);
                    builder.Append(n);
                }

                return builder.ToString();
            }

            /// <summary>
            /// 使用SecretKey对数据data进行解密
            /// </summary>
            /// <param name="data">待解密的数据</param>
            /// <param name="SecretKey">解密密码串</param>
            /// <returns></returns>
            public static string Decrypt(string data, string SecretKey)
            {
                string key = MD5.Encrypt(SecretKey);

                StringBuilder builder = new StringBuilder();

                int i = 0;
                foreach (char A in data)
                {
                    if (i >= key.Length)
                    {
                        i = i % key.Length;
                        key = MD5.Encrypt(key);
                    }
                    char B = key[i++];

                    char n = ToChar(ToInt(A) - ToInt(B) * 3);
                    builder.Append(n);
                }
                data = builder.ToString();
                data = Encoder.DecodeAlphabet(data);

                return data;
            }

            /// <summary>
            /// "0-9 a-z"映射为 0到35
            /// </summary>
            /// <param name="c"></param>
            /// <returns></returns>
            private static int ToInt(char c)
            {
                if ('0' <= c && c <= '9') return c - '0';
                else if ('a' <= c && c <= 'z') return 10 + c - 'a';
                else return 0;
            }

            /// <summary>
            /// 0到35依次映射为"0-9 a-z"，
            /// 超出35取余数映射
            /// </summary>
            /// <param name="n"></param>
            /// <returns></returns>
            private static char ToChar(int n)
            {
                if (n >= 36) n = n % 36;
                else if (n < 0) n = n % 36 + 36;

                if (n > 9) return (char)(n - 10 + 'a');
                else return (char)(n + '0');
            }


            ///// <summary>
            ///// "0-9 a-z A-Z"映射为 0到9 10到35 36到61
            ///// </summary>
            ///// <param name="c"></param>
            ///// <returns></returns>
            //public static int ToInt(char c)
            //{
            //    if ('0' <= c && c <= '9') return c - '0';
            //    else if ('a' <= c && c <= 'z') return 10 + c - 'a';
            //    else if ('A' <= c && c <= 'Z') return 36 + c - 'A';
            //    else return 0;
            //}

            ///// <summary>
            ///// 0到61依次映射为"0-9 a-z A-Z"，
            ///// 超出61取余数映射
            ///// </summary>
            ///// <param name="n"></param>
            ///// <returns></returns>
            //public static char ToChar(int n)
            //{
            //    if (n >= 62) n = n % 62;
            //    else if (n < 0) n = n % 62 + 62;

            //    if (n > 35) return (char)(n - 36 + 'A');
            //    else if (n > 9) return (char)(n - 10 + 'a');
            //    else return (char)(n + '0');
            //}
        }

        class MD5
        {

            #region MD5调用接口

            /// <summary>
            /// 计算data的MD5值
            /// </summary>
            public static string Encrypt(string data)
            {
                uint[] X = To16Array(data);
                string Str = ToNativeStr(X);

                return calculate(X);
            }

            /// <summary>
            /// 计算byte数组的MD5值
            /// </summary>
            public static string Encrypt(byte[] Bytes)
            {
                uint[] X = To16Array(Bytes);
                return calculate(X);
            }

            /// <summary>
            /// 计算文件的MD5值
            /// </summary>
            public static string Encrypt(FileInfo file)
            {
                uint[] X = To16Array(file);
                return calculate(X);
            }

            #endregion


            #region MD5计算逻辑

            /// <summary>
            /// 转化byte数组为uint数组，数组长度为16的倍数
            /// 
            /// 1、字符串转化为字节数组，每4个字节转化为一个uint，依次存储到uint数组
            /// 2、附加0x80作为最后一个字节
            /// 3、在uint数组最后位置记录文件字节长度信息
            /// </summary>
            public static uint[] To16Array(byte[] Bytes)
            {
                uint DataLen = (uint)Bytes.Length;

                // 计算FileLen对应的uint长度（要求为16的倍数、预留2个uint、最小为16）
                uint ArrayLen = (((DataLen + 8) / 64) + 1) * 16;
                uint[] Array = new uint[ArrayLen];

                uint ArrayPos = 0;
                int pos = 0;
                uint ByteCount = 0;
                for (ByteCount = 0; ByteCount < DataLen; ByteCount++)
                {
                    // 每4个Byte转化为1个uint
                    ArrayPos = ByteCount / 4;
                    pos = (int)(ByteCount % 4) * 8;
                    Array[ArrayPos] = Array[ArrayPos] | ((uint)Bytes[ByteCount] << pos);
                }

                // 附加0x80作为最后一个字节，添加到uint数组对应位置
                ArrayPos = ByteCount / 4;
                pos = (int)(ByteCount % 4) * 8;
                Array[ArrayPos] = Array[ArrayPos] | ((uint)0x80 << pos);

                // 记录总长度信息
                Array[ArrayLen - 2] = (DataLen << 3);
                Array[ArrayLen - 1] = (DataLen >> 29);

                return Array;
            }

            /// <summary>
            /// 转化字符串为uint数组，数组长度为16的倍数
            /// 
            /// 1、字符串转化为字节数组，每4个字节转化为一个uint，依次存储到uint数组
            /// 2、附加0x80作为最后一个字节
            /// 3、在uint数组最后位置记录文件字节长度信息
            /// </summary>
            public static uint[] To16Array(string data)
            {
                byte[] datas = System.Text.Encoding.Default.GetBytes(data);
                return To16Array(datas);
            }

            /// <summary>
            /// 转化文件为uint数组，数组长度为16的倍数
            /// 
            /// 1、读取文件字节信息，每4个字节转化为一个uint，依次存储到uint数组
            /// 2、附加0x80作为最后一个字节
            /// 3、在uint数组最后位置记录文件字节长度信息
            /// </summary>
            public static uint[] To16Array(FileInfo info)
            {
                FileStream fs = new FileStream(info.FullName, FileMode.Open);// 读取方式打开，得到流
                int SIZE = 1024 * 1024 * 10;        // 10M缓存
                byte[] datas = new byte[SIZE];      // 要读取的内容会放到这个数组里
                int countI = 0;
                long offset = 0;

                // 计算FileLen对应的uint长度（要求为16的倍数、预留2个uint、最小为16）
                uint FileLen = (uint)info.Length;
                uint ArrayLen = (((FileLen + 8) / 64) + 1) * 16;
                uint[] Array = new uint[ArrayLen];

                int pos = 0;
                uint ByteCount = 0;
                uint ArrayPos = 0;
                while (ByteCount < FileLen)
                {
                    if (countI == 0)
                    {
                        fs.Seek(offset, SeekOrigin.Begin);// 定位到指定字节
                        fs.Read(datas, 0, datas.Length);

                        offset += SIZE;
                    }

                    // 每4个Byte转化为1个uint
                    ArrayPos = ByteCount / 4;
                    pos = (int)(ByteCount % 4) * 8;
                    Array[ArrayPos] = Array[ArrayPos] | ((uint)datas[countI] << pos);

                    ByteCount = ByteCount + 1;

                    countI++;
                    if (countI == SIZE) countI = 0;
                }

                // 附加0x80作为最后一个字节，添加到uint数组对应位置
                ArrayPos = ByteCount / 4;
                pos = (int)(ByteCount % 4) * 8;
                Array[ArrayPos] = Array[ArrayPos] | ((uint)0x80 << pos);

                // 记录总长度信息
                Array[ArrayLen - 2] = (FileLen << 3);
                Array[ArrayLen - 1] = (FileLen >> 29);

                fs.Close();
                return Array;
            }

            /// <summary>
            /// 将X数组还原为原有字符串
            /// </summary>
            private static String ToNativeStr(uint[] X)
            {
                List<byte> list = new List<byte>();

                for (int i = 0; i < 8; i++)
                {
                    uint n = X[i];
                    for (int j = 0; j < 4; j++)
                    {
                        byte b = (byte)(n & 0xFF);
                        list.Add(b);
                        n = n >> 8;
                    }
                }
                byte[] Bytes = list.ToArray();
                String Str = System.Text.Encoding.Default.GetString(Bytes);
                return Str;
            }

            private static uint F(uint x, uint y, uint z)
            {
                return (x & y) | ((~x) & z);
            }
            private static uint G(uint x, uint y, uint z)
            {
                return (x & z) | (y & (~z));
            }

            // 0^0^0 = 0
            // 0^0^1 = 1
            // 0^1^0 = 1
            // 0^1^1 = 0
            // 1^0^0 = 1
            // 1^0^1 = 0
            // 1^1^0 = 0
            // 1^1^1 = 1
            private static uint H(uint x, uint y, uint z)
            {
                return (x ^ y ^ z);
            }
            private static uint I(uint x, uint y, uint z)
            {
                return (y ^ (x | (~z)));
            }

            // 循环左移
            private static uint RL(uint x, int y)
            {
                y = y % 32;
                return (x << y) | (x >> (32 - y));
            }

            private static void md5_FF(ref uint a, uint b, uint c, uint d, uint x, int s, uint ac)
            {
                //uint a1 = a;    // 1732584193   4023233417

                uint f = F(b, c, d);    // 2562383102
                a = x + ac + a + f;     // 3614090487

                a = RL(a, s);           // 3042081771
                a = a + b;

                //md5_FF2(ref a1, b, c, d, x, s, ac);

                //if (a != a1)    // 2770347892 2770347893
                //    MessageBox.Show("");

            }

            private static void md5_FF2(ref uint a, uint b, uint c, uint d, uint x, int s, uint ac)
            {
                uint f = F(b, c, d);    // 2562383102
                a = x + ac + a + f;     // 3614090487

                uint b2 = RL(b, 32 - s);    // 333421399
                a = a + b2;

                a = RL(a, s);
            }

            private static void md5_GG(ref uint a, uint b, uint c, uint d, uint x, int s, uint ac)
            {
                uint g = G(b, c, d);
                a = x + ac + a + g;

                a = RL(a, s);
                a = a + b;
            }
            private static void md5_HH(ref uint a, uint b, uint c, uint d, uint x, int s, uint ac)
            {
                uint h = H(b, c, d);
                a = x + ac + a + h;

                a = RL(a, s);
                a = a + b;
            }
            //uint a = 350508294;
            //uint b = 4283343851;
            //uint c = 1846584438;
            //uint d = 1917254355;
            private static void md5_II(ref uint a, uint b, uint c, uint d, uint x, int s, uint ac)
            {
                //uint xa = x + a;    // 806551938

                uint i = I(b, c, d);// 2448360345
                a = x + ac + a + i; // 2911426732

                a = RL(a, s);       // 362131739
                a = a + b;
            }

            private static string RHex(uint n)
            {
                string S = Convert.ToString(n, 16);
                return ReOrder(S);
            }

            // 16进制串重排序 67452301 -> 01234567
            private static string ReOrder(String S)
            {
                string T = "";
                for (int i = S.Length - 2; i >= 0; i = i - 2)
                {
                    if (i == -1) T = T + "0" + S[i + 1];
                    else T = T + "" + S[i] + S[i + 1];
                }
                return T;
            }


            /// <summary>
            /// 对长度为16倍数的uint数组，执行md5数据摘要，输出md5信息
            /// </summary>
            public static string calculate(uint[] x)
            {
                //uint time1 = DateTime.Now.Ticks;

                // 7   12  17   22
                // 5   9   14   20
                // 4   11  16   23
                // 6   10  15   21
                const int S11 = 7;
                const int S12 = 12;
                const int S13 = 17;
                const int S14 = 22;
                const int S21 = 5;
                const int S22 = 9;
                const int S23 = 14;
                const int S24 = 20;
                const int S31 = 4;
                const int S32 = 11;
                const int S33 = 16;
                const int S34 = 23;
                const int S41 = 6;
                const int S42 = 10;
                const int S43 = 15;
                const int S44 = 21;

                uint a = 0x67452301;
                uint b = 0xEFCDAB89;
                uint c = 0x98BADCFE;
                uint d = 0x10325476;

                for (int k = 0; k < x.Length; k += 16)
                {
                    uint AA = a;
                    uint BB = b;
                    uint CC = c;
                    uint DD = d;

                    md5_FF(ref a, b, c, d, x[k + 0], S11, 0xD76AA478);  // 3604027302
                    md5_FF(ref d, a, b, c, x[k + 1], S12, 0xE8C7B756);  // 877880356
                    md5_FF(ref c, d, a, b, x[k + 2], S13, 0x242070DB);  // 2562383102
                    md5_FF(ref b, c, d, a, x[k + 3], S14, 0xC1BDCEEE);
                    md5_FF(ref a, b, c, d, x[k + 4], S11, 0xF57C0FAF);
                    md5_FF(ref d, a, b, c, x[k + 5], S12, 0x4787C62A);
                    md5_FF(ref c, d, a, b, x[k + 6], S13, 0xA8304613);
                    md5_FF(ref b, c, d, a, x[k + 7], S14, 0xFD469501);
                    md5_FF(ref a, b, c, d, x[k + 8], S11, 0x698098D8);
                    md5_FF(ref d, a, b, c, x[k + 9], S12, 0x8B44F7AF);
                    md5_FF(ref c, d, a, b, x[k + 10], S13, 0xFFFF5BB1);
                    md5_FF(ref b, c, d, a, x[k + 11], S14, 0x895CD7BE);
                    md5_FF(ref a, b, c, d, x[k + 12], S11, 0x6B901122);
                    md5_FF(ref d, a, b, c, x[k + 13], S12, 0xFD987193);
                    md5_FF(ref c, d, a, b, x[k + 14], S13, 0xA679438E);
                    md5_FF(ref b, c, d, a, x[k + 15], S14, 0x49B40821); //3526238649
                    md5_GG(ref a, b, c, d, x[k + 1], S21, 0xF61E2562);
                    md5_GG(ref d, a, b, c, x[k + 6], S22, 0xC040B340);  //1572812400
                    md5_GG(ref c, d, a, b, x[k + 11], S23, 0x265E5A51);
                    md5_GG(ref b, c, d, a, x[k + 0], S24, 0xE9B6C7AA);
                    md5_GG(ref a, b, c, d, x[k + 5], S21, 0xD62F105D);
                    md5_GG(ref d, a, b, c, x[k + 10], S22, 0x2441453);
                    md5_GG(ref c, d, a, b, x[k + 15], S23, 0xD8A1E681);
                    md5_GG(ref b, c, d, a, x[k + 4], S24, 0xE7D3FBC8);
                    md5_GG(ref a, b, c, d, x[k + 9], S21, 0x21E1CDE6);
                    md5_GG(ref d, a, b, c, x[k + 14], S22, 0xC33707D6);
                    md5_GG(ref c, d, a, b, x[k + 3], S23, 0xF4D50D87);
                    md5_GG(ref b, c, d, a, x[k + 8], S24, 0x455A14ED);
                    md5_GG(ref a, b, c, d, x[k + 13], S21, 0xA9E3E905);
                    md5_GG(ref d, a, b, c, x[k + 2], S22, 0xFCEFA3F8);
                    md5_GG(ref c, d, a, b, x[k + 7], S23, 0x676F02D9);
                    md5_GG(ref b, c, d, a, x[k + 12], S24, 0x8D2A4C8A);
                    md5_HH(ref a, b, c, d, x[k + 5], S31, 0xFFFA3942);  // 3750198684 2314002400 1089690627 990001115 0 4 -> 2749600077
                    md5_HH(ref d, a, b, c, x[k + 8], S32, 0x8771F681);  // 990001115
                    md5_HH(ref c, d, a, b, x[k + 11], S33, 0x6D9D6122); // 1089690627
                    md5_HH(ref b, c, d, a, x[k + 14], S34, 0xFDE5380C); // 2314002400
                    md5_HH(ref a, b, c, d, x[k + 1], S31, 0xA4BEEA44);  // 555633090
                    md5_HH(ref d, a, b, c, x[k + 4], S32, 0x4BDECFA9);
                    md5_HH(ref c, d, a, b, x[k + 7], S33, 0xF6BB4B60);
                    md5_HH(ref b, c, d, a, x[k + 10], S34, 0xBEBFBC70);
                    md5_HH(ref a, b, c, d, x[k + 13], S31, 0x289B7EC6);
                    md5_HH(ref d, a, b, c, x[k + 0], S32, 0xEAA127FA);
                    md5_HH(ref c, d, a, b, x[k + 3], S33, 0xD4EF3085);
                    md5_HH(ref b, c, d, a, x[k + 6], S34, 0x4881D05);
                    md5_HH(ref a, b, c, d, x[k + 9], S31, 0xD9D4D039);
                    md5_HH(ref d, a, b, c, x[k + 12], S32, 0xE6DB99E5);
                    md5_HH(ref c, d, a, b, x[k + 15], S33, 0x1FA27CF8);
                    md5_HH(ref b, c, d, a, x[k + 2], S34, 0xC4AC5665);  // 1444303940


                    md5_II(ref a, b, c, d, x[k + 0], S41, 0xF4292244);  // 808311156
                    md5_II(ref d, a, b, c, x[k + 7], S42, 0x432AFF97);

                    md5_II(ref c, d, a, b, x[k + 14], S43, 0xAB9423A7);
                    md5_II(ref b, c, d, a, x[k + 5], S44, 0xFC93A039);

                    md5_II(ref a, b, c, d, x[k + 12], S41, 0x655B59C3);
                    md5_II(ref d, a, b, c, x[k + 3], S42, 0x8F0CCC92);

                    md5_II(ref c, d, a, b, x[k + 10], S43, 0xFFEFF47D);
                    md5_II(ref b, c, d, a, x[k + 1], S44, 0x85845DD1);

                    md5_II(ref a, b, c, d, x[k + 8], S41, 0x6FA87E4F);
                    md5_II(ref d, a, b, c, x[k + 15], S42, 0xFE2CE6E0);

                    md5_II(ref c, d, a, b, x[k + 6], S43, 0xA3014314);
                    md5_II(ref b, c, d, a, x[k + 13], S44, 0x4E0811A1);

                    md5_II(ref a, b, c, d, x[k + 4], S41, 0xF7537E82);
                    md5_II(ref d, a, b, c, x[k + 11], S42, 0xBD3AF235);
                    md5_II(ref c, d, a, b, x[k + 2], S43, 0x2AD7D2BB);
                    md5_II(ref b, c, d, a, x[k + 9], S44, 0xEB86D391);  // 4120542881
                                                                        // 350508294 4283343851 1846584438 1917254355
                    a = a + AA; //3844921825
                    b = b + BB;
                    c = c + CC;
                    d = d + DD;
                }

                string MD5 = RHex(a) + RHex(b) + RHex(c) + RHex(d);

                //uint time2 = DateTime.Now.Ticks;
                //MessageBox.Show("MD5计算耗时：" + ((time2 - time1) / 10000000f) + "秒");

                return MD5;
            }

            #endregion
        }
        public class Encoder
        {
            public static void example()
            {
                String data = "test encode";
                string encode = Encode(data);
                string decode = Decode(encode);
                bool b = data.Equals(decode);
                bool b2 = b;
            }

            /// <summary>  
            /// 转码data为全字母串，并添加前缀  
            /// </summary>
            public static string Encode(string data)
            {
                string str = data;
                if (!data.StartsWith("ALPHABETCODE@"))
                {
                    str = "ALPHABETCODE@" + EncodeAlphabet(data);
                }
                return str;
            }


            /// <summary>  
            /// 解析字母串为原有串  
            /// </summary>  
            public static string Decode(string data)
            {
                string str = data;
                if (data.StartsWith("ALPHABETCODE@"))
                {
                    str = DecodeAlphabet(data.Substring("ALPHABETCODE@".Length));
                }
                return str;
            }

         

            #region 字符串字母编码逻辑

            /// <summary>  
            /// 转化为字母字符串  
            /// </summary>  
            public static string EncodeAlphabet(string data)
            {
                byte[] B = Encoding.UTF8.GetBytes(data);
                return ToStr(B);
            }

            /// <summary>  
            /// 每个字节转化为两个字母  
            /// </summary>  
            private static string ToStr(byte[] B)
            {
                StringBuilder Str = new StringBuilder();
                foreach (byte b in B)
                {
                    Str.Append(ToStr(b));
                }
                return Str.ToString();
            }

            private static string ToStr(byte b)
            {
                return "" + ToChar(b / 16) + ToChar(b % 16);
            }

            private static char ToChar(int n)
            {
                return (char)('a' + n);
            }

            /// <summary>  
            /// 解析字母字符串  
            /// </summary>  
            public static string DecodeAlphabet(string data)
            {
                byte[] B = ToBytes(data);
                return Encoding.UTF8.GetString(B);
            }

            /// <summary>  
            /// 解析字符串为Bytes数组
            /// </summary>  
            public static byte[] ToBytes(string data)
            {
                byte[] B = new byte[data.Length / 2];
                try
                {
                    char[] C = data.ToCharArray();

                    for (int i = 0; i < C.Length; i += 2)
                    {
                        byte b = ToByte(C[i], C[i + 1]);
                        B[i / 2] = b;
                    }
                }
                catch { }
                return B;
            }

            /// <summary>  
            /// 每两个字母还原为一个字节  
            /// </summary>  
            private static byte ToByte(char a1, char a2)
            {
                return (byte)((a1 - 'a') * 16 + (a2 - 'a'));
            }

            #endregion

        }
    }
}