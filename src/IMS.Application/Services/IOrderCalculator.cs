using System;
using System.Collections.Generic;
using IMS.Domain;

namespace IMS.Application.Services
{
    public interface IOrderCalculator
    {
        Order Calculate(
            Order order,
            string region,
            IReadOnlyDictionary<Guid, Product> productsById,
            Discount discount);

        decimal CalculateLocationCharge(string region);
    }
}
