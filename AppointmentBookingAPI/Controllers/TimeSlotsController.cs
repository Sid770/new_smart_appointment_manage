using Microsoft.AspNetCore.Mvc;
using AppointmentBookingAPI.DTOs;
using AppointmentBookingAPI.Services;

namespace AppointmentBookingAPI.Controllers
{
    /// <summary>
    /// Controller for managing time slots
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class TimeSlotsController : ControllerBase
    {
        private readonly ITimeSlotService _service;
        private readonly ILogger<TimeSlotsController> _logger;
        
        public TimeSlotsController(ITimeSlotService service, ILogger<TimeSlotsController> logger)
        {
            _service = service;
            _logger = logger;
        }
        
        /// <summary>
        /// Get all time slots
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<TimeSlotDto>>>> GetAll()
        {
            try
            {
                var slots = await _service.GetAllTimeSlotsAsync();
                return Ok(ApiResponse<IEnumerable<TimeSlotDto>>.SuccessResponse(slots));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving time slots");
                return StatusCode(500, ApiResponse<IEnumerable<TimeSlotDto>>.ErrorResponse("Internal server error"));
            }
        }
        
        /// <summary>
        /// Get available time slots with optional filters
        /// </summary>
        [HttpGet("available")]
        public async Task<ActionResult<ApiResponse<IEnumerable<TimeSlotDto>>>> GetAvailable([FromQuery] TimeSlotFilterDto filter)
        {
            try
            {
                var slots = await _service.GetAvailableTimeSlotsAsync(filter);
                return Ok(ApiResponse<IEnumerable<TimeSlotDto>>.SuccessResponse(slots));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving available time slots");
                return StatusCode(500, ApiResponse<IEnumerable<TimeSlotDto>>.ErrorResponse("Internal server error"));
            }
        }
        
        /// <summary>
        /// Get time slot by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<TimeSlotDto>>> GetById(int id)
        {
            try
            {
                var slot = await _service.GetTimeSlotByIdAsync(id);
                if (slot == null)
                {
                    return NotFound(ApiResponse<TimeSlotDto>.ErrorResponse("Time slot not found"));
                }
                return Ok(ApiResponse<TimeSlotDto>.SuccessResponse(slot));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving time slot {Id}", id);
                return StatusCode(500, ApiResponse<TimeSlotDto>.ErrorResponse("Internal server error"));
            }
        }
        
        /// <summary>
        /// Create a new time slot (Admin only)
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<TimeSlotDto>>> Create([FromBody] CreateTimeSlotDto dto)
        {
            try
            {
                var slot = await _service.CreateTimeSlotAsync(dto);
                return CreatedAtAction(
                    nameof(GetById),
                    new { id = slot.Id },
                    ApiResponse<TimeSlotDto>.SuccessResponse(slot, "Time slot created successfully"));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation while creating time slot");
                return BadRequest(ApiResponse<TimeSlotDto>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating time slot");
                return StatusCode(500, ApiResponse<TimeSlotDto>.ErrorResponse("Internal server error"));
            }
        }
        
        /// <summary>
        /// Update a time slot (Admin only)
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<TimeSlotDto>>> Update(int id, [FromBody] CreateTimeSlotDto dto)
        {
            try
            {
                var slot = await _service.UpdateTimeSlotAsync(id, dto);
                return Ok(ApiResponse<TimeSlotDto>.SuccessResponse(slot, "Time slot updated successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Time slot {Id} not found", id);
                return NotFound(ApiResponse<TimeSlotDto>.ErrorResponse(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation while updating time slot {Id}", id);
                return BadRequest(ApiResponse<TimeSlotDto>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating time slot {Id}", id);
                return StatusCode(500, ApiResponse<TimeSlotDto>.ErrorResponse("Internal server error"));
            }
        }
        
        /// <summary>
        /// Delete a time slot (Admin only)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
        {
            try
            {
                var result = await _service.DeleteTimeSlotAsync(id);
                if (!result)
                {
                    return NotFound(ApiResponse<bool>.ErrorResponse("Time slot not found"));
                }
                return Ok(ApiResponse<bool>.SuccessResponse(true, "Time slot deleted successfully"));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Cannot delete time slot {Id}", id);
                return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting time slot {Id}", id);
                return StatusCode(500, ApiResponse<bool>.ErrorResponse("Internal server error"));
            }
        }
        
        /// <summary>
        /// Make a booked time slot available again (Admin only)
        /// </summary>
        [HttpPut("{id}/make-available")]
        public async Task<ActionResult<ApiResponse<bool>>> MakeAvailable(int id)
        {
            try
            {
                var result = await _service.MakeSlotAvailableAsync(id);
                if (!result)
                {
                    return NotFound(ApiResponse<bool>.ErrorResponse("Time slot not found"));
                }
                return Ok(ApiResponse<bool>.SuccessResponse(true, "Time slot is now available"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error making time slot {Id} available", id);
                return StatusCode(500, ApiResponse<bool>.ErrorResponse("Internal server error"));
            }
        }
    }
}
