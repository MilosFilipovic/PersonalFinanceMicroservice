
using Application.DTOs;
using Application.Features.Transactions.Commands.CreateTransaction;
using Application.Features.Transactions.Commands.ImportTransactions;
using Application.Features.Transactions.Commands.SplitTransaction;
using Application.Features.Transactions.Queries.GetAllTransactions;
using Domain.Entities.Enums;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;


namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TransactionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/transactions
        [HttpGet]
        public async Task<ActionResult> GetAllTransactions([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _mediator.Send(new GetAllTransactionsQuery(pageNumber, pageSize));
            return Ok(result);
        }

        [HttpGet("kinds")]
        public IActionResult GetKinds()
        {
            var kinds = Enum.GetNames(typeof(TransactionKind));
            return Ok(kinds);
        }


        //GET: api/transactions/id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var dto = await _mediator.Send(new GetTransactionByIdQuery(id));
                return Ok(dto);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        // POST: api/transactions
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTransactionCommand command)
        {
            
            var createdId = await _mediator.Send(command);

            var transactionDto = await _mediator.Send(new GetTransactionByIdQuery(createdId));

            
            return CreatedAtAction(
                nameof(GetById),            
                new { id = createdId },    
                value: transactionDto       
            );
        }

        [HttpPost("import")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BusinessProblemResponse), 440)]
        [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DefaultErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Import(IFormFile file)
        {
            
            if (file == null || file.Length == 0)
                throw new ValidationException(new[]
                {
                new ValidationFailure("file", "CSV fajl nije prosleđen.")
            });

            await using var stream = file.OpenReadStream();

            var cmd = new ImportTransactionsCommand
            {
                FileStream = stream,
                FileName = file.FileName
            };

            
            var count = await _mediator.Send(cmd);

            
            return Ok(new { Imported = count });
        }


        [HttpGet("by-date-range")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByDateRange(
    [FromQuery] DateTime startDate,
    [FromQuery] DateTime endDate,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] List<TransactionKind>? kinds = null,

    // <<< DODAVAMO >>>
    [FromQuery(Name = "sort-by")] string? sortBy = null,
    [FromQuery(Name = "sort-order")] string sortOrder = "asc"
)
        {
            var query = new GetTransactionsByDateRangeQuery
            {
                StartDate = startDate,
                EndDate = endDate,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Kinds = kinds,

                // <<< POSTAVLJAMO >>
                SortBy = sortBy,
                SortOrder = sortOrder.ToLower()
            };

            try
            {
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (FluentValidation.ValidationException vex)
            {
                var problems = new ValidationProblemDetails(vex.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                      g => g.Key,
                      g => g.Select(e => e.ErrorMessage).ToArray()
                    ))
                {
                    Title = "Validation failed"
                };
                return BadRequest(problems);
            }
            catch (Exception ex)
            {
                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "An unexpected error occurred",
                    Detail = ex.Message
                };
                return StatusCode(StatusCodes.Status500InternalServerError, problem);
            }
        }


        [HttpPost("{id}/categorize")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BusinessProblemResponse), 440)]
        [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DefaultErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Categorize(string id, [FromBody] CategorizeDto dto)
        {
            
            if (string.IsNullOrWhiteSpace(id))
                throw new FluentValidation.ValidationException(new[]
                {
                new ValidationFailure("id", "Parametar 'id' je obavezan.")
            });

            
            if (dto == null)
                throw new FluentValidation.ValidationException(new[]
                {
                new ValidationFailure("dto", "Telo zahteva nije prosleđeno.")
            });

            
            var cmd = new CategorizeTransactionCommand
            {
                TransactionId = id,
                CategoryCode = dto.CategoryCode
            };

            await _mediator.Send(cmd);

           
            return Ok();
        }


        [HttpPost("{id}/split")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Split(
        [FromRoute] string id,
        [FromBody] List<SplitItemDto> splits)
        {
            var cmd = new SplitTransactionCommand(id, splits);
            try
            {
                await _mediator.Send(cmd);
                return NoContent();
            }
            catch (ValidationException vex)
            {
                var pd = new ValidationProblemDetails
                {
                    Title = "Validation failed",
                    Detail = vex.Message
                };
                return BadRequest(pd);
            }
            catch (KeyNotFoundException knf)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Not found",
                    Detail = knf.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ProblemDetails
                {
                    Title = "Internal error",
                    Detail = ex.Message
                });
            }
        }

        [HttpPost("auto-categorize")]
        public async Task<IActionResult> AutoCategorize(CancellationToken ct)
        {
            await _mediator.Send(new AutoCategorizeTransactionsCommand(), ct);
            return Ok();
        }
    }
}



    