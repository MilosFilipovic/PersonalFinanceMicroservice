using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;
    public CategoriesController(IMediator mediator) => _mediator = mediator;


    [HttpPost("import")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BusinessProblemResponse), 440)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DefaultErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Import(IFormFile file)
    {
        
        if (file == null || file.Length == 0)
            throw new FluentValidation.ValidationException(new[]
            {
            new ValidationFailure("file", "CSV fajl nije prosleđen.")
        });

        await using var stream = file.OpenReadStream();

        var cmd = new ImportCategoriesCommand
        {
            FileStream = stream,
            FileName = file.FileName
        };

        
        var importedCount = await _mediator.Send(cmd);

        return Ok(new { Imported = importedCount });
    }


    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _mediator.Send(new GetAllCategoriesQuery());
        return Ok(categories);
    }
}

