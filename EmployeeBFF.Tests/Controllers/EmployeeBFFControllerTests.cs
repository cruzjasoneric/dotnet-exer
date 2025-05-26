using EmployeeBFF.Controllers;
using EmployeeBFF.Models.Requests;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.InMemory.Query.Internal;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Dynamic;
using System.Net;
using Xunit;

namespace EmployeeBFF.Tests.Controllers
{
    public class EmployeeBFFControllerTests
    {
        private readonly Mock<ILogger<EmployeeBFFController>> _loggerMock;
        private readonly Mock<IValidator<EmployeeCreateRequest>> _createValidatorMock;
        private readonly Mock<IValidator<EmployeeUpdateRequest>> _updateValidatorMock;
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;

        public EmployeeBFFControllerTests()
        {
            _loggerMock = new Mock<ILogger<EmployeeBFFController>>();
            _createValidatorMock = new Mock<IValidator<EmployeeCreateRequest>>();
            _updateValidatorMock = new Mock<IValidator<EmployeeUpdateRequest>>();
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
        }

        private EmployeeBFFController CreateController(HttpClient? client = null)
        {
            if (client != null)
            {
                _httpClientFactoryMock
                    .Setup(f => f.CreateClient("EmployeeApi"))
                    .Returns(client);
            }

            return new EmployeeBFFController(
                _loggerMock.Object,
                _createValidatorMock.Object,
                _updateValidatorMock.Object,
                _httpClientFactoryMock.Object
            );
        }

        [Fact]
        public async Task GetAll_ReturnsOkResponse_WhenApiReturnsSuccess()
        {
            var expectedContent = "[{\"id\":1,\"firstName\":\"Test\"}]";

            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri.ToString() == "http://localhost/api/employees"),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(expectedContent)
                });

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };

            //_httpClientFactoryMock
            //    .Setup(f => f.CreateClient("EmployeeApi"))
            //    .Returns(httpClient);

            var controller = CreateController(httpClient);

                var result = await controller.GetAll();

                var objectResult = Assert.IsType<ObjectResult>(result);
                Assert.Equal((int)HttpStatusCode.OK, objectResult.StatusCode);
                Assert.Equal(expectedContent, objectResult.Value);
            
        }

        [Fact]
        public async Task GetAll_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            _httpClientFactoryMock
                .Setup(f => f.CreateClient("EmployeeApi"))
                .Throws(new Exception("API failure"));

            //var controller = new EmployeeBFFController(
            //    _loggerMock.Object,
            //    _createValidatorMock.Object,
            //    _updateValidatorMock.Object,
            //    _httpClientFactoryMock.Object
            //);
            var controller = CreateController();

            var result = await controller.GetAll();

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
            Assert.Equal("Internal server error. Please try again later.", objectResult.Value);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenValidationFails()
        {
            var badRequestModel = new BffEmployeeCreateRequest
            {
                Data = new EmployeeCreateRequest { FirstName = "123" }
            };

            // validation fails
            _createValidatorMock
                .Setup(v => v.ValidateAsync(badRequestModel.Data, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(new[]
                {
                    new ValidationFailure("FirstName", "Invalid FirstName")
                }));

            var controller = CreateController();

            var result = await controller.Create(badRequestModel);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("There were invalid field(s) in create request", badRequest.Value);
        }

        [Fact]
        public async Task Create_ReturnsCreatedResponse_WhenApiReturnsSuccess()
        {

            // valid test data
            var validModel = new BffEmployeeCreateRequest
            {
                Data = new EmployeeCreateRequest
                {
                    FirstName = "test",
                    LastName = "test",
                    Email = "test@email.com",
                    Phone = "1111111",
                    Department = "HR",
                    HireDate = DateTime.Today,
                    Salary = 100.00m
                }
            };
            var expectedContent = "{\"id\":1}";

            // validator returns success
            _createValidatorMock
                .Setup(v => v.ValidateAsync(validModel.Data, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            // mock HttpClient
            var handlerMock = new Mock<HttpMessageHandler>();

            handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri.ToString() == "http://localhost/api/employees"),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Created,
                Content = new StringContent(expectedContent)
            });

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };

            var controller = CreateController(httpClient);

            var result = await controller.Create(validModel);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.Created, objectResult.StatusCode);
            Assert.Equal(expectedContent, objectResult.Value);
        }

        [Fact]
        public async Task Create_ReturnsInternalServerError_WhenHttpClientThrows()
        {
            // valid test data
            var validModel = new BffEmployeeCreateRequest
            {
                Data = new EmployeeCreateRequest
                {
                    FirstName = "test",
                    LastName = "test",
                    Email = "test@email.com",
                    Phone = "1111111",
                    Department = "HR",
                    HireDate = DateTime.Today,
                    Salary = 100.00m
                }
            };

            _createValidatorMock
                .Setup(v => v.ValidateAsync(validModel.Data, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _httpClientFactoryMock
                .Setup(f => f.CreateClient("EmployeeApi"))
                .Throws(new Exception("Network down"));

            var controller = CreateController();

            var result = await controller.Create(validModel);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);

        }

    }
}
