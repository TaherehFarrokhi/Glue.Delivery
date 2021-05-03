using System;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Glue.Delivery.Core.Domain;
using Glue.Delivery.Core.Dto;
using Glue.Delivery.Core.Stores;
using Glue.Delivery.WebApi.IntegrationTests.Extensions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Xunit;

namespace Glue.Delivery.WebApi.IntegrationTests
{
    public class DeliveriesControllerTests : IClassFixture<ApplicationFactory>
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly Fixture _fixture = new();

        public DeliveriesControllerTests(ApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Should_Get_ReturnDeliveryCorrectly_WhenTheDeliveryIsAvailable()
        {
            // Arrange
            var client = _factory.CreateClient();
            var delivery = _fixture.Create<OrderDelivery>();
            var context = _factory.Server.Services.GetRequiredService<DeliveryDbContext>();
            context.Add(delivery);
            await context.SaveChangesAsync();

            // Act
            var response = await client.GetAsync($"deliveries/{delivery.DeliveryId}");

            // Assert
            var actual = await response.ValidateAndReadContentAsync<OrderDeliveryDto>();
            actual.Should().BeEquivalentTo(delivery);
        }

        [Fact]
        public async Task Should_Get_ReturnNotFound_WhenTheDeliveryIdIsNotValid()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync($"deliveries/{Guid.NewGuid()}");

            // Assert
            response.IsSuccessStatusCode.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Should_Post_CreateAndReturnNewDeliveryCorrectly_WhenThePayloadIsValid()
        {
            // Arrange
            var client = _factory.CreateClient();
            var delivery = _fixture.Create<DeliveryRequestDto>();
            var context = _factory.Server.Services.GetRequiredService<DeliveryDbContext>();
            var httpContent = new StringContent(JsonConvert.SerializeObject(delivery), Encoding.UTF8,
                MediaTypeNames.Application.Json);

            // Act
            var response = await client.PostAsync("deliveries", httpContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var actual = await response.ValidateAndReadContentAsync<OrderDeliveryDto>();
            var persisted = await context.Deliveries.FindAsync(actual.DeliveryId);

            actual.Order.Should().BeEquivalentTo(delivery.Order);
            actual.Recipient.Should().BeEquivalentTo(delivery.Recipient);
            actual.AccessWindow.Should().BeEquivalentTo(delivery.AccessWindow);
            actual.State.Should().Be(DeliveryState.Created);

            persisted.Order.Should().BeEquivalentTo(delivery.Order);
            persisted.Recipient.Should().BeEquivalentTo(delivery.Recipient);
            persisted.AccessWindow.Should().BeEquivalentTo(delivery.AccessWindow);
            persisted.State.Should().Be(DeliveryState.Created);
            persisted.DeliveryId.Should().Be(actual.DeliveryId);

            response.Headers.Location?.AbsoluteUri
                .EndsWith($"deliveries/{actual.DeliveryId}", StringComparison.CurrentCultureIgnoreCase).Should()
                .BeTrue();
        }

        [Fact]
        public async Task Should_Put_UpdateAndReturnUpdatedDeliveryCorrectly_WhenThePayloadIsValidAndDeliveryExists()
        {
            // Arrange
            var client = _factory.CreateClient();
            var delivery = _fixture.Create<OrderDelivery>();
            var context = _factory.Server.Services.GetRequiredService<DeliveryDbContext>();
            context.Add(delivery);
            await context.SaveChangesAsync();

            var updatedDelivery = _fixture.Create<DeliveryRequestDto>();
            var httpContent = new StringContent(JsonConvert.SerializeObject(updatedDelivery), Encoding.UTF8,
                MediaTypeNames.Application.Json);

            // Act
            var response = await client.PutAsync($"deliveries/{delivery.DeliveryId}", httpContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var actual = await response.ValidateAndReadContentAsync<OrderDeliveryDto>();
            var persisted = await context.Deliveries.FindAsync(actual.DeliveryId);

            actual.Order.Should().BeEquivalentTo(updatedDelivery.Order);
            actual.Recipient.Should().BeEquivalentTo(updatedDelivery.Recipient);
            actual.AccessWindow.Should().BeEquivalentTo(updatedDelivery.AccessWindow);
            actual.State.Should().Be(DeliveryState.Created);

            persisted.Order.Should().BeEquivalentTo(updatedDelivery.Order);
            persisted.Recipient.Should().BeEquivalentTo(updatedDelivery.Recipient);
            persisted.AccessWindow.Should().BeEquivalentTo(updatedDelivery.AccessWindow);
            persisted.State.Should().Be(DeliveryState.Created);

            response.Headers.Location?.AbsoluteUri
                .EndsWith($"deliveries/{actual.DeliveryId}", StringComparison.CurrentCultureIgnoreCase).Should()
                .BeTrue();
        }

        [Fact]
        public async Task Should_Put_ReturnsNotFound_WhenDeliveryIsNotExist()
        {
            // Arrange
            var client = _factory.CreateClient();
            var updatedDelivery = _fixture.Create<DeliveryRequestDto>();
            var httpContent = new StringContent(JsonConvert.SerializeObject(updatedDelivery), Encoding.UTF8,
                MediaTypeNames.Application.Json);

            // Act
            var response = await client.PutAsync($"deliveries/{Guid.NewGuid()}", httpContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Should_Delete_RemoveTheDelivery_WhenTheDeliveryIdIsValid()
        {
            // Arrange
            var client = _factory.CreateClient();
            var delivery = _fixture.Create<OrderDelivery>();
            var context = _factory.Server.Services.GetRequiredService<DeliveryDbContext>();
            context.Add(delivery);
            await context.SaveChangesAsync();

            // Act
            var response = await client.DeleteAsync($"deliveries/{delivery.DeliveryId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var persisted = await context.Deliveries.FindAsync(delivery.DeliveryId);
            persisted.Should().BeNull();
        }

        [Fact]
        public async Task Should_Delete_ReturnNotFound_WhenTheDeliveryIdIsNotValid()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.DeleteAsync($"deliveries/{Guid.NewGuid()}");

            // Assert
            response.IsSuccessStatusCode.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        [Fact]
        public async Task Should_ChangeDeliveryState_ReturnsNotFound_WhenDeliveryIsNotExist()
        {
            // Arrange
            var client = _factory.CreateClient();
            
            // Act
            client.DefaultRequestHeaders.Add("role", "User");
            var response = await client.PostAsync($"deliveries/{Guid.NewGuid()}/state?action=approve", null!);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        [Fact]
        public async Task Should_ChangeDeliveryState_ReturnsFailure_WhenRoleIsNotAvailableInHeader()
        {
            // Arrange
            var client = _factory.CreateClient();
            var delivery = _fixture.Create<OrderDelivery>();
            var context = _factory.Server.Services.GetRequiredService<DeliveryDbContext>();
            context.Add(delivery);
            await context.SaveChangesAsync();

            // Act
            var response = await client.PostAsync($"deliveries/{delivery.DeliveryId}/state?action=approve", null!);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Be("Role should be defined in header. Values: User | Partner");
        }        
        
        [Theory]
        [InlineData(DeliveryState.Created, "User", "approve", DeliveryState.Approved)]
        [InlineData(DeliveryState.Created,"User", "cancel", DeliveryState.Cancelled)]
        [InlineData(DeliveryState.Approved,"User", "cancel", DeliveryState.Cancelled)]
        [InlineData(DeliveryState.Approved, "Partner", "cancel", DeliveryState.Cancelled)]
        [InlineData(DeliveryState.Created, "Partner", "cancel", DeliveryState.Cancelled)]
        [InlineData(DeliveryState.Approved,"Partner", "complete", DeliveryState.Completed)]
        public async Task Should_ChangeDeliveryState_ReturnsSuccess_WhenDeliveryExistsAndRoleEligibleForStateChange(
            DeliveryState currentState, 
            string role,
            string action,
            DeliveryState expectedState)
        {
            // Arrange
            var client = _factory.CreateClient();
            var delivery = _fixture.Create<OrderDelivery>();
            delivery.State = currentState;
            var context = _factory.Server.Services.GetRequiredService<DeliveryDbContext>();
            context.Add(delivery);
            await context.SaveChangesAsync();

            // Act
            client.DefaultRequestHeaders.Add("role", role);
            var response = await client.PostAsync($"deliveries/{delivery.DeliveryId}/state?action={action}", null!);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var persisted = await context.Deliveries.FindAsync(delivery.DeliveryId);
            persisted.State.Should().Be(expectedState);
        }
        
        [Theory]
        [InlineData(DeliveryState.Approved, "User", "approve")]
        [InlineData(DeliveryState.Cancelled, "User", "approve")]
        [InlineData(DeliveryState.Completed, "User", "approve")]
        [InlineData(DeliveryState.Expired, "User", "approve")]
        [InlineData(DeliveryState.Created, "User", "complete")]
        [InlineData(DeliveryState.Approved, "User", "complete")]
        [InlineData(DeliveryState.Cancelled, "User", "complete")]
        [InlineData(DeliveryState.Completed, "User", "complete")]
        [InlineData(DeliveryState.Expired, "User", "complete")]
        [InlineData(DeliveryState.Completed, "User", "cancel")]
        [InlineData(DeliveryState.Cancelled, "User", "cancel")]
        [InlineData(DeliveryState.Expired, "User", "cancel")]
        [InlineData(DeliveryState.Created, "Partner", "approve")]
        [InlineData(DeliveryState.Approved, "Partner", "approve")]
        [InlineData(DeliveryState.Cancelled, "Partner", "approve")]
        [InlineData(DeliveryState.Completed, "Partner", "approve")]
        [InlineData(DeliveryState.Expired, "Partner", "approve")]
        [InlineData(DeliveryState.Created, "Partner", "complete")]
        [InlineData(DeliveryState.Cancelled, "Partner", "complete")]
        [InlineData(DeliveryState.Completed, "Partner", "complete")]
        [InlineData(DeliveryState.Expired, "Partner", "complete")]
        [InlineData(DeliveryState.Completed, "Partner", "cancel")]
        [InlineData(DeliveryState.Cancelled, "Partner", "cancel")]
        [InlineData(DeliveryState.Expired, "Partner", "cancel")]
        public async Task Should_ChangeDeliveryState_ReturnsConflictResult_WhenDeliveryStateChangeIsForbidden(
            DeliveryState currentState, 
            string role,
            string action)
        {
            // Arrange
            var client = _factory.CreateClient();
            var delivery = _fixture.Create<OrderDelivery>();
            delivery.State = currentState;
            var context = _factory.Server.Services.GetRequiredService<DeliveryDbContext>();
            context.Add(delivery);
            await context.SaveChangesAsync();

            // Act
            client.DefaultRequestHeaders.Add("role", role);
            var response = await client.PostAsync($"deliveries/{delivery.DeliveryId}/state?action={action}", null!);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);

            var persisted = await context.Deliveries.FindAsync(delivery.DeliveryId);
            persisted.State.Should().Be(currentState);
        }
    }
}