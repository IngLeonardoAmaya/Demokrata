using Demokrata.Context;
using Demokrata.Dtos;
using Demokrata.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Demokrata.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<User>> CreateUser(CreateUserDto userDto)
        {
            var user = new User
            {
                FirstName = userDto.FirstName,
                SecondName = userDto.SecondName,
                FirstLastName = userDto.FirstLastName,
                SecondLastName = userDto.SecondLastName,
                DateOfBirth = userDto.DateOfBirth,
                Salary = userDto.Salary,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserDto userDto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            user.FirstName = userDto.FirstName;
            user.SecondName = userDto.SecondName;
            user.FirstLastName = userDto.FirstLastName;
            user.SecondLastName = userDto.SecondLastName;
            user.DateOfBirth = userDto.DateOfBirth;
            user.Salary = userDto.Salary;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            return user;
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<User>>> SearchUsers(
            [FromQuery] string? firstName,
            [FromQuery] string? firstLastName,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(firstName))
                query = query.Where(u => u.FirstName.Contains(firstName));

            if (!string.IsNullOrEmpty(firstLastName))
                query = query.Where(u => u.FirstLastName.Contains(firstLastName));

            var users = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(users);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }
    }
}
