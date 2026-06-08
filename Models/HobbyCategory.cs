using System;
using System.Collections.Generic;

namespace HobbyTracker.Models
{
    public class HobbyCategory
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<HobbyItem> Items { get; set; } = new List<HobbyItem>();
    }
}