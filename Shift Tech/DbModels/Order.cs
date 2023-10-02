﻿using System.ComponentModel.DataAnnotations.Schema;

namespace Shift_Tech.DbModels
{
    public enum Status
    {
        NotPaid,Paid,Sent,Received
    };

    public class Order
    {
        public Guid Guid { get; set; }
        public int Id { get; set; }
        public User User { get; set; }
		public virtual ICollection<OrderProduct> Products { get; set; }
		public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string PostalCode { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public Status Status { get; set; }

        public double TotalPrice()
        {
            return Math.Floor(Products.Sum(x=> x.ProductCount * x.Product.Price) * 100) / 100;
        }
    }
}
 