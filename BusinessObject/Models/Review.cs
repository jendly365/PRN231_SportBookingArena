﻿using System;
using System.Collections.Generic;

namespace BusinessObject.Models
{
    public partial class Review
    {
        public int ReviewId { get; set; }
        public int? UserId { get; set; }
        public int? SubCourtId { get; set; }
        public int? Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime? ReviewDate { get; set; }

        public virtual SubCourt? SubCourt { get; set; }
        public virtual User? User { get; set; }
    }
}
