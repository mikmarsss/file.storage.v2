using file.storage.v2.Domain.Entities.Business;
using file.storage.v2.Domain.Interfaces;
using file_storage.Controllers;
using file_storage.DAL;
using file_storage.Domain.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mime;
using System.Text;
using System.Text.Json;

namespace file.storage.v2.Controllers
{
    [Route("api/logging")]
    [ApiController]
    public class LoggingController : ControllerBase
    {
        private readonly FileStorageDbContext _context;
        private readonly IConfiguration _configuration;
        public LoggingController(FileStorageDbContext context,
                               IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllLoggedActions()
        {
            try
            {
                var allLoggedActions = await _context.Loggings.ToListAsync();

                if (allLoggedActions == null)
                {
                    return Ok(new { message = "Файлов нет" });
                }

                return Ok(allLoggedActions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }


        [HttpGet("download")]
        public async Task<IActionResult> DownloadAllLoggedActions()
        {
            try
            {
                byte[] fileBytes;
                string fileName;
                string contentType;

                var allLoggedActions = await _context.Loggings.ToListAsync();

                if (allLoggedActions == null)
                {
                    return Ok(new { message = "Файлов нет" });
                }
                (fileBytes, fileName, contentType) = GenerateJsonFile(allLoggedActions);
               return File(fileBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        private (byte[] fileBytes, string fileName, string contentType) GenerateJsonFile(List<LoggingEntity> actions)
        {
            var exportData = new
            {
                ExportDate = DateTime.UtcNow,
                TotalActions = actions.Count,
                UserActions = actions
            };

            var json = JsonSerializer.Serialize(exportData, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var fileName = $"user-actions-{DateTime.UtcNow:yyyyMMdd-HHmmss}.json";
            return (Encoding.UTF8.GetBytes(json), fileName, "application/json");
        }
    }
}
