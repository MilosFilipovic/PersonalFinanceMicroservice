using Application.Common.Models;
using Application.DTOs;
using AutoMapper;
using Domain.Interfaces;
using MediatR;


public class GetTransactionsByDateRangeQueryHandler : IRequestHandler<GetTransactionsByDateRangeQuery, PaginatedList<TransactionDto>>
{
    private readonly ITransactionRepository _repository;
    private readonly IMapper _mapper;

    public GetTransactionsByDateRangeQueryHandler(
        ITransactionRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PaginatedList<TransactionDto>> Handle(
    GetTransactionsByDateRangeQuery request,
    CancellationToken cancellationToken)
    {
        var startUtc = DateTime.SpecifyKind(request.StartDate, DateTimeKind.Utc);
        var endUtc = DateTime.SpecifyKind(request.EndDate, DateTimeKind.Utc);

        // *** Proslijedi i sortBy + sortOrder ***
        var (entities, totalCount) = await _repository
            .GetByDateRangeAsync(
                startUtc,
                endUtc,
                request.PageNumber,
                request.PageSize,
                request.Kinds,
                request.SortBy,
                request.SortOrder,
                cancellationToken);

        var dtos = _mapper.Map<IEnumerable<TransactionDto>>(entities);

        return new PaginatedList<TransactionDto>(
            dtos,
            totalCount,
            request.PageNumber,
            request.PageSize);
    }

}
