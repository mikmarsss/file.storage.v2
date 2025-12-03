using file.storage.v2.Domain.Interfaces;
using file.storage.v2.Domain.Services;
using file_storage.Controllers;
using file_storage.DAL;
using file_storage.Domain.Dto;
using file_storage.Domain.Entities.Business;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mime;

namespace file.storage.v2.Controllers
{
    [Route("api/files")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly FileStorageDbContext _context;
        private readonly ILogger<AuthorizationController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenService;
        private readonly ILoggingService _loggingService;
        private readonly IWebHostEnvironment _appEnvironment;
        public FilesController(FileStorageDbContext context,
                               ILogger<AuthorizationController> logger,
                               IConfiguration configuration,
                               ITokenService tokenService,
                               ILoggingService loggingService,
                               IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
            _tokenService = tokenService;
            _loggingService = loggingService;
            _appEnvironment = appEnvironment;
        }

        [HttpPost("upload/{userId}")]
        public async Task<IActionResult> UploadFiles(Guid userId)
        {
            try
            {
                var files = Request.Form.Files;
                if (files == null || files.Count == 0)
                {
                    return BadRequest(new { message = "Файлы для загрузки отсутствуют!" });
                }
                var results = new List<object>();

                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
                if(user == null)
                {
                    return BadRequest(new { message = "Пользователя с данным Id не существует!" });
                }

                foreach (var file in files)
                {
                    var fileId = Guid.NewGuid();
                    var fileExtension = Path.GetExtension(file.FileName);
                    var storedFileName = fileId.ToString() + fileExtension;

                    var uploadsPath = Path.Combine(_configuration["FileStorage:UploadPath"], "uploads");
                    if (!Directory.Exists(uploadsPath))
                        Directory.CreateDirectory(uploadsPath);

                    var fullPath = Path.Combine(uploadsPath, storedFileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    var normalizedContentType = NormalizeContentType(file.ContentType);
                    var storedFile = new FileEntity
                    {
                        Name = file.FileName,
                        FileName = storedFileName,
                        Size = file.Length,
                        ContentType = normalizedContentType,
                        OwnerId = user.Id
                    };

                    _context.UserFiles.Add(storedFile);
                    await _context.SaveChangesAsync();

                    results.Add(new
                    {
                        id = storedFile.Id,
                        fileName = storedFile.FileName,
                        size = storedFile.Size,
                        ContentType = normalizedContentType,
                        downloadUrl = Url.Action("Download", "Files", new { id = storedFile.Id })
                    });

                }
                _loggingService.WriteLogging(user, "Upload");
                return Ok(new
                {
                    success = true,
                    files = results
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("all/{userId}")]
        public async Task<IActionResult> GetAllUserFiles(Guid userId)
        {
            try
            {
                var user = await _context.Users.Include(x => x.Files).FirstOrDefaultAsync(x => x.Id == userId);

                if (user == null)
                {
                    return BadRequest(new { message = "Пользователя с данным Id не существует!" });
                }
                var userFiles = user.Files;
                var preparedFiles = new List<FileDto>();
                if(userFiles == null)
                {
                    return Ok(new { message = "Файлов нет"});
                }
                foreach(var file in userFiles)
                {
                    var fileDto = file.ToDto();
                    preparedFiles.Add(fileDto);

                }
               
                return Ok(preparedFiles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllFiles()
        {
            try
            {
                var alliles = await _context.UserFiles.Include(x => x.Owner).ToListAsync();
                var preparedFiles = new List<FileDto>();
                if (alliles == null)
                {
                    return Ok(new { message = "Файлов нет" });
                }
                foreach (var file in alliles)
                {
                    var fileDto = file.ToDto();
                    preparedFiles.Add(fileDto);

                }
                    
                return Ok(preparedFiles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("allusers")]
        public async Task<IActionResult> GetAllUsersWithFiles()
        {
            try
            {
                var allUsers = await _context.Users.Include(x => x.Files).ToListAsync();
                var preparedUsers = new List<UserDto>();
                if (allUsers == null)
                {
                    return Ok(new { message = "Файлов нет" });
                }
                foreach (var user in allUsers)
                {
                    var userDto = user.ToDto();
                    preparedUsers.Add(userDto);

                }

                return Ok(preparedUsers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("delete/{FileId}")]
        public async Task<IActionResult> DeleteFile(Guid FileId)
        {
            try
            {
                var file = await _context.UserFiles.Include(x => x.Owner).FirstOrDefaultAsync(x => x.Id == FileId);

                if (file == null)
                {
                    return BadRequest(new { message = "файла с данным Id не существует!" });
                }

                _context.UserFiles.Remove(file);
                await _context.SaveChangesAsync();
                _loggingService.WriteLogging(file.Owner, "Delete");

                return Ok(new {message = "Файл удален!"});
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("download/{FileId}/{UserId}")]
        public async Task<IActionResult> DownloadFile(Guid FileId, Guid UserId)
        {
            try
            {
                var file = await _context.UserFiles.Include(x => x.Owner).FirstOrDefaultAsync(x => x.Id == FileId);

                if (file == null)
                {
                    return BadRequest(new { message = "Файла с данным Id не существует!" });
                }

                if (file.OwnerId != UserId)
                {
                    return BadRequest(new { message = "Данный файл не принадлежит вам!" });
                }

                var fileName = file.FileName;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "uploads", fileName);

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound(new { message = "Файл не найден на сервере!" });
                }

                var fileStream = System.IO.File.OpenRead(filePath);
                var mimeType = GetMimeType(filePath);
                Response.Headers.Append("Content-Disposition", $"attachment; filename=\"{fileName}\"");
                _loggingService.WriteLogging(file.Owner, "Download");

                return File(fileStream, mimeType, fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        private string GetMimeType(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();

            return extension switch
            {
                ".txt" => "text/plain",
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".zip" => "application/zip",
                ".rar" => "application/x-rar-compressed",
                ".mp4" => "video/mp4",
                ".mp3" => "audio/mpeg",
                _ => "application/octet-stream"
            };
        }
        private string NormalizeContentType(string contentType)
        {
            var mimeTypeMappings = new Dictionary<string, string>
    {
                {"image/jpeg", "image"},
                {"image/png", "image"},
                {"image/gif", "image"},
                {"image/bmp", "image"},
                {"image/svg+xml", "image"},
                {"image/webp", "image"},
        
                {"application/pdf", "document"},
                {"application/msword", "document"},
                {"application/vnd.openxmlformats-officedocument.wordprocessingml.document", "document"},
                {"application/vnd.ms-excel", "document"},
                {"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "document"},
                {"application/vnd.ms-powerpoint", "document"},
                {"application/vnd.openxmlformats-officedocument.presentationml.presentation", "document"},
                {"text/plain", "document"},
                {"text/csv", "document"},
        
                {"application/zip", "archive"},
                {"application/x-rar-compressed", "archive"},
                {"application/x-7z-compressed", "archive"},
        
                {"audio/mpeg", "audio"},
                {"audio/wav", "audio"},
                {"audio/ogg", "audio"},
        
                {"video/mp4", "video"},
                {"video/avi", "video"},
                {"video/mpeg", "video"}
            };

            return mimeTypeMappings.TryGetValue(contentType, out string normalizedType)
                ? normalizedType
                : "other";
        }
    }
}
