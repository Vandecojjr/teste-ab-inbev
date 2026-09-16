using System.Net;
using System.Net.Http.Json;
using Ambev.DeveloperEvaluation.WebApi;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelSaleItem;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Ambev.DeveloperEvaluation.Functional.Sales;

public class SalesApiFunctionalTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public SalesApiFunctionalTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ORM.DefaultContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }
                var dbName = "FunctionalTestDb_" + Guid.NewGuid().ToString();
                services.AddDbContext<ORM.DefaultContext>(options =>
                {
                    options.UseInMemoryDatabase(dbName);
                });
            });
        });
        _client = _factory.CreateClient();
        
        using var scope = _factory.Services.CreateScope();
        var jwtGenerator = scope.ServiceProvider.GetRequiredService<Ambev.DeveloperEvaluation.Common.Security.IJwtTokenGenerator>();
        var user = new Domain.Entities.User
        {
            Id = Guid.NewGuid(),
            Username = "admin",
            Role = Domain.Enums.UserRole.Admin
        };
        var token = jwtGenerator.GenerateToken(user);
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }

    [Fact(DisplayName = "POST /api/sales - Should create sale and return 201 Created")]
    public async Task CreateSale_ReturnsCreatedResponse()
    {
        var request = new CreateSaleRequest
        {
            SaleNumber = "FN-SALE-001",
            CustomerId = 100,
            CustomerName = "Functional Customer",
            BranchId = 5,
            BranchName = "Functional Branch",
            Items = new List<CreateSaleItemRequest>
            {
                new CreateSaleItemRequest { ProductId = 10, ProductName = "Functional Product", Quantity = 2, UnitPrice = 50m }
            }
        };

        var response = await _client.PostAsJsonAsync("/api/sales", request);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseWithData<CreateSaleResponse>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().NotBeEmpty();
    }

    [Fact(DisplayName = "GET /api/sales/{id} - Should return 404 for non-existent sale")]
    public async Task GetSale_ReturnsNotFound()
    {
        var response = await _client.GetAsync($"/api/sales/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<Guid> CreateTestSaleAsync()
    {
        var request = new CreateSaleRequest
        {
            SaleNumber = $"FN-SALE-{Guid.NewGuid().ToString().Substring(0, 5)}",
            CustomerId = 101,
            CustomerName = "Setup Customer",
            BranchId = 10,
            BranchName = "Setup Branch",
            Items =
            [
                new CreateSaleItemRequest() { ProductId = 20, ProductName = "Setup Product", Quantity = 1, UnitPrice = 100m }
            ]
        };

        var response = await _client.PostAsJsonAsync("/api/sales", request);
        var result = await response.Content.ReadFromJsonAsync<ApiResponseWithData<CreateSaleResponse>>();
        return result!.Data!.Id;
    }

    [Fact(DisplayName = "GET /api/sales/{id} - Should return 200 OK for existing sale")]
    public async Task GetSale_ReturnsOk()
    {
        var saleId = await CreateTestSaleAsync();

        var response = await _client.GetAsync($"/api/sales/{saleId}");
        var contentString = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, "because API returned: " + contentString);

        var result = await response.Content.ReadFromJsonAsync<ApiResponseWithData<GetSaleResponse>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data!.Id.Should().Be(saleId);
    }

    [Fact(DisplayName = "GET /api/sales - Should return 200 OK and paginated list")]
    public async Task ListSales_ReturnsOk()
    {
        await CreateTestSaleAsync();
        await CreateTestSaleAsync();

        var response = await _client.GetAsync("/api/sales?_page=1&_size=10");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PaginatedResponse<GetSaleResponse>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeEmpty();
        result.TotalCount.Should().BeGreaterThanOrEqualTo(2);
    }

    [Fact(DisplayName = "PUT /api/sales/{id} - Should update sale and return 200 OK")]
    public async Task UpdateSale_ReturnsOk()
    {
        var saleId = await CreateTestSaleAsync();

        var updateRequest = new UpdateSaleRequest
        {
            SaleNumber = "FN-SALE-UPDATED",
            CustomerId = 200,
            CustomerName = "Updated Customer",
            BranchId = 15,
            BranchName = "Updated Branch"
        };

        var response = await _client.PutAsJsonAsync($"/api/sales/{saleId}", updateRequest);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ApiResponseWithData<UpdateSaleResponse>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data!.Id.Should().Be(saleId);
    }

    [Fact(DisplayName = "DELETE /api/sales/{id} - Should cancel sale and return 200 OK")]
    public async Task CancelSale_ReturnsOk()
    {
        var saleId = await CreateTestSaleAsync();

        var response = await _client.DeleteAsync($"/api/sales/{saleId}");
        var content = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, "because: " + content);

        var result = await response.Content.ReadFromJsonAsync<ApiResponseWithData<CancelSaleResponse>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
    }

    [Fact(DisplayName = "DELETE /api/sales/{saleId}/items/{itemId} - Should cancel sale item and return 200 OK")]
    public async Task CancelSaleItem_ReturnsOk()
    {
        var saleId = await CreateTestSaleAsync();
        
        var getResponse = await _client.GetAsync($"/api/sales/{saleId}");
        var saleData = await getResponse.Content.ReadFromJsonAsync<ApiResponseWithData<GetSaleResponse>>();
        var itemId = saleData!.Data!.Items.First().Id;

        var response = await _client.DeleteAsync($"/api/sales/{saleId}/items/{itemId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<CancelSaleItemResponse>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
    }
}
