using FluentResults;

namespace OrderCompletion.Api.Shared.Errors;

public class TimedOutError(string message) : Error(message);