using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TalentsApi.Models
{
    public class ResultModel<T> where T : class
    {
        public T Data { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}
