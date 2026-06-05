using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IMS.Domain;

namespace IMS.Application.Services
{
    public interface IOrderCalculator
    {
        Task Calculate(
            Order order,
            Address address,
            IReadOnlyDictionary<Guid, Product> productsById,
            CancellationToken cancellationToken);

        decimal CalculateLocationCharge(Address address);
    }
}
