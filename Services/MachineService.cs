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
    }
}
