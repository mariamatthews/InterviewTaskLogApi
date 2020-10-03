using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InterviewTaskLogAPI
{
    /// <summary>
    /// InputLog object class defining assumed functional requirements.
    /// </summary>
    public class InputLog
    {
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public string Content { get; set; }
    }
}