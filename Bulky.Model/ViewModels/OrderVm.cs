using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models.ViewModels
{
    public class OrderVm
    {
        public OrderHeader oderHeader { get; set; }
        public IEnumerable<OrderDetail> orderDetail { get; set; }
    }
}
