using BusinessLayer.Interface;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.ResponseModel;
using RepositoryLayer.Entity;

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserBusinessLayer _userBusinessLayer;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserBusinessLayer userBusinessLayer, ILogger<UserController> logger)
        {
            _userBusinessLayer = userBusinessLayer;
            _logger = logger;
        }

        [HttpPost("create")]
        public async Task<ActionResult<ResponseModel<int>>> CreateUser([FromBody] User user)
        {
            try
            {
                var userId = await _userBusinessLayer.CreateUserAsync(user.FullName, user.EmailId, user.Password, user.MobileNumber);
                var response = new ResponseModel<int>
                {
                    Success = true,
                    Message = "User created successfully",
                    Data = userId
                };
                return Ok(response);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                var response = new ResponseModel<int>
                {
                    Success = false,
                    Message = "Failed to create user"
                };
                return StatusCode(500, response);
            }
        }

        [HttpGet("getall")]
        public async Task<ActionResult<ResponseModel<List<User>>>> GetUsers()
        {
            try
            {
                var users = await _userBusinessLayer.GetUsersAsync();
                var response = new ResponseModel<List<User>>
                {
                    Success = true,
                    Message = "Users retrieved successfully",
                    Data = users
                };
                return Ok(response);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users");
                var response = new ResponseModel<List<User>>
                {
                    Success = false,
                    Message = "Failed to retrieve users"
                };
                return StatusCode(500, response);
            }
        }

        [HttpGet("getbyid/{userId}")]
        public async Task<ActionResult<ResponseModel<User>>> GetUserById(int userId)
        {
            try
            {
                var user = await _userBusinessLayer.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return NotFound();
                }
                var response = new ResponseModel<User>
                {
                    Success = true,
                    Message = "User retrieved successfully",
                    Data = user
                };
                return Ok(response);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user");
                var response = new ResponseModel<User>
                {
                    Success = false,
                    Message = "Failed to retrieve user"
                };
                return StatusCode(500, response);
            }
        }

        [HttpPut("update/{userId}")]
        public async Task<IActionResult> UpdateUser(int userId, [FromBody] User user)
        {
            try
            {
                await _userBusinessLayer.UpdateUserAsync(userId, user.FullName, user.EmailId, user.Password, user.MobileNumber);
                var response = new ResponseModel<object>
                {
                    Success = true,
                    Message = "User updated successfully"
                };
                return Ok(response);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error updating user");
                var response = new ResponseModel<object>
                {
                    Success = false,
                    Message = "Failed to update user"
                };
                return StatusCode(500, response);
            }
        }

        [HttpDelete("delete/{userId}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            try
            {
                await _userBusinessLayer.DeleteUserAsync(userId);
                var response = new ResponseModel<object>
                {
                    Success = true,
                    Message = "User deleted successfully"
                };
                return Ok(response);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error deleting user");
                var response = new ResponseModel<object>
                {
                    Success = false,
                    Message = "Failed to delete user"
                };
                return StatusCode(500, response);
            }
        }
    }
}
