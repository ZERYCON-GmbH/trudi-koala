using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TRuDI.Backend.KoaLaExtension.Dtos
{
   public class JobRequestDto
   {
      /// <summary>
      /// 
      /// </summary>
      public RequestType Type;

      /// <summary>
      /// 
      /// </summary>
      public object data;
   }

   public enum RequestType
   {
      // Todo
   }
}
