﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MrX.DynamicDatabaseApi.CallBack;
using MrX.DynamicDatabaseApi.Worker;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace MrX.DynamicDatabaseApi.Api.Handler
{
    public class LoginAuthentication : AuthenticationHandler<CustomBasicAuthenticationSchemeOptions>
    {
        DBWLogins _WL;
        DBWUser _WU;
        Web.Logger.SecurityLogger _SL;


        public LoginAuthentication(IOptionsMonitor<CustomBasicAuthenticationSchemeOptions> options,
           ILoggerFactory logger, UrlEncoder encoder, DBWUser UM, DBWLogins LM,
           Web.Logger.SecurityLogger SL) : base(options, logger, encoder)
        {
            _WL = LM;
            _WU = UM;
            _SL = SL;
            Message = new Return(Return.Loc.NotHandeled, 0, "Null");
        }
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var headerValue = Request.RouteValues["Session"] ?? "";
            Console.WriteLine(headerValue);
            if (string.IsNullOrEmpty(headerValue.ToString()))
            {
                Message = CallBack.Error.Authenticate.ForSend(CallBack.Error.Authenticate.Error.ID_Not_In_Route);
                goto Error;
            }
            Context.Items["LoginId"] = headerValue;
            if (!Guid.TryParse(headerValue.ToString(), out var guid))
            {
                Message = CallBack.Error.Authenticate.ForSend(CallBack.Error.Authenticate.Error.ID_Not_Guid);
                goto Error;
            }
            var SessionID = _WL.Find(guid);
            if (SessionID == null)
            {

                Message = CallBack.Error.Authenticate.ForSend(CallBack.Error.Authenticate.Error.ID_Not_In_Database);
                goto Error;
            }
            var user = _WU.GetAll().AsEnumerable()
                .Where(p => p.UserName == SessionID.Username && p.Password == (SessionID.Password)).FirstOrDefault();
            if (user == null)
            {
                Message = CallBack.Error.Authenticate.ForSend(CallBack.Error.Authenticate.Error.Session_Data_Not_Valid);
                goto Error;
            }
            if (user.LastUpdate != SessionID.LastUpdate)
            {
                Message = CallBack.Error.Authenticate.ForSend(CallBack.Error.Authenticate.Error.Account_Data_Is_Updated);
                goto Error;
            }
            Context.Items.Add("user", user);
            return Task.FromResult(AuthenticateResult.Success(
                new AuthenticationTicket(
                    new ClaimsPrincipal(
                        new ClaimsIdentity(
                            new[]
                            {
                                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()!),
                                new Claim(ClaimTypes.Name, user.UserName!),
                            }, Scheme.Name)
                    ), Scheme.Name)
            ));

        Error:
            return Task.FromResult(AuthenticateResult.Fail(Message.ToString()));
        }

        Return Message;

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.StatusCode = 200;
            Response.WriteAsJsonAsync<Return>(Message);
            return Task.CompletedTask;
        }
    }

    public class CustomBasicAuthenticationSchemeOptions : AuthenticationSchemeOptions
    {
    }
}