using FirebaseAdmin;

namespace Dev.TestMate.WebAPI.Common;

public class TestMateException:Exception
{
    public TestMateException(FirebaseException firebaseException) : base() 
    {
    
    
    }
   
}
