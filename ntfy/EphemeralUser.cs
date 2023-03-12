namespace ntfy;

public class EphemeralUser: User
{
    public Client Client { get; }

    public async Task Delete()
    {
        return;
    }

    public EphemeralUser(Client client, string username, string password) : base(username, password)
    {
        Client = client;
    }
    
    public EphemeralUser(Client client, string accessToken) : base(accessToken)
    {
        Client = client;
    }
}
