using Application.Common;
using Microsoft.AspNetCore.Http;

namespace Application.Extensions;

public static class ResultExtensions
{
    public static IResult ToApiResult(this Result result, IResult resultResponseIfSuccess)
        => result.IsSuccess
            ? resultResponseIfSuccess
            : Results.Problem(
                title: result.Problem?.Title,
                detail: result.Problem?.ErrorMessage,
                statusCode: result.Problem?.Status
            );

    public static IResult ToApiResult<T>(this Result<T> result)
        => result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.Problem(
                title: result.Problem?.Title,
                detail: result.Problem?.ErrorMessage,
                statusCode: result.Problem?.Status
            );
}