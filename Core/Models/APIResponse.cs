using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class APIResponse
    {
            public int ApiCode { get; set; }
            public string? DisplayCode { get; set; }
            public required string DisplayMessage { get; set; }
            public string? CorrelationId { get; set; }
            public dynamic? Data { get; set; }
    }
}
