namespace MrX.DynamicDatabaseApi.CallBack.Error;

public static class Authenticate
{
    public enum Error
    {
        ID_Not_In_Route,
        ID_Not_Guid,
        ID_Not_In_Database,
        Session_Data_Not_Valid,
        Account_Data_Is_Updated
    }

    public static Dictionary<Error, string> ErrorMessages = new()
    {
        { Error.ID_Not_In_Route, "Session ID Not In Route" },
        { Error.ID_Not_Guid, "Session ID Is Not Guid" },
        { Error.ID_Not_In_Database, "Session ID Is Not In Database" },
        { Error.Session_Data_Not_Valid, "Invalid Username Or Password" },
        { Error.Account_Data_Is_Updated, "Your Account Data Is Updated" }
    };

    public static Return ForSend(Error error, object? data = null)
    {
        switch (error)
        {
            case Error.ID_Not_In_Route:
                return Return.ParameterNotFound(Return.Loc.Authentication, ErrorMessages[error],
                    new { Error = error, data });
            case Error.ID_Not_Guid:
                return Return.Invalid(Return.Loc.Authentication, ErrorMessages[error], new { Error = error, data });
            case Error.ID_Not_In_Database:
                return Return.NotFound(Return.Loc.Authentication, ErrorMessages[error], new { Error = error, data });
            case Error.Session_Data_Not_Valid:
                return Return.Invalid(Return.Loc.Authentication, ErrorMessages[error], new { Error = error, data });
            case Error.Account_Data_Is_Updated:
                return Return.Expire(Return.Loc.Authentication, ErrorMessages[error], new { Error = error, data });
            default:
                return new Return(Return.Loc.Check, 0, "");
        }
    }
}