using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Common.Models;
public class Result<T> : Result
{
    private Result(bool succeeded, T? data, IEnumerable<string> errors)
        : base(succeeded, errors)
    {
        Data = data;
    }

    public T? Data { get; }

    public static Result<T> Success(T data)
        => new(true, data, Array.Empty<string>());

    public static new Result<T> Failure(IEnumerable<string> errors)
        => new(false, default, errors);
}
