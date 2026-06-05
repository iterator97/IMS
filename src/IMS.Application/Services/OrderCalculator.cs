using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IMS.Application.Shared;
using IMS.Domain;
using Microsoft.EntityFrameworkCore;

namespace IMS.Application.Services
{
    public sealed class OrderCalculator : IOrderCalculator
    {
        private readonly IAppDbContext _context;

        public OrderCalculator(IAppDbContext context)
        {
            _context = context;
        }

        public async Task Calculate(
            Order order,
            Address address,
            IReadOnlyDictionary<Guid, Product> productsById,
            CancellationToken cancellationToken)
        {
            order.LocationCharge = CalculateLocationCharge(address);

            foreach (var item in order.Items)
            {
                EnsureProductExists(item.ProductId, productsById);
                item.Discount = CalculateQuantityDiscount(item.Quantity);
            }

            var seasonalDiscount = await GetActiveDiscountAsync(
                address,
                order.CreatedAt,
                cancellationToken);

            if (seasonalDiscount != null)
            {
                ApplySeasonalDiscount(order, productsById, seasonalDiscount);
            }

            order.TotalAmount = CalculateTotalAmount(order, productsById);
        }

        public decimal CalculateLocationCharge(Address address)
        {
            return address.Continent.ToLower() switch
            {
                "us" => 0.0m,
                "europe" => 0.15m,
                "asia" => 0.05m,
                _ => 0m
            };
        }

        private async Task<Discount> GetActiveDiscountAsync(
            Address address,
            DateTime orderDate,
            CancellationToken cancellationToken)
        {
            return await _context.Discounts
                .AsNoTracking()
                .Where(discount =>
                    discount.Enabled &&
                    discount.StartDate <= orderDate &&
                    discount.EndDate >= orderDate &&
                    discount.Country.ToLower() == address.Country.ToLower() &&
                    discount.Continent.ToLower() == address.Continent.ToLower())
                .OrderByDescending(discount => discount.Amount)
                .ThenByDescending(discount => discount.StartDate)
                .FirstOrDefaultAsync(cancellationToken);
        }

        private static void ApplySeasonalDiscount(
            Order order,
            IReadOnlyDictionary<Guid, Product> productsById,
            Discount seasonalDiscount)
        {
            switch (seasonalDiscount.Mode)
            {
                case DiscountMode.OnAll:
                    foreach (var item in order.Items)
                    {
                        item.Discount = Math.Max(item.Discount, seasonalDiscount.Amount);
                    }

                    break;

                case DiscountMode.OnHighest:
                    ApplyDiscountToSingleItem(
                        order,
                        productsById,
                        seasonalDiscount.Amount);
                    break;
            }
        }

        private static void ApplyDiscountToSingleItem(
            Order order,
            IReadOnlyDictionary<Guid, Product> productsById,
            decimal discountAmount)
        {
            var item = order.Items
                .OrderByDescending(orderItem => CalculateLineAmount(orderItem, productsById))
                .ThenBy(orderItem => orderItem.ProductId)
                .FirstOrDefault();

            if (item != null)
            {
                item.Discount = Math.Max(item.Discount, discountAmount);
            }
        }

        private static decimal CalculateTotalAmount(
            Order order,
            IReadOnlyDictionary<Guid, Product> productsById)
        {
            var amount = order.Items.Sum(item =>
            {
                return CalculateLineAmount(item, productsById) * (1 - item.Discount);
            });

            return Math.Round(amount * (1 + order.LocationCharge), 2, MidpointRounding.AwayFromZero);
        }

        private static decimal CalculateLineAmount(
            OrderItem item,
            IReadOnlyDictionary<Guid, Product> productsById)
        {
            var product = EnsureProductExists(item.ProductId, productsById);

            return item.Quantity * product.Price;
        }

        private static Product EnsureProductExists(
            Guid productId,
            IReadOnlyDictionary<Guid, Product> productsById)
        {
            if (!productsById.TryGetValue(productId, out var product))
                throw new InvalidOperationException($"Product '{productId}' was not found for order calculation.");

            return product;
        }

        private static decimal CalculateQuantityDiscount(int quantity) =>
            quantity switch
            {
                > 50 => 0.3m,
                > 10 => 0.2m,
                > 5 => 0.1m,
                _ => 0m
            };
    }
}
