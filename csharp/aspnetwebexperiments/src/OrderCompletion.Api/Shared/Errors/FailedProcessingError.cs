using FluentResults;

namespace OrderCompletion.Api.Shared.Errors;

public class FailedProcessingError(string message) : Error(message);