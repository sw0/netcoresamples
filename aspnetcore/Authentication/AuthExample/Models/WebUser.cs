using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthExample.Models
{
    public class WebUser : IdentityUser
    {
        [MaxLength(20)]
        public string Wechat { get; set; }

        [MaxLength(36)]
        public string UnionId { get; set; }
    }
}
