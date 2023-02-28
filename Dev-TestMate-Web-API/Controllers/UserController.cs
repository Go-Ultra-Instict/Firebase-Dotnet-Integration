using Dev.TestMate.WebAPI.Models;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Dev.TestMate.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
  //  [Authorize]
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

    [HttpGet("GetUserRecords")]
    public async Task<List<ExportedUserRecord>> GetUserRecords()
    {
      List<ExportedUserRecord> userList2= new List<ExportedUserRecord>() { };
      IAsyncEnumerator<ExportedUserRecord>  enumerator = FirebaseAuth.DefaultInstance.ListUsersAsync(null).GetAsyncEnumerator();
        while (await enumerator.MoveNextAsync())
        {
            userList2.Add(enumerator.Current);
        }
        return userList2;
       
    }




    [HttpDelete("DeleteUser")]
    public async Task<object?> DeleteUser(string id)
    {

        if (!String.IsNullOrEmpty(id))
        {
            await FirebaseAuth.DefaultInstance.DeleteUserAsync(id);
        }


        return null;
    }
}
