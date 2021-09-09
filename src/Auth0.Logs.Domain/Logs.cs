using System;
using System.Collections.Generic;
using System.Linq;
using Auth0.ManagementApi;
using Auth0.ManagementApi.Clients;
using Auth0.ManagementApi.Models;
using Auth0.ManagementApi.Paging;

namespace Auth0.Logs.Domain
{
    public class Logs
    {
        private readonly IManagementConnection _connection;
        private readonly string _token;
        private readonly LogsClient _client;
        private bool _moreAvailable = true;
        private string _checkpoint;

        public Logs(string token)
        {
            _connection = new HttpClientManagementConnection();
            _token = token;
            _client = new LogsClient(
                _connection,
                new Uri("https://societyoflloyds.eu.auth0.com/api/v2/"),
                GetAuthHeaders());
        }

        public async IAsyncEnumerable<LogEntry> GetSuccessfulLogins()
        {
            while (_moreAvailable)
            {
                await foreach (var log in GetNext100Logs())
                {
                    yield return log;
                }
            }
        }

        private static string MaxCheckpoint(IList<LogEntry> logs)
        {
            var check = "X";
            try
            {
                var maxDate = logs.Max(l => l.Date);
                check = logs.Where(l => l.Date == maxDate).Select(l => l.Id).First();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return check;
        }

        private Dictionary<string, string> GetAuthHeaders()
        {
            return new Dictionary<string, string>
            {
                { "Authorization", $"Bearer {_token}" },
            };
        }

        private async IAsyncEnumerable<LogEntry> GetNext100Logs()
        {
            var request = (_checkpoint is null)
                ? new GetLogsRequest { Sort = "date:1", Take = 100, Fields = "date,client_name,user_name", Query = "type:s" }
                : new GetLogsRequest { From = _checkpoint, Take = 100, Fields = "date,client_name,user_name", Query = "type:s" };

            var logs = (await _client.GetAllAsync(request, new PaginationInfo(0, 100))).ToList();
            _checkpoint = MaxCheckpoint(logs);
            _moreAvailable = logs.Any();
            if (!_moreAvailable) yield break;
            foreach (var log in logs.Where(x => x.Type == "s"))
            {
                yield return log;
            }
        }
    }
}
