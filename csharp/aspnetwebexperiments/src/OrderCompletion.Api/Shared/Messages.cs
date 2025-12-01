namespace OrderCompletion.Api.Shared;

public static class Messages
{
    public static readonly string OneOrMoreOrderIdsRequired = "One or more orderIds required";
    public static readonly string OneOrMoreOrdersNotFound = "Orders with these IDs not found: ";
    public static readonly string OneOrMoreOrdersAlreadyProcessed = "Orders with these IDs are already completed or under attempt to be completed: ";
    public static readonly string SuccessfullyCompletedAllOrders = "All orders completed successfully";
    public static readonly string FailedCompletingOrders = "Failed to complete one or more orders: ";
    public static readonly string TimedOutCompletingOrders = "Time out processing completing orders request, will be processed in the background";
    public static readonly string InternalProcessingError = "Error occurred while processing one or more orders: ";
}