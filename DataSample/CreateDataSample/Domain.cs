using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateDataSample
{
    public class Domain
    {
    }

    public class GroupRSS
    {
        public string GroupName { get; set; }
        public string LinkRSS { get; set; }
    }

    public class User
    {
        public string FullName { get; set; }
        public string Region { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
    }
    
}
