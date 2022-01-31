using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TalentsApi.Models.User
{
    public class ToggleLockModel
    {
        public int Id { get; set; }
        public bool IsLocked { get; set; }
    }
}
