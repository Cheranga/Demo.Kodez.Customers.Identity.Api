namespace Demo.Kodez.Customers.Identity.Api.Shared.Constants
{
    public static class ErrorCodes
    {
        public const string InvalidRequest = nameof(InvalidRequest);
        public const string TableClientNotFound = nameof(TableClientNotFound);
        public const string InsertError = nameof(InsertError);
    }

    public static class ErrorMessages
    {
        public const string InvalidRequest = "invalid request";
        public const string TableClientNotFound = "no table found to get a table client";
        public const string InsertError = "error occurred when inserting data to the table";
    }
}