using NortwindDAL;
using System;
using System.Collections.Generic;

namespace NorthwindDAL
{
  public enum Status { InProgress, New, Done}
  public class Order
  {
    public int OrderId { get; set; }
    public DateTime? OrderDate { get; set; }
    public string CustomerID { get; set; }
    public int EmployeeID { get; set; }
    public DateTime RequiredDate { get; set; }
    public DateTime? ShippedDate { get; set; }
    public int ShipVia { get; set; }
    public decimal Freight { get; set;}
    public string ShipName { get; set; }
    public string ShipAddress { get; set; }
    public string ShipCity { get; set; }
    public string ShipRegion { get; set; }
    public string ShipPostalCode { get; set; }
    public string ShipCountry { get; set; }

    Status StatusEnum;
    public Status StatusProperty
    {
      get
      {
        return StatusEnum;
      }
      set
      {
        StatusEnum = value;
      }
    }
    public List<OrderDetail> Details {get;set;}

    public List<Product> Products { get; set; }
  }
}