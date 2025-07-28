namespace Application.Features.Transactions.Queries.GetAllTransactions;

using AutoMapper;
using Domain.Interfaces;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Models;
using Application.DTOs;

public class GetAllTransactionsQueryHandler : IRequestHandler<GetAllTransactionsQuery, PaginatedList<TransactionDto>>
{
    private readonly ITransactionRepository _repository;
    private readonly IMapper _mapper;

    public GetAllTransactionsQueryHandler(ITransactionRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PaginatedList<TransactionDto>> Handle(GetAllTransactionsQuery request, CancellationToken cancellationToken)
    {
        var (entities, totalCount) = await _repository
                .GetPagedAsync(request.PageNumber, request.PageSize, cancellationToken);

        var dtos = _mapper.Map<IEnumerable<TransactionDto>>(entities);
        return new PaginatedList<TransactionDto>(dtos, totalCount, request.PageNumber, request.PageSize);
    }
}

