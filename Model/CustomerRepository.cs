using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;

namespace HttpRepl.Model
{
    public class CustomerRepository
    {
        private readonly IList<Customer> customers;

        public CustomerRepository()
        {
            var doc = XDocument.Load("Customers.xml");
            customers = new ObservableCollection<Customer>((from c in doc.Descendants("Customer")
                                                            select new Customer
                                                            {
                                                                CustomerId = GetValueOrDefault(c, "CustomerID"),
                                                                CompanyName = GetValueOrDefault(c, "CompanyName"),
                                                                ContactName = GetValueOrDefault(c, "ContactName"),
                                                                ContactTitle = GetValueOrDefault(c, "ContactTitle"),
                                                                Address = GetValueOrDefault(c, "Address"),
                                                                City = GetValueOrDefault(c, "City"),
                                                                Region = GetValueOrDefault(c, "Region"),
                                                                PostalCode = GetValueOrDefault(c, "PostalCode"),
                                                                Country = GetValueOrDefault(c, "Country"),
                                                                Phone = GetValueOrDefault(c, "Phone"),
                                                                Fax = GetValueOrDefault(c, "Fax")
                                                            }).ToList());
        }

        #region ICustomerRepository Members

        public bool Add(Customer customer)
        {
            if (customers.FirstOrDefault(c => c.CustomerId == customer.CustomerId) == null)
            {
                customers.Add(customer);
                return true;
            }
            return false;
        }

        public bool Remove(Customer customer)
        {
            if (customers.IndexOf(customer) >= 0)
            {
                customers.Remove(customer);
                return true;
            }
            return false;
        }

        public bool Update(Customer customer)
        {
            var currentCustomer = GetCustomer(customer.CustomerId);
            if (currentCustomer == null)
                return false;
            currentCustomer.CustomerId = customer.CustomerId;
            currentCustomer.CompanyName = customer.CompanyName;
            currentCustomer.ContactName = customer.ContactName;
            currentCustomer.ContactTitle = customer.ContactTitle;
            currentCustomer.Address = customer.Address;
            currentCustomer.City = customer.City;
            currentCustomer.Region = customer.Region;
            currentCustomer.PostalCode = customer.PostalCode;
            currentCustomer.Country = customer.Country;
            currentCustomer.Phone = customer.Phone;
            currentCustomer.Fax = customer.Fax;
            return true;    
        }

        public bool Commit()
        {
            try
            {
                var doc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));
                var root = new XElement("Customers");
                foreach (Customer customer in customers)
                {
                    root.Add(new XElement("Customer",
                                          new XElement("CustomerID", customer.CustomerId),
                                          new XElement("CompanyName", customer.CompanyName),
                                          new XElement("ContactName", customer.ContactName),
                                          new XElement("ContactTitle", customer.ContactTitle),
                                          new XElement("Address", customer.Address),
                                          new XElement("City", customer.City),
                                          new XElement("Region", customer.Region),
                                          new XElement("PostalCode", customer.PostalCode),
                                          new XElement("Country", customer.Country),
                                          new XElement("Phone", customer.Phone),
                                          new XElement("Fax", customer.Fax)
                                 ));
                }
                doc.Add(root);
                doc.Save("Customers.xml");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public IEnumerable<Customer> GetAll() => customers;

        public Customer GetCustomer(string id) => customers.FirstOrDefault(c => string.Equals(c.CustomerId, id, StringComparison.CurrentCultureIgnoreCase));

        #endregion

        private static string GetValueOrDefault(XContainer el, string propertyName)
        {
            return el.Element(propertyName) == null ? string.Empty : el.Element(propertyName).Value;
        }
    }
}