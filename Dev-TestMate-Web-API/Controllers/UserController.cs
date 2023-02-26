using Dev.TestMate.WebAPI.Models;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Dev.TestMate.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    [HttpPost("Create")]
    public async Task<UserRecord> CreateAsync(UserInvitation request)
    {
        try
        {
            UserRecordArgs userRecordArgs = new UserRecordArgs()
            {
                Email = request.Email,
                EmailVerified = true,
                Password = request.Password
            };

            UserRecord userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(
                userRecordArgs
            );
            await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(
                userRecord.Uid,
               new Dictionary<string, object>()
               {
                       {"role","Admin" }
               } );

            return await FirebaseAuth.DefaultInstance.GetUserAsync(userRecord.Uid);

        }
        catch (FirebaseAuthException ex)
        {
            if (ex.AuthErrorCode == AuthErrorCode.EmailAlreadyExists)
            {
                throw new BadHttpRequestException(
                    "Email already exists",
                    StatusCodes.Status409Conflict
                );
            }

            throw ex;
        }
    }
}
