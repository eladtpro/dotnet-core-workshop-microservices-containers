using System;

namespace ApiGateway.API.Infrastructure
{
    internal class CorrelationTokenManager
    {
        internal static string GenerateToken()
        {
            return Guid.NewGuid().ToString();


            //return token.ToString();//var token = Guid.NewGuid().ToString();
            //var token = new StringBuilder();
            //token.Append(DateTime.Now.ToShortDateString().Replace("/", "."));
            //token.Append("-");
            //token.Append(DateTime.Now.ToShortTimeString());
            //token.Append("-");
            //token.Append(new Random().Next(0, 1000000).ToString("D6"));

            //return token.ToString();
        }
    }
}