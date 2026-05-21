using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.HumandClient {
    public class HumandTimeResponseSerialize {
        public int id { get; set; }
        public int userId { get; set; }
        public string employeeInternalId { get; set; }
        public string type { get; set; }
    }
}
