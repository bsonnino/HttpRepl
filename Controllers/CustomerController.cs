using System;
using System.Collections.Generic;
using HttpRepl.Model;
using Microsoft.AspNetCore.Mvc;

namespace HttpRepl.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<Customer> Get()
        {
            var customerRepository = new CustomerRepository();
            return customerRepository.GetAll();
        }

        [HttpGet("{id}")]
        public IActionResult GetCustomer(string id)
        {
            var customerRepository = new CustomerRepository();
            Customer customer = customerRepository.GetCustomer(id);
            return customer != null ? Ok(customer) : NotFound();
        }

        [HttpPost]
        public IActionResult Add([FromBody] Customer customer)
        {
            if (string.IsNullOrWhiteSpace(customer.CustomerId))
              return BadRequest();
            var customerRepository = new CustomerRepository();
            if (customerRepository.Add(customer))
            {
                customerRepository.Commit();
                return CreatedAtAction(nameof(Get), new { id = customer.CustomerId }, customer);
            }
            return Conflict();
        }

        [HttpPut]
        public IActionResult Update([FromBody] Customer customer)
        {
             if (string.IsNullOrWhiteSpace(customer.CustomerId))
              return BadRequest();
            var customerRepository = new CustomerRepository();
            var currentCustomer = customerRepository.GetCustomer(customer.CustomerId);
            if (currentCustomer == null)
                return NotFound();
            if (customerRepository.Update(customer))
            {
                customerRepository.Commit();
                return Ok(customer);
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete([FromRoute]string id)
        {
            Console.WriteLine(id);
            if (string.IsNullOrWhiteSpace(id))
              return BadRequest();
            var customerRepository = new CustomerRepository();
            var currentCustomer = customerRepository.GetCustomer(id);
            if (currentCustomer == null)
                return NotFound();
            if (customerRepository.Remove(currentCustomer))
            {
                customerRepository.Commit();
                return Ok();
            }
            return NoContent();
        }
    }
}