using System.Collections.Generic;

namespace Assets.DTOs
{
    public class SessionDto
    {
        public SessionDto(string sessionCode, int connectionPort, string connectionIp, List<string> players, string host)
        {
            SessionCode = sessionCode;
            ConnectionPort = connectionPort;
            ConnectionIp = connectionIp;
            Players = players;
            Host = host;
        }

        public string SessionCode { get; set; }
        public int ConnectionPort { get; set; }
        public string ConnectionIp { get; set; }
        public List<string> Players { get; set; }
        public string Host { get; set; }
    }
}
