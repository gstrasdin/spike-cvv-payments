using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spike.Payments
{
   public class BraintreeConfiguration
   {
      public bool IsLive { get; set; }
      public string MerchantId { get; set; }
      public string MerchantAccountId { get; set; }
      public string BackupMerchantAccountId { get; set; }
      public string PublicKey { get; set; }
      public string PrivateKey { get; set; }
      public string EncryptionKey { get; set; }
   }
}
