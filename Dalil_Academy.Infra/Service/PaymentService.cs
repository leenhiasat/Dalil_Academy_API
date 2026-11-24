using Dalil_Academy.Core.DTOs.Enrollments;
using Dalil_Academy.Core.DTOs.Payments;
using Dalil_Academy.Core.Entities;
using Dalil_Academy.Core.Exceptions;
using Dalil_Academy.Core.Mappings;
using Dalil_Academy.Core.Repository;
using Dalil_Academy.Core.Service;
using Microsoft.Extensions.Configuration;
using System.Globalization;

namespace Dalil_Academy.Infra.Service;

public class PaymentService(
    IEnrollmentRepository enrollmentRepository,
    IPaymentRepository paymentRepository,
    IPaymentGateway paymentGateway,
    IConfiguration configuration)
    : IPaymentService
{
    private const int OrderExpirationHours = 3;

    public async Task<PayEnrollmentResponseDto> PayAsync(
        int enrollmentId)
    {
        var enrollment =
            await enrollmentRepository.GetByIdAsync(enrollmentId)
            ?? throw new NotFoundException(
                $"Enrollment {enrollmentId} was not found.");

        if (enrollment.Status == EnrollmentStatus.Paid)
        {
            throw new ConflictException(
                "This enrollment is already paid.");
        }

        if (enrollment.Course is null)
        {
            throw new NotFoundException(
                "The course associated with this enrollment was not found.");
        }

        if (enrollment.Course.Price <= 0)
        {
            throw new ConflictException(
                "This enrollment does not require payment.");
        }

        // Expire the latest old payment attempt if it is older than 3 hours.
        var oldPayment =
            await paymentRepository.GetLatestCreatedAsync(enrollmentId);

        if (oldPayment is not null)
        {
            var age = DateTime.UtcNow - oldPayment.CreatedAt;

            if (age > TimeSpan.FromHours(OrderExpirationHours))
            {
                oldPayment.Status = PaymentStatus.Failed;

                await paymentRepository.UpdateAsync(oldPayment);
            }
        }

        // Read the exchange rate from configuration.
        var rateValue = configuration["PayPal:JodToUsdRate"];

        if (!decimal.TryParse(
                rateValue,
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out var rate) ||
            rate <= 0)
        {
            throw new InvalidOperationException(
                "PayPal exchange rate must be greater than zero.");
        }

        // Calculate the amount on the server.
        var amount = Math.Round(
            enrollment.Course.Price * rate,
            2,
            MidpointRounding.AwayFromZero);

        // Create PayPal order.
        var order = await paymentGateway.CreateOrderAsync(amount);

        // Save payment attempt.
        var payment = new Payment
        {
            EnrollmentId = enrollmentId,
            Enrollment = enrollment,
            Amount = amount,
            Currency = "USD",
            PayPalOrderId = order.OrderId,
            Status = PaymentStatus.Created,
            CreatedAt = DateTime.UtcNow
        };

        var createdPayment =
            await paymentRepository.CreateAsync(payment);

        return new PayEnrollmentResponseDto(
            createdPayment.Id,
            order.OrderId,
            amount,
            "USD",
            order.ApprovalUrl
        );
    }

    public async Task<EnrollmentResponseDto> CaptureAsync(
        int enrollmentId)
    {
        var enrollment =
            await enrollmentRepository.GetByIdAsync(enrollmentId)
            ?? throw new NotFoundException(
                $"Enrollment {enrollmentId} was not found.");

        if (enrollment.Status == EnrollmentStatus.Paid)
        {
            throw new ConflictException(
                "This enrollment is already paid.");
        }

        if (enrollment.Course is null)
        {
            throw new NotFoundException(
                "The course associated with this enrollment was not found.");
        }

        var payment =
            await paymentRepository.GetLatestCreatedAsync(enrollmentId);

        if (payment is null)
        {
            throw new PaymentFailedException(
                "Call /pay first.");
        }

        // Check whether the PayPal order has expired.
        if (DateTime.UtcNow - payment.CreatedAt >
            TimeSpan.FromHours(OrderExpirationHours))
        {
            payment.Status = PaymentStatus.Failed;

            await paymentRepository.UpdateAsync(payment);

            throw new PaymentFailedException(
                "The PayPal order has expired. Call /pay again.");
        }

        // IMPORTANT:
        // Check course capacity BEFORE capturing the PayPal payment.
        if (enrollment.Course.MaxStudents is int maxStudents)
        {
            var paidCount =
                await enrollmentRepository
                    .CountPaidEnrollmentsByCourseIdAsync(
                        enrollment.CourseId);

            if (paidCount >= maxStudents)
            {
                throw new ConflictException(
                    "This course is full. Payment cannot be completed.");
            }
        }

        // Capture payment only after all local checks pass.
        var completed = await paymentGateway.CaptureOrderAsync(
            payment.PayPalOrderId);

        if (!completed)
        {
            payment.Status = PaymentStatus.Failed;

            await paymentRepository.UpdateAsync(payment);

            throw new PaymentFailedException(
                "Payment was not approved or capture failed. Call /pay again.");
        }

        // PayPal confirmed COMPLETED.
        payment.Status = PaymentStatus.Completed;
        payment.PaidAt = DateTime.UtcNow;

        enrollment.Status = EnrollmentStatus.Paid;

        // Both tracked entities are saved using the same DbContext.
        await paymentRepository.UpdateAsync(payment);

        return enrollment.ToDto();
    }

    public async Task<List<PaymentHistoryResponseDto>>
        GetPaymentHistoryAsync(int enrollmentId)
    {
        // Ensure enrollment exists.
        _ = await enrollmentRepository.GetByIdAsync(enrollmentId)
            ?? throw new NotFoundException(
                $"Enrollment {enrollmentId} was not found.");

        // Retrieve all payment attempts, newest first.
        var payments =
            await paymentRepository.GetByEnrollmentIdAsync(
                enrollmentId);

        return payments.Select(p =>
            new PaymentHistoryResponseDto(
                p.Id,
                p.EnrollmentId,
                p.Amount,
                p.Currency,
                p.PayPalOrderId,
                p.Status,
                p.CreatedAt,
                p.PaidAt
            )
        ).ToList();
    }
}