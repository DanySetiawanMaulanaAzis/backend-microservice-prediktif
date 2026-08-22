using smart_table.Models;

namespace smart_table.Interfaces
{
    public interface IMachineRepository
    {
        Task<int> CreateMachineAsync(CreateandUpdateMachineRequest request);
        Task<IEnumerable<Machine>> GetAllMachinesAsync();
        Task<Machine?> GetMachineByIdAsync(int id);
        Task<bool> UpdateMachineAsync(int id, CreateandUpdateMachineRequest request);
        Task<bool> DeleteMachineAsync(int id);
        Task<IEnumerable<MachineDetail>> GetAllMachineDetailsAsync();
        Task<MachineDetail?> GetMachineDetailByIdAsync(int id);
        Task<bool> UpdateOperationHoursAsync(int machineId, int secondsToAdd);
        Task<bool> UpdateDowntimeHoursAsync(int machineId, int secondsToAdd);
        Task<byte[]?> GetQrCodeImageAsync(int id);
        Task<IEnumerable<UnderMaintenance>> GetUnderMaintenanceAsync();
        Task<UnderMaintenance?> GetUnderMaintenanceByIdAsync(int id);
        Task<int> CreateUnderMaintenanceAsync(CreateUnderMaintenanceRequest request);
    }
}
