﻿using System.Web.Mvc;
using Kata4.Core.Contract.Enum;
using Kata4.Core.Contract.Exception;
using Kata4.Core.Contract.Infrastructure.LogContract;

namespace Kata4.WebApp.Handler
{
    public class ExceptionHandler : HandleErrorAttribute
    {
        private readonly ILogger _logger;

        public ExceptionHandler(ILogger logger)
        {
            _logger = logger;
        }

        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled)
            {
                return;
            }

            var e = filterContext.Exception;

            if (e is IUserFriendlyException)
            {
                _logger.Log(LogLevel.Info, e.Message);
            }
            else
            {
                _logger.Log(LogLevel.Error, e.Message, e);
            }

            filterContext.ExceptionHandled = true;

            filterContext.HttpContext.Response.Clear();
            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;

            filterContext.Result = new ViewResult
            {
                ViewName = "Index",
                ViewData = { ["ErrorMessage"] = e.Message }
            };
        }
    }
}