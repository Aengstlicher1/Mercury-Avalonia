using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Mercury.Core.Models
{
    public class StreamingData
    {
        public string Title { get; init; } = "";
        public string Id { get; init; } = "";
        public int ViewCount { get; init; }

        public TimeSpan Duration { get; init; }
        public DateTime ExpiresAt { get; init; }
        public StreamInfo[] Streams { get; set; } = new StreamInfo[0];
    }
}
