﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Data.SqlClient;
using DAL.Interfaces;
using Domain.Models;

namespace DAL.Repositories
{
    public class UniversityRepository : IUniversityRepository
    {
        private readonly string _connectionString;

        public UniversityRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<int?> GetUniversityIdByEmailDomainAsync(string domain)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var cmd = new SqlCommand(@"
                SELECT TOP 1 UniversityId 
                FROM UniversityEmailDomain 
                WHERE LOWER(Domain) = @Domain", conn);
            cmd.Parameters.AddWithValue("@Domain", domain.ToLower());

            var result = await cmd.ExecuteScalarAsync();
            return result != null ? Convert.ToInt32(result) : null;
        }

        public async Task<List<University>> GetAllAsync()
        {
            var universities = new List<University>();

            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var cmd = new SqlCommand("SELECT UniversityId, Name, City, Latitude, Longitude, Location FROM University", conn);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                universities.Add(new University
                {
                    UniversityId = reader.GetInt32(reader.GetOrdinal("UniversityId")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    City = reader.GetString(reader.GetOrdinal("City")),
                    Latitude = reader.IsDBNull(reader.GetOrdinal("Latitude")) ? null : reader.GetDouble(reader.GetOrdinal("Latitude")),
                    Longitude = reader.IsDBNull(reader.GetOrdinal("Longitude")) ? null : reader.GetDouble(reader.GetOrdinal("Longitude")),
                    Location = reader.IsDBNull(reader.GetOrdinal("Location")) ? null : reader.GetString(reader.GetOrdinal("Location"))
                });
            }

            return universities;
        }


        public async Task<University?> GetByIdAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var query = "SELECT UniversityId, Name FROM University WHERE UniversityId = @Id";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new University
                {
                    UniversityId = reader.GetInt32(reader.GetOrdinal("UniversityId")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                };
            }

            return null;
        }

    }
}

