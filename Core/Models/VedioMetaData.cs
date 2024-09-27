using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class VedioMetaData
    {
        public class VideoMetadata
        {
            [Key]
            public Guid UUID { get; set; }
            public string? Name { get; set; }
            public string? Category { get; set; }
            public string? Genre { get; set; }
            public string? Filename { get; set; }
            public string? Url { get; set; }
            public DateTime? Created { get; set; }
            public string? CreatedBy { get; set; } = string.Empty;
            public DateTime? Updated { get; set; }
            public string? UpdatedBy { get; set; } = string.Empty;
            public DateTime? Deleted { get; set; }
            public string? DeletedBy { get; set; } = string.Empty;
            public ICollection<UserWatchHistory> WatchHistories { get; set; } = new List<UserWatchHistory>();
        }
    }
}
