using DAL.Interfaces;
using Domain.Models;
using Microsoft.Data.SqlClient;

public class BookingRepository : IBookingRepository
{
    private readonly string _connectionString;

    public BookingRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<Booking?> GetByIdAsync(int bookingId)
    {
        using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        var cmd = new SqlCommand("SELECT * FROM Booking WHERE BookingId = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", bookingId);

        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return ReadBooking(reader);
        }

        return null;
    }

    public async Task<IEnumerable<Booking>> GetByStudentAsync(int studentId)
    {
        var list = new List<Booking>();
        using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        var cmd = new SqlCommand("SELECT * FROM Booking WHERE StudentId = @StudentId", conn);
        cmd.Parameters.AddWithValue("@StudentId", studentId);

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(ReadBooking(reader));
        }

        return list;
    }

    public async Task<IEnumerable<Booking>> GetByAccommodationAsync(int accommodationId)
    {
        var list = new List<Booking>();
        using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        var cmd = new SqlCommand("SELECT * FROM Booking WHERE AccommodationId = @AccommodationId", conn);
        cmd.Parameters.AddWithValue("@AccommodationId", accommodationId);

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(ReadBooking(reader));
        }

        return list;
    }

    public async Task AddAsync(Booking booking)
    {
        using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        var cmd = new SqlCommand(@"
        INSERT INTO Booking 
        (StudentId, AccommodationId, StartDate, EndDate, TotalAmount, StatusId, ApplicationId)
        VALUES 
        (@StudentId, @AccommodationId, @StartDate, @EndDate, @TotalAmount, @StatusId, @ApplicationId);
        SELECT SCOPE_IDENTITY();", conn);

        cmd.Parameters.AddWithValue("@StudentId", booking.StudentId);
        cmd.Parameters.AddWithValue("@AccommodationId", booking.AccommodationId);
        cmd.Parameters.AddWithValue("@StartDate", booking.StartDate);
        cmd.Parameters.AddWithValue("@EndDate", booking.EndDate);
        cmd.Parameters.AddWithValue("@TotalAmount", booking.TotalAmount);
        cmd.Parameters.AddWithValue("@StatusId", booking.StatusId);
        cmd.Parameters.AddWithValue("@ApplicationId", booking.ApplicationId);

        var id = await cmd.ExecuteScalarAsync();
        booking.BookingId = Convert.ToInt32(id);
    }


    public async Task UpdateAsync(Booking booking)
    {
        using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        var cmd = new SqlCommand(@"
            UPDATE Booking
            SET StartDate = @StartDate,
                EndDate = @EndDate,
                StatusId = @StatusId
            WHERE BookingId = @BookingId", conn);

        cmd.Parameters.AddWithValue("@StartDate", booking.StartDate);
        cmd.Parameters.AddWithValue("@EndDate", booking.EndDate);
        cmd.Parameters.AddWithValue("@StatusId", booking.StatusId);
        cmd.Parameters.AddWithValue("@BookingId", booking.BookingId);

        await cmd.ExecuteNonQueryAsync();
    }

    public async Task DeleteAsync(int bookingId)
    {
        using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        var cmd = new SqlCommand("DELETE FROM Booking WHERE BookingId = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", bookingId);

        await cmd.ExecuteNonQueryAsync();
    }

    private Booking ReadBooking(SqlDataReader reader)
    {
        return new Booking
        {
            BookingId = Convert.ToInt32(reader["BookingId"]),
            StudentId = Convert.ToInt32(reader["StudentId"]),
            AccommodationId = Convert.ToInt32(reader["AccommodationId"]),
            StatusId = Convert.ToInt32(reader["StatusId"]),
            ApplicationId = Convert.ToInt32(reader["ApplicationId"]),
            TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),

            StartDate = Convert.ToDateTime(reader["StartDate"]),
            EndDate = Convert.ToDateTime(reader["EndDate"])
        };
    }

    public async Task<Booking?> GetAcceptedBookingByAccommodationIdAsync(int accommodationId)
    {
        using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        var cmd = new SqlCommand(@"
        SELECT TOP 1 * FROM Booking
        WHERE AccommodationId = @AccommodationId AND StatusId = 2", conn);

        cmd.Parameters.AddWithValue("@AccommodationId", accommodationId);

        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return ReadBooking(reader);
        }

        return null;
    }




}
