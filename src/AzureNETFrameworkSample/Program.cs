namespace AzureNETFrameworkSample
{
    class Program: CommonBaseClass
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
