using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDRAC_IPMI.DTOs
{
    public class Credentials
    {
        private string _ip = "192.168.0.120";
        public string Ip
        {
            get => _ip;
            set => _ip = value;
        }

        private string _username = "root";
        public string Username
        {
            get => _username;
            set => _username = value;
        }
        
        private string _password = "calvin";
        public string Password
        {
            get => _password;
            set => _password = value;
        }
    }
}
