using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TRuDI.Backend.KoaLaExtension.Dtos
{
   public class StatusResponseDto
   {
      public int JobId;
      public JobStatus Status;
      public int Progress;
   }

   public enum JobStatus
   {
      JobUnknown,
      Running,
      Done,
      Failed
   }
}
