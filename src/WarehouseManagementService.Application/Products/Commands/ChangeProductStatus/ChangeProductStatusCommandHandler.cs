using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WarehouseManagementService.Application.Common.Exceptions;
using WarehouseManagementService.Application.Common.Interfaces;
using WarehouseManagementService.Application.Common.Models;

namespace WarehouseManagementService.Application.Products.Commands.ChangeProductStatus;

public sealed class ChangeProductStatusCommandHandler : IRequestHandler<ChangeProductStatusCommand, Result<ProductDto>>
{
    private readonly IAppDbContext _dbContext;
    private readonly IMapper _mapper;

    public ChangeProductStatusCommandHandler(IAppDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<Result<ProductDto>> Handle(
        ChangeProductStatusCommand request,
        CancellationToken cancellationToken)
    {
        var product = await _dbContext.Products
            .Include(product => product.Category)
            .FirstOrDefaultAsync(product => product.Id == request.Id, cancellationToken);

        if (product is null)
        {
            throw new NotFoundException($"Product with id '{request.Id}' was not found.");
        }

        try
        {
            product.ChangeStatus(request.Request.Status);
        }
        catch (InvalidOperationException exception)
        {
            throw new DomainRuleException(exception.Message);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<ProductDto>.Success(_mapper.Map<ProductDto>(product));
    }
}
