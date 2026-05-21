using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.HttpClients.AccessControlClient {
    public class CaptureFingerResponse {
        public string FingerData { get; set; }
        public int FingerNo { get; set; }
        public int FingerPrintQuality { get; set; }

        public string Message { get; set; }

        public int TotalStatus { get; set; }
    }
}
