using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Domain.Interfaces
{
    public interface IPaymentService
    {
        Task<bool> ProcessPaymentAsync(decimal amount, string cardNumber, string expiry, string cvc);
    }
}
