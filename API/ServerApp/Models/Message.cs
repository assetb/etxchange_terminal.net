using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServerApp.Models
{
    public class Message<T>
    {
        public int code { get; set; }
        public string description { get; set; }
        public Dictionary<string, object> responseParams { get; set; } = new Dictionary<string, object>();
        public T data { get; set; }
    }
}