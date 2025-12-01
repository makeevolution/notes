using FluentResults;

namespace OrderCompletion.Api.Shared.Errors;

public class ConflictError(string message) : Error(message);