using System;
using System.Collections.Generic;
using System.Text;

namespace NorthwindDAL
{
  public interface IOrderRepository
  {
    IEnumerable<Order> GetOrders();
    Order Add(Order order);
    Order GetOrderById(int id);
    Order Update(int id);
    void Delete(int id);
    Order MoveInProgress(int id, DateTime date);
    Order MoveToDone(int id, DateTime date);
  }

}
