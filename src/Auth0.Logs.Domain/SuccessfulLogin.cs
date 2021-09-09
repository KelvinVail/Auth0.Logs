using System;
using System.Runtime.Serialization;

namespace Auth0.Logs.Domain
{
    [DataContract]
    public class SuccessfulLogin
    {
        [DataMember(Name="date")]
        public DateTime Date { get; set; }

        [DataMember(Name="client_name")]
        public string ClientName { get; set; }

        [DataMember(Name="user_name")]
        public string UserName { get; set; }

        [DataMember(Name="_id")]
        public string Id { get; set; }
    }
}
