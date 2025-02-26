using Microsoft.Extensions.Options;
using OrderManagement.Domain.Interfaces;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Application.Services
{
    public class PaymentService : IPaymentService   
    {
        private readonly StripeSettings _stripeSettings;

        public PaymentService(IOptions<StripeSettings> stripeSettings)
        {
            _stripeSettings = stripeSettings.Value;

            // Initialize Stripe with the Secret Key
            StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
        }

        public async Task<bool> ProcessPaymentAsync(decimal amount, string cardNumber, string expiry, string cvc)
        {
            try
            {
                // Parse expiry date
                var expiryParts = expiry.Split('/');
                if (expiryParts.Length != 2)
                    throw new Exception("Invalid expiry format. Use MM/YY.");

                var expMonth = expiryParts[0];
                var expYear = expiryParts[1];

                // Convert amount to cents (Stripe processes in smallest currency unit)
                var amountInCents = (long)(amount * 100);

                // Create a Stripe token for the card
                var tokenOptions = new TokenCreateOptions
                {
                    Card = new TokenCardOptions
                    {
                        Number = cardNumber,
                        ExpMonth = expMonth,
                        ExpYear = expYear,
                        Cvc = cvc
                    }
                };

                var tokenService = new TokenService();
                Token stripeToken = await tokenService.CreateAsync(tokenOptions);

                // Create the charge
                var chargeOptions = new ChargeCreateOptions
                {
                    Amount = amountInCents,
                    Currency = "usd",
                    Description = "Order Payment",
                    Source = stripeToken.Id
                };

                var chargeService = new ChargeService();
                Charge charge = await chargeService.CreateAsync(chargeOptions);

                // Check if the payment succeeded
                return charge.Status == "succeeded";
            }
            catch (Exception ex)
            {
                // Log or handle the error
                Console.WriteLine($"Payment failed: {ex.Message}");
                return false;
            }
        }
    }
}
