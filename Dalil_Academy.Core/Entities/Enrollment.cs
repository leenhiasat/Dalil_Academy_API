namespace Dalil_Academy.Core.Entities;

public class Enrollment
{
    public int Id { get; set; }

    public int StudentId { get; set; }

    public Student? Student { get; set; }

    public int CourseId { get; set; }

    public Course? Course { get; set; }

    public DateTime EnrolledAt { get; set; }

    public EnrollmentStatus Status { get; set; }
        = EnrollmentStatus.Pending;

    public ICollection<Payment> Payments { get; set; }
        = new List<Payment>();
}