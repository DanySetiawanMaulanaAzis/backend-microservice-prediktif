using smart_table.Interfaces;
using smart_table.Models;
using System.Net.Http.Json;

namespace smart_table.Services
{
    public class MachineService : IMachineService
    {
        private readonly IMachineRepository _repository;
        private readonly HttpClient _httpClient;

        public MachineService(IMachineRepository repository, HttpClient httpClient)
        {
            _repository = repository;
            _httpClient = httpClient;
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

        public async Task<IEnumerable<UnderMaintenance>> GetUnderMaintenanceAsync()
        {
            return await _repository.GetUnderMaintenanceAsync();
        }

        public async Task<UnderMaintenance?> GetUnderMaintenanceByIdAsync(int id)
        {
            return await _repository.GetUnderMaintenanceByIdAsync(id);
        }

        public async Task<IEnumerable<CompletedMaintenance>> GetCompletedMaintenanceHistoryByMachineDetailIdAsync(int machineDetailId)
        {
            return await _repository.GetCompletedMaintenanceHistoryByMachineDetailIdAsync(machineDetailId);
        }

        public async Task<CompletedMaintenanceForAI?> GetCompletedMaintenanceHistoryByMachineDetailIdAsyncForAI(int machineDetailId)
        {
            return await _repository.GetCompletedMaintenanceHistoryByMachineDetailIdAsyncForAI(machineDetailId);
        }

        public async Task<IEnumerable<SmartPrioritization>> GetCompletedMaintenanceForSmartPrioritizationAsync()
        {
            return await _repository.GetCompletedMaintenanceForSmartPrioritizationAsync();
        }

        public async Task<SmartPrioritizationSummary?> GetCompletedMaintenanceSummaryAsync()
        {
            return await _repository.GetCompletedMaintenanceSummaryAsync();
        }

        public async Task<int> CreateUnderMaintenanceAsync(CreateUnderMaintenanceRequest request)
        {
            return await _repository.CreateUnderMaintenanceAsync(request);
        }

        public async Task<bool> UpdateUnderMaintenanceStatusToFalseAsync(int id, UpdateUnderMaintenanceStatusRequest request)
        {
            return await _repository.UpdateUnderMaintenanceStatusToFalseAsync(id, request);
        }

        public async Task<MachinePredictResponse?> GetMachinePredictionAsync(int machineDetailId)
        {
            // 1. Ambil data dari SQL via Repository
            var data = await _repository.GetCompletedMaintenanceHistoryByMachineDetailIdAsyncForAI(machineDetailId);
            if (data == null) return null;

            // 2. Mapping data SQL ke DTO Request Flask
            // Catatan: OperationHours & DowntimeHours bukan nullable, jadi tidak menggunakan '??'
            // Casting (double) dilakukan untuk mencocokkan tipe properti MachinePredictRequest
            var mlPayload = new MachinePredictRequest
            {
                MachineAge = data.MachineAge ?? 0,
                OperatingSeconds = (double)data.OperationHours,
                DowntimeSeconds = (double)data.DowntimeHours,
                DaysSinceLastService = data.DaysSinceLastService ?? 0,
                DaysBetweenEvents = data.DaysBetweenEvents ?? 0
            };

            // 3. Tembak endpoint Flask ML API (base address = ML_API_URL, configured in Program.cs)
            var response = await _httpClient.PostAsJsonAsync("predict", mlPayload);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Gagal menghubungi Flask ML API. Status: {response.StatusCode}");
            }

            // PERBAIKAN DI SINI: Gunakan response.Content.ReadFromJsonAsync
            var result = await response.Content.ReadFromJsonAsync<MachinePredictFlaskResponse>();

            if (result == null || !result.Success)
            {
                throw new Exception($"Prediksi gagal: {result?.Message ?? "Unknown error"}");
            }

            // 4. Return DTO gabungan ke Controller
            return new MachinePredictResponse
            {
                MachineId = data.MachineId,
                MachineName = data.MachineName,
                Severity = result.Severity,
                HealthScore = result.HealthScore
            };
        }
    }
}
