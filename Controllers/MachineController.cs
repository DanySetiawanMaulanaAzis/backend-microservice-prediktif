using Microsoft.AspNetCore.Mvc;
using smart_table.Interfaces;
using smart_table.Models;

namespace smart_table.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class MachineController : ControllerBase
    {
        private readonly IMachineService _service;

        public MachineController(IMachineService service)
        {
            _service = service;
        }


        [HttpPost]
        public async Task<IActionResult> CreateMachine(CreateandUpdateMachineRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var id = await _service.CreateMachineAsync(request);

            return Ok(new
            {
                message = "Machine created successfully",
                machineId = id
            });
        }


        [HttpGet]
        public async Task<IActionResult> GetAllMachines()
        {
            var machines = await _service.GetAllMachinesAsync();

            return Ok(machines);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetMachineById(int id)
        {
            var machine = await _service.GetMachineByIdAsync(id);

            if (machine == null)
            {
                return NotFound(new
                {
                    message = "Machine not found."
                });
            }

            return Ok(machine);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMachine(int id, CreateandUpdateMachineRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.UpdateMachineAsync(id, request);

            if (!result)
            {
                return NotFound(new
                {
                    message = "Machine not found."
                });
            }

            return Ok(new
            {
                message = "Machine updated successfully."
            });
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMachine(int id)
        {
            var result = await _service.DeleteMachineAsync(id);

            if (!result)
            {
                return NotFound(new
                {
                    message = "Machine not found."
                });
            }

            return Ok(new
            {
                message = "Machine deleted successfully."
            });
        }

        // GET: api/MachineDetail
        [HttpGet("details")]
        public async Task<ActionResult<IEnumerable<MachineDetail>>> GetAllMachineDetails()
        {
            var result = await _service.GetAllMachineDetailsAsync();
            return Ok(result);
        }

        // GET: api/MachineDetail/5
        [HttpGet("details/{id}")]
        public async Task<ActionResult<MachineDetail>> GetMachineDetailById(int id)
        {
            var result = await _service.GetMachineDetailByIdAsync(id);

            if (result == null)
            {
                return NotFound(new { message = $"Machine detail dengan ID {id} tidak ditemukan." });
            }

            return Ok(result);
        }

        // PUT: api/MachineDetail/operation-hours
        [HttpPut("operation-hours")]
        public async Task<IActionResult> UpdateOperationHours([FromBody] UpdateOperationHoursRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _service.UpdateOperationHoursAsync(request);
            if (!updated)
                return NotFound(new { message = "Machine detail not found" });

            return Ok(new { message = "Operation hours updated successfully" });
        }

        // PUT: api/Machine/downtime-hours
        [HttpPut("downtime-hours")]
        public async Task<IActionResult> UpdateDowntimeHours([FromBody] UpdateDowntimeHoursRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _service.UpdateDowntimeHoursAsync(request);
            if (!updated)
                return NotFound(new { message = "Machine detail not found" });

            return Ok(new { message = "Downtime hours updated successfully" });
        }

        [HttpGet("qr-code/{id}")]
        public async Task<IActionResult> GetMachineQrCode(int id)
        {
            var qrBytes = await _service.GetQrCodeImageAsync(id);

            if (qrBytes == null || qrBytes.Length == 0)
            {
                return NotFound(new
                {
                    message = "QR Code tidak ditemukan untuk mesin ini."
                });
            }

            // Mengembalikan file biner gambar PNG secara langsung
            return File(qrBytes, "image/png");
        }

        [HttpGet("under-maintenance")]
        public async Task<IActionResult> GetUnderMaintenance()
        {
            var data = await _service.GetUnderMaintenanceAsync();
            return Ok(data);
        }

        [HttpGet("under-maintenance/{id}")]
        public async Task<IActionResult> GetUnderMaintenanceById(int id)
        {
            var data = await _service.GetUnderMaintenanceByIdAsync(id);
            if (data == null)
            {
                return NotFound(new { message = $"Data maintenance dengan ID {id} tidak ditemukan." });
            }

            return Ok(data);
        }

     
        [HttpPost("under-maintenance")]
        public async Task<IActionResult> CreateUnderMaintenance([FromBody] CreateUnderMaintenanceRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var id = await _service.CreateUnderMaintenanceAsync(request);
            return Ok(new { message = "Permintaan maintenance berhasil dibuat", id });
        }


        [HttpPut("under-maintenance/complete/{id}")]
        public async Task<IActionResult> CompleteUnderMaintenance(int id, [FromBody] UpdateUnderMaintenanceStatusRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var isSuccess = await _service.UpdateUnderMaintenanceStatusToFalseAsync(id, request);

            if (!isSuccess)
            {
                return NotFound(new { message = $"Data UnderMaintenance dengan ID {id} tidak ditemukan." });
            }

            return Ok(new
            {
                message = "Status maintenance berhasil diubah ke 0 dan log action berhasil disimpan.",
                id = id
            });
        }
    }
}
