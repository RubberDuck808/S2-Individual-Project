using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Data.SqlClient;
using DAL.Interfaces;

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
    }
}

