using Dev.TestMate.WebAPI.Common;
using Dev.TestMate.WebAPI.Models;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;

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

    // TODO  Write a post API for Sign In to Firebase 
    // Use REST API of Firebase


    // TODO  Write Signup


    [HttpGet]
    public async Task<object> getUserAsync( string token)
    {
        try
        {
            //var httpClient = new HttpClient();

            //using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://identitytoolkit.googleapis.com/v1/accounts:lookup?key=[API_KEY]"))
            //{
            //    var requestBody = new
            //    {
            //        idToken = token,

            //    };

            //    request.Content = 
            //    request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

            //    var response = await httpClient.SendAsync(request);
            //}

            var httpClient = new HttpClient();

            var request = new HttpRequestMessage(new HttpMethod("POST"), "https://identitytoolkit.googleapis.com/v1/accounts:lookup?key=AIzaSyDsRqHl36anY_xBrPOf1lzKXt4IsSDAVk8 ");
            
                var requestBody = new
                {
                    idToken = token,
                };

                request.Content = new StringContent(JsonConvert.SerializeObject(requestBody));
                request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                var response = await httpClient.SendAsync(request);

            string responseString = await response.Content.ReadAsStringAsync();

            return responseString;

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Failed to verify user token: {errorMessage}");
                }

            
            

            return null;
        }
        catch (Exception ex)
        {

            throw;
        }
    }
    

    [HttpPost("SignUp")]
    public async Task<string> SignUp(string email, string password)
    {
        

        var httpClient = new HttpClient();

        var requestBody = new
        {
            email = email,
            returnSecureToken=true,
            password = password
        };

        var request = new HttpRequestMessage(new HttpMethod("POST"), "https://identitytoolkit.googleapis.com/v1/accounts:signUp?key=AIzaSyDsRqHl36anY_xBrPOf1lzKXt4IsSDAVk8");
            
                //request.Content = new StringContent("{\"email\":\"deepti@gmail.com\",\"password\":\"test12345\",\"returnSecureToken\":true}");
                request.Content = new StringContent(JsonConvert.SerializeObject(requestBody));
                request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                var response = await httpClient.SendAsync(request);

        return null;
            
        
    }



    [HttpPost("SignIn")]
    public async Task<string> SignIn(string email, string password)
    {


        var httpClient = new HttpClient();

        var requestBody = new
        {
            email = email,
            returnSecureToken = true,
            password = password
        };

        var request = new HttpRequestMessage(new HttpMethod("POST"), "https://identitytoolkit.googleapis.com/v1/accounts:signUp?key=AIzaSyDsRqHl36anY_xBrPOf1lzKXt4IsSDAVk8");

        //request.Content = new StringContent("{\"email\":\"deepti@gmail.com\",\"password\":\"test12345\",\"returnSecureToken\":true}");
        request.Content = new StringContent(JsonConvert.SerializeObject(requestBody));
        request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

        var response = await httpClient.SendAsync(request);

        return null;


    }

    [HttpPost("SendPasswordResetEmail")]
    public async Task SendPasswordResetEmail(string email)
    {


        var httpClient = new HttpClient();

        var requestBody = new
        {
            email = email,
            requestType = "PASSWORD_RESET"

        };

        var request = new HttpRequestMessage(new HttpMethod("POST"), $"https://identitytoolkit.googleapis.com/v1/accounts:sendOobCode?key=AIzaSyDsRqHl36anY_xBrPOf1lzKXt4IsSDAVk8");
              

                request.Content = new StringContent(JsonConvert.SerializeObject(requestBody));
                request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                var response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
            
        


    }

    [HttpPost("IsCodeValid")]
    public async Task IsCodeValid(string oobCode)
    {


        var httpClient = new HttpClient();

        var requestBody = new
        {
            oobCode = oobCode,
            requestType = "PASSWORD_RESET"

        };

        var request = new HttpRequestMessage(new HttpMethod("POST"), "https://identitytoolkit.googleapis.com/v1/accounts:resetPassword?key=AIzaSyDsRqHl36anY_xBrPOf1lzKXt4IsSDAVk8");
            
                request.Content = new StringContent(JsonConvert.SerializeObject(requestBody));
                request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                var response = await httpClient.SendAsync(request);
            

    }
    

    
    
    
    [HttpPost("ConfirmPasswordReset")]
    public async Task ConfirmPasswordReset(string oobCode, string newPassword)
    {


        var httpClient = new HttpClient();

        var requestBody = new
        {
            oobCode = oobCode,
            newPassword = newPassword

        };

        var request = new HttpRequestMessage(new HttpMethod("POST"), "https://identitytoolkit.googleapis.com/v1/accounts:resetPassword?key=AIzaSyDsRqHl36anY_xBrPOf1lzKXt4IsSDAVk8");
            
                request.Content = new StringContent(JsonConvert.SerializeObject(requestBody));
                request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                var response = await httpClient.SendAsync(request);
            
        


    }


}
