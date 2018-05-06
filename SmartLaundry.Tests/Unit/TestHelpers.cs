using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace SmartLaundry.Tests.Unit
{
    public class TestHelpers
    {
        public static void SetFakeHttpRequestSchemeTo(Controller controller, string requestScheme)
        {
            SetFakeHttpContextIfNotAlreadySet(controller);
            Mock.Get(controller.Request).SetupGet(httpRequest => httpRequest.Scheme).Returns(requestScheme);
        }

        private static void SetFakeHttpContextIfNotAlreadySet(Controller controller)
        {
            if (controller.ControllerContext.HttpContext == null)
                controller.ControllerContext.HttpContext = FakeHttpContext();
        }

        private static HttpContext FakeHttpContext()
        {
            var mockHttpContext = new Mock<HttpContext>();
            var mockHttpRequest = new Mock<HttpRequest>();
            var mockHttpResponse = new Mock<HttpResponse>();

            mockHttpContext.SetupGet(httpContext => httpContext.Request).Returns(mockHttpRequest.Object);
            mockHttpContext.SetupGet(httpContext => httpContext.Response).Returns(mockHttpResponse.Object);

            mockHttpRequest.Setup(httpRequest => httpRequest.Cookies[It.IsAny<string>()])
                .Returns(() => It.IsAny<string>());

            return mockHttpContext.Object;
        }
    }
}