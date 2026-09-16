using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Sales;

public class SaleIntegrationTests : IDisposable
{
    private readonly DefaultContext _context;
    private readonly SaleRepository _repository;
    private readonly IMapper _mapper;

    public SaleIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new DefaultContext(options);
        _repository = new SaleRepository(_context);

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddMaps(typeof(CreateSaleProfile).Assembly);
        });
        _mapper = mapperConfig.CreateMapper();
    }

    [Fact(DisplayName = "Should create a sale and save it to the database successfully")]
    public async Task CreateSale_ShouldPersistInDatabase()
    {
        var mediator = Substitute.For<MediatR.IMediator>();
        var handler = new CreateSaleHandler(_repository, _mapper, mediator);
        var command = new CreateSaleCommand
        {
            SaleNumber = "INT-SALE-001",
            CustomerId = 1,
            CustomerName = "John Doe",
            BranchId = 1,
            BranchName = "Main Branch",
            Items = new List<CreateSaleItemCommand>
            {
                new CreateSaleItemCommand
                {
                    ProductId = 1,
                    ProductName = "Product A",
                    Quantity = 5,
                    UnitPrice = 100m
                }
            }
        };

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();

        var dbSale = await _context.Sales.Include(s => s.Items).FirstOrDefaultAsync(s => s.Id == result.Id);
        
        dbSale.Should().NotBeNull();
        dbSale!.SaleNumber.Should().Be("INT-SALE-001");
        dbSale.Customer.Name.Should().Be("John Doe");
        dbSale.Items.Should().HaveCount(1);
        dbSale.TotalSaleAmount.Value.Should().Be(450m);
    }

    [Fact(DisplayName = "Should retrieve an existing sale from the database")]
    public async Task GetSale_ShouldRetrieveFromDatabase()
    {
        var mediator = Substitute.For<MediatR.IMediator>();
        var createHandler = new CreateSaleHandler(_repository, _mapper, mediator);
        var createCommand = new CreateSaleCommand
        {
            SaleNumber = "INT-SALE-002",
            CustomerId = 2,
            CustomerName = "Jane Doe",
            BranchId = 2,
            BranchName = "Secondary Branch",
            Items = new List<CreateSaleItemCommand>
            {
                new CreateSaleItemCommand
                {
                    ProductId = 2,
                    ProductName = "Product B",
                    Quantity = 2,
                    UnitPrice = 50m
                }
            }
        };
        var createResult = await createHandler.Handle(createCommand, CancellationToken.None);

        var getHandler = new GetSaleHandler(_repository, _mapper);
        var getCommand = new GetSaleCommand(createResult.Id);

        var result = await getHandler.Handle(getCommand, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(createResult.Id);
        result.SaleNumber.Should().Be("INT-SALE-002");
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
