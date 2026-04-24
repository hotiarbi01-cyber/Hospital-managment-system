namespace HospitalManagementSystem.Models;

public record User(int UserId, string Username, string Role);
public record Patient(int PatientId, string FullName, DateTime DateOfBirth, string Phone, string Address);
public record Doctor(int DoctorId, string FullName, string Specialization, string Phone, string Email, string WorkSchedule);
public record Appointment(int AppointmentId, int PatientId, int DoctorId, DateTime AppointmentDateTime, string Status);
public record MedicalRecord(int RecordId, int AppointmentId, string Diagnosis, string Symptoms, string Medicines, string DoctorNotes);
public record Bill(int BillId, int PatientId, string ServiceName, decimal Price, string Status, DateTime CreatedAt);
public record Room(int RoomId, string RoomNumber, string RoomType, string Status);
