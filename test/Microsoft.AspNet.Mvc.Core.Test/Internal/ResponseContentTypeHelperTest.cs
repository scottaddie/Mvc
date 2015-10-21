// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Net.Http.Headers;
using Xunit;

namespace Microsoft.AspNet.Mvc.Internal
{
    public class ResponseContentTypeHelperTest
    {
        public static TheoryData<MediaTypeHeaderValue, string, string> ResponseContentTypeData
        {
            get
            {
                return new TheoryData<MediaTypeHeaderValue, string, string>
                {
                    {
                        null,
                        null,
                        "text/default; p1=p1-value; charset=utf-8"
                    },
                    {
                        new MediaTypeHeaderValue("text/foo"),
                        null,
                        "text/foo; charset=utf-8"
                    },
                    {
                        MediaTypeHeaderValue.Parse("text/foo; charset=us-ascii"),
                        null,
                        "text/foo; charset=us-ascii"
                    },
                    {
                        MediaTypeHeaderValue.Parse("text/foo; p1=p1-value"),
                        null,
                        "text/foo; p1=p1-value; charset=utf-8"
                    },
                    {
                        MediaTypeHeaderValue.Parse("text/foo; p1=p1-value; charset=us-ascii"),
                        null,
                        "text/foo; p1=p1-value; charset=us-ascii"
                    },
                    {
                        null,
                        "text/bar",
                        "text/bar; charset=utf-8"
                    },
                    {
                        null,
                        "text/bar; p1=p1-value",
                        "text/bar; p1=p1-value; charset=utf-8"
                    },
                                        {
                        null,
                        "text/bar; p1=p1-value; charset=us-ascii",
                        "text/bar; p1=p1-value; charset=us-ascii"
                    },
                    {
                        MediaTypeHeaderValue.Parse("text/foo; charset=us-ascii"),
                        "text/bar",
                        "text/foo; charset=us-ascii"
                    },
                    {
                        MediaTypeHeaderValue.Parse("text/foo; charset=us-ascii"),
                        "text/bar; charset=utf-8",
                        "text/foo; charset=us-ascii"
                    }
                };
            }
        }

        [Theory]
        [MemberData(nameof(ResponseContentTypeData))]
        public void GetsExpectedContentTypeAndEncoding(
            MediaTypeHeaderValue contentType,
            string responseContentType,
            string expectedContentType)
        {
            // Arrange
            var defaultContentType = MediaTypeHeaderValue.Parse("text/default; p1=p1-value; charset=utf-8");

            // Act
            var actualContentType = ResponseContentTypeHelper.GetContentType(
                contentType,
                responseContentType,
                defaultContentType);

            // Assert
            Assert.Equal(expectedContentType, actualContentType.ToString());
        }

        [Fact]
        public void DoesNotModify_UserProvidedContentTypeObject()
        {
            // Arrange
            var defaultContentType = MediaTypeHeaderValue.Parse("text/blah");
            var contentType = MediaTypeHeaderValue.Parse("text/foo");

            // Act
            var actualContentType = ResponseContentTypeHelper.GetContentType(
                contentType,
                httpResponseContentType: null,
                defaultContentType: defaultContentType);

            // Assert
            Assert.NotSame(contentType, actualContentType);
        }
    }
}
