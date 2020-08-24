using antique_api.Models.Antique;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace antique_api.ViewModels
{
    public class OrderModel
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string City { get; set; }
        public string Number { get; set; }
        public string Delivery { get; set; }
        public string DeliveryNum { get; set; }
        public double TotalPrice { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
