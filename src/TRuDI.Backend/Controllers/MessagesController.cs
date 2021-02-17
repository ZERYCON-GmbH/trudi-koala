namespace TRuDI.Backend.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    using TRuDI.Backend.MessageHandlers;

    public class MessagesController : Controller
    {
        private readonly NotificationsMessageHandler notificationsMessageHandler;

        public MessagesController(NotificationsMessageHandler notificationsMessageHandler)
        {
            this.notificationsMessageHandler = notificationsMessageHandler;
        }
    }
}
