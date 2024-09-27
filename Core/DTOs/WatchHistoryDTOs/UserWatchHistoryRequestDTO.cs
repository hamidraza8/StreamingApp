using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Core.Models.VedioMetaData;

namespace Core.DTOs.WatchHistoryDTOs
{
    public class UserWatchHistoryRequestDTO
    {
        public Guid? Id { get; set; }
        public Guid UserId { get; set; }
        public Guid VideoId { get; set; }
        public string? vedioName { get; set; }
        public DateTime WatchedOn { get; set; }
        public TimeSpan WatchDuration { get; set; }
        public bool IsCompleted { get; set; }
    }
}
