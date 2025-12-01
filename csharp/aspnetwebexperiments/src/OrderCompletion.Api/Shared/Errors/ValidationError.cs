using FluentResults;

namespace OrderCompletion.Api.Shared.Errors;

public class ValidationError(string message) : Error(message);