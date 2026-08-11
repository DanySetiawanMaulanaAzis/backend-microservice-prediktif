using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using smart_table.Interfaces;
using smart_table.Models;

namespace smart_table.Repositories
{
    public class MachineRepository : IMachineRepository
    {
        private readonly IConfiguration _configuration;

        public MachineRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private IDbConnection Connection
        {
            get
            {
                return new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            }
        }

        public async Task<int> CreateMachineAsync(CreateandUpdateMachineRequest request)
        {
            using var db = Connection;

            var sql = @"
                INSERT INTO machine(machine_name,location,production_year,created_at)
                VALUES(@MachineName,@Location,@ProductionYear,GETDATE());

                -- Mengambil ID Auto-Increment yang baru saja dibuat
                SELECT CAST(SCOPE_IDENTITY() as int);
            ";

            // Returns: ID baru dari data yang di-insert (misal: 1, 2, 3...)
            return await db.ExecuteScalarAsync<int>(sql, request);
        }


        public async Task<IEnumerable<Machine>> GetAllMachinesAsync()
        {
            using var db = Connection;

            var sql = @"
                SELECT
                    id AS Id,
                    machine_name AS MachineName,
                    location AS Location,
                    production_year AS ProductionYear,
                    created_at AS CreatedAt
                FROM machine
                ORDER BY id;
            ";

            return await db.QueryAsync<Machine>(sql);
        }


        public async Task<Machine?> GetMachineByIdAsync(int id)
        {
            using var db = Connection;

            var sql = @"
                SELECT
                    id AS Id,
                    machine_name AS MachineName,
                    location AS Location,
                    production_year AS ProductionYear,
                    created_at AS CreatedAt
                FROM machine
                WHERE id = @Id;
            ";

            return await db.QueryFirstOrDefaultAsync<Machine>(sql, new { Id = id });
        }


        public async Task<bool> UpdateMachineAsync(int id, CreateandUpdateMachineRequest request)
        {
            using var db = Connection;

            var sql = @"
                UPDATE machine
                SET
                    machine_name = @MachineName,
                    location = @Location,
                    production_year = @ProductionYear
                WHERE id = @Id;
            ";

            var affectedRows = await db.ExecuteAsync(sql, new
            {
                Id = id,
                request.MachineName,
                request.Location,
                request.ProductionYear
            });

            return affectedRows > 0;
        }


        public async Task<bool> DeleteMachineAsync(int id)
        {
            using var db = Connection;

            var sql = @"
                DELETE FROM machine
                WHERE id = @Id;
            ";

            var affectedRows = await db.ExecuteAsync(sql, new { Id = id });

            return affectedRows > 0;
        }
    }
}
