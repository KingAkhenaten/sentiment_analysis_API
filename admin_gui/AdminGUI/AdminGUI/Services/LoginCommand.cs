namespace AdminGUI.Services;

public class LoginCommand
{
    public static bool Execute(string username, string password)
    {
        if (username == "admin" && password == "admin")
        {
            return true;
        }

        return false;
    }
}