using System.Net;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using WebAPI.Models;
using Domain.Entities.Exceptions;

namespace WebAPI.Extensions
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (ValidationException vex)
            {
                await HandleValidationExceptionAsync(httpContext, vex);
            }
            catch (BusinessException bex)
            {
                await HandleBusinessExceptionAsync(httpContext, bex);
            }
            catch (Exception ex)
            {
                await HandleUnknownExceptionAsync(httpContext, ex);
            }
        }

        private static Task HandleBusinessExceptionAsync(HttpContext context, BusinessException bex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 440; 

            var problem = new BusinessProblemResponse
            {
                Code = bex.Code,
                Title = "Business Policy Violation",
                Detail = bex.Message
            };

            var json = JsonSerializer.Serialize(problem);
            return context.Response.WriteAsync(json);
        }

        private static Task HandleValidationExceptionAsync(HttpContext context, ValidationException vex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            var errorResponse = new ValidationErrorResponse
            {
                Title = "Validation Failed",
                Status = context.Response.StatusCode,
                Errors = vex.Errors
                            .GroupBy(e => e.PropertyName)
                            .ToDictionary(
                                g => g.Key,
                                g => g.Select(e => e.ErrorMessage).ToArray())
            };

            var json = JsonSerializer.Serialize(errorResponse);
            return context.Response.WriteAsync(json);
        }

        private static Task HandleUnknownExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var errorResponse = new DefaultErrorResponse
            {
                Title = "Internal Server Error",
                Status = context.Response.StatusCode,
                Detail = ex.Message   
            };

            var detail = ex.InnerException is not null
                 ? $"{ex.Message} ({ex.InnerException.Message})"
                 : ex.Message;
            
            var errorResponsee = new DefaultErrorResponse
                {
                Title = "Internal Server Error",
                Status = context.Response.StatusCode,
                Detail = detail
                };

            var json = JsonSerializer.Serialize(errorResponsee);
            return context.Response.WriteAsync(json);
        }
    }
}
