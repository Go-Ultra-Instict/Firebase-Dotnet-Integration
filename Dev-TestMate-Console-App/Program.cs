using Firebase.Auth;
using FirebaseAdmin.Auth;

namespace Dev.Test.Mate.ConsoleApp;
public class Program
{
    private const string API_KEY = "AIzaSyDugDJG7csKUYxCtE0YBL-Fnb57Wdk4A5g";

    static async Task Main(string[] args)
    {
        FirebaseAuthProvider firebaseAuthProvider = new FirebaseAuthProvider(new FirebaseConfig(API_KEY));

        //FirebaseAuthLink firebaseAuthLink = await firebaseAuthProvider.CreateUserWithEmailAndPasswordAsync("singletonsean@gmail.com", "test123", "SingletonSean");
        FirebaseAuthLink firebaseAuthLink = await firebaseAuthProvider.SignInWithEmailAndPasswordAsync("test.vishnukumarps@gmail.com", "vISHNU@1234");

        Console.WriteLine(firebaseAuthLink.FirebaseToken);

        
    }
}

