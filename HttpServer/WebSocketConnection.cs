using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace HttpServer
{
    public class WebSocketConnection
    {

        public bool FIN;
        public bool RSV1;
        public bool RSV2;
        public bool RSV3;
        public byte OPCODE;
        public byte[] LENGTH;
        public bool IsMasked;
        public byte[] MASK;
        public byte[] MESSAGE;
        public byte[] FULLFRAME;

        public WebSocketConnection(byte[] WebSocketRecieveMessage)
        {
            try
            {
                FULLFRAME = WebSocketRecieveMessage;
                string WSstringMessage = "";
                for (int i = 0; i < WebSocketRecieveMessage.Length; i++)
                    WSstringMessage += Convert.ToString(WebSocketRecieveMessage[i], 2) + " ";

                FIN = (WSstringMessage[0] == '1') ? true : false;
                RSV1 = (WSstringMessage[1] == '1') ? true : false;
                RSV2 = (WSstringMessage[2] == '1') ? true : false;
                RSV2 = (WSstringMessage[3] == '1') ? true : false;

                string opcode = "";
                for (int i = 0; i < 4; i++) opcode += WSstringMessage[i + 4].ToString();
                OPCODE = Convert.ToByte(opcode, 2);

                IsMasked = (WSstringMessage.Split(' ')[1][0] == '1') ? true : false;

                string sLength = "";
                for (int i = 1; i < 8; i++)
                {
                    try { sLength += WSstringMessage.Split(' ')[1][i]; }
                    catch { break; }
                }
                int Length = Convert.ToInt32(sLength, 2);
                if (Length < 126)
                {
                    LENGTH = BitConverter.GetBytes(Length);
                    MESSAGE = new byte[Length];

                    if (IsMasked)
                    {
                        MASK = new byte[4];
                        string sMask = "";
                        for (int i = 0; i < 4; i++)
                        {
                            for (int j = 0; j < 8; j++)
                            {
                                try { sMask += WSstringMessage.Split(' ')[i + 2][j].ToString(); }
                                catch { break; }
                            }
                            MASK[i] = Convert.ToByte(sMask, 2);
                            sMask = "";
                        }

                        for (int i = 0; i < Length; i++)
                            MESSAGE[i] = WebSocketRecieveMessage[i + 6];

                        for (int i = 0; i < Length; i++)
                            MESSAGE[i] = (byte)(MESSAGE[i] ^ MASK[i % 4]);

                    }
                    else
                    {
                        for (int i = 0; i < Length; i++)
                            MESSAGE[i] = WebSocketRecieveMessage[i + 2];
                    }

                }
                else if (Length == 126)
                {
                    sLength = "";
                    for (int i = 0; i < 2; i++)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            try { sLength += WSstringMessage.Split(' ')[i + 2][j].ToString(); }
                            catch { break; }
                        }
                    }
                    Length = Convert.ToInt32(sLength, 2);
                    LENGTH = BitConverter.GetBytes(Length);
                    MESSAGE = new byte[Length];

                    if (IsMasked)
                    {
                        MASK = new byte[4];
                        string sMask = "";
                        for (int i = 0; i < 4; i++)
                        {
                            for (int j = 0; j < 8; j++)
                            {
                                try { sMask += WSstringMessage.Split(' ')[i + 4][j].ToString(); }
                                catch { break; }
                            }
                            MASK[i] = Convert.ToByte(sMask, 2);
                            sMask = "";
                        }

                        for (int i = 0; i < Length; i++)
                            MESSAGE[i] = WebSocketRecieveMessage[i + 8];

                        for (int i = 0; i < Length; i++)
                            MESSAGE[i] = (byte)(MESSAGE[i] ^ MASK[i % 4]);

                    }
                    else
                    {
                        for (int i = 0; i < Length; i++)
                            MESSAGE[i] = WebSocketRecieveMessage[i + 4];
                    }

                }
            }
            catch
            {
                string s = "";
                for (int i = 0; i < WebSocketRecieveMessage.Length; i++) s += Convert.ToString(WebSocketRecieveMessage[i], 2);
                Console.WriteLine(s);
            }
        }

        public WebSocketConnection(bool fin, bool rsv1, bool rsv2, bool rsv3, byte opcode, byte[] mask, int length, byte[] message)
        {
            string WSMessage = "";

            WSMessage += (fin) ? "1" : "0";
            WSMessage += (rsv1) ? "1" : "0";
            WSMessage += (rsv2) ? "1" : "0";
            WSMessage += (rsv3) ? "1" : "0";

            string sOpcode = Convert.ToString(opcode, 2);

            while (sOpcode.Length < 4)
            {
                sOpcode = "0" + sOpcode;
            }
            for (int i = 0; i < sOpcode.Length; i++)
                WSMessage += sOpcode[i];
            //int Len = 4 - sOpcode.Length;
            //int LenOffset = 0;
            //for (int i = 0; i < 4; i++)
            //{   if (Len != 0)
            //    {
            //        WSMessage += "0";
            //        Len--;
            //        LenOffset++;
            //    }
            //    else WSMessage += sOpcode[i- LenOffset];
            //}

            WSMessage += " ";

            bool IsMasked = false;
            for (int i = 0; i < 4; i++)
            {
                try
                {
                    if (mask[i] != 0x0)
                        IsMasked = true;
                }
                catch { break; }
            }

            WSMessage += (IsMasked) ? "1" : "0";

            string sLength = Convert.ToString(length, 2);
            string formatLength = "";
            for (int i = 0; i < sLength.Length; i++)
            {
                if (i == 7 || (i % 8 == 0 && i > 15) && i != 0) formatLength += " ";
                formatLength += sLength[i].ToString();
            }

            if (formatLength.Length < 7)
                while (formatLength.Length < 7)
                    formatLength = "0" + formatLength;

            if (length < 126)
            {
                for (int i = 0; i < formatLength.Length; i++)
                    WSMessage += formatLength[i].ToString();
                WSMessage += " ";
            }


            if (length >= 126 && length < 65536)
            {
                for (int i = 0; i < 6; i++)
                    WSMessage += "1";
                WSMessage += "0";
                WSMessage += " ";

                for (int i = 0; i < formatLength.Length; i++)
                    WSMessage += formatLength[i].ToString();
            }

            if (length >= 65536)
            {
                for (int i = 0; i < 7; i++)
                    WSMessage += "1";
                WSMessage += " ";

                for (int i = 0; i < formatLength.Length; i++)
                    WSMessage += formatLength[i].ToString();
            }

            for (int i = 0; i < message.Length; i += 2)
            {
                WSMessage += Convert.ToString(message[i], 2);
                try
                {
                    WSMessage += Convert.ToString(message[i + 1], 2);
                    WSMessage += " ";
                }
                catch { }
            }

            FULLFRAME = BinaryString2Bytes(WSMessage.Replace(" ", ""));

            /* if (length < 126)
             {
                 string sLength = Convert.ToString(length, 2);
                 for (int i = 0; i < sLength.Length; i++)
                     WSMessage += sLength[i].ToString();
                 WSMessage += " ";
             }
             else if (length >= 126 && length < 65536)
             {
                 for (int i = 0; i < 6; i++)
                     WSMessage += "1";
                 WSMessage += "0";
                 WSMessage += " ";

                 string sLength = Convert.ToString(length, 2);
                 for (int i = 0; i < sLength.Length; i++)
                 {
                     if (i == 8)
                         WSMessage += " ";
                     WSMessage += sLength[i].ToString();
                 }
                 WSMessage += " ";
             }
             else if (length >= 65536)
             {
                 for (int i = 0; i < 7; i++)
                     WSMessage += "1";
                 WSMessage += " ";

                 string sLength = Convert.ToString(length, 2);
                 int bytes = 0;
                 for (int i = 0; i < sLength.Length; i++)
                 {
                     if (i == 8 || i == 16 || i == 24 || i == 32 || i == 40 || i == 48 || i == 56 || i == 64)
                     {
                         WSMessage += " ";
                     }
                     WSMessage += sLength[i].ToString();
                 }
                 WSMessage += " ";
             }

             if(IsMasked)
             {
                 for(int i=0;i<4;i++)
                 {
                     string sMask = Convert.ToString(mask[i], 2);
                     for(int j=0;i<8;j++)
                     {
                         try{  WSMessage += sMask[j];}
                         catch { break; }
                     }
                     WSMessage += " ";
                 }
             }*/
        }

        private byte[] BinaryString2Bytes(string bits)
        {
            var numOfBytes = (int)Math.Ceiling(bits.Length / 8m);
            var bytes = new byte[numOfBytes];
            var chunkSize = 8;

            for (int i = 1; i <= numOfBytes; i++)
            {
                var startIndex = bits.Length - 8 * i;
                if (startIndex < 0)
                {
                    chunkSize = 8 + startIndex;
                    startIndex = 0;
                }
                bytes[numOfBytes - i] = Convert.ToByte(bits.Substring(startIndex, chunkSize), 2);
            }
            return bytes;
        }

        public byte[] GetBytes()
        {
            return FULLFRAME;
        }

        public static byte[] Ping()
        {
            string sPing = "10001001 1 1";
            byte[] Ping = new byte[3];
            for (int i = 0; i < sPing.Split(' ').Length; i++)
                Ping[i] = Convert.ToByte(sPing.Split(' ')[i], 2);

            return Ping;
        }

        public byte[] Pong()
        {
            string sPong = "10001010 1 1";
            byte[] Pong = new byte[3];
            for (int i = 0; i < sPong.Split(' ').Length; i++)
                Pong[i] = Convert.ToByte(sPong.Split(' ')[i], 2);

            return Pong;
        }

        public static byte[] Close()
        {
            string sClose = "10001000";
            byte[] Close = new byte[3];
            for (int i = 0; i < sClose.Split(' ').Length; i++)
                Close[i] = Convert.ToByte(sClose.Split(' ')[i], 2);

            return Close;
        }

        public void LogToConsole()
        {
            Console.WriteLine($"FIN:{FIN}");
            Console.WriteLine($"RSV1:{RSV1}");
            Console.WriteLine($"RSV2:{RSV2}");
            Console.WriteLine($"RSV3:{RSV3}");
            Console.WriteLine($"OPCODE:{OPCODE}");
            Console.WriteLine($"IsMasked:{IsMasked}");
        }

        public byte[] Recieve(ref Socket client)
        {
            return null;
        }

        public void Send(byte[] data, bool IsEndFrame = true, bool IsUsingMask = false)
        {
            int length = data.Length;
            byte[] dataToSend = new byte[data.Length + 2];

            if (IsEndFrame) dataToSend[0] = 0x81;
            else dataToSend[0] = 0x01;

            if (length == 126) { }
            if (length == 127) { }
            if (length > 127) { }

        }

        public void Send(Socket client, byte[] webSocketFrame)
        {
            client.Send(webSocketFrame);
        }
    }
}
//RFC 6455
//0                   1                   2                   3
//    0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
//   + -+-+-+-+-------+-+-------------+-------------------------------+
//   | F | R | R | R | опкод | М | Длина тела | Расширенная длина тела     |
//   |I|S|S|S|(4бита)| А | (7бит)    | (1 байт)           |

//| N | V | V | V |       | С |             | (если длина тела==126 или 127) |
//   | | 1 | 2 | 3 |       | К |             |                               |
//   | | | | |       | А |             |                               |
//   +-+-+-+-+-------+-+-------------+- - - - - - - - - - - - - - -+
//   | Продолжение расширенной длины тела, если длина тела = 127    |
//   + - - - - - - - - - - - - - - - +-------------------------------+
//   |                               |  Ключ маски, если МАСКА = 1   |
//   +-------------------------------+-------------------------------+
//   | Ключ маски (продолжение)      | Данные фрейма("тело") |
//+--------------------------------- - - - - - - - - - - - - - -+
//   :                     Данные продолжаются ...                   :
//   +- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -+
//   | Данные продолжаются...                   |
//+---------------------------------------------------------------+
