using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication4.Models
{
    public class VisitSchedule
    {
        public int VisitId { get; set; }
        public int PropertyId { get; set; }
        public int UserId { get; set; }
        public DateTime VisitDate { get; set; }
    }

}