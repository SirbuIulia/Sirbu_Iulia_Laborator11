using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Sirbu_Iulia_Lab2.Data;
using Sirbu_Iulia_Lab2.Models;
using Microsoft.AspNetCore.Authorization;



namespace Sirbu_Iulia_Lab2.Controllers
{
    [Authorize(Policy = "SalesManager")]
    public class CustomersController : Controller
    {
        private readonly LibraryContext _context;
        private string _baseUrl = "https://localhost:7037/api/Customers";

        public CustomersController(LibraryContext context)
        {
            _context = context;
        }

        // GET: Customers
        public async Task<IActionResult> Index()
        {
            var customers = await _context.Customer.Include(c => c.City).ToListAsync();
            return View(customers); // Verifică tipul trimis
        }


        // GET: Customers/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var client = new HttpClient();
            var response = await client.GetAsync($"{_baseUrl}/{id.Value}");
            if (response.IsSuccessStatusCode)
            {
                var customer = JsonConvert.DeserializeObject<Customer>(
                    await response.Content.ReadAsStringAsync());
                return View(customer);
            }
            return NotFound();
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            ViewBag.CityID = new SelectList(_context.City, "ID", "CityName");
            return View();
        }

        // POST: Customers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind("CustomerID,Name,Adress,BirthDate,CityID")] Customer customer)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.CityID = new SelectList(_context.City, "ID", "CityName", customer.CityID);
                return View(customer);
            }

            try
            {
                var client = new HttpClient();
                string json = JsonConvert.SerializeObject(customer);

                var response = await client.PostAsync(_baseUrl,
                    new StringContent(json, Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Unable to create record: {ex.Message}");
            }

            ViewBag.CityID = new SelectList(_context.City, "ID", "CityName", customer.CityID);
            return View(customer);
        }

        // GET: Customers/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var client = new HttpClient();
            var response = await client.GetAsync($"{_baseUrl}/{id.Value}");
            if (response.IsSuccessStatusCode)
            {
                var customer = JsonConvert.DeserializeObject<Customer>(
                    await response.Content.ReadAsStringAsync());
                ViewBag.CityID = new SelectList(_context.City, "ID", "CityName", customer.CityID);
                return View(customer);
            }
            return NotFound();
        }

        // POST: Customers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind("CustomerID,Name,Adress,BirthDate,CityID")] Customer customer)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.CityID = new SelectList(_context.City, "ID", "CityName", customer.CityID);
                return View(customer);
            }

            try
            {
                var client = new HttpClient();
                string json = JsonConvert.SerializeObject(customer);

                var response = await client.PutAsync($"{_baseUrl}/{customer.CustomerID}",
                    new StringContent(json, Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Unable to edit record: {ex.Message}");
            }

            ViewBag.CityID = new SelectList(_context.City, "ID", "CityName", customer.CityID);
            return View(customer);
        }

        // GET: Customers/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var client = new HttpClient();
            var response = await client.GetAsync($"{_baseUrl}/{id.Value}");
            if (response.IsSuccessStatusCode)
            {
                var customer = JsonConvert.DeserializeObject<Customer>(
                    await response.Content.ReadAsStringAsync());
                return View(customer);
            }
            return NotFound();
        }

        // POST: Customers/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete([Bind("CustomerID")] Customer customer)
        {
            try
            {
                var client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete,
                    $"{_baseUrl}/{customer.CustomerID}")
                {
                    Content = new StringContent(JsonConvert.SerializeObject(customer),
                        Encoding.UTF8, "application/json")
                };

                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Unable to delete record: {ex.Message}");
            }

            return View(customer);
        }
    }
}