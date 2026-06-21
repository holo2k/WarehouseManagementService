namespace WarehouseManagementService.Application.Common.Models;

public static class ErrorCodes
{
    public const string Validation = "validation_error";
    public const string NotFound = "not_found";
    public const string Conflict = "conflict";
    public const string DomainRuleViolation = "domain_rule_violation";
    public const string ConcurrencyConflict = "concurrency_conflict";
    public const string DatabaseConflict = "database_conflict";
    public const string InternalServerError = "internal_server_error";
    public const string MethodNotAllowed = "method_not_allowed";
    public const string HttpError = "http_error";
}
