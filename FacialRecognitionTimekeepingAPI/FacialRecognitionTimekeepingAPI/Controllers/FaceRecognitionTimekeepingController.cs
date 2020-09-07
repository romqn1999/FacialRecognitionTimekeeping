using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FacialRecognitionTimekeepingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FaceRecognitionTimekeepingController : ControllerBase
    {
        private readonly Services.FaceRecognitionTimekeepingPipelines _pipelines;
        private readonly ILogger<FaceRecognitionTimekeepingController> _logger;

        public FaceRecognitionTimekeepingController(
            Services.FaceRecognitionTimekeepingPipelines pipelines,
            ILogger<FaceRecognitionTimekeepingController> logger)
        {
            _pipelines = pipelines;
            _logger = logger;

            Task.Run(async () => _logger.LogInformation((await _pipelines.TestPipeline.Execute("dacrom test")).ToString()));
        }

        // GET: api/<FaceRecognitionTimekeepingController>
        [HttpGet]
        public async Task<IEnumerable<bool>> Get()
        {
            return new bool[] {
                await _pipelines.TestPipeline.Execute("dacrom1"),
                await _pipelines.TestPipeline.Execute("test")
            };
        }

        // GET api/<FaceRecognitionTimekeepingController>/5
        [HttpGet("{strInput}")]
        public async Task<bool> Get(string strInput)
        {
            return await _pipelines.TestPipeline.Execute(strInput);
        }

        // POST api/<FaceRecognitionTimekeepingController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // POST api/FaceRecognitionTimekeeping/register
        [HttpPost("register")]
        public async Task<string> Register([FromForm] Models.RegisterInputModel input)
        {
            _logger.LogInformation(input.AliasId);
            _logger.LogInformation(input.FormFile?.FileName);
            return await _pipelines.RegisterPersonPipeline.Execute(input).Result;
        }

        // POST api/FaceRecognitionTimekeeping/timekeeping
        [HttpPost("timekeeping")]
        public async Task<string> Timekeeping()
        {
            var input = new Models.TimekeepingInputModel();
            return await _pipelines.TimekeepingPipeline.Execute(input);
        }

        // PUT api/<FaceRecognitionTimekeepingController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<FaceRecognitionTimekeepingController>/5
        [HttpDelete("{id}")]
        public async Task<string> Delete(string id)
        {
            return await _pipelines.DeletePersonPipeline.Execute(id).Result;
        }
    }
}
