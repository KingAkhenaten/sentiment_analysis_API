namespace AdminGUI.Services;

public class LoginCommand
{
    public bool Execute(string username, string password)
    {
        if (username == "admin" && password == "admin")
        {
            return true;
        }

        return false;
    }
}