﻿using Dev.TestMate.WebAPI.Common;
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
                       {"role","Admin" },
                       { "hub_user_id",5}
               });

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
        try
        {

            if (!String.IsNullOrEmpty(id))
            {
                await FirebaseAuth.DefaultInstance.DeleteUserAsync(id);
            }


            return null;
        }
        catch (FirebaseAuthException ex)
        {

            if (ex.AuthErrorCode == FirebaseAdmin.Auth.AuthErrorCode.UserNotFound)
            {

                throw new TestMateException( ex);
            }
            throw;
        }
    }

    [HttpPost("UpdateUser")]
    public async Task<UserRecord> UpdateUser(
      string uId,string dbId, string email
    )
    {
        try
        {
            var claims = new Dictionary<string, object>()
               {
                       {"role","Admin" },
                       { "hub_user_id",dbId}
               };
            if (claims != null)
            {
                await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(
                    uId,
                    claims
                );
            }
            UserRecordArgs userArgs = new UserRecordArgs { Uid = uId ,Email=email};
          

            UserRecord userRecord = await FirebaseAuth.DefaultInstance.UpdateUserAsync(
                userArgs
            );
            return userRecord;
        }
        catch (FirebaseAuthException ex)
        {
            if (ex.AuthErrorCode == AuthErrorCode.UserNotFound)
            {
                throw new BadHttpRequestException("User not found", StatusCodes.Status404NotFound);
            }
            throw ex;
        }
    }
    // TODO  Write a post API for Sign In to Firebase 
    // Use REST API of Firebase


    // TODO  Write Signup

}
