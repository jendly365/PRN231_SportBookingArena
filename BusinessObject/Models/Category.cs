using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BusinessObject.Models
{
    public partial class Category
    {
        public Category()
        {
            Courts = new HashSet<Court>();
        }

        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? Description { get; set; }


        [JsonIgnore]
        public virtual ICollection<Court> Courts { get; set; }
    }
}
