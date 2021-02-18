using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;


namespace TRuDI.Backend.KoaLaExtension
{
   public class TcpWrapper
   {
      private static ManualResetEvent clientConnected = new ManualResetEvent(false);
      private TcpListener tcp;
      private Action<NetworkStream> action;

      public TcpWrapper(int port, Action<NetworkStream> action)
      {
         this.action = action;
         clientConnected.Reset();
         tcp = new TcpListener(IPAddress.Any, port);
         tcp.BeginAcceptSocket(new AsyncCallback(AcceptCallback), tcp);
      }
      private void AcceptCallback(IAsyncResult ar)
      {
         var listener = (TcpListener)ar.AsyncState;
         var client = listener.AcceptTcpClient();
         var stream = client.GetStream();

         action.Invoke(stream);

         stream.Close();
         client.Close();

         clientConnected.Set();
      }
   }
}
