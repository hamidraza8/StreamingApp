using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.SearchDTO
{
        public class VideoSearchRequest
        {
            public string? Name { get; set; }
            public string? Genre { get; set; }
            public string? Category { get; set; }
        }
}
