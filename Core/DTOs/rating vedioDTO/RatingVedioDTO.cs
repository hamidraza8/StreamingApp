using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.rating_vedioDTO
{
    public  class RatingVedioDTO
    {
            public Guid VideoId { get; set; }
            public Guid UserId { get; set; }
            public int Rating { get; set; }
    }
}
