using FluentValidation;
using AppointmentBookingAPI.DTOs;

namespace AppointmentBookingAPI.Validators
{
    public class CreateAppointmentDtoValidator : AbstractValidator<CreateAppointmentDto>
    {
        public CreateAppointmentDtoValidator()
        {
            RuleFor(x => x.TimeSlotId)
                .GreaterThan(0)
                .WithMessage("Valid time slot ID is required");
            
            RuleFor(x => x.ClientName)
                .NotEmpty()
                .WithMessage("Client name is required")
                .MaximumLength(100)
                .WithMessage("Client name must not exceed 100 characters");
            
            RuleFor(x => x.ClientEmail)
                .NotEmpty()
                .WithMessage("Email is required")
                .EmailAddress()
                .WithMessage("Invalid email format")
                .MaximumLength(100)
                .WithMessage("Email must not exceed 100 characters");
            
            RuleFor(x => x.ClientPhone)
                .NotEmpty()
                .WithMessage("Phone number is required")
                .Matches(@"^[\d\s\-\+\(\)]+$")
                .WithMessage("Invalid phone number format")
                .MaximumLength(20)
                .WithMessage("Phone number must not exceed 20 characters");
            
            RuleFor(x => x.ServiceType)
                .NotEmpty()
                .WithMessage("Service type is required")
                .MaximumLength(100)
                .WithMessage("Service type must not exceed 100 characters");
        }
    }
    
    public class CreateTimeSlotDtoValidator : AbstractValidator<CreateTimeSlotDto>
    {
        public CreateTimeSlotDtoValidator()
        {
            RuleFor(x => x.StartTime)
                .NotEmpty()
                .WithMessage("Start time is required")
                .GreaterThan(DateTime.UtcNow)
                .WithMessage("Start time must be in the future");
            
            RuleFor(x => x.EndTime)
                .NotEmpty()
                .WithMessage("End time is required")
                .GreaterThan(x => x.StartTime)
                .WithMessage("End time must be after start time");
            
            RuleFor(x => x.ServiceProvider)
                .NotEmpty()
                .WithMessage("Service provider is required")
                .MaximumLength(100)
                .WithMessage("Service provider name must not exceed 100 characters");
        }
    }
}
