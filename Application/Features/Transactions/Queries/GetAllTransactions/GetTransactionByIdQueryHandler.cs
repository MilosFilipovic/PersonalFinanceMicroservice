using Application.Features.Transactions.Queries.GetAllTransactions;
using AutoMapper;
using Domain.Interfaces;
using MediatR;
using Application.DTOs;

public class GetTransactionByIdQueryHandler
    : IRequestHandler<GetTransactionByIdQuery, TransactionDto>
{
    private readonly ITransactionRepository _repo;
    private readonly IMapper _mapper;

    public GetTransactionByIdQueryHandler(
        ITransactionRepository repo,
        IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<TransactionDto> Handle(
        GetTransactionByIdQuery request,
        CancellationToken cancellationToken)
    {
        var entity = await _repo.GetByIdAsync(request.id)
                    ?? throw new KeyNotFoundException($"Transaction with Id {request.id} not found");

        return _mapper.Map<TransactionDto>(entity);
    }
}
