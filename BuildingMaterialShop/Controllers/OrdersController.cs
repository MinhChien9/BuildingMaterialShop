﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BuildingMaterialShop.Models;
using Microsoft.AspNetCore.Authorization;

namespace BuildingMaterialShop.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly BuildingMaterialsShopContext _context;

        public OrdersController(BuildingMaterialsShopContext context)
        {
            _context = context;
        }
        // GET: Orders/customerId
        [HttpGet("{orderId}")]
        public async Task<ActionResult<Order>> GetOrderDetails(int orderId)
        {
            var order = await _context.Orders.Where(order => order.OrderId == orderId)
                                            .Include(order => order.OrderDetails)
                                                .ThenInclude(detail => detail.Product)
                                            .Include(order => order.OrderStatus)
                                            .FirstOrDefaultAsync();
            if (order == null)
            {
                return NotFound();
            }

            return order;
        }
        // GET: Orders/customerId
        [HttpGet("GetOrdersByCustomerId/{customerId}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrdersByCustomerId(int customerId)
        {
            return await _context.Orders.Where(order => order.CustomerId == customerId)
                                            .Include(order => order.OrderDetails)
                                            .ToListAsync();
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            return await _context.Orders.ToListAsync();
        }

        // PUT: api/Orders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, Order order)
        {
            //Tạo đơn hàng
            //    Thêm chi tiết đơn hàng
            //        Cập nhật tổng tiền cho đơn hàng
            //    Thêm trạng thái cho đơn hàng

            if (id != order.OrderId)
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

        // POST: api/Orders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder( [FromBody] Order order)
        {
            _context.Orders.Add(order);

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrderDetails", new { orderId = order.OrderId }, order);
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}
