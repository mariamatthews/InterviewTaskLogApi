using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InterviewTaskLogAPI.Objects
{
    /// <summary>
    /// OutputLog object class inherinting from InputLog.
    /// This is for protecting the original object from being directly saved to any actual storage. Futureproofing development.
    /// </summary>
    public class OutputLog : InputLog
    {
        //SameContent
    }
}