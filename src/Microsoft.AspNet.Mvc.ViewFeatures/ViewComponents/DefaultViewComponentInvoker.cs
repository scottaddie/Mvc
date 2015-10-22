// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc.Controllers;
using Microsoft.AspNet.Mvc.Diagnostics;
using Microsoft.AspNet.Mvc.Infrastructure;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.AspNet.Mvc.ViewFeatures;

namespace Microsoft.AspNet.Mvc.ViewComponents
{
    public class DefaultViewComponentInvoker : IViewComponentInvoker
    {
        private readonly ITypeActivatorCache _typeActivatorCache;
        private readonly IViewComponentActivator _viewComponentActivator;
        private readonly DiagnosticSource _diagnosticSource;

        public DefaultViewComponentInvoker(
            ITypeActivatorCache typeActivatorCache,
            IViewComponentActivator viewComponentActivator,
            DiagnosticSource diagnosticSource)
        {
            if (typeActivatorCache == null)
            {
                throw new ArgumentNullException(nameof(typeActivatorCache));
            }

            if (viewComponentActivator == null)
            {
                throw new ArgumentNullException(nameof(viewComponentActivator));
            }

            if (diagnosticSource == null)
            {
                throw new ArgumentNullException(nameof(diagnosticSource));
            }

            _typeActivatorCache = typeActivatorCache;
            _viewComponentActivator = viewComponentActivator;
            _diagnosticSource = diagnosticSource;
        }

        public void Invoke(ViewComponentContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var method = ViewComponentMethodSelector.FindSyncMethod(
                context.ViewComponentDescriptor.Type.GetTypeInfo(),
                context.Arguments);
            if (method == null)
            {
                throw new InvalidOperationException(
                    Resources.FormatViewComponent_CannotFindMethod(ViewComponentMethodSelector.SyncMethodName));
            }

            if (_diagnosticSource.IsEnabled("Microsoft.AspNet.Mvc.BeforeViewComponent"))
            {
                _diagnosticSource.Write(
                    "Microsoft.AspNet.Mvc.BeforeViewComponent",
                    new
                    {
                        actionDescriptor = context.ViewContext.ActionDescriptor,
                        viewComponentContext = context
                    });
            }

            var result = InvokeSyncCore(method, context);

            if (_diagnosticSource.IsEnabled("Microsoft.AspNet.Mvc.AfterViewComponent"))
            {
                _diagnosticSource.Write(
                    "Microsoft.AspNet.Mvc.AfterViewComponent",
                    new
                    {
                        actionDescriptor = context.ViewContext.ActionDescriptor,
                        viewComponentContext = context,
                        viewComponentResult = result
                    });
            }

            result.Execute(context);
        }

        public async Task InvokeAsync(ViewComponentContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            IViewComponentResult result;

            var asyncMethod = ViewComponentMethodSelector.FindAsyncMethod(
                context.ViewComponentDescriptor.Type.GetTypeInfo(),
                context.Arguments);
            if (asyncMethod == null)
            {
                // We support falling back to synchronous if there is no InvokeAsync method, in this case we'll still
                // execute the IViewResult asynchronously.
                var syncMethod = ViewComponentMethodSelector.FindSyncMethod(
                    context.ViewComponentDescriptor.Type.GetTypeInfo(),
                    context.Arguments);
                if (syncMethod == null)
                {
                    throw new InvalidOperationException(
                        Resources.FormatViewComponent_CannotFindMethod_WithFallback(
                        ViewComponentMethodSelector.SyncMethodName, ViewComponentMethodSelector.AsyncMethodName));
                }
                else
                {
                    _diagnosticSource.BeforeViewComponent(context.ViewContext.ActionDescriptor, context);

                    result = InvokeSyncCore(syncMethod, context);

                    _diagnosticSource.AfterViewComponent(context.ViewContext.ActionDescriptor, context, result);
                }
            }
            else
            {
                if (_diagnosticSource.IsEnabled("Microsoft.AspNet.Mvc.BeforeViewComponent"))
                {
                    _diagnosticSource.Write(
                        "Microsoft.AspNet.Mvc.BeforeViewComponent",
                        new
                        {
                            actionDescriptor = context.ViewContext.ActionDescriptor,
                            viewComponentContext = context
                        });
                }

                result = await InvokeAsyncCore(asyncMethod, context);

                if (_diagnosticSource.IsEnabled("Microsoft.AspNet.Mvc.AfterViewComponent"))
                {
                    _diagnosticSource.Write(
                        "Microsoft.AspNet.Mvc.AfterViewComponent",
                        new
                        {
                            actionDescriptor = context.ViewContext.ActionDescriptor,
                            viewComponentContext = context,
                            viewComponentResult = result
                        });
                }
            }

            await result.ExecuteAsync(context);
        }

        private object CreateComponent(ViewComponentContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var services = context.ViewContext.HttpContext.RequestServices;
            var component = _typeActivatorCache.CreateInstance<object>(
                services,
                context.ViewComponentDescriptor.Type);
            _viewComponentActivator.Activate(component, context);
            return component;
        }

        private async Task<IViewComponentResult> InvokeAsyncCore(
            MethodInfo method,
            ViewComponentContext context)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var component = CreateComponent(context);

            var result = await ControllerActionExecutor.ExecuteAsync(method, component, context.Arguments);

            return CoerceToViewComponentResult(result);
        }

        public IViewComponentResult InvokeSyncCore(MethodInfo method, ViewComponentContext context)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var component = CreateComponent(context);

            object result = null;

            try
            {
                result = method.Invoke(component, context.Arguments);
            }
            catch (TargetInvocationException ex)
            {
                // Preserve callstack of any user-thrown exceptions.
                var exceptionInfo = ExceptionDispatchInfo.Capture(ex.InnerException);
                exceptionInfo.Throw();
            }

            return CoerceToViewComponentResult(result);
        }

        private static IViewComponentResult CoerceToViewComponentResult(object value)
        {
            if (value == null)
            {
                throw new InvalidOperationException(Resources.ViewComponent_MustReturnValue);
            }

            var componentResult = value as IViewComponentResult;
            if (componentResult != null)
            {
                return componentResult;
            }

            var stringResult = value as string;
            if (stringResult != null)
            {
                return new ContentViewComponentResult(stringResult);
            }

            var htmlStringResult = value as HtmlString;
            if (htmlStringResult != null)
            {
                return new ContentViewComponentResult(htmlStringResult);
            }

            throw new InvalidOperationException(Resources.FormatViewComponent_InvalidReturnValue(
                typeof(string).Name,
                typeof(HtmlString).Name,
                typeof(IViewComponentResult).Name));
        }
    }
}