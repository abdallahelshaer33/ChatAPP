using ChatAPI.Data;
using ChatAPI.DTO;
using ChatAPI.Extensions;
using ChatAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Numerics;

namespace ChatAPI.Hubs
{
    //[Authorize]
    public class ChatHub (UserManager<APPUser> _usermanager,Appdbcontext context):Hub
    {
        public static readonly ConcurrentDictionary<string, OnlineUserDto> 
            onlineUsers = new() ;

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var recieverID = httpContext?.Request.Query["senderid"].ToString();
            var username = Context.User.Identity!.Name;
            var currentuser = await _usermanager.FindByNameAsync(username);
            var connectionid = Context.ConnectionId;
            if (onlineUsers.ContainsKey(username))
            {
                onlineUsers[username].ConnectionID = connectionid;

            }
            else
            {
                var user = new OnlineUserDto
                {
                    ConnectionID = connectionid,
                    Username = username,
                    Profilepicture = currentuser!.ProfileImage,
                    FullName = currentuser.Fullname
                };
                onlineUsers.TryAdd(username, user);
                await Clients.AllExcept(connectionid).SendAsync("Notify", currentuser);
            }
            if(!string.IsNullOrEmpty(recieverID))
            {
                await LoadMessage(recieverID);
            }

            await Clients.All.SendAsync("onlineUsers" ,await GetAllUsers());

        }

        public async Task LoadMessage(string Recipientid ,int pageNumber = 1)
        {
            int pagesize = 10;
            var username = Context.User!.Identity!.Name;
            var currentuser = await _usermanager.FindByNameAsync(username);
            if(currentuser is null)
            {
                return;
            }
            List<MessageResponseDTO> messages = await context.messages
                .Where(x => x.RecieverID == currentuser!.Id && x.SenderId == Recipientid || x.SenderId == currentuser!.Id && x.RecieverID == Recipientid)
                .OrderByDescending(x => x.CreatedDate).Skip((pageNumber - 1) * pagesize).Take(pagesize).OrderBy(x => x.CreatedDate)
                .Select(x => new MessageResponseDTO
                {
                    id = x.ID,
                    Content = x.Content,
                    CreatedDate = x.CreatedDate,
                    RecieverId = x.RecieverID,
                    SenderId = x.SenderId
                }).ToListAsync();
            foreach(var msg in messages)
            {
                var mesage = await context.messages.FirstOrDefaultAsync(x=>x.ID == msg.id);
                if (mesage != null && mesage.RecieverID == currentuser.Id)
                {
                    mesage.IsRead = true;
                    await context.SaveChangesAsync();
                }
            }
            await Clients.User(currentuser.Id).SendAsync("RecieveMessageList", messages);
        }
       
        public  async Task sendMessage(MessageRequestDTO message)
        {
            var senderId = Context.User!.Identity!.Name;
            var reciepentId = message.RecieverId;
            var NewMsg = new Message
            {
                Sender = await _usermanager.FindByNameAsync(senderId!),
                Reciever = await _usermanager.FindByIdAsync(reciepentId!),
                IsRead = false,
                CreatedDate =DateTime.UtcNow,
                Content = message.Content

            };
            await context.messages.AddAsync(NewMsg);
            await context.SaveChangesAsync();
            await Clients.User(reciepentId!).SendAsync("RecieveMessageList", NewMsg);

        } 

        public async Task NotifyTyping (string recipentUsername)
        {
            var senderUsername = Context.User!.Identity.Name;
            if(senderUsername is null)
            {
                return;
            }
            var connectionid = onlineUsers.Values.FirstOrDefault(x => x.Username == recipentUsername)?.ConnectionID;
            if(connectionid != null)
            {
                await Clients.Client(connectionid).SendAsync("NotifyTypingToUser", senderUsername);
            }
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var Username = Context.User!.Identity!.Name;
            onlineUsers.TryRemove(Username!, out _);
            await Clients.All.SendAsync("onlineUsers", await GetAllUsers());

        }
        private async Task<IEnumerable<OnlineUserDto>> GetAllUsers()
        {
            var user = Context.User!.GetUserName();
            var onlineUserSet = new HashSet<string>(onlineUsers.Keys);
            var users = await _usermanager.Users.Select(u => new OnlineUserDto
            {
                ID = u.Id,
                Username = u.UserName,
                FullName = u.Fullname,
                Profilepicture = u.ProfileImage,
                IsOnline = onlineUserSet.Contains(u.UserName),
                UnreadCount =   context.messages.Count(x=>x.RecieverID== u.UserName && x.SenderId == u.Id && !x.IsRead)  
            }).OrderByDescending(u=>u.IsOnline).ToListAsync();
            return users;   

        }
    }
}
