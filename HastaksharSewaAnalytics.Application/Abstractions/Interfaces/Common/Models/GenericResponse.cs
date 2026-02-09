namespace HastaksharSewaAnalytics.Application.Abstractions.Interfaces.Common.Models;

public record GenericResponse<T>
{
    public bool Success { get; init; }
    public string? Message { get; init; }
    public T? Data { get; init; }

    public static GenericResponse<T> Ok(T data, string? message = null)
        => new() { Success = true, Message = message, Data = data };

    public static GenericResponse<T> Fail(string message)
        => new() { Success = false, Message = message, Data = default };
}