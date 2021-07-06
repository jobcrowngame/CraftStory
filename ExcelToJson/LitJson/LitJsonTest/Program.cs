using System;

namespace LitJsonTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            string json = @"
              {
                ""album"" : {
                  ""name""   : ""The Dark Side of the Moon"",
                  ""artist"" : ""Pink Floyd"",
                  ""year""   : 1973,
                  ""tracks"" : [
                    ""Speak To Me"",
                    ""Breathe"",
                    ""On The Run""
                  ]
                }
              }
            ";
            

        }
    }
}
