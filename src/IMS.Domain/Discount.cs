using System;

namespace IMS.Domain
{
    public class Discount
    {
        public Guid Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public required string Country { get; set; }
        public required string Region { get; set; }
        public decimal Amount { get; set; }
        public bool Enabled { get; set; }
        public DiscountMode Mode { get; set; }
    }
}
