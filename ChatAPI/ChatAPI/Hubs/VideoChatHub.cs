using Microsoft.AspNetCore.SignalR;

namespace ChatAPI.Hubs
{
    public class VideoChatHub : Hub
    {
        public async Task SendOffer(string recieverID, string offer)
        {
            await Clients.User(recieverID).SendAsync("recieverID");
      }   
    }
}