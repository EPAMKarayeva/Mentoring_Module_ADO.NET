using Microsoft.VisualStudio.TestTools.UnitTesting;
using NorthwindDAL;
using NortwindDAL;
using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;

namespace NorthwindTest
{
  [TestClass]
  public class NorthwindTests
  {
    public static string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=Northwind;Integrated Security=True";
  
    [TestMethod]
    public void NorthwindDALOrderRepositoryGetOrdersTest()
    {
      //Arrange
      DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance);
      OrderRepository orderRepository = new OrderRepository(connectionString, "System.Data.SqlClient");
      var order = new Order();

      //Act
      var listOrders = orderRepository.GetOrders();
      var orderFromQuery = listOrders.FirstOrDefault();

      //Assert
      Assert.AreEqual(listOrders.FirstOrDefault().GetType(), order.GetType());
      Assert.IsNotNull(listOrders);
      Assert.AreEqual(orderFromQuery.OrderId.GetType(), order.OrderId.GetType());
      
    }


    [TestMethod]
    public void NorthwindDALOrderRepositoryGetOrderByIdTest()
    {
      //Arrange
      DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance);
      OrderRepository orderRepository = new OrderRepository(connectionString, "System.Data.SqlClient");
      int id = 11072;
      var product = new Product();
      var orderDetail = new OrderDetail();
      //Act
      var order = orderRepository.GetOrderById(11072);

      //Assert
      Assert.IsNotNull(order);
      Assert.AreEqual(order.OrderId, id);
      Assert.AreEqual(order.ShipCity, "Graz");
      Assert.IsNotNull(order.Products);
      Assert.AreEqual(order.Products.FirstOrDefault().GetType(), product.GetType());
      Assert.IsNotNull(order.Details);
      Assert.IsTrue(order.Products.Count>= 1);
      Assert.AreEqual(order.Details.FirstOrDefault().GetType(), orderDetail.GetType());
      Assert.IsTrue(order.Details.Count>= 1);
    }

    [TestMethod]
    public void NorthwindDALOrderRepositoryUpdateTest()
    {
      //Arrange
      DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance);
      OrderRepository orderRepository = new OrderRepository(connectionString, "System.Data.SqlClient");
      Order order = orderRepository.GetOrderById(10256);

      //Act
      var newOrder = orderRepository.Update(10256);

      //Assert
      Assert.IsNotNull(newOrder);
      Assert.AreEqual(order.GetType(), newOrder.GetType());
      Assert.AreEqual(order.OrderId, newOrder.OrderId);
      Assert.AreEqual(order.OrderDate, newOrder.OrderDate);
      Assert.AreEqual(order.StatusProperty, newOrder.StatusProperty);
      Assert.AreEqual(order.ShippedDate, newOrder.ShippedDate);
      Assert.AreEqual(newOrder.ShipCity, "Paris");

    }


    [TestMethod]
    public void NorthwindDALOrderRepositoryDeleteTest()
    {
      //Arrange
      DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance);
      OrderRepository orderRepository = new OrderRepository(connectionString, "System.Data.SqlClient");
      
      //Act
      orderRepository.Delete(11101);
      var order = orderRepository.GetOrderById(11101);

      //Assert
      Assert.AreEqual(order.OrderId, 0);


    }

    [TestMethod]
    public void NorthwindDALOrderRepositoryMoveToDoneTest()
    {
      //Arrange
      DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance);
      OrderRepository orderRepository = new OrderRepository(connectionString, "System.Data.SqlClient");
      var date = DateTime.Now;
      //Act
      orderRepository.MoveToDone(11076, date);
      var order = orderRepository.GetOrderById(11076);

      //Assert
      Assert.AreEqual(order.StatusProperty, Status.Done);

    }


  }
}
