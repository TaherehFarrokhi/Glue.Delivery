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
using Microsoft.AspNetCore.Mvc.Testing;
using Glue.Delivery.WebApi.IntegrationTests.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Xunit;

namespace Glue.Delivery.WebApi.IntegrationTests
{
    public class DeliveriesControllerTests : IClassFixture<ApplicationFactory>
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly Fixture _fixture = new Fixture();

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
            var delivery = _fixture.Create<NewDeliveryDto>();
            var context = _factory.Server.Services.GetRequiredService<DeliveryDbContext>();
            var httpContent = new StringContent(JsonConvert.SerializeObject(delivery), Encoding.UTF8, MediaTypeNames.Application.Json);

            // Act
            var response = await client.PostAsync($"deliveries", httpContent);
        
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            
            var actual = await response.ValidateAndReadContentAsync<OrderDeliveryDto>();
            var persisted = await context.Set<OrderDelivery>().FindAsync(actual.DeliveryId);
            
            actual.Order.Should().BeEquivalentTo(delivery.Order);
            actual.Recipient.Should().BeEquivalentTo(delivery.Recipient);
            actual.AccessWindow.Should().BeEquivalentTo(delivery.AccessWindow);
            actual.State.Should().Be(DeliveryState.Created);            
            
            persisted.Order.Should().BeEquivalentTo(delivery.Order);
            persisted.Recipient.Should().BeEquivalentTo(delivery.Recipient);
            persisted.AccessWindow.Should().BeEquivalentTo(delivery.AccessWindow);
            persisted.State.Should().Be(DeliveryState.Created);
            persisted.DeliveryId.Should().Be(actual.DeliveryId);

            response.Headers.Location?.AbsoluteUri.EndsWith($"deliveries/{actual.DeliveryId}", StringComparison.CurrentCultureIgnoreCase).Should().BeTrue();
        }  
    }
}