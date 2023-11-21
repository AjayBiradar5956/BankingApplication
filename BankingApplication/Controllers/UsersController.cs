using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BankingApplication.Models;
using System.Numerics;

namespace BankingApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly APIDbContext _context;

        public UsersController(APIDbContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Users>>> GetUsers()
        {
          if (_context.Users == null)
          {
              return NotFound();
          }
            return await _context.Users.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Users>> GetUsers(int id)
        {
          if (_context.Users == null)
          {
              return NotFound();
          }
            var users = await _context.Users.FindAsync(id);

            if (users == null)
            {
                return NotFound();
            }

            return users;
        }

        //GET : api/Users/GetBalance/5
        [HttpGet("GetBalance/{id}")]
        public async Task<ActionResult<int>> GetUserBalance(int id)
        {
            if (_context.BankAccounts == null)
            {
                return NotFound("Bank Account Not Found");
            }
            var bankAccount = await _context.BankAccounts.FirstOrDefaultAsync(b => b.UserId == id);
            if (bankAccount == null)
            {
                return NotFound("Bank Account Not Found");
            }
            return bankAccount.CurrentBalance;  
        }

        public class UpdateBalanceRequest
        {
            public int Amount { get; set; }
        }

        //PATCH: ai/Users/AddBalance/5
        [HttpPatch("AddBalance/{id}")]
        public async Task<IActionResult> AddBalance(int id, [FromBody] UpdateBalanceRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var bankAccount = await _context.BankAccounts
                .Where(b => b.UserId == id)
                .FirstOrDefaultAsync();

            if (bankAccount == null)
            {
                return NotFound($"Bank account not found for user with ID {id}.");
            }

            // Update the current balance
            bankAccount.CurrentBalance += request.Amount;

            try
            {
                await _context.SaveChangesAsync();
                return Ok("Funds Added");
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                return StatusCode(500, ex.Message);
            }
        }

        //PATCH - api/Users/WithdrawBalance/6
        [HttpPatch("WithdrawBalance{id}")]
        public async Task<ActionResult> WithdrawBalance(int id, [FromBody] UpdateBalanceRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var bankAccount = await _context.BankAccounts.FirstOrDefaultAsync(b => b.UserId == id);
            if (bankAccount == null) {
                return NotFound("Invalid Request");
            }
            if (bankAccount.CurrentBalance >= request.Amount)
            {
                bankAccount.CurrentBalance -= request.Amount;
            }
            else
            {
                return BadRequest("Insufficient Funds");
            }

            try
            {
                await _context.SaveChangesAsync();
                return Ok("Funds Withdrawn Successfully");
            }
            catch(Exception ex)
                {
                    return StatusCode(500, ex.Message);
                }
        }



        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsers(int id, Users users)
        {
            if (id != users.Id)
            {
                return BadRequest();
            }

            _context.Entry(users).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsersExists(id))
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

        // POST: api/Users/Signup
        // SIGN UP 
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("Signup")]
        public async Task<ActionResult<Users>> PostUsers(Users users)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'APIDbContext.Users' is null.");
            }

            // Check if the email already exists
            if (_context.Users.Any(u => u.Email == users.Email))
            {
                return Conflict("User already exists.");
            }

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    _context.Users.Add(users);
                    await _context.SaveChangesAsync();

                    // Create a corresponding bank account for the user
                    var bankAccount = new BankAccount
                    {
                        // Set current balance to reflect the initial deposit
                        CurrentBalance = users.InitialDeposit,

                        // Use the UserId of the added user
                        UserId = users.Id
                    };

                    _context.BankAccounts.Add(bankAccount);
                    await _context.SaveChangesAsync();

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    // Handle the exception as needed
                    throw;
                }
            }

            return CreatedAtAction("GetUsers", new { id = users.Id }, users);
        }




        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsers(int id)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var users = await _context.Users.FindAsync(id);
            if (users == null)
            {
                return NotFound();
            }

            _context.Users.Remove(users);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UsersExists(int id)
        {
            return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
