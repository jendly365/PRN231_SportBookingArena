﻿using System;
using System.Collections.Generic;

namespace BusinessObject.Models
{
    public partial class Role
    {
        public Role()
        {
            Users = new HashSet<User>();
        }

        public int RoleId { get; set; }
        public string? RoleName { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
