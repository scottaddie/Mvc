// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace Microsoft.AspNet.Mvc.Logging
{
    public static class ObjectResultExecutorLoggerExtensions
    {
        private static Action<ILogger, string, Exception> _objectResultExecuting;
        private static Action<ILogger, string, Exception> _noFormatter;
        private static Action<ILogger, string, string, Exception> _formatterSelected;
        private static Action<ILogger, string, Exception> _skippedContentNegotiation;
        private static Action<ILogger, string, Exception> _noAcceptForNegotiation;
        private static Action<ILogger, string, Exception> _noFormatterFromNegotiation;

        static ObjectResultExecutorLoggerExtensions()
        {
            _noFormatter = LoggerMessage.Define<string>(
                LogLevel.Warning,
                1,
                "No output formatter was found for content type '{ContentType}' to write the response.");
            _objectResultExecuting = LoggerMessage.Define<string>(
                LogLevel.Information,
                1,
                "Executing ObjectResult, writing value {Value}.");
            _formatterSelected = LoggerMessage.Define<string, string>(
                LogLevel.Verbose,
                2,
                "Selected output formatter '{OutputFormatter}' and content type " +
                "'{ContentType}' to write the response.");
            _skippedContentNegotiation = LoggerMessage.Define<string>(
                LogLevel.Verbose,
                3,
                "Skipped content negotiation as content type '{ContentType}' is explicitly set for the response.");
            _noAcceptForNegotiation = LoggerMessage.Define<string>(
                LogLevel.Verbose,
                4,
                "No information found on request to perform content negotiation.");
            _noFormatterFromNegotiation = LoggerMessage.Define<string>(
                LogLevel.Verbose,
                5,
                "Could not find an output formatter based on content negotiation. Accepted types were ({AcceptTypes})");
        }

        public static void ObjectResultExecuting(this ILogger logger, object value)
        {
            _objectResultExecuting(logger, Convert.ToString(value), null);
        }
        
	public static void NoFormatter(this ILogger logger, MediaTypeHeaderValue contentType)
        {
            _noFormatter(logger, Convert.ToString(contentType), null);
        }

        public static void FormatterSelected(
            this ILogger logger, 
            string formatter,
            MediaTypeHeaderValue contentType)
        {
            _formatterSelected(logger, formatter, Convert.ToString(contentType), null);
        }

        public static void SkippedContentNegotiation(this ILogger logger, MediaTypeHeaderValue contentType)
        {
            _skippedContentNegotiation(logger, Convert.ToString(contentType), null);
        }

        public static void NoAcceptForNegotiation(this ILogger logger)
        {
            _noAcceptForNegotiation(logger, null, null);
        }

        public static void NoFormatterFromNegotiation(this ILogger logger, string acceptTypes)
        {
            _noFormatterFromNegotiation(logger, acceptTypes, null);
        }
    }
}
