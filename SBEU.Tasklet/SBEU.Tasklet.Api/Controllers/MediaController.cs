using Microsoft.AspNetCore.Mvc;
using SBEU.Tasklet.Api.Service;

namespace SBEU.Tasklet.Api.Controllers
{
    [Route("[controller]")]
    public class MediaController : ControllerExt
    {
        private string url = "https://sbeusilent.space/";

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (!Directory.Exists("Content")) Directory.CreateDirectory("Content");
            var name = Guid.NewGuid()+"."+file.FileName.Split('.').Last();
            using var stream = System.IO.File.Create("Content/" + name);
            await file.CopyToAsync(stream);
            return Json(new { Link = url + "Media/" + name });
        }

        [HttpGet("{name}")]
        public async Task<FileResult> Load(string name)
        {
            var file = await System.IO.File.ReadAllBytesAsync("Content/" + name);
            return new FileContentResult(file, "application/octet-stream");
        }
    }
}
