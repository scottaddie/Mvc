// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.AspNet.Mvc.Razor
{
    /// <summary>
    /// Represents a <see cref="IRazorPageFactory"/> that creates <see cref="RazorPage"/> instances
    /// from razor files in the file system.
    /// </summary>
    public class DefaultRazorPageFactory : IRazorPageFactory
    {
        /// <inheritdoc />
        public IRazorPage CreateInstance(Type type, string relativePath)
        {
            var page = (IRazorPage)Activator.CreateInstance(type);
            page.Path = relativePath;

            return page;
        }
    }
}