﻿using System.IO;
namespace AzureNETFrameworkSample
{
    class Program : CommonBaseClass //even if you do not extend from the CommonBaseClass, this code will execute normally
    {
        string AzureSample()
        {
            string fileName = @"C:\iotest\testfile.txt";

            return File.ReadAllText(fileName);
        }

        static void Main(string[] args)
        {
            System.Console.Write( (new Program()).AzureSample() );
            System.Console.ReadKey();
        }
    }
}
