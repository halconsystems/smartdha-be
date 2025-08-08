using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.ViewModels;

namespace DHAFacilitationAPIs.Application.Feature.Room.Commands.CreateRoom;
public record CreateRoomCommand() : IRequest<SuccessResponse<string>>;

