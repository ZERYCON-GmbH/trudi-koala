namespace TRuDI.Backend.MessageHandlers
{
   using System;
   using System.Diagnostics;
   using System.Net.WebSockets;
   using System.Threading.Tasks;

   using Serilog;

   using WebSocketManager;

   /// <summary>
   /// Manages sending of messages using a web socket connection to the frontend.
   /// This is used to update the progress status page.
   /// </summary>
   /// <seealso cref="WebSocketManager.WebSocketHandler" />
   public class NotificationsMessageHandler : WebSocketHandler
   {
      private WebSocket lastSocket;

      public NotificationsMessageHandler(WebSocketConnectionManager webSocketConnectionManager) : base(webSocketConnectionManager)
      {
      }

      public override Task OnConnected(WebSocket socket)
      {
         this.lastSocket = socket;

         Log.Debug("OnConnected: {0}", socket);
         return base.OnConnected(socket);
      }

      public override Task OnDisconnected(WebSocket socket)
      {
         Log.Debug("OnDisconnected: {0}", socket);
         return base.OnDisconnected(socket);
      }

      public async Task ProgressUpdate(string message, int progress)
      {
         Log.Information("Progress update: {0}, {1} %", message, progress);

         if (this.lastSocket == null || this.lastSocket.State == WebSocketState.Closed)
         {
            return;
         }

         try
         {
            await this.InvokeClientMethodAsync(this.WebSocketConnectionManager.GetId(this.lastSocket), "ProgressUpdate", new object[] { System.Net.WebUtility.HtmlEncode(message), progress });
         }
         catch (Exception ex)
         {
            Log.Error(ex, "Updating process failed: {0}", ex.Message);
         }
      }

      public async Task LoadNextPage(string url)
      {
         Log.Debug("LoadNextPage: {0}", url);

         if (this.lastSocket == null)
         {
            return;
         }

         try
         {
            await this.InvokeClientMethodAsync(this.WebSocketConnectionManager.GetId(this.lastSocket), "LoadNextPage", new object[] { url });
         }
         catch (Exception ex)
         {
            Log.Error(ex, "LoadNextPage: {0}", ex.Message);
         }
      }
   }
}
