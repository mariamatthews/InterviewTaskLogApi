using InterviewTaskLogAPI.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InterviewTaskLogAPI.Objects
{
    /// <summary>
    /// CustomError object class for custom exceptions handling.
    /// </summary>
    public class CustomError
    {
        // constructor to combine unhandled exception as unknown errors
        public CustomError()
        {
            this.ID = 0;
            this.ErrorCode = 0; // Unknown error
            this.ErrorSummary = "";
            this.Severity = "CRITICAL";
            this.ErrorDate = DateTime.Now;
            this.CustomErrorText = ValidationResult.UNKNOWN_ERROR.ToString();
            this.RawErrorData = null;
        }
        public int ID { get; set; }
        public int ErrorCode { get; set; }
        public string Severity { get; set; }
        public string ErrorSummary { get; set; }
        public DateTime ErrorDate { get; set; }
        public string CustomErrorText { get; set; }
        public object RawErrorData { get; set; }
    }
}
