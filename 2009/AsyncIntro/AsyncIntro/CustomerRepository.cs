using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.Threading;

namespace AsyncIntro
{
    public class CustomerRepository
    {
        public List<Customer> GetCustomers()
        {
            Thread.Sleep(10000);
            List<Customer> resultList = new List<Customer>();
            resultList.Add(new Customer() { Address = "New York", Id = Guid.NewGuid(), Name = "Bank ABC" });
            resultList.Add(new Customer() { Address = "Berlin", Id = Guid.NewGuid(), Name = "Manufactor XYZ" });
            resultList.Add(new Customer() { Address = "Paris", Id = Guid.NewGuid(), Name = "Test 123" });
            resultList.Add(new Customer() { Address = "Tokyo", Id = Guid.NewGuid(), Name = "Bank DDD" });
            resultList.Add(new Customer() { Address = "London", Id = Guid.NewGuid(), Name = "Bank HHH" });
            return resultList;
        }

        public void GetCustomersAsync()
        {
            ThreadPool.QueueUserWorkItem(y =>
            {
                List<Customer> result = this.GetCustomers();
                this.OnGetCustomersAsyncCompleted(result);
            });
        }

        private void OnGetCustomersAsyncCompleted(List<Customer> customers)
        {
            if (this.GetCustomersAsyncCompleted != null)
            {
                this.GetCustomersAsyncCompleted(this, new GenericEventArgs<List<Customer>>(customers));
            }
        }

        public event EventHandler<GenericEventArgs<List<Customer>>> GetCustomersAsyncCompleted;
    }
}
