using InterviewTaskLogAPI.Enums;
using InterviewTaskLogAPI.Objects;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace InterviewTaskLogAPI.Logic
{
    /// <summary>
    /// Due to simplicity of a single operation I have put all logic in the Business Logic class
    /// As the product would grow those methods would be delegated to their own classes
    /// </summary>
    public class BusinessLogic : IBusinessLogic
    {
        /// <summary>
        /// Main method called by the controller during POST action. Processes the log file provided to the API
        /// </summary>
        /// <param name="inputLog"></param>
        /// <returns>True/False boolean to confirm success or failure of execution</returns>
        public bool ProcessLog(InputLog inputLog)
        {
            try // to process the log provided
            {
                var validationResult = ValidateLog(inputLog);

                switch(validationResult)
                {
                    case ValidationResult.VALID:
                        var validOutputLog = new OutputLog { ID = inputLog.ID, Date = inputLog.Date, Content = inputLog.Content };
                        return SaveFile(validOutputLog);

                    case ValidationResult.ID_MISSING:
                    case ValidationResult.DATE_MISSING:
                        GenerateCustomError(validationResult, null, inputLog);
                        var warningOutputLog = new OutputLog { ID = inputLog.ID, Date = inputLog.Date, Content = inputLog.Content };
                        return SaveFile(warningOutputLog);
                    default:
                        GenerateCustomError(validationResult, null, inputLog);
                        break;
                }
            }
            catch (Exception e)
            {
                GenerateCustomError(ValidationResult.UNKNOWN_ERROR, e, inputLog);
            }

            return false;
        }
        /// <summary>
        /// Validates the log and assigns it appropiate ValidationResult enum value.
        /// </summary>
        /// <param name="inputLog">The log to validate.</param>
        /// <returns>ValidationResult enum value.</returns>
        private ValidationResult ValidateLog(InputLog inputLog)
        {
            if (inputLog.Content is null)
            {
                return ValidationResult.INPUT_LOG_NULL;
            }
            else if(inputLog.Date == default(DateTime))
            {
                return ValidationResult.DATE_MISSING;
            }
            else if (inputLog.ID == default(int))
            {
                return ValidationResult.ID_MISSING;
            }
            else if (inputLog.Content.Length > 255)
            {
                return ValidationResult.CONTENT_LONGER_THAN_255;
            }

            return ValidationResult.VALID;
        }
        /// <summary>
        /// Generates custom error based on ValidationResult and other conditions.
        /// </summary>
        /// <param name="exception">System/Default Exception when unhandled error happens.</param>
        /// <param name="invalidInputLog">Input log that has caused the issue.</param>
        /// <param name="validationResult">ValidationResult enum received.</param>
        /// <returns></returns>
        private bool GenerateCustomError(ValidationResult validationResult, Exception exception = null, InputLog invalidInputLog = null)
        {
            var customError = new CustomError(); //let constructor build default object

            // if input log is provided, copy the ID and date
            if (invalidInputLog != null)
            {
                customError.ID = invalidInputLog.ID;
                customError.ErrorDate = invalidInputLog.Date;
            }

            // provide the content in RawErrorData 
            customError.RawErrorData = invalidInputLog.Content ?? null;

            // if exception is provided, copy the message
            if (exception != null)
            {
                customError.RawErrorData += $" Raw Exception: {exception.Message ?? null}";
            }

            customError.Severity = "HIGH";

            // refactor on next push, the complexity of method suggest moving the whole functionality to a class level
            if (validationResult == ValidationResult.DATE_MISSING || validationResult == ValidationResult.ID_MISSING)
            {
                customError.Severity = "MEDIUM";
            }

            customError.ErrorSummary = validationResult.ToString() ?? null;
            customError.ErrorCode = Convert.ToInt32(validationResult);
            customError.CustomErrorText = validationResult.Description();

            return ReportException(customError).Result;
        }
        /// <summary>
        /// Async method to send POST request to xMatters API
        /// </summary>
        /// <param name="customError">Custom error created by GenerateCustomError method.</param>
        /// <returns>Result containing boolean of true/false which is provided back to the requestor.</returns>
        private async Task<bool> ReportException(CustomError customError)
        {
            // Visual Studio Debug only output
            Debug.WriteLine("exception found!");
            Debug.WriteLine("ID:" + customError.ID);
            Debug.WriteLine("Error Code:" + customError.ErrorCode);
            Debug.WriteLine("ErrorDate:" + customError.ErrorDate);
            Debug.WriteLine("CustomErrorText:" + customError.CustomErrorText);
            Debug.WriteLine("RawErrorData:" + customError.RawErrorData);

            // Create the HttpClient and submit the custom error to xMatters API Endpoint to be processed by the Workflow

            using (var client = new HttpClient())
            {
                var uri = "https://mariamatthews.xmatters.com/api/integration/1/functions/79312140-319c-438e-aec7-2f84cf0d86d1/triggers?apiKey=8382e5fc-b711-4185-81f7-66b4741dc5bf";
                var jsonString = JsonSerializer.Serialize(customError);

                var response = await client.PostAsync(uri, new StringContent(jsonString, Encoding.UTF8, "application/json"));
                if (response.IsSuccessStatusCode)
                {
                    // return success if the response was a success
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Saves the file with current date on the Azure function storage.
        /// </summary>
        /// <param name="outputLog">OutputLog generated during log processing in ProcessLog.</param>
        /// <returns>Result containing boolean of true/false which is provided back to the requestor.</returns>
        private bool SaveFile(OutputLog outputLog)
        {
            // Get the subdirectories for the specified directory.
            string[] allDirectories = Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory);
            var logsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log");

            // If the destination directory doesn't exist/first run of the application, create it.
            if (!allDirectories.Contains<string>(logsDirectory))
            {
                Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log"));
            }

            try
            {
                // Concatinate object properties with string
                var outputString = $"{outputLog.Date} ID: {outputLog.ID} {outputLog.Content} \n";

                // Save the log data to a file with the current date
                File.AppendAllText(Path.Combine(logsDirectory, DateTime.Now.ToString("yyyy-MM-dd") + ".log"), outputString);
                return true;
            }
            catch (Exception e)
            {
                GenerateCustomError(ValidationResult.CANNOT_SAVE_FILE, e, null);
            }

            return false;
        }
    }
}
