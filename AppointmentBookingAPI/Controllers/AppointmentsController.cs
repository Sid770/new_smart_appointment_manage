using Microsoft.AspNetCore.Mvc;
using AppointmentBookingAPI.DTOs;
using AppointmentBookingAPI.Services;
using AppointmentBookingAPI.Models;

namespace AppointmentBookingAPI.Controllers
{
    /// <summary>
    /// Controller for managing appointments
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _service;
        private readonly ILogger<AppointmentsController> _logger;
        
        public AppointmentsController(IAppointmentService service, ILogger<AppointmentsController> logger)
        {
            _service = service;
            _logger = logger;
        }
        
        /// <summary>
        /// Get all appointments
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<AppointmentDto>>>> GetAll()
        {
            try
            {
                var appointments = await _service.GetAllAppointmentsAsync();
                return Ok(ApiResponse<IEnumerable<AppointmentDto>>.SuccessResponse(appointments));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving appointments");
                return StatusCode(500, ApiResponse<IEnumerable<AppointmentDto>>.ErrorResponse("Internal server error"));
            }
        }
        
        /// <summary>
        /// Get appointment by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<AppointmentDto>>> GetById(int id)
        {
            try
            {
                var appointment = await _service.GetAppointmentByIdAsync(id);
                if (appointment == null)
                {
                    return NotFound(ApiResponse<AppointmentDto>.ErrorResponse("Appointment not found"));
                }
                return Ok(ApiResponse<AppointmentDto>.SuccessResponse(appointment));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving appointment {Id}", id);
                return StatusCode(500, ApiResponse<AppointmentDto>.ErrorResponse("Internal server error"));
            }
        }
        
        /// <summary>
        /// Get appointments by status
        /// </summary>
        [HttpGet("status/{status}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<AppointmentDto>>>> GetByStatus(AppointmentStatus status)
        {
            try
            {
                var appointments = await _service.GetAppointmentsByStatusAsync(status);
                return Ok(ApiResponse<IEnumerable<AppointmentDto>>.SuccessResponse(appointments));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving appointments by status {Status}", status);
                return StatusCode(500, ApiResponse<IEnumerable<AppointmentDto>>.ErrorResponse("Internal server error"));
            }
        }
        
        /// <summary>
        /// Create a new appointment
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<AppointmentDto>>> Create([FromBody] CreateAppointmentDto dto)
        {
            try
            {
                var appointment = await _service.CreateAppointmentAsync(dto);
                return CreatedAtAction(
                    nameof(GetById),
                    new { id = appointment.Id },
                    ApiResponse<AppointmentDto>.SuccessResponse(appointment, "Appointment created successfully"));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation while creating appointment");
                return BadRequest(ApiResponse<AppointmentDto>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating appointment");
                return StatusCode(500, ApiResponse<AppointmentDto>.ErrorResponse("Internal server error"));
            }
        }
        
        /// <summary>
        /// Update appointment status
        /// </summary>
        [HttpPatch("{id}/status")]
        public async Task<ActionResult<ApiResponse<AppointmentDto>>> UpdateStatus(int id, [FromBody] UpdateAppointmentStatusDto dto)
        {
            try
            {
                var appointment = await _service.UpdateAppointmentStatusAsync(id, dto);
                return Ok(ApiResponse<AppointmentDto>.SuccessResponse(appointment, "Status updated successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Appointment {Id} not found", id);
                return NotFound(ApiResponse<AppointmentDto>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating appointment {Id} status", id);
                return StatusCode(500, ApiResponse<AppointmentDto>.ErrorResponse("Internal server error"));
            }
        }
        
        /// <summary>
        /// Cancel an appointment
        /// </summary>
        [HttpPost("{id}/cancel")]
        public async Task<ActionResult<ApiResponse<bool>>> Cancel(int id)
        {
            try
            {
                var result = await _service.CancelAppointmentAsync(id);
                if (!result)
                {
                    return NotFound(ApiResponse<bool>.ErrorResponse("Appointment not found"));
                }
                return Ok(ApiResponse<bool>.SuccessResponse(true, "Appointment cancelled successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling appointment {Id}", id);
                return StatusCode(500, ApiResponse<bool>.ErrorResponse("Internal server error"));
            }
        }
    }
}
