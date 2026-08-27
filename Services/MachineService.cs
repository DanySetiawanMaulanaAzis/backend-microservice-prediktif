using smart_table.Interfaces;
using smart_table.Models;

namespace smart_table.Services
{
    public class MachineService : IMachineService
    {
        private readonly IMachineRepository _repository;

        public MachineService(IMachineRepository repository)
        {
            _repository = repository;
        }

        public async Task<int> CreateMachineAsync(CreateandUpdateMachineRequest request)
        {
            return await _repository.CreateMachineAsync(request);
        }

        public async Task<IEnumerable<Machine>> GetAllMachinesAsync()
        {
            return await _repository.GetAllMachinesAsync();
        }

        public async Task<Machine?> GetMachineByIdAsync(int id)
        {
            return await _repository.GetMachineByIdAsync(id);
        }

        public async Task<bool> UpdateMachineAsync(int id, CreateandUpdateMachineRequest request)
        {
            var machine = await _repository.GetMachineByIdAsync(id);

            if (machine == null)
                return false;

            return await _repository.UpdateMachineAsync(id, request);
        }

        public async Task<bool> DeleteMachineAsync(int id)
        {
            var machine = await _repository.GetMachineByIdAsync(id);

            if (machine == null)
                return false;

            return await _repository.DeleteMachineAsync(id);
        }

        public async Task<IEnumerable<MachineDetail>> GetAllMachineDetailsAsync()
        {
            return await _repository.GetAllMachineDetailsAsync();
        }

        public async Task<MachineDetail?> GetMachineDetailByIdAsync(int id)
        {
            return await _repository.GetMachineDetailByIdAsync(id);
        }

        public async Task<bool> UpdateOperationHoursAsync(UpdateOperationHoursRequest request)
        {
            return await _repository.UpdateOperationHoursAsync(request.MachineId, request.SecondsToAdd);
        }

        public async Task<bool> UpdateDowntimeHoursAsync(UpdateDowntimeHoursRequest request)
        {
            return await _repository.UpdateDowntimeHoursAsync(request.MachineId, request.SecondsToAdd);
        }

        public async Task<byte[]?> GetQrCodeImageAsync(int id)
        {
            return await _repository.GetQrCodeImageAsync(id);
        }

        public async Task<IEnumerable<MachineHistoryItem>> GetMachineHistoryAsync(int machineId)
        {
            return await _repository.GetMachineHistoryAsync(machineId);
        }

        public async Task<IEnumerable<UnderMaintenance>> GetUnderMaintenanceAsync()
        {
            return await _repository.GetUnderMaintenanceAsync();
        }

        public async Task<UnderMaintenance?> GetUnderMaintenanceByIdAsync(int id)
        {
            return await _repository.GetUnderMaintenanceByIdAsync(id);
        }

        public async Task<int> CreateUnderMaintenanceAsync(CreateUnderMaintenanceRequest request)
        {
            return await _repository.CreateUnderMaintenanceAsync(request);
        }

        public async Task<bool> UpdateUnderMaintenanceStatusToFalseAsync(int id, UpdateUnderMaintenanceStatusRequest request)
        {
            return await _repository.UpdateUnderMaintenanceStatusToFalseAsync(id, request);
        }
    }
}
