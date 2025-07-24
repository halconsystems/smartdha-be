using DHAFacilitationAPIs.Application.Feature.TodoLists.Commands.DeleteTodoList;
using DHAFacilitationAPIs.Application.Feature.TodoLists.Commands.UpdateTodoList;
using DHAFacilitationAPIs.Application.Feature.TodoLists.Queries.GetTodos;
using DHAFacilitationAPIs.Application.Feature.User.Commands.GenerateToken;
using DHAFacilitationAPIs.Application.Feature.TodoLists.Commands.CreateTodoList;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;
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

    //public async Task<IResult> UpdateTodoList(ISender sender, int id, UpdateTodoListCommand command)
    //{
    //    if (id != command.Id) return Results.BadRequest();
    //    await sender.Send(command);
    //    return Results.NoContent();
    //}

    //public async Task<IResult> DeleteTodoList(ISender sender, int id)
    //{
    //    await sender.Send(new DeleteTodoListCommand(id));
    //    return Results.NoContent();
    //}
}
