using System;

namespace RSA
{
    class Program
    {
        public static void Main(string[] args)
        {
            RSAEncryptor test = new RSAEncryptor();
            //541, 1009, 1409
            
            Console.WriteLine("Adj meg egy p értéket");
            test.setP(Int32.Parse(Console.ReadLine()));

            Console.WriteLine("Adj meg egy q értéket");
            test.setQ(Int32.Parse(Console.ReadLine()));

            Console.WriteLine("Adj meg egy e értéket");
            test.setE(Int32.Parse(Console.ReadLine()));

             Console.WriteLine("Add meg a titkosítandó számot");
             long titkositando = long.Parse(Console.ReadLine());
             long tikosított = test.Encryptor(titkositando);

             Console.WriteLine("A tiktosított szám: " + tikosított);


             Console.WriteLine( titkositando + " ?= "+test.Decryptor(tikosított));

            /*String titkositando = Console.ReadLine();
            String titkositott = test.textEncryptor(titkositando);

            Console.WriteLine(titkositando + " =? " + test.textDecryptor(titkositott));*/
        }
    }
}
