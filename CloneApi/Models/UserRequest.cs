using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloneApi.Models
{
    public class UserRequest
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Request { get; set; }
        public string Time { get; set; }
        public string UserName { get; set; }
    }
}
