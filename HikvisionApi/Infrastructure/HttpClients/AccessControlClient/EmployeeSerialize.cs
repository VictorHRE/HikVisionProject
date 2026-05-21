using Domain.Employee;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.HttpClients.AccessControlClient {
    public class EmployeeSerialize {

        public string identificationNumber { get; set; }

        public string Name { get; set; }

        public string LastName { get; set; }

        public string? Position { get; set; }

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public int BranchId { get; set; }//idStore

        public string Status { get; set; }

        // changed to public so callers can set the gender when mapping
        public string? Gender { get; set; }

		public string BirthDate { get; set; }

		public string BeginDate { get; set; }

		public string EndDate { get; set; }

	}
}
