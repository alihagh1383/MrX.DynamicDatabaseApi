namespace MrX.DynamicDatabaseApi.CallBack;

public static class FromResponse
{
    public enum Status
    {
        Nothing,
        Sucsses
    }

    public static void Pars(Return @return, out bool Success, out string Message, out object? data, out Status status)
    {
        status = Status.Nothing;
        if (@return == null)
            throw new ArgumentNullException();
        Message = @return.Message;
        data = @return.Data;
        if (@return.StatusCode == 200)
        {
            Success = true;
            status = Status.Sucsses;
        }
        else
        {
            Success = false;
        }
    }
}