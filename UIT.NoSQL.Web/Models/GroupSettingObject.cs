using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UIT.NoSQL.Web.Models
{
    public class GroupSettingObject
    {
        public string SettingID { get; set; }
        public GroupObject Group { get; set; }
        public string GroupDescription { get; set; }
        public string WelcomeMessage { get; set; }
        
        //permission
        public List<GroupRoleObject> CanViewTopic { get; set; }
        public List<GroupRoleObject> CanPost { get; set; }
        public int CanJoinGroup { get; set; }

        //posting permission
        public List<GroupRoleObject> CanAttachFile { get; set; }
    }
}