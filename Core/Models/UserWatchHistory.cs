using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Core.Models.VedioMetaData;

namespace Core.Models
{
    public class UserWatchHistory
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid VideoId { get; set; }
        public string vedioName { get; set; }
        public DateTime WatchedOn { get; set; }
        public TimeSpan WatchDuration { get; set; }
        public bool IsCompleted { get; set; }
        // Navigation Properties
        public User User { get; set; }
        public VideoMetadata Video { get; set; }
        public DateTime? Created { get; set; }
        public string? CreatedBy { get; set; } = string.Empty;
        public DateTime? Updated { get; set; }
        public string? UpdatedBy { get; set; } = string.Empty;
        public DateTime? Deleted { get; set; }
        public string? DeletedBy { get; set; } = string.Empty;
    }
}
