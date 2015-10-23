// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.AspNet.Mvc.ViewComponents;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNet.Mvc.ViewFeatures.Logging
{
    public static class DefaultViewComponentInvokerLoggerExtensions
    {
        private static readonly Action<ILogger, string, Exception> _viewComponentExecuting;
        private static readonly Action<ILogger, string, int, Exception> _viewComponentExecuted;

        static DefaultViewComponentInvokerLoggerExtensions()
        {
            _viewComponentExecuting = LoggerMessage.Define<string>(
                LogLevel.Verbose,
                1,
                "Executing view component {ViewComponentName}");
        }

        public static IDisposable ViewComponentScope(this ILogger logger, ViewComponentDescriptor descriptor)
        {
            return logger.BeginScopeImpl(new ViewComponentLogScope(descriptor));
        }

        public static void ViewComponentExecuting(this ILogger logger, ViewComponentDescriptor descriptor)
        {

        }

        public static void ViewComponentExecuted(
            this ILogger logger,
            ViewComponentDescriptor descriptor,
            int elapsedMilliseconds)
        {

        }

        private class ViewComponentLogScope : ILogValues
        {
            private readonly ViewComponentDescriptor _descriptor;

            public ViewComponentLogScope(ViewComponentDescriptor descriptor)
            {
                _descriptor = descriptor;
            }

            public IEnumerable<KeyValuePair<string, object>> GetValues()
            {
                throw new NotImplementedException();
            }

            public override string ToString()
            {
                return base.ToString();
            }
        }
    }
}
