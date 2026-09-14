using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Debtorslist1.Models
{
    public class HistoryRecord
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid DebtorId { get; set; }
        public decimal Amount { get; set; }
        public DateTime ChangeDate { get; set; } = DateTime.Now;
    }
}
