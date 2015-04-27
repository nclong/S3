using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public static class AccountManager
{
    private static S3AccountModel db;
    static AccountManager()
    {
        db = new S3AccountModel();
    }

    public static S3DataResponse RequestRegister(S3DataRequest registerRequest)
    {
        S3Accounts result = db.S3Accounts.Find( registerRequest.UserName );
        if( result == null)
        {
            S3Accounts newAccount = new S3Accounts
            {
                UserName = registerRequest.UserName,
                passwordHash = registerRequest.passwordHash
            };

            db.S3Accounts.Add( newAccount );
            db.SaveChanges();

            return new S3DataResponse
            {
                responseCode = 1,
                message = "Account successfully created!"
            };
        }
        else
        {
            return new S3DataResponse
            {
                responseCode = -1,
                message = "Cannot Register. User name already exists."
            };
        }
    }

    public static S3DataResponse RequestLogin(S3DataRequest loginRequest)
    {
        S3Accounts result = db.S3Accounts.Find( loginRequest.UserName );
        if( result != null && result.passwordHash == loginRequest.passwordHash )
        {
            return new S3DataResponse
            {
                responseCode = 1,
                message = "Login Successful."
            };
        }
        else
        {
            return new S3DataResponse
            {
                responseCode = -1,
                message = "User name or password is incorrect."
            };
        }
    }
}

