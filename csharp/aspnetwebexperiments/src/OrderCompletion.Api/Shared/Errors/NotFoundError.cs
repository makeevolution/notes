using FluentResults;

namespace OrderCompletion.Api.Shared.Errors;

public class NotFoundError(string message) : Error(message);