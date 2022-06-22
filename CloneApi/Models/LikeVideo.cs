using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloneApi.Models
{
    public class LikeVideo
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string ChannelId { get; set; }
        public string ChannelTitle { get; set; }
        public string VideolId { get; set; }
        public string VideoTitle { get; set; }
    }
}
