// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNet.Mvc.Logging
{
    public static class MvcRouteHandlerLoggerExtensions
    {
        private static Action<ILogger, string, Exception> _noMatchingActions;
        private static Action<ILogger, string, Exception> _executingAction;

        private static Func<ILogger, string, IDisposable> _actionScope;

        static MvcRouteHandlerLoggerExtensions()
        {
            _noMatchingActions = LoggerMessage.Define<string>(
                LogLevel.Verbose,
                1,
                "No actions matched the current request.");
            _executingAction = LoggerMessage.Define<string>(
                LogLevel.Verbose,
                2,
                "Executing action {ActionDisplayName}");
            _actionScope = LoggerMessage.DefineScope<string>(
                "ActionId: {ActionId}");
        }

        public static IDisposable BeginActionScope(this ILogger logger, string actionId)
        {
            return _actionScope(logger, actionId);
        }

        public static void NoMatchingActions(this ILogger logger)
        {
            _noMatchingActions(logger, string.Empty, null);
        }

        public static void ExecutingAction(this ILogger logger, string actionDisplayName)
        {
            _executingAction(logger, actionDisplayName, null);
        }
    }
}
