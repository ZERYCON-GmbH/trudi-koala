using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Xml.Serialization;
using TRuDI.Backend.KoaLaExtension.Dtos;

namespace TRuDI.Backend.KoaLaExtension
{
   public class KoaLaHanListener : IKoaLaHanListener
   {
      private const int PORT = 1337;
      
      KoaLaHanListener()
      {    
         var JobRequestDeseiralizer = new XmlSerializer(typeof(JobRequestDto));
         var JobResponseSerializer = new XmlSerializer(typeof(JobResponseDto));
         var StatusRequestDeserializer = new XmlSerializer(typeof(StatusRequestDto));
         var StatusResponseSerializer = new XmlSerializer(typeof(StatusResponseDto));
         var DataRequestSerializer = new XmlSerializer(typeof(DataRequestDto));
         var DataResponseSerializer = new XmlSerializer(typeof(DataResponseDto));

         object jobDetailLock = new object();
         var jobDone = false;
         var jobProgress = 0;
         object jobResult;


         var tcp = new TcpWrapper(PORT, stream =>
         {
            var jobResponse = JobRequestDeseiralizer.Deserialize(stream);

            var jobId = new Random().Next();
            JobResponseSerializer.Serialize(stream, new JobResponseDto
            {
               JobId = jobId
            });

            // detatch worker thread here, use labda to or call by reference to use job*-Variables

            while (true)
            {
               var statusRequest = StatusRequestDeserializer.Deserialize(stream);

               lock (jobDetailLock)
               {
                  if (jobDone)
                  {
                     StatusResponseSerializer.Serialize(stream, new StatusResponseDto
                     {
                        JobId = jobId,
                        Status = JobStatus.Done,
                        Progress = 100
                     });
                  }
                  else
                  {
                     StatusResponseSerializer.Serialize(stream, new StatusResponseDto
                     {
                        JobId = jobId,
                        Status = JobStatus.Running,
                        Progress = jobProgress
                     });
                     break;
                  }
               }
            }

            var dataRequest = DataRequestSerializer.Deserialize(stream);
            // ToDo: return 
         });
      }
   }
}
