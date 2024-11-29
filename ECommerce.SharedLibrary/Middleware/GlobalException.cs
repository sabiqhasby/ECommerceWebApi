using ECommerce.SharedLibrary.Logs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace ECommerce.SharedLibrary.Middleware
{
   public class GlobalException(RequestDelegate next)
   {
      public async Task InvokeAsync(HttpContext context)
      {
         //Declare Default variables
         string message = "sorry, internal server error occured. Please try again";
         int statusCode = (int)HttpStatusCode.InternalServerError;
         string title = "Error";

         try
         {
            await next(context);

            //Check if Response is too many request // 429 status code.
            if (context.Response.StatusCode == StatusCodes.Status429TooManyRequests)
            {
               title = "Warning";
               message = "Too many Request is made.";
               statusCode = (int)StatusCodes.Status429TooManyRequests;
               await ModifyHeader(context, title, message, statusCode);
            }

            // if Response is UnAuthorized // 401 status code.
            if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
            {
               title = "Alert";
               message = "You are not authorized to access";
               statusCode = StatusCodes.Status401Unauthorized;
               await ModifyHeader(context, title, message, statusCode);
            }

            //if Response is Forbidden // 403 status code.
            if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
            {
               title = "Out of Access";
               message = "You are not allowed/required to access";
               statusCode = StatusCodes.Status403Forbidden;
               await ModifyHeader(context, title, message, statusCode);
            }
         }
         catch (Exception ex)
         {
            // Log Original Exception /File, Debugger,  Console
            LogException.LogExceptions(ex);

            //check if Exception is Timeout // 408 RTO.
            if (ex is TaskCanceledException || ex is TimeoutException)
            {
               title = "Out of time!";
               message = "Request timeout ... try again";
               statusCode = StatusCodes.Status408RequestTimeout;
            }

            // if Exception is caught.
            // if None of the exceptions then do the default
            await ModifyHeader(context, title, message, statusCode);

         }
      }

      // Replace original Header with custom header.
      private async Task ModifyHeader(HttpContext context, string title, string message, int statusCode)
      {
         // display scary-free message to client
         context.Response.ContentType = "application/json";
         await context.Response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails()
         {
            Detail = message,
            Status = statusCode,
            Title = title
         }), CancellationToken.None);
         return;
      }
   }
}
