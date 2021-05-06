using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace RSA
{
    class RSAEncryptor
    {
        public long p { get; set; }
        public long q { get; set; }

        long PhiN;
        long n;
        long e;
        long d;

        public RSAEncryptor()
        {
            p = q = 0;
        }

        public void setP(long p)
        {
            if (isPrime(p))
            {
                this.p = p;
            }
            else
            {
                Console.WriteLine("A P nem prímszám, adj meg egy újat");
                setP(Int64.Parse(Console.ReadLine()));
            }

        }

        public void setQ(long q)
        {
            if (isPrime(q))
            {
                this.q = q;
                n = p * q;
                PhiN = (p - 1) * (q - 1);
            }
            else
            {
                Console.WriteLine("A Q nem prímszám, adj meg egy újat");
                setQ(Int64.Parse(Console.ReadLine()));
            }
        }


        
        /// <summary>
        /// Teszteli az E-t az Euklidészi algoritmussal, hogy a PhiN-el relatív prímek e. 
        /// </summary>
        /// <param name="e"></param>
        public void setE(int e)
        {
            if (Euclid(e, PhiN) == 1)
            {
                this.e = e;
                setD();
            }
            else
            {
                Console.WriteLine("Nem relatív prím, adj meg másik E-t");
                setE(Int32.Parse(Console.ReadLine()));
            }
            
        }
        /// <summary>
        /// Az e és a PhiN Inverz modolúját veszi, és az lesz a majdani D érték.
        /// </summary>
        public void setD()
        {
            this.d = ModInverse(e, PhiN); 
        }

        //Euklideszi Algoritmus
        public long Euclid(long a, long b)
        {
            if (b != 0)
            {
                return Euclid(b, a % b);
            }
            return a;
        }

        /// <summary>
        /// A megadott számokkal kiszámolja, hogy mennyi az inverz modolusa az 'a'-nak, ha a modulo értéke 'm'.
        /// </summary>
        /// <param name="a"> A szám alapja</param>
        /// <param name="m"> A modulos értéke.</param>
        /// <returns></returns>
        public long ModInverse(long a, long m)
        {
            long m0 = m;
            long x = 1;
            long y = 0;

            if (m == 1) return 0;

            while(a > 1)
            {
                long b = a / m;
                long c = m;
                m = a % m;
                a = c;
                c = y;
                y = x - b * y;
                x = c;
            }
            if (x < 0) x += m0;

            return x;
        }
        /// <summary>
        /// A kiszámolt c értékkel a tanult kínai maradéktételt számolja ki. A kapott érték lesz a visszafejtet szám. 
        /// </summary>
        /// <param name="m">Esetünkben a p és q értékeket tartalmazza egy tömbben.</param>
        /// <param name="c"></param>
        /// <returns></returns>
        public long KinaiMaradek( long[] m, long[] c)
        {
            long M = m[0];
            long[] Mi = new long[m.Length];
            long[] Yi = new long[m.Length];
            for (int i= 1; i<m.Length; i++)
            {
                M *= m[i];
            }

            long x = 0;
            for(int i = 0; i < c.Length; i++)
            {
                Mi[i] = M / m[i];
                Yi[i] = ModInverse(Mi[i], m[i]);
                x += c[i] * Yi[i] * Mi[i];
            }

            x = x % M;

            return x;
        }

        /// <summary>
        /// A függvény leteszteli, hogy a p vagy a q számunk prím e. Ezt egy 3 körös Miller Rabin teszten keresztül valósítja meg. Ha egy is false, a szám nem prím. 
        /// </summary>
        /// <param name="szam">A q illetve a p</param>
        /// <returns></returns>
        public bool isPrime(long szam)
        {
            if (szam % 2 == 0) return false;
            int[] a_numbers = { 3, 7, 8 };
            for(int i = 0; i<3; i++)
            {
                if(MillerRabin(szam, a_numbers[i]) == false) return false;
            }
            return true; ;
        }
        /// <summary>
        /// Miller rabin prímteszt megvalósítása. Követi a tanult metódust. Ha a szám nem teljesíti a feltételeket, akkor false értékkel tér vissza.
        /// </summary>
        /// <param name="szam">a tesztelendő szám</param>
        /// <param name="a">a gyorshatványozásnál az alapot szolgálja</param>
        /// <returns></returns>
        public bool MillerRabin(long szam, int a)
        {
            long m = szam - 1;

            if (szam <= 1 || szam == 4) return false;
            else if (szam <= 3) return true;

            do
            {
                m /= 2;
            } while (m % 2 == 0);

            var x = modPow(a, m, szam);

            if ( x == 1 ||x == szam - 1)
            {
                return true;
            }

            while( m != szam - 1)
            {
                x = (modPow((long)x, 2, szam));
                m = m * 2;

                if (x == 1) return false;
                if ((int) x == szam - 1) return true;
            }
            return false ;
        }
        /// <summary>
        /// A gyorshatványozást megvalósító képlet. Tulajdonképpen az (a^e) % m képletet valósítja meg. 
        /// </summary>
        /// <param name="a">alap</param>
        /// <param name="e">hatványkitevő</param>
        /// <param name="m">modulos</param>
        /// <returns></returns>
        public long modPow(long a, long e, long m)
        {
            long result = 1;
            long apow = a;
            while(e != 0)
            {
                if((e & 0x01) == 0x01)
                {
                    result = (result * apow) % m;
                    
                }
                e >>= 1;
                apow = (apow * apow) % m;
            }
            return result;
        }



        public long Encryptor(long input)
        {
            long output = modPow(input, e, n);
            return output;
        }

        public long Decryptor(long input)
        {
            
            long q_to_crt = modPow(input % q, d % (q-1), q);
            long p_to_crt = modPow(input % p, d % (p-1), p);

            long[] c = { p_to_crt, q_to_crt };
            long[] m = { p, q };

            return KinaiMaradek(m, c);
        }

        //TODO: A szöveges visszafejtés/titkosítás még hibás.
        /*public String textEncryptor(String input)
        {
            String titkositott_szoveg = String.Empty;

            foreach(char ch in input)
            {
                long output = modPow(ch, e, n);
                titkositott_szoveg += (char)output;
                
            }

            return titkositott_szoveg;
        }

        

        public String textDecryptor(String input)
        {
            String visszafejtett_szoveg = String.Empty;

            foreach (char ch in input)
            {

                long q_to_crt = modPow(ch % q, d % (q - 1), q);
                long p_to_crt = modPow(ch % p, d % (p - 1), p);

                long[] c = { p_to_crt, q_to_crt };
                long[] m = { p, q };

                visszafejtett_szoveg += (char)KinaiMaradek(m, c);
            }

            return visszafejtett_szoveg;
        }*/
    }
}
