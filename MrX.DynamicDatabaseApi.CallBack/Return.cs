using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MrX.DynamicDatabaseApi.CallBack;

public class Return
{
    public enum Loc
    {
        Authentication,
        Authorization,
        Endpoint,
        Check,
        NotHandeled
    }

    public Return(Loc Location, float StatusCode, string Message, object? data = null)
    {
        this.Location = Location;
        this.StatusCode = StatusCode;
        this.Message = Message;
        Data = data;
    }

    public Loc Location { get; set; }
    public float StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public object? Data { get; set; }

    /// <summary>
    ///     409
    /// </summary>
    public static Return ThisExist(Loc Location, string Message, object? data = null)
    {
        return new Return(Location, 409, Message, data);
    }

    /// <summary>
    ///     403
    /// </summary>
    public static Return AccessDeny(Loc Location, string Message, object? data = null)
    {
        return new Return(Location, 403, Message, data);
    }

    /// <summary>
    ///     200
    /// </summary>
    public static Return Sucsses(Loc Location, string Message, object? data = null)
    {
        return new Return(Location, 200, Message, data);
    }

    /// <summary>
    ///     404
    /// </summary>
    public static Return NotExist(Loc Location, string Message, object? data = null)
    {
        return new Return(Location, 404, Message, data);
    }

    /// <summary>
    ///     400
    /// </summary>
    public static Return HeaderNotFound(Loc Location, string Message, object? data = null)
    {
        return new Return(Location, 400, Message, data);
    }

    /// <summary>
    ///     400
    /// </summary>
    public static Return Invalid(Loc Location, string Message, object? data = null)
    {
        return new Return(Location, 400, Message, data);
    }

    /// <summary>
    ///     404
    /// </summary>
    public static Return NotFound(Loc Location, string Message, object? data = null)
    {
        return new Return(Location, 404, Message, data);
    }

    /// <summary>
    ///     400
    /// </summary>
    public static Return ParameterNotFound(Loc Location, string Message, object? data = null)
    {
        return new Return(Location, 400, Message, data);
    }

    /// <summary>
    ///     419
    /// </summary>
    public static Return Expire(Loc Location, string Message, object? data = null)
    {
        return new Return(Location, 419, Message, data);
    }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this, Formatting.Indented,
            new StringEnumConverter());
    }
}