using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Data.Common;
using NortwindDAL;

namespace NorthwindDAL
{
  public class OrderRepository : IOrderRepository
  {
    private readonly DbProviderFactory providerFactory;
    private readonly string ConnectionString;

    public OrderRepository(string connectionString, string provider)
    {
      providerFactory = DbProviderFactories.GetFactory(provider);
      ConnectionString = connectionString;
    }

    public virtual IEnumerable<Order> GetOrders()
    {
      var resultOrders = new List<Order>();

      using (var connection = providerFactory.CreateConnection())
      {
        connection.ConnectionString = ConnectionString;
        connection.Open();

        using (var command = connection.CreateCommand())
        {
          command.CommandText = "select OrderID, CustomerID, " +
            "EmployeeID, RequiredDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, " +
            "ShipCountry, ShipPostalCode, ShipRegion, OrderDate, ShippedDate from dbo.Orders";
          command.CommandType = CommandType.Text;

          using (var reader = command.ExecuteReader())
          {
            while (reader.Read())
            {
              var order = new Order();

              order.OrderId = reader.GetInt32(0);
              order.CustomerID = reader.GetString(1);
              order.EmployeeID = reader.GetInt32(2);
              order.RequiredDate = reader.GetDateTime(3);
              order.ShipVia = reader.GetInt32(4);
              order.Freight = reader.GetDecimal(5);
              order.ShipName = reader.GetString(6);
              order.ShipAddress = reader.GetString(7);
              order.ShipCity = reader.GetString(8);
              order.ShipCountry = reader.GetString(9);

              if (reader["ShipPostalCode"] != DBNull.Value) order.ShipPostalCode = reader.GetString(10);

              if (reader["ShipRegion"] != DBNull.Value) order.ShipRegion = reader.GetString(11);

              if (reader["OrderDate"] != DBNull.Value)
              {
                order.OrderDate = reader.GetDateTime(12);
              }
              else
              {
                order.StatusProperty = Status.New;
              }

              if (reader["ShippedDate"] != DBNull.Value)
              {
                order.ShippedDate = reader.GetDateTime(13);
                order.StatusProperty = Status.Done;
              }
              else
              {
                order.StatusProperty = Status.InProgress;
              }

              resultOrders.Add(order);
            }
          }
        }
      }

      return resultOrders;
    }


    public Order GetOrderById(int id)
    {

      using (var connection = providerFactory.CreateConnection())
      {
        connection.ConnectionString = ConnectionString;
        connection.Open();

        using (var command = connection.CreateCommand())
        {
          command.CommandText = "select OrderList.OrderID, OrderList.CustomerID, OrderList.EmployeeID, OrderList.RequiredDate, " +
            "OrderList.ShipVia, OrderList.Freight, OrderList.ShipName,OrderList.ShipAddress, OrderList.ShipCity, " +
            "OrderList.ShipCountry, OrderList.ShipPostalCode, OrderList.ShipRegion, OrderList.OrderDate, OrderList.ShippedDate, " +
            "Details.*, ProductsList.ProductID, ProductsList.ProductName from dbo.[Order Details]  as Details " +
            "INNER JOIN  dbo.Orders as OrderList on Details.OrderID = OrderList.OrderID " +
            "INNER JOIN dbo.Products as ProductsList on Details.ProductID = ProductsList.ProductID where OrderList.OrderID= @id ";
          command.CommandType = CommandType.Text;

          var paramId = command.CreateParameter();
          paramId.ParameterName = "@id";
          paramId.Value = id;
          command.Parameters.Add(paramId);

          using (var reader = command.ExecuteReader())
          {
            var order = new Order();

            while (reader.Read())
            {
              order.OrderId = reader.GetInt32(0);
              order.CustomerID = reader.GetString(1);
              order.EmployeeID = reader.GetInt32(2);
              order.RequiredDate = reader.GetDateTime(3);
              order.ShipVia = reader.GetInt32(4);
              order.Freight = reader.GetDecimal(5);
              order.ShipName = reader.GetString(6);
              order.ShipAddress = reader.GetString(7);
              order.ShipCity = reader.GetString(8);
              order.ShipCountry = reader.GetString(9);

              if (reader["ShipPostalCode"] != DBNull.Value) order.ShipPostalCode = reader.GetString(10);

              if (reader["ShipRegion"] != DBNull.Value) order.ShipRegion = reader.GetString(11);

              if (reader["OrderDate"] != DBNull.Value)
              {
                order.OrderDate = reader.GetDateTime(12);
              }
              else
              {
                order.StatusProperty = Status.New;
              }

              if (reader["ShippedDate"] != DBNull.Value)
              {
                order.ShippedDate = reader.GetDateTime(13);
                order.StatusProperty = Status.Done;
              }
              else
              {
                order.StatusProperty = Status.InProgress;
              }


              order.Details = new List<OrderDetail>();
              order.Products = new List<Product>();

              while (reader.Read())
              {
                if (!reader.HasRows) return null;

                var details = new OrderDetail
                {
                  OrderID = (Int32)reader["OrderID"],
                  ProductId = (Int32)reader["ProductID"],
                  UnitPrice = (decimal)reader["UnitPrice"],
                  Quantity = (Int16)reader["Quantity"],
                  Discount = (float)reader["Discount"]
                };

                order.Details.Add(details);

                var product = new Product
                {
                  ProductID = (int)reader["ProductID"],
                  ProductName = (string)reader["ProductName"]
                };

                order.Products.Add(product);

              }


            }
            return order;
          }
        }
      }
    }

    public Order Add(Order order)
    {
      using (var connection = providerFactory.CreateConnection())
      {
        connection.ConnectionString = ConnectionString;
        connection.Open();

        using (var command = connection.CreateCommand())
        {
          command.CommandText = "INSERT INTO dbo.Orders(CustomerID, EmployeeID, OrderDate, RequiredDate, " +
            "ShippedDate, ShipVia,  Freight, ShipName, ShipAddress, ShipCity, ShipCountry, ShipRegion) " +
            "VALUES (@customerId, @employeeId, @orderDate, @reqDate, @shippedDate, @shipVia, @freight, @shipName, @shipAddress,@shipCity, @shipCountry, @region)";
          command.CommandType = CommandType.Text;

          var customerId = command.CreateParameter();
          customerId.ParameterName = "@customerId";
          customerId.Value = order.CustomerID;
          command.Parameters.Add(customerId);

          var employeeId = command.CreateParameter();
          employeeId.ParameterName = "@employeeId";
          employeeId.Value = order.EmployeeID;
          command.Parameters.Add(employeeId);

          var orderDate = command.CreateParameter();
          orderDate.ParameterName = "@orderDate";
          orderDate.Value = order.OrderDate;
          command.Parameters.Add(orderDate);

          var reqDate = command.CreateParameter();
          reqDate.ParameterName = "@reqDate";
          reqDate.Value = order.RequiredDate;
          command.Parameters.Add(reqDate);

          var shippedDate = command.CreateParameter();
          shippedDate.ParameterName = "@shippedDate";
          shippedDate.Value = order.ShippedDate;
          command.Parameters.Add(shippedDate);

          var shipVia = command.CreateParameter();
          shipVia.ParameterName = "@shipVia";
          shipVia.Value = order.ShipVia;
          command.Parameters.Add(shipVia);

          var freight = command.CreateParameter();
          freight.ParameterName = "@freight";
          freight.Value = order.Freight;
          command.Parameters.Add(freight);

          var shipName = command.CreateParameter();
          shipName.ParameterName = "@shipName";
          shipName.Value = order.ShipName;
          command.Parameters.Add(shipName);

          var shipAddress = command.CreateParameter();
          shipAddress.ParameterName = "@shipAddress";
          shipAddress.Value = order.ShipAddress;
          command.Parameters.Add(shipAddress);

          var shipCity = command.CreateParameter();
          shipCity.ParameterName = "@shipCity";
          shipCity.Value = order.ShipCity;
          command.Parameters.Add(shipCity);

          var shipCountry = command.CreateParameter();
          shipCountry.ParameterName = "@shipCountry";
          shipCountry.Value = order.ShipCountry;
          command.Parameters.Add(shipCountry);

          var region = command.CreateParameter();
          region.ParameterName = "@region";
          region.Value = order.ShipRegion;
          command.Parameters.Add(region);

          command.ExecuteNonQuery();

          Console.WriteLine("1 row was added.");
        }

        return GetOrderById(order.OrderId);
      }
    }

    public Order Update(int id)
    {
      using (var connection = providerFactory.CreateConnection())
      {
        connection.ConnectionString = ConnectionString;
        connection.Open();

        using (var command = connection.CreateCommand())
        {
          command.CommandText = "UPDATE dbo.Orders SET ShipCity = 'Paris' WHERE dbo.Orders.OrderID = @id ";
          command.CommandType = CommandType.Text;

          var paramId = command.CreateParameter();
          paramId.ParameterName = "@id";
          paramId.Value = id;
          command.Parameters.Add(paramId);

          var order = GetOrderById(id);

          using (var reader = command.ExecuteReader())
          {
            

            while (reader.Read())
            {
              if (reader["ShippedDate"] != DBNull.Value || reader["ShippedDate"] == DBNull.Value)
              {
                Console.WriteLine("This order can not be updated.");
                break;
              }

              order.OrderId = reader.GetInt32(0);
              order.CustomerID = reader.GetString(1);
              order.EmployeeID = reader.GetInt32(2);
              order.RequiredDate = reader.GetDateTime(3);
              order.ShipVia = reader.GetInt32(4);
              order.Freight = reader.GetDecimal(5);
              order.ShipName = reader.GetString(6);
              order.ShipAddress = reader.GetString(7);
              order.ShipCity = reader.GetString(8);
              order.ShipCountry = reader.GetString(9);

              if (reader["ShipPostalCode"] != DBNull.Value) order.ShipPostalCode = reader.GetString(10);

              if (reader["ShipRegion"] != DBNull.Value) order.ShipRegion = reader.GetString(11);

              if (reader["OrderDate"] != DBNull.Value)
              {
                order.OrderDate = reader.GetDateTime(12);
              }
              else
              {
                order.StatusProperty = Status.New;
              }

              Console.WriteLine("1 row was updated.");
            }

            return order;
          }

 
        }
        
      }
    }

    public void Delete(int id)
    {
      using (var connection = providerFactory.CreateConnection())
      {
        connection.ConnectionString = ConnectionString;
        connection.Open();

        using (var command = connection.CreateCommand())
        {
          command.CommandText = "DELETE FROM dbo.Orders WHERE dbo.Orders.OrderID = @id";
          command.CommandType = CommandType.Text;

          var paramId = command.CreateParameter();
          paramId.ParameterName = "@id";
          paramId.Value = id;
          command.Parameters.Add(paramId);

          command.ExecuteNonQuery();
          Console.WriteLine("1 row was deleted.");

        }

      }

    }

    public Order MoveInProgress(int id, DateTime date)
    {
      using (var connection = providerFactory.CreateConnection())
      {
        connection.ConnectionString = ConnectionString;
        connection.Open();

        using (var command = connection.CreateCommand())
        {
          command.CommandText = "UPDATE dbo.Orders SET OrderDate= @newDate WHERE dbo.Orders.OrderID=@id";
          command.CommandType = CommandType.Text;

          var paramId = command.CreateParameter();
          paramId.ParameterName = "@id";
          paramId.Value = id;
          command.Parameters.Add(paramId);

          var newDate = command.CreateParameter();
          newDate.ParameterName = "@newDate";
          newDate.Value = date;
          command.Parameters.Add(newDate);

          command.ExecuteNonQuery();

          Console.WriteLine("Order was moved in 'Done' status");
        }

      }

      return GetOrderById(id);
    }


    public Order MoveToDone(int id, DateTime date)
    {
      using (var connection = providerFactory.CreateConnection())
      {
        connection.ConnectionString = ConnectionString;
        connection.Open();

        using (var command = connection.CreateCommand())
        {
          command.CommandText = "UPDATE dbo.Orders SET ShippedDate= @newDate WHERE dbo.Orders.OrderID=@id";
          command.CommandType = CommandType.Text;

          var paramId = command.CreateParameter();
          paramId.ParameterName = "@id";
          paramId.Value = id;
          command.Parameters.Add(paramId);

          var newDate = command.CreateParameter();
          newDate.ParameterName = "@newDate";
          newDate.Value = date;
          command.Parameters.Add(newDate);

          command.ExecuteNonQuery();

          Console.WriteLine("Order was moved in 'Done' status");
        }

      }

      return GetOrderById(id);
    }

    public void CustomerOrder(int id)
    {
      using (var connection = providerFactory.CreateConnection())
      {
        connection.ConnectionString = ConnectionString;
        connection.Open();

        using (var command = connection.CreateCommand())
        {
          command.CommandType = CommandType.StoredProcedure;
          var paramId = command.CreateParameter();
          paramId.ParameterName = "@id";
          paramId.Value = id;
          command.Parameters.Add(paramId);
        }

      }
    }

  }

}


  
  

