using Grpc.Core;
using GrpcCustomersService;
using GrpcCustomersService.Services;
using DataAccess = Sirbu_Iulia_Lab2.Data;
using ModelAccess = Sirbu_Iulia_Lab2.Models;
namespace GrpcCustomersService.Services;
public class GrpcCrudService : CustomerService.CustomerServiceBase
{

    private DataAccess.LibraryContext db = null;
    public GrpcCrudService(DataAccess.LibraryContext db)
    {
        this.db = db;
    }
    public override Task<CustomerList> GetAll(Empty empty, ServerCallContext
   context)
    {

        CustomerList pl = new CustomerList();
        var query = from cust in db.Customer
                    select new Customer()
                    {
                        CustomerId = cust.CustomerID,
                        Name = cust.Name,
                        Adress = cust.Adress,
                        Birthdate = cust.BirthDate.ToString()

                    };
        pl.Item.AddRange(query.ToArray());
        return Task.FromResult(pl);
    }
    public override Task<Empty> Insert(Customer requestData, ServerCallContext context)
    {
        db.Customer.Add(new ModelAccess.Customer
        {
            CustomerID = requestData.CustomerId,
            Name = requestData.Name,
            Adress = requestData.Adress,
            BirthDate = DateTime.Parse(requestData.Birthdate)
        });
        db.SaveChanges();
        return Task.FromResult(new Empty());
    }
    public override Task<Customer> Get(CustomerId requestData, ServerCallContext context)
    {
        var data = db.Customer.Find(requestData.Id);

        Customer emp = new Customer()
        {
            CustomerId = data.CustomerID,
            Name = data.Name,
            Adress = data.Adress,
            Birthdate = Customer.BirthdateFieldNumber.ToString()
        };
        return Task.FromResult(emp);
    }

    public override Task<Empty> Delete(CustomerId requestData, ServerCallContext
   context)
    {
        var data = db.Customer.Find(requestData.Id);
        db.Customer.Remove(data);

        db.SaveChanges();
        return Task.FromResult(new Empty());
    }
    public override Task<Customer> Update(Customer requestData, ServerCallContext context)
    {
       
        DateTime? birthDate = null;
        if (!string.IsNullOrEmpty(requestData.Birthdate))
        {
            birthDate = DateTime.ParseExact(requestData.Birthdate, "yyyy-MM-dd", null);
        }

        var updatedCustomer = new ModelAccess.Customer()
        {
            CustomerID = requestData.CustomerId,
            Name = requestData.Name,
            Adress = requestData.Adress,
            // BirthDate = birthDate 

        };

        db.Customer.Update(updatedCustomer);
        db.SaveChanges();

        var grpcCustomer = new Customer()
        {
            CustomerId = updatedCustomer.CustomerID,
            Name = updatedCustomer.Name,
            Adress = updatedCustomer.Adress,
            Birthdate = updatedCustomer.BirthDate.ToString("yyyy-MM-dd") ?? ""
        };

        return Task.FromResult(grpcCustomer);
    }




}
