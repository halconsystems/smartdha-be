using DHAFacilitationAPIs.Application.Feature.TodoItems.Commands.CreateTodoItem;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MobileAPI.Controllers;
public class TodoItemsController : BaseApiController
{
    [HttpPost("CreateTodoItem"), AllowAnonymous]
    public async Task<IActionResult> CreateTodoItem(CreateTodoItemCommand command)
    {
        return Ok(await Mediator.Send(command));
    }
}
