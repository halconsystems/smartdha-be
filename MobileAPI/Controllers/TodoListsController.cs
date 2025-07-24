using DHAFacilitationAPIs.Application.Feature.TodoLists.Commands.DeleteTodoList;
using DHAFacilitationAPIs.Application.Feature.TodoLists.Commands.UpdateTodoList;
using DHAFacilitationAPIs.Application.Feature.TodoLists.Queries.GetTodos;
using DHAFacilitationAPIs.Application.Feature.User.Commands.GenerateToken;
using DHAFacilitationAPIs.Application.Feature.TodoLists.Commands.CreateTodoList;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MobileAPI.Controllers;
public class TodoListsController : BaseApiController
{
    [HttpGet("GetTodoLists"), AllowAnonymous]
    public async Task<IActionResult> GetTodoLists()
    {
        return Ok(await Mediator.Send(new GetTodosQuery()));
    }
    [HttpPost("CreateTodoList"), AllowAnonymous]
    public async Task<IActionResult> CreateTodoList(CreateTodoListCommand command)
    {
        return Ok(await Mediator.Send(command));
    }
}
