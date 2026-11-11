using Microsoft.AspNetCore.Mvc;

namespace CourtOnlineWeek5.Generic
{
    public static class ErrorHandler
    {
        public static async Task<IActionResult> ExecuteAsync(Func<Task<IActionResult>> action,ILogger? logger = null)
        {
            try
            {
                return await action();
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "An unexpected error occurred");
                // or: Console.WriteLine($"Error occurred: {ex.Message}");
                return new ObjectResult("An error occurred while processing your request.")
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
    }
}