using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace HttpServer.CipherSystem
{
    public static class RSA
    {
        /// <summary>
        /// Генерирует RSA ключи
        /// </summary>
        /// <returns>Возвращает массив BigInteger {n, e, d, p, q, fi}</returns>
        public static BigInteger[] GeneratePair()
        {
            Random r = new Random();
            BigInteger p = FastPowFunc(r.Next() % 100000 + 5, r.Next() % 100000 + 5, BigInteger.Pow(Int64.MaxValue, 4));
            BigInteger q = FastPowFunc(r.Next() % 100000 + 5, r.Next() % 100000 + 5, BigInteger.Pow(Int64.MaxValue, 4));
            while (!Ferma(p))
            {
                p--;
            }

            while (!Ferma(q))
            {
                q--;
            }

            BigInteger n = BigInteger.Multiply(p, q);
            BigInteger fi = BigInteger.Multiply((p - 1), (q - 1));
            BigInteger e = Calculate_e(fi);
            BigInteger d = Evklid(fi, e);//Calculate_d(e, fi);
            //if (e_ == d) GeneratePair();

            BigInteger[] Pairs = new BigInteger[6] { n, e, d, p, q, fi };

            return Pairs;
        }

        private static bool Ferma(BigInteger x)
        {
            if (x == 2)
                return true;
            Random rand = new Random();
            for (int i = 0; i < 100; i++)
            {
                BigInteger a = (rand.Next() % (x - 2)) + 2;
                if (GCD(a, x) != 1)
                    return false;
                if (FastPowFunc(a, x - 1, x) != 1)
                    return false;
            }
            return true;
        }

        //зашифровать
        public static List<string> RSA_Endoce(string s, BigInteger e, BigInteger n)
        {
            List<string> result = new List<string>();

            //BigInteger bi;

            byte[] sBytes = Encoding.UTF8.GetBytes(s);

            for (int i = 0; i < sBytes.Length; i++)
            {
                int index = Convert.ToInt32(Convert.ToString(sBytes[i], 10));
                result.Add(FastPowFunc(index, e, n).ToString());

            }

            return result;
        }

        //расшифровать
        public static string RSA_Dedoce(List<string> input, BigInteger d, BigInteger n)
        {
            byte[] sBytes = new byte[input.Count];

            for (int i = 0; i < input.Count; i++)
            {

                if (input[i].Contains('→'))
                {
                    string fullInput = input[i].Replace("→", "") + input[i + 1];
                    int index = Convert.ToInt32(FastPowFunc(Convert.ToInt64(fullInput), d, n).ToString());
                    sBytes[i] = Convert.ToByte(Convert.ToString(index), 10);
                    i++;
                }
                else
                {
                    BigInteger bigint = BigInteger.Parse(input[i]);
                    long index = Convert.ToInt64(FastPowFunc(bigint, d, n).ToString());
                    sBytes[i] = Convert.ToByte(Convert.ToString(index), 10);
                }
            }

            return Encoding.UTF8.GetString(sBytes);
        }
        private static BigInteger GCD(BigInteger x, BigInteger y) //НОД
        {
            return y == 0 ? x : GCD(y, x % y);
        }

        // расширенный евклидов алгоритм

        private static BigInteger gcdExtended(BigInteger a, BigInteger b, BigInteger x, BigInteger y)
        {
            // Базовый вариант
            if (a == 0)
            {
                x = 0;

                y = 1;
                return b;
            }
            BigInteger x1 = 1, y1 = 1;
            BigInteger gcd = gcdExtended(b % a, a, x1, y1);
            x = y1 - (b / a) * x1;
            y = x1;
            return gcd;
        }

        private static BigInteger Evklid(BigInteger fi, BigInteger e)
        {

            BigInteger q, r, x1, x2, y1, y2;
            BigInteger[] mas = new BigInteger[2];
            if (e == 0)
            {
                mas[0] = 1;
                mas[1] = 0;
                if (Ferma(fi) && fi > 0)
                    return fi;
            }

            x2 = 1;
            x1 = 0;
            y2 = 0;
            y1 = 1;



            while (e > 0)
            {
                q = fi / e;
                r = fi - q * e;
                mas[0] = x2 - q * x1;
                mas[1] = y2 - q * y1;
                fi = e;
                e = r;
                x2 = x1;
                x1 = mas[0];
                y2 = y1;
                y1 = mas[1];
            }

            mas[0] = x2;
            mas[1] = y2;
            if (y2 < 0 && Ferma(fi - y2) && (fi - y2) > 0)
                return (fi - y2);
            return y2;
        }

        private static BigInteger Calculate_e(BigInteger fi)
        {
            List<BigInteger> maybe_e = new List<BigInteger>();
            Random r = new Random();
        begin:
            BigInteger begin = FastPowFunc(r.Next() % 100000 + 5, r.Next() % 100000 + 5, fi);
            if (begin < 6) goto begin;
            for (BigInteger i = begin; i > 5; i--)
                if (Ferma(i) && GCD(i, fi) == 1)
                {
                    maybe_e.Add(i);
                    if (maybe_e.Count > 1) break;
                }

            return maybe_e[r.Next() % maybe_e.Count];
        }

        private static BigInteger FastPowFunc(BigInteger Number, BigInteger Pow, BigInteger Mod)
        {
            BigInteger Result = 1;
            BigInteger Bit = Number % Mod;

            while (Pow > 0)
            {
                if ((Pow & 1) == 1)
                {
                    Result *= Bit;
                    Result %= Mod;
                }
                Bit *= Bit;
                Bit %= Mod;
                Pow >>= 1;
            }
            return Result;
        }
    }
}
