﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using antique_api.DBContext;
using antique_api.Models.Antique;
using antique_api.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace Antique.Controllers
{
    [Route("api/orders")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly CTX _context;

        public OrdersController(CTX context)
        {
            _context = context;
        }

        // GET: api/Orders
        [HttpGet("allOrders")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrder()
        {
            //var model = _context.Order.;
            var model = _context.Order.Select(x => new
            {
                x.ID,
                x.Name,
                x.Surname,
                x.TotalPrice,
                x.Delivery,
                x.DeliveryNum,
                x.Number,
                x.Items,
                x.City,
                x.Invoice,
                x.isDone

            }).OrderByDescending(x => x.ID);
            return Ok(model);
        }
        [HttpGet("undoneOrders")]
        public async Task<ActionResult<IEnumerable<Order>>> GetUndoneOrders()
        {
            var model = _context.Order.Select(x => new
            {
                x.ID,
                x.Name,
                x.Surname,
                x.TotalPrice,
                x.Delivery,
                x.DeliveryNum,
                x.Number,
                x.Items,
                x.isDone,
                x.City,
                x.Invoice
            }).Where(x => x.isDone == false).OrderByDescending(x => x.ID);
            return Ok(model);
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        [AllowAnonymous]

        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            //var order =  _context.Order.Select(x=> new{
            //    x.ID,
            //    x.Name,
            //    x.Surname,
            //    x.TotalPrice,
            //    x.Delivery,
            //    x.DeliveryNum,
            //    x.Number,
            //    x.Items
            //}).FirstOrDefault(x=>x.ID==id);
            var order = _context.Order.Include("Items.Product").Include("Items.Product.Photos").FirstOrDefault(x => x.ID == id);
            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }
        // PUT: api/Orders/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, Order order)
        {
            if (id != order.ID)
            {
                return BadRequest();
            }

            _context.Entry(order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        [HttpPost("addInvoice")]
        public async Task<IActionResult> AddInvoice([FromBody] InvoiceModel invoice)
        {
            Order order = _context.Order.FirstOrDefault(x => x.ID == invoice.ID);

            using (CTX db = _context)
            {
                // Редактирование
                if (order != null)
                {
                    order.Invoice = invoice.Invoice;
                    db.SaveChanges();
                    return Ok(invoice);
                }
                else
                {
                    return BadRequest();
                }
                // выводим данные после обновления
            }
        }
        [HttpPost("markAsDone/{id}")]
        public async Task<IActionResult> markAsDone(int id)
        {
            Order order = _context.Order.FirstOrDefault(x => x.ID == id);

            using (CTX db = _context)
            {
                // Редактирование
                if (order != null)
                {
                    order.isDone = true;
                    db.SaveChanges();
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
                // выводим данные после обновления
            }
        }

        //public async Task<IActionResult> AddInvoice([FromBody]InvoiceModel Invoice)
        //{
        //    Order order = _context.Order.FirstOrDefault(x => x.ID == Invoice.ID);

        //    using (CTX db = _context)
        //    {
        //        // Редактирование
        //        if (order != null)
        //        {
        //            order.Invoice = Invoice.Invoice;
        //            db.SaveChanges();
        //            return Ok(Invoice);
        //        }
        //        else
        //        {
        //            return BadRequest();
        //        }
        //        // выводим данные после обновления
        //    }
        //}

        // POST: api/Orders
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder([FromBody] OrderModel order)
        {

            Order o;
            try
            {
                List<OrderItem> products = new List<OrderItem>();
                foreach (var x in order.Products)
                {
                    OrderItem a = new OrderItem
                    {
                        Product = _context.Products.FirstOrDefault(p => p.ID == x.ID),
                        ProductId = _context.Products.FirstOrDefault(p => p.ID == x.ID).ID
                    };
                    products.Add(a);
                }

                o = new Order
                {
                    Name = order.Name,
                    Surname = order.Surname,
                    City = order.City,
                    Delivery = order.Delivery,
                    DeliveryNum = order.DeliveryNum,
                    Number = order.Number,
                    TotalPrice = order.TotalPrice,
                    Items = products,
                    isDone = false

                };
                _context.Order.Add(o);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
            return Ok(o.ID);
            /*_context.Order.Add(order);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetOrder", new { id = order.ID }, order);*/
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Order>> DeleteOrder(int id)
        {
            var order = await _context.Order.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Order.Remove(order);
            await _context.SaveChangesAsync();

            return order;
        }

        private bool OrderExists(int id)
        {
            return _context.Order.Any(e => e.ID == id);
        }
    }
}