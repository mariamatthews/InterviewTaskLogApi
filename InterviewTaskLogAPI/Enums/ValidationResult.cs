using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace InterviewTaskLogAPI.Enums
{
    public enum ValidationResult
    {
        [Description("Unknown error has occured")]
        UNKNOWN_ERROR,
        [Description("valid")]
        VALID,
        [Description("Date was not provided")]
        DATE_MISSING,
        [Description("ID was not provided/was 0")]
        ID_MISSING,
        [Description("The Log provided is null")]
        INPUT_LOG_NULL,
        [Description("The Content of the log is longer than 255 characters")]
        CONTENT_LONGER_THAN_255,
        [Description("Cannot save file")]
        CANNOT_SAVE_FILE,

    }

    public static class EnumerationExtension
    {
        public static string Description(this Enum value)
        {
            // get attributes  
            var field = value.GetType().GetField(value.ToString());
            var attributes = field.GetCustomAttributes(false);

            // Description is in a hidden Attribute class called DisplayAttribute
            // Not to be confused with DisplayNameAttribute
            dynamic displayAttribute = null;

            if (attributes.Any())
            {
                displayAttribute = attributes.ElementAt(0);
            }

            // return description
            return displayAttribute?.Description ?? "Description Not Found";
        }
    }
}
