using Microsoft.AspNetCore.Mvc;
using UserManagementAPI.DTOs;
using UserManagementAPI.Models;

namespace UserManagementAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    // Demo storage only: data resets whenever the application restarts.
    private static readonly List<User> Users = new();
    private static readonly object SyncRoot = new();
    private static int _nextId = 1;

    [HttpGet]
    public ActionResult<IEnumerable<User>> GetAll()
    {
        lock (SyncRoot)
        {
            return Ok(Users.OrderBy(user => user.Id).ToList());
        }
    }

    [HttpGet("{id:int}")]
    public ActionResult<User> GetById(int id)
    {
        lock (SyncRoot)
        {
            var user = Users.FirstOrDefault(user => user.Id == id);
            return user is null ? NotFound(new { message = $"User with ID {id} was not found." }) : Ok(user);
        }
    }

    [HttpPost]
    public ActionResult<User> Create(CreateUserRequest request)
    {
        lock (SyncRoot)
        {
            if (Users.Any(user => string.Equals(user.Email, request.Email, StringComparison.OrdinalIgnoreCase)))
            {
                return Conflict(new { message = "A user with this email already exists." });
            }

            var user = new User
            {
                Id = _nextId++,
                Name = request.Name.Trim(),
                Email = request.Email.Trim()
            };

            Users.Add(user);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }
    }

    [HttpPut("{id:int}")]
    public ActionResult<User> Update(int id, UpdateUserRequest request)
    {
        lock (SyncRoot)
        {
            var user = Users.FirstOrDefault(user => user.Id == id);
            if (user is null)
            {
                return NotFound(new { message = $"User with ID {id} was not found." });
            }

            if (Users.Any(other =>
                other.Id != id &&
                string.Equals(other.Email, request.Email, StringComparison.OrdinalIgnoreCase)))
            {
                return Conflict(new { message = "Another user with this email already exists." });
            }

            user.Name = request.Name.Trim();
            user.Email = request.Email.Trim();
            return Ok(user);
        }
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        lock (SyncRoot)
        {
            var user = Users.FirstOrDefault(user => user.Id == id);
            if (user is null)
            {
                return NotFound(new { message = $"User with ID {id} was not found." });
            }

            Users.Remove(user);
            return NoContent();
        }
    }
}
