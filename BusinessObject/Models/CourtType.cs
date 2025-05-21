using System;
using System.Collections.Generic;

namespace BusinessObject.Models
{
    public partial class CourtType
    {
        public CourtType()
        {
            Bookings = new HashSet<Booking>();
            SubCourts = new HashSet<SubCourt>();
        }

        public int CourtTypeId { get; set; }
        public int CourtId { get; set; }
        public string TypeName { get; set; } = null!;
        public string? Description { get; set; }

        public virtual Court Court { get; set; } = null!;
        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<SubCourt> SubCourts { get; set; }
    }
}
