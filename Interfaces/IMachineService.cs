using smart_table.Models;

namespace smart_table.Interfaces
{
    public interface IMachineService
    {
        Task<int> CreateMachineAsync(CreateandUpdateMachineRequest request);
        Task<IEnumerable<Machine>> GetAllMachinesAsync();
        Task<Machine?> GetMachineByIdAsync(int id);
        Task<bool> UpdateMachineAsync(int id, CreateandUpdateMachineRequest request);
        Task<bool> DeleteMachineAsync(int id);
        Task<IEnumerable<MachineDetail>> GetAllMachineDetailsAsync();
        Task<MachineDetail?> GetMachineDetailByIdAsync(int id);
        Task<bool> UpdateOperationHoursAsync(UpdateOperationHoursRequest request);
        Task<bool> UpdateDowntimeHoursAsync(UpdateDowntimeHoursRequest request);
        Task<byte[]?> GetQrCodeImageAsync(int id);
        Task<IEnumerable<UnderMaintenance>> GetUnderMaintenanceAsync();
        Task<UnderMaintenance?> GetUnderMaintenanceByIdAsync(int id);
        Task<IEnumerable<CompletedMaintenance>> GetCompletedMaintenanceHistoryByMachineDetailIdAsync(int machineDetailId);
        Task<int> CreateUnderMaintenanceAsync(CreateUnderMaintenanceRequest request);
        Task<bool> UpdateUnderMaintenanceStatusToFalseAsync(int id, UpdateUnderMaintenanceStatusRequest request);
    }
}
