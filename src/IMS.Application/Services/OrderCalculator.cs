using System;
using System.Collections.Generic;
using System.Linq;
using IMS.Domain;

namespace IMS.Application.Services
{
    public sealed class OrderCalculator : IOrderCalculator
    {
        public Order Calculate(
            Order order,
            string region,
            IReadOnlyDictionary<Guid, Product> productsById,
            Discount? discount = null)
        {
            ArgumentNullException.ThrowIfNull(order);
            ArgumentNullException.ThrowIfNull(productsById);

            order.LocationCharge = CalculateLocationCharge(region);

            foreach (var item in order.Items)
            {
                EnsureProductExists(item.ProductId, productsById);
                item.Discount = CalculateQuantityDiscount(item.Quantity);
            }

            if (discount != null)
            {
                ApplySeasonalDiscount(order, productsById, discount);
            }

            order.TotalAmount = CalculateTotalAmount(order, productsById);

            return order;
        }

        public decimal CalculateLocationCharge(string region)
        {
            if (string.IsNullOrWhiteSpace(region))
                return 0m;

            return region.Trim().ToLowerInvariant() switch
            {
                "us" => 0.0m,
                "europe" => 0.15m,
                "asia" => 0.05m,
                _ => 0m
            };
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
                >= 50 => 0.3m,
                >= 10 => 0.2m,
                >= 5 => 0.1m,
                _ => 0m
            };
    }
}
