using System;
using System.Collections.Generic;

namespace BusinessObject.Models
{
    public partial class Court
    {
        public Court()
        {
            CourtTypes = new HashSet<CourtType>();
            SubCourts = new HashSet<SubCourt>();
        }

        public int CourtId { get; set; }
        public int? CategoryId { get; set; }
        public int? UserId { get; set; }
        public string? CourtName { get; set; }
        public string? Address { get; set; }
        public string? CourtDescription { get; set; }
        public int? Quantity { get; set; }
        public double? PricePerHour { get; set; }
        public string? ImageUrl { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? Status { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool? IsFeatured { get; set; }
        public string? LinkMap { get; set; }

        public virtual Category? Category { get; set; }
        public virtual User? User { get; set; }
        public virtual ICollection<CourtType> CourtTypes { get; set; }
        public virtual ICollection<SubCourt> SubCourts { get; set; }
    }
}
