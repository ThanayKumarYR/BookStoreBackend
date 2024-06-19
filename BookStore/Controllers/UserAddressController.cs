using BusinessLayer.Interface;
using Microsoft.AspNetCore.Mvc;
using RepositoryLayer.Entity;
using ModelLayer.ResponseModel;

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAddressController : ControllerBase
    {
        private readonly IUserAddressBusinessLayer _userAddressBusinessLayer;
        private readonly ILogger<UserAddressController> _logger;

        public UserAddressController(IUserAddressBusinessLayer userAddressBusinessLayer, ILogger<UserAddressController> logger)
        {
            _userAddressBusinessLayer = userAddressBusinessLayer;
            _logger = logger;
        }

        [HttpPost("create")]
        public async Task<ActionResult<ResponseModel<int>>> CreateAddress([FromBody] UserAddress userAddress)
        {
            try
            {
                _logger.LogInformation("Creating new address");
                var addressId = await _userAddressBusinessLayer.InsertAddressAsync(userAddress);
                _logger.LogInformation("Address created successfully with ID: {AddressId}", addressId);
                return Ok(new ResponseModel<int> { Success = true, Data = addressId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating address");
                return StatusCode(500, new ResponseModel<int> { Success = false, Message = "An error occurred while creating the address" });
            }
        }

        [HttpGet("getall")]
        public async Task<ActionResult<ResponseModel<List<UserAddress>>>> GetAddresses()
        {
            try
            {
                _logger.LogInformation("Fetching all addresses");
                var addresses = await _userAddressBusinessLayer.GetAddressesAsync();
                return Ok(new ResponseModel<List<UserAddress>> { Success = true, Data = addresses });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching addresses");
                return StatusCode(500, new ResponseModel<List<UserAddress>> { Success = false, Message = "An error occurred while fetching addresses" });
            }
        }

        [HttpGet("getbyid/{addressId}")]
        public async Task<ActionResult<ResponseModel<UserAddress>>> GetAddressById(int addressId)
        {
            try
            {
                _logger.LogInformation("Fetching address with ID: {AddressId}", addressId);
                var address = await _userAddressBusinessLayer.GetAddressByIdAsync(addressId);
                if (address == null)
                {
                    _logger.LogWarning("Address with ID: {AddressId} not found", addressId);
                    return NotFound(new ResponseModel<UserAddress> { Success = false, Message = "Address not found" });
                }
                return Ok(new ResponseModel<UserAddress> { Success = true, Data = address });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching address with ID: {AddressId}", addressId);
                return StatusCode(500, new ResponseModel<UserAddress> { Success = false, Message = "An error occurred while fetching the address" });
            }
        }

        [HttpPut("update/{addressId}")]
        public async Task<IActionResult> UpdateAddress(int addressId, [FromBody] UserAddress userAddress)
        {
            try
            {
                _logger.LogInformation("Updating address with ID: {AddressId}", addressId);
                userAddress.AddressId = addressId;
                await _userAddressBusinessLayer.UpdateAddressAsync(userAddress);
                _logger.LogInformation("Address with ID: {AddressId} updated successfully", addressId);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating address with ID: {AddressId}", addressId);
                return StatusCode(500, new ResponseModel<bool> { Success = false, Message = "An error occurred while updating the address" });
            }
        }

        [HttpDelete("delete/{addressId}")]
        public async Task<IActionResult> DeleteAddress(int addressId)
        {
            try
            {
                _logger.LogInformation("Deleting address with ID: {AddressId}", addressId);
                await _userAddressBusinessLayer.DeleteAddressAsync(addressId);
                _logger.LogInformation("Address with ID: {AddressId} deleted successfully", addressId);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting address with ID: {AddressId}", addressId);
                return StatusCode(500, new ResponseModel<bool> { Success = false, Message = "An error occurred while deleting the address" });
            }
        }
    }
}
