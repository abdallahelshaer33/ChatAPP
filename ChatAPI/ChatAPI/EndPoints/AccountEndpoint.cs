

using ChatAPI.Common;
using ChatAPI.Data;
using ChatAPI.DTO;
using ChatAPI.Extensions;
using ChatAPI.Models;
using ChatAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Cryptography.Xml;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChatAPI.EndPoints
{
      
    public static class AccountEndpoint
    {
        public static async Task<RouteGroupBuilder> MapAccountEndpoint(this WebApplication app)
        {
            


            var group = app.MapGroup("/api/account").WithTags("account");
            group.MapPost("/register", async (HttpContext context, 
                UserManager<APPUser> _usermanager, 
                [FromForm] string Fullname
                , [FromForm] string email
                , [FromForm] string password ,[FromForm] string username  , [FromForm] IFormFile? profileimage ) =>
            {
                var userFromDB = await _usermanager.FindByEmailAsync(email);
                if (userFromDB is not null)
                {
                    return Results.BadRequest(Response<string>.failure("User is Already Exist"));
                }
                if(profileimage is null)
                {
                    return Results.BadRequest(Response<string>.failure("Profile Image is Required"));
                }
                var picture = await FileUpload.Upload(profileimage);
                picture = $"{context.Request.Scheme}://{context.Request.Host}/upload/{picture}";
                var user = new APPUser
                {
                    Email = email,
                    Fullname = Fullname,
                    UserName = username,
                    ProfileImage = picture
                    
                };
                var result = _usermanager.CreateAsync(user, password);
                if (!result.IsCompletedSuccessfully)
                {
                    return Results.BadRequest(Response<string>.failure(result.Result.Errors.Select(x => x.Description).FirstOrDefault()!));

                }
                return Results.Ok(Response<string>.success("", "user created successfully"));

            }).DisableAntiforgery();

            group.MapPost("/login", async (UserManager<APPUser> _usermanager , TokenService _tokenservice ,LoginDTO login) => {
            
                if (login is null)
                {
                    return Results.BadRequest(Response<string>.failure("Invalid Login Details"));
                }
                var user = await _usermanager.FindByEmailAsync(login.Email);
                if (user is null)
                {
                    return Results.BadRequest(Response<string>.failure("User Not Found "));
                }
                var result = await _usermanager.CheckPasswordAsync(user!, login.Password);
                if (!result)
                {
                    return Results.BadRequest(Response<string>.failure("Invalid Password  "));
                }
                var token = _tokenservice.GenerateToken(user.Id, user.UserName!);
                return Results.Ok(Response<string>.success( token, "Login Success  "));

            });

            group.MapGet("/me", async (HttpContext context, UserManager<APPUser> user) =>
            {
                var curuserloggedInUserId = context.User.GetUserID();
                var currentloggedinuser = await user.Users.SingleOrDefaultAsync(x => x.Id == curuserloggedInUserId.ToString());

               /* return currentloggedinuser != null
                ? Results.Ok(Response<APPUser>.success(currentloggedinuser, "user fetched Successfully"))
                : Results.NotFound("user not found");*/

                 return Results.Ok(Response<APPUser>.success(currentloggedinuser!, "user fetched Successfully"));
            });

            return group;
        }
    }
}
