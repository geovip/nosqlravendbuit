using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace UIT.NoSQL.Web.Models
{
    public class GroupModels
    {
        [Required]
        [Display(Name="Group Name")]
        public string GroupName { get; set; }
        public string Description { get; set; }
    }
}