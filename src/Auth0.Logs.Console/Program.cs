using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Auth0.Logs.Console
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var token = "[Auth0Token]";
            var client = new Domain.Logs(token);

            await using var file = new StreamWriter(@"path");
            await foreach (var log in client.GetSuccessfulLogins())
            {
                var values = new List<string>
                {
                    log.UserName,
                    log.ClientName,
                    $"{log.Date:yyyy-MM-dd hh:mm:ss}",
                    log.Type,
                    log.Id,
                };

                await file.WriteLineAsync(string.Join(",", values));
            }
        }
    }
}
