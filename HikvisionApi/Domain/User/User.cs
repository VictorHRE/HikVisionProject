namespace Domain.User;

public class User
{
    public UserId Id { get; set; }
    
    public string Username { get; set; }
    
    public UserLevel UserLevel { get; set; }
    
    public User(UserId id, string username, UserLevel userLevel)
    {
        Id = id;
        Username = username;
        UserLevel = userLevel;
    }
    
    public User()
    {
        Id = new UserId(0);
        Username = string.Empty;
        UserLevel = UserLevel.Unknown;
    }
}