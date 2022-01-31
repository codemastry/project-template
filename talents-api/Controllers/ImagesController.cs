using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TalentsApi.Services;

namespace TalentsApi.Controllers
{
    [Route("images")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly FilesService _filesService;

        public ImagesController(FilesService filesService)
        {
            _filesService = filesService;
        }

        [HttpGet]
        public IActionResult Get(string token)
        {
            var image = _filesService.Get(token);
            if (image == null) return NotFound();
            return File(image, "image/jpeg", "image.jpg");
        }
    }
}