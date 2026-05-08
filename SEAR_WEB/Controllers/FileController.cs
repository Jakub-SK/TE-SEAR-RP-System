using Microsoft.AspNetCore.Mvc;
using SEAR_DataContract.Models;
using SEAR_WEB.Models;
using SEAR_WEB.Session;

namespace SEAR_WEB.Controllers
{
    public class FileController : Controller
    {
        private readonly SessionCache _sessionCache;
        public FileController(SessionCache sessionCache)
        {
            _sessionCache = sessionCache;
        }
        public async Task<IActionResult> Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No File uploaded");
            
            //30MB
            const long maxFileSize = 30 * 1024 * 1024;
            if (file.Length > maxFileSize)
                return BadRequest("File size exceeded 30MB");

            if (!file.ContentType.StartsWith("image/"))
                return BadRequest("Only images are allowed");

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            byte[] fileBytes = memoryStream.ToArray();
            if (await FileModel.SaveFileToDatabase(file.FileName, file.ContentType, fileBytes))
                return Ok("Upload Successfully");

            return BadRequest("Upload failed :( please try again");
        }
        [HttpGet("/File/Download/{id}")]
        public async Task<IActionResult> Download(int id)
        {
            ReturnDownloadFile file = await FileModel.GetFileFromDb(id);

            if (file == null)
                return NotFound();
            
            return File(file.FileBytes, file.ContentType);
        }
    }
}