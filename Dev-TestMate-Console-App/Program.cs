using Firebase.Auth;
using FirebaseAdmin.Auth;

namespace Dev.Test.Mate.ConsoleApp;
public class Program
{
    private const string API_KEY = "AIzaSyDsRqHl36anY_xBrPOf1lzKXt4IsSDAVk8";

    static async Task Main(string[] args)
    {
        FirebaseAuthProvider firebaseAuthProvider = new FirebaseAuthProvider(new FirebaseConfig(API_KEY));

        //FirebaseAuthLink firebaseAuthLink = await firebaseAuthProvider.CreateUserWithEmailAndPasswordAsync("anushree@gmail.com", "test1234", "anushree");
        FirebaseAuthLink firebaseAuthLink = await firebaseAuthProvider.SignInWithEmailAndPasswordAsync("anushri1811@gmail.com", "anushree@123");

        Console.WriteLine(firebaseAuthLink.FirebaseToken);

        
    }
}

