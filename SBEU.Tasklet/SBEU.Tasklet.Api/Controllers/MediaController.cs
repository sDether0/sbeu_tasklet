using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SBEU.Tasklet.Api.Service;
using SBEU.Tasklet.DataLayer.DataBase;
using SBEU.Tasklet.DataLayer.DataBase.Entities;
using SBEU.Tasklet.Models.Enums;
using SBEU.Tasklet.Models.Requests;

namespace SBEU.Tasklet.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MediaController : ControllerExt
    {
        //private const string url = "https://sbeusilent.space/";
        private readonly ApiDbContext _context;

        public MediaController(ApiDbContext context)
        {
            _context = context;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file, [FromQuery] XContentType content)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!Directory.Exists("Content")) Directory.CreateDirectory("Content");
            var id = Guid.NewGuid()+Guid.NewGuid().ToString();
            using var stream = System.IO.File.Create("Content/" + id);
            await file.CopyToAsync(stream);
            var ext = file.FileName.Split('.').Last();
            var name = string.Join('.',file.FileName.Split('.').Where(x => x != file.FileName.Split('.').Last()));
            var xContent = new XContent()
            {
                Id = id,
                ContentType = content,
                Name = name,
                Extension = ext
            };
            _context.Add(xContent);
            await _context.SaveChangesAsync();
            return Json(new { Link = "Media/" + id, Id = id });
        }

        [HttpGet("{name}")]
        public async Task<FileResult> Load(string name)
        {
            var content = await _context.Contents.FirstOrDefaultAsync(x=>x.Id==name && !x.IsDeleted);
            if (content == null)
            {
                NotFound();
            }
            var file = await System.IO.File.ReadAllBytesAsync("Content/" + name);
            return File(file, "application/octet-stream",content.Name+"."+content.Extension);
        }
    }
}
