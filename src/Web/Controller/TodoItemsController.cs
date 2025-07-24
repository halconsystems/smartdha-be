using DHAFacilitationAPIs.Application.Feature.TodoItems.Commands.CreateTodoItem;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DHAFacilitationAPIs.Web.Controller;
public class TodoItemsController : BaseApiController
{
    [HttpPost("CreateTodoItem"), AllowAnonymous]
    public async Task<IActionResult> CreateTodoItem(CreateTodoItemCommand command)
    {
        return Ok(await Mediator.Send(command));
    }
    //public Task<PaginatedList<TodoItemBriefDto>> GetTodoItemsWithPagination(ISender sender, [AsParameters] GetTodoItemsWithPaginationQuery query)
    //{
    //    return sender.Send(query);
    //}

    //public Task<int> CreateTodoItem(ISender sender, CreateTodoItemCommand command)
    //{
    //    return sender.Send(command);
    //}

    //public async Task<IResult> UpdateTodoItem(ISender sender, int id, UpdateTodoItemCommand command)
    //{
    //    if (id != command.Id) return Results.BadRequest();
    //    await sender.Send(command);
    //    return Results.NoContent();
    //}

    //public async Task<IResult> UpdateTodoItemDetail(ISender sender, int id, UpdateTodoItemDetailCommand command)
    //{
    //    if (id != command.Id) return Results.BadRequest();
    //    await sender.Send(command);
    //    return Results.NoContent();
    //}

    //public async Task<IResult> DeleteTodoItem(ISender sender, int id)
    //{
    //    await sender.Send(new DeleteTodoItemCommand(id));
    //    return Results.NoContent();
    //}
}
