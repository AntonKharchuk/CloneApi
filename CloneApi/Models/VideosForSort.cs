using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloneApi.Models
{
    public class VideosForSort
    {
        public string ChannelTitle { get; set; }
        public string VideoId { get; set; }
        public string VideoTitle { get; set; }
        public ulong Vievs { get; set; }
    }
}
