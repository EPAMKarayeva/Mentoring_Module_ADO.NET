using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using DevExpress.Xpo.DB;
using NorthwindDAL;

namespace ADOModule
{
  class Program
  {
    static void Main(string[] args)
    {
      string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=Northwind;Integrated Security=True";
      DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance);

      OrderRepository orderRepository = new OrderRepository(connectionString, "System.Data.SqlClient");
      
      var listOrders = orderRepository.GetOrders();

      //foreach (var order in listOrders)
      //{
      //  Console.WriteLine($"{order.OrderId}, {order.OrderDate}, {order.CustomerID}, " +
      //    $"{order.EmployeeID},{order.RequiredDate}, {order.ShippedDate}, {order.ShipName},{order.ShipVia}, " +
      //    $"{order.ShipRegion}, {order.ShipCity}, {order.ShipAddress}, {order.ShipPostalCode}, {order.StatusProperty}");
      //}

      var orderDetail = orderRepository.GetOrderById(10248);

      Console.WriteLine($"{orderDetail.OrderId}, {orderDetail.OrderDate}, {orderDetail.CustomerID}, " +
          $"{orderDetail.EmployeeID},{orderDetail.RequiredDate}, {orderDetail.ShippedDate}, {orderDetail.ShipName},{orderDetail.ShipVia}, " +
          $"{orderDetail.ShipRegion}, {orderDetail.ShipCity}, {orderDetail.ShipAddress}, {orderDetail.ShipPostalCode}, {orderDetail.StatusProperty}");

      foreach (var product in orderDetail.Products)
      {
        Console.WriteLine($"{product.ProductID}, {product.ProductName}");
      }

      foreach (var detail in orderDetail.Details)
      {
        Console.WriteLine($"{detail.OrderID}, {detail.ProductId}, {detail.Quantity}, {detail.UnitPrice}, {detail.Discount}");
      }


      var newOrder = new Order
      {
        CustomerID= "VINET",
        EmployeeID=1,
        OrderDate= DateTime.Now,
        RequiredDate = DateTime.Now,
        ShippedDate = DateTime.Now,
        ShipVia = 1,
        Freight = 123,
        ShipName = "Ernts Handle",
        ShipAddress="Pr. Nursultan Nazarbaev",
        ShipCity= "Nur-Sultan",
        ShipCountry = "Kazakhstan",
        ShipRegion = "New Region"
      };

      //orderRepository.Add(newOrder);

      //orderRepository.Update(10255);

      //orderRepository.Delete(11086);

      var order = orderRepository.MoveToDone(11008, DateTime.Now);

      Console.WriteLine($"{order.OrderId}, {order.ShippedDate}, {order.StatusProperty}");

      Console.Read();
    }
  }
}
