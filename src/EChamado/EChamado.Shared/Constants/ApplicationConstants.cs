namespace EChamado.Shared.Constants;

/// <summary>
/// Constantes da aplicação centralizadas para evitar magic strings
/// </summary>
public static class ApplicationConstants
{
    public static class Urls
    {
        public const string DefaultAuthServerUrl = "https://localhost:7132";
        public const string DefaultApiServerUrl = "https://localhost:7296";
        public const string DefaultBlazorClientUrl = "https://localhost:7274";
    }

    public static class Authentication
    {
        public const string ExternalCookieName = "EChamado.External";
        public const string ExternalScheme = "External";
        public const int CookieExpirationMinutes = 30;
        public const int SessionExpirationHours = 8;
    }

    public static class Endpoints
    {
        public const string ConnectAuthorize = "/connect/authorize";
        public const string ConnectToken = "/connect/token";
        public const string ConnectIntrospect = "/connect/introspect";
        public const string AccountLogin = "/Account/Login";
        public const string AccountDoLogin = "/Account/DoLogin";
        public const string AccountLogout = "/Account/Logout";
    }

    public static class Scopes
    {
        public const string OpenId = "openid";
        public const string Profile = "profile";
        public const string Email = "email";
        public const string Roles = "roles";
        public const string Api = "api";
        public const string Chamados = "chamados";
    }

    public static class Roles
    {
        public const string Admin = "Admin";
        public const string User = "User";
        public const string Support = "Support";
        public const string Manager = "Manager";
    }

    public static class CorsPolicy
    {
        public const string AllowBlazorClient = "AllowBlazorClient";
    }

    public static class RateLimiting
    {
        public const string LoginPolicy = "login";
        public const int GlobalLimitPerMinute = 100;
        public const int LoginLimitPerMinute = 5;
    }

    public static class CacheKeys
    {
        public const string CategoriesAll = "categories:all";
        public const string DepartmentsAll = "departments:all";
        public const string StatusTypesAll = "statustypes:all";
        public const string OrderTypesAll = "ordertypes:all";

        public static string CategoryById(Guid id) => $"category:{id}";
        public static string DepartmentById(Guid id) => $"department:{id}";
    }

    public static class Validation
    {
        public const int MinPasswordLength = 12;
        public const int MinUniqueChars = 4;
        public const int MaxTitleLength = 200;
        public const int MaxDescriptionLength = 2000;
        public const int MaxCommentLength = 1000;
    }

    public static class Pagination
    {
        public const int DefaultPageSize = 25;
        public const int MaxPageSize = 100;
        public const int MinPageSize = 10;
    }
}
