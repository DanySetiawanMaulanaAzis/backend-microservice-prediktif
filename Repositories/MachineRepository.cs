using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using smart_table.Interfaces;
using smart_table.Models;
using QRCoder;
using smart_table.Helpers;

namespace smart_table.Repositories
{
    public class MachineRepository : IMachineRepository
    {
        private readonly IConfiguration _configuration;

        public MachineRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            }
        }

        public async Task<int> CreateMachineAsync(CreateandUpdateMachineRequest request)
        {
            using var db = Connection;
            await db.OpenAsync();

            await using var transaction = await db.BeginTransactionAsync();

            try
            {
                // 1. Insert ke tabel machine & ambil Machine ID yang baru
                var sqlInsertMachine = @"
                    INSERT INTO machine (machine_name, location, production_year, created_at)
                    VALUES (@MachineName, @Location, @ProductionYear, GETDATE());
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                int newMachineId = await db.ExecuteScalarAsync<int>(sqlInsertMachine, request, transaction);

                // 2. Generate QR Code (Contoh payload: ID / URL / JSON data mesin)
                string qrContent = $"MACHINE-ID:{newMachineId}|NAME:{request.MachineName}";
                byte[] qrCodeBytes = QrCodeHelper.GeneratePngQrCode(qrContent);

                // 3. Update kolom qr_code di tabel machine
                var sqlUpdateQrCode = @"
                    UPDATE machine 
                    SET qr_code = @QrCode 
                    WHERE id = @Id;";

                await db.ExecuteAsync(sqlUpdateQrCode, new { QrCode = qrCodeBytes, Id = newMachineId }, transaction);

                // 4. Insert ke tabel machine_detail
                var sqlInsertDetail = @"
                    INSERT INTO machine_detail (
                        machine_id, 
                        machine_name, 
                        location, 
                        production_year,
                        operation_hours,
                        downtime_hours,
                        ahs,
                        status_id, 
                        first_update, 
                        last_update
                    ) 
                    VALUES (
                        @MachineId, 
                        @MachineName, 
                        @Location, 
                        @ProductionYear,
                        @OperationHours,
                        @DowntimeHours,
                        @Ahs,
                        (SELECT id FROM machine_status WHERE status_name = @StatusName), 
                        GETDATE(), 
                        GETDATE()
                    );";

                var detailParameters = new
                {
                    MachineId = newMachineId,
                    MachineName = request.MachineName,
                    Location = request.Location,
                    ProductionYear = request.ProductionYear,
                    OperationHours = 0,
                    DowntimeHours = 0.00m,
                    Ahs = 100,
                    StatusName = "Routine"
                };

                await db.ExecuteAsync(sqlInsertDetail, detailParameters, transaction);

                // 5. Commit transaksi jika semua berhasil
                await transaction.CommitAsync();

                return newMachineId;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
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
            await db.OpenAsync();

            await using var transaction = await db.BeginTransactionAsync();

            try
            {
                // 1. Update data pada tabel utama machine
                var sqlUpdateMachine = @"
            UPDATE machine 
            SET 
                machine_name = @MachineName, 
                location = @Location, 
                production_year = @ProductionYear 
            WHERE id = @Id;";

                var affectedRows = await db.ExecuteAsync(sqlUpdateMachine, new
                {
                    Id = id,
                    request.MachineName,
                    request.Location,
                    request.ProductionYear
                }, transaction);

                // 2. Update data terkait pada tabel machine_detail
                var sqlUpdateDetail = @"
            UPDATE machine_detail 
            SET 
                machine_name = @MachineName, 
                location = @Location, 
                production_year = @ProductionYear, 
                last_update = GETDATE() 
            WHERE machine_id = @Id;";

                await db.ExecuteAsync(sqlUpdateDetail, new
                {
                    Id = id,
                    request.MachineName,
                    request.Location,
                    request.ProductionYear
                }, transaction);

                // 3. Commit transaksi jika kedua query berhasil
                await transaction.CommitAsync();

                return affectedRows > 0;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


        public async Task<bool> DeleteMachineAsync(int id)
        {
            using var db = Connection;
            await db.OpenAsync();

            await using var transaction = await db.BeginTransactionAsync();

            try
            {
                // 1. Hapus data pada tabel machine terlebih dahulu
                var sqlDeleteMachine = @"
            DELETE FROM machine 
            WHERE id = @Id;";

                var affectedRows = await db.ExecuteAsync(sqlDeleteMachine, new { Id = id }, transaction);

                // 2. Kemudian hapus data terkait pada tabel machine_detail
                var sqlDeleteDetail = @"
            DELETE FROM machine_detail 
            WHERE machine_id = @Id;";

                await db.ExecuteAsync(sqlDeleteDetail, new { Id = id }, transaction);

                // 3. Commit transaksi jika semua query berhasil
                await transaction.CommitAsync();

                return affectedRows > 0;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<MachineDetail>> GetAllMachineDetailsAsync()
        {
            using var db = Connection;

            var sql = @"
                SELECT
                    md.id AS Id,
                    md.machine_id AS MachineId,
                    md.machine_name AS MachineName,
                    md.location AS Location,
                    md.production_year AS ProductionYear,
                    md.operation_hours AS OperationHours,
                    md.downtime_hours AS DowntimeHours,
                    md.ahs AS Ahs,
                    md.status_id AS StatusId,
                    ms.status_name AS StatusName,
                    md.action_id AS ActionId,
                    md.last_7_days AS Last7Days,
                    md.last_30_days AS Last30Days,
                    md.last_90_days AS Last90Days,
                    md.first_update AS FirstUpdate,
                    md.last_update AS LastUpdate
                FROM machine_detail md
                INNER JOIN machine_status ms
                    ON md.status_id = ms.id
                ORDER BY md.id DESC;";

            return await db.QueryAsync<MachineDetail>(sql);
        }

        public async Task<MachineDetail?> GetMachineDetailByIdAsync(int id)
        {
            using var db = Connection;

            var sql = @"
                SELECT 
                    md.id AS Id, 
                    md.machine_id AS MachineId, 
                    md.machine_name AS MachineName, 
                    md.location AS Location, 
                    md.production_year AS ProductionYear,
                    md.operation_hours AS OperationHours,
                    md.downtime_hours AS DowntimeHours,
                    md.ahs AS Ahs, 
                    md.status_id AS StatusId, 
                    ms.status_name AS StatusName,
                    md.action_id AS ActionId, 
                    md.last_7_days AS Last7Days, 
                    md.last_30_days AS Last30Days, 
                    md.last_90_days AS Last90Days, 
                    md.first_update AS FirstUpdate, 
                    md.last_update AS LastUpdate
                FROM machine_detail md
                INNER JOIN machine_status ms 
                    ON md.status_id = ms.id
                WHERE md.id = @Id;";

            return await db.QueryFirstOrDefaultAsync<MachineDetail>(sql, new { Id = id });
        }

        public async Task<bool> UpdateOperationHoursAsync(int machineId, int secondsToAdd)
        {
            using var db = Connection;
            var sql = @"
        UPDATE machine_detail 
        SET operation_hours = operation_hours + @SecondsToAdd,
            last_update = GETDATE()
        WHERE machine_id = @MachineId;";

            var rowsAffected = await db.ExecuteAsync(sql, new { MachineId = machineId, SecondsToAdd = secondsToAdd });
            return rowsAffected > 0;
        }

        public async Task<bool> UpdateDowntimeHoursAsync(int machineId, int secondsToAdd)
        {
            using var db = Connection;
            var sql = @"
        UPDATE machine_detail 
        SET downtime_hours = downtime_hours + @SecondsToAdd,
            last_update = GETDATE()
        WHERE machine_id = @MachineId;";

            var rowsAffected = await db.ExecuteAsync(sql, new { MachineId = machineId, SecondsToAdd = secondsToAdd });
            return rowsAffected > 0;
        }

        public async Task<byte[]?> GetQrCodeImageAsync(int machineDetailId)
        {
            using var db = Connection;

            var sql = @"
        SELECT m.qr_code 
        FROM machine m
        INNER JOIN machine_detail md ON m.id = md.machine_id
        WHERE md.id = @DetailId;";

            return await db.QueryFirstOrDefaultAsync<byte[]?>(sql, new { DetailId = machineDetailId });
        }

        public async Task<IEnumerable<UnderMaintenance>> GetUnderMaintenanceAsync()
        {
            using var db = Connection;

            var sql = @"SELECT 
                    um.id AS Id, 
                    um.machine_id AS MachineId, 
                    um.machine_name AS MachineName, 
                    um.maintenance AS Maintenance, 
                    um.created_at AS CreatedAt, 
                    um.event_id AS EventId,
                    em.event AS Event,
                    em.maintenance_type AS MaintenanceType,
                    md.id AS MachineDetailId, 
                    md.location AS Location, 
                    md.ahs AS Ahs,
                    md.production_year AS ProductionYear,
                    md.operation_hours AS OperationHours,
                    md.downtime_hours AS DowntimeHours,
                    ms.status_name AS StatusName
                FROM undermaintenance um 
                LEFT JOIN machine_detail md ON um.machine_id = md.machine_id
                LEFT JOIN machine_status ms ON md.status_id = ms.id
                LEFT JOIN event_maintenance em ON um.event_id = em.id
                WHERE um.maintenance = 1;";

            return await db.QueryAsync<UnderMaintenance>(sql);
        }

        public async Task<UnderMaintenance?> GetUnderMaintenanceByIdAsync(int id)
        {
            using var db = Connection;

            var sql = @"
        SELECT 
            um.id AS Id, 
            um.machine_id AS MachineId, 
            um.machine_name AS MachineName, 
            um.maintenance AS Maintenance, 
            um.created_at AS CreatedAt, 
            um.event_id AS EventId,

            em.event AS Event,
            em.maintenance_type AS MaintenanceType,

            md.id AS MachineDetailId, 
            md.location AS Location, 
            md.ahs AS Ahs,
            md.production_year AS ProductionYear,
            md.operation_hours AS OperationHours,
            md.downtime_hours AS DowntimeHours,

            ms.status_name AS StatusName,

            -- Days since last Service
            DATEDIFF(
                DAY,
                last_service.created_at,
                um.created_at
            ) AS DaysSinceLastService,

            -- Days between previous Failure and current event
            DATEDIFF(
                DAY,
                previous_failure.created_at,
                um.created_at
            ) AS DaysBetweenEvents

        FROM undermaintenance um 

        LEFT JOIN machine_detail md 
            ON um.machine_id = md.machine_id 

        LEFT JOIN machine_status ms 
            ON md.status_id = ms.id

        LEFT JOIN event_maintenance em 
            ON um.event_id = em.id

        OUTER APPLY (
            SELECT TOP 1
                service.created_at
            FROM undermaintenance service
            WHERE service.machine_id = um.machine_id
              AND service.event_id = 1
              AND service.created_at < um.created_at
            ORDER BY service.created_at DESC
        ) last_service

        OUTER APPLY (
            SELECT TOP 1
                failure.created_at
            FROM undermaintenance failure
            WHERE failure.machine_id = um.machine_id
              AND failure.event_id = 2
              AND failure.created_at < um.created_at
            ORDER BY failure.created_at DESC
        ) previous_failure

        WHERE um.id = @Id;
    ";

            return await db.QueryFirstOrDefaultAsync<UnderMaintenance>(
                sql,
                new { Id = id }
            );
        }


        public async Task<IEnumerable<MachineHistoryItem>> GetMachineHistoryAsync(int machineId)
        {
            using var db = Connection;

            var sql = @"
                SELECT
                    um.id AS Id,
                    um.machine_id AS MachineId,
                    um.machine_name AS MachineName,
                    um.maintenance AS Maintenance,
                    um.created_at AS CreatedAt,
                    um.event_id AS EventId,
                    em.event AS Event,
                    em.maintenance_type AS MaintenanceType,
                    um.action_id AS ActionId,
                    a.name AS ActionBy,
                    a.action AS ActionTaken,
                    a.created_at AS ActionCreatedAt
                FROM undermaintenance um
                LEFT JOIN event_maintenance em ON um.event_id = em.id
                LEFT JOIN [action] a ON um.action_id = a.id
                WHERE um.machine_id = @MachineId
                ORDER BY um.created_at DESC;";

            return await db.QueryAsync<MachineHistoryItem>(sql, new { MachineId = machineId });
        }

        public async Task<int> CreateUnderMaintenanceAsync(CreateUnderMaintenanceRequest request)
        {
            using var db = Connection;
            var sql = @"
                INSERT INTO undermaintenance
                    (machine_id, machine_name, maintenance, event_id, created_at)
                VALUES 
                    (@MachineId, @MachineName, @Maintenance, @EventId, GETDATE());

                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            return await db.ExecuteScalarAsync<int>(sql, request);
        }

        public async Task<bool> UpdateUnderMaintenanceStatusToFalseAsync(int id, UpdateUnderMaintenanceStatusRequest request)
        {
            using var db = Connection;
            await db.OpenAsync();

            await using var transaction = await db.BeginTransactionAsync();

            try
            {
                // 1. Update status maintenance ke 0
                var sqlUpdateStatus = @"
                    UPDATE undermaintenance
                    SET maintenance = 0
                    WHERE id = @Id;";

                int rowsAffected = await db.ExecuteAsync(sqlUpdateStatus, new { Id = id }, transaction);

                if (rowsAffected == 0)
                {
                    await transaction.RollbackAsync();
                    return false;
                }

                // 2. Insert ke tabel [action] dan ambil Generated ID (SQL Server OUTPUT)
                var sqlInsertAction = @"
                    INSERT INTO [action] (user_id, name, [action], created_at)
                    OUTPUT INSERTED.id
                    VALUES (@UserId, @Name, @Action, GETDATE());";

                int actionId = await db.ExecuteScalarAsync<int>(sqlInsertAction, new
                {
                    UserId = request.UserId,
                    Name = request.Name,
                    Action = string.IsNullOrWhiteSpace(request.Action) ? "Maintenance Completed" : request.Action
                }, transaction);

                // 3. Update kolom action_id di tabel undermaintenance
                var sqlUpdateActionId = @"
                    UPDATE undermaintenance
                    SET action_id = @ActionId
                    WHERE id = @Id;";

                await db.ExecuteAsync(sqlUpdateActionId, new { ActionId = actionId, Id = id }, transaction);

                // 4. Commit transaksi jika semua langkah berhasil
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
