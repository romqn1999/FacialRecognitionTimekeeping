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
        private readonly Services.TimekeepingContext _timekeepingContext;

        public FaceRecognitionTimekeepingController(
            Services.FaceRecognitionTimekeepingPipelines pipelines,
            ILogger<FaceRecognitionTimekeepingController> logger,
            Services.TimekeepingContext timekeepingContext)
        {
            _pipelines = pipelines;
            _logger = logger;
            _timekeepingContext = timekeepingContext;

            Task.Run(async () => _logger.LogInformation((await _pipelines.TestPipeline.Execute("dacrom test")).ToString()));
        }

        // GET: api/<FaceRecognitionTimekeepingController>
        [HttpGet]
        public ActionResult<object> GetByQuery([FromQuery(Name = "aliasId")]string aliasId)
        {
            return _timekeepingContext.TimekeepingRecords
                .Where(r => string.IsNullOrEmpty(aliasId) ? true : r.AliasId == aliasId)
                .OrderByDescending(r => r.TimekeepingRecordUnixTimestampSeconds)
                .ToList();
        }

        // GET api/<FaceRecognitionTimekeepingController>/5
        [HttpGet("{aliasId}")]
        public ActionResult<object> Get(string aliasId)
        {
            return _timekeepingContext.TimekeepingRecords
                .Where(r => r.AliasId == aliasId)
                .OrderByDescending(r => r.TimekeepingRecordUnixTimestampSeconds)
                .ToList();
        }

        // POST api/<FaceRecognitionTimekeepingController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // POST api/FaceRecognitionTimekeeping/register
        [HttpPost("register")]
        public async Task<ActionResult<object>> Register([FromForm] Models.RegisterPipelineModel input)
        {
            _logger.LogInformation(input.AliasId);
            _logger.LogInformation(input.FormFile?.FileName);
            input.TimekeepingContext = _timekeepingContext;
            Models.RegisterPipelineModel pipelineModel = await _pipelines.RegisterPersonPipeline.Execute(input).Result;
            if (pipelineModel.HasError)
            {
                return pipelineModel.Message;
            }
            else
            {
                return pipelineModel.TimekeepingPerson;
            }
        }

        // POST api/FaceRecognitionTimekeeping/timekeeping
        [HttpPost("timekeeping")]
        public async Task<ActionResult<object>> Timekeeping([FromForm] Models.RecognizeTimekeepingPipelineModels input)
        {
            _logger.LogInformation(input.FormFile?.FileName);
            input.TimekeepingContext = _timekeepingContext;
            Models.RecognizeTimekeepingPipelineModels pipelineModel = await _pipelines.TimekeepingPipeline.Execute(input).Result;
            if (pipelineModel.HasError)
            {
                return pipelineModel.Message;
            }
            else
            {
                return pipelineModel.TimekeepingPeople;
            }
        }

        // PUT api/<FaceRecognitionTimekeepingController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<FaceRecognitionTimekeepingController>/5
        [HttpDelete("{aliasId}")]
        public async Task<string> Delete(string aliasId)
        {
            Models.DeletePipelineModel pipelineModel = new Models.DeletePipelineModel { AliasId = aliasId };
            _logger.LogInformation(pipelineModel.AliasId);
            pipelineModel.TimekeepingContext = _timekeepingContext;
            pipelineModel = await _pipelines.DeletePersonPipeline.Execute(pipelineModel).Result;
            if (pipelineModel.HasError)
            {
                return pipelineModel.Message;
            }
            else
            {
                return null;
            }
        }
    }
}
