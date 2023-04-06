using AutoMapper;
using FirebaseAdmin.Messaging;

using Microsoft.AspNetCore.SignalR;

using SBEU.Tasklet.Api.Hubs;
using SBEU.Tasklet.DataLayer.DataBase.Entities;
using SBEU.Tasklet.Models.Responses;

namespace SBEU.Tasklet.Api.Service
{
    public static class Notifier
    {
        private static IMapper _mapper;

        public static void SetMapper(IMapper mapper)
        {
            _mapper = mapper;
        }
        public static async Task FireBaseNotify(XTask task)
        {
            if (task.Executor.PushToken != null && task.Executor.IsPushOn)
            {
                MulticastMessage message;
                message = new MulticastMessage()
                {
                    Tokens = new[] { task.Executor.PushToken },
                    Data = new Dictionary<string, string>()
                    {
                        { "title", "У вас новая задача" },
                        { "body", $"{task.Title}" },
                    },
                    Notification = new Notification()
                    {
                        Title = "У вас новая задача",
                        Body = $"{task.Title}"
                    }
                };
                await FirebaseMessaging.DefaultInstance.SendMulticastAsync(message);
            }
        }

        public static async Task FireBaseNewMessage(List<string> tokens, string chat, string text)
        {
            MulticastMessage message;
            message = new MulticastMessage()
            {
                Tokens = tokens,
                Data = new Dictionary<string, string>()
                {
                    {"title", chat},
                    {"body", text},
                },
                Notification = new Notification()
                {
                    Title = chat,
                    Body = text
                }
            };
            await FirebaseMessaging.DefaultInstance.SendMulticastAsync(message);
        }

        public static async Task SocketNotify(XTask task, IHubContext<TaskHub, IXTask> taskHub,
            IEnumerable<string> tableUsers)
        {
            var taskDto = _mapper.Map<TaskDto>(task);
            await taskHub.Clients.Users(task.Hidden ? new[] { task.Author.Id, task.Executor.Id } : tableUsers).UpdateTask(taskDto);
        }

        public static async Task FullNotify(XTask task,  IHubContext<TaskHub, IXTask> taskHub, IEnumerable<string> tableUsers, string userId)
        {
            var taskDto = _mapper.Map<TaskDto>(task);
            await SocketNotify(task, taskHub, tableUsers);
            if (task.Executor.IsMailNotify)
            {
                await ConfirmationEmail.SendNotification(task, task.Executor, true);
            }

            if (task.Executor.PushToken != null && task.Executor.IsPushOn)
            {
                await FireBaseNotify(task);
            }
        }

        public static async Task FullMessageNotify(List<string> ids, string title, string message, IHubContext<ChatHub,IChat> chatHub, XMessage mess)
        {
            await FireBaseNewMessage(ids, title, message);
            var messDto = _mapper.Map<MessageDto>(mess);
            await chatHub.Clients.Group(mess.Chat.Id).Message(messDto);
        }
    }
}
