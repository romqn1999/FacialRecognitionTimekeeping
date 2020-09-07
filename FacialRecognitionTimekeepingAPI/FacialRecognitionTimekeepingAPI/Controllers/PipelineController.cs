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
    public class PipelineController : ControllerBase
    {
        private readonly Services.BlockingCollectionPipelineAwaitable<string, bool> _pipeline;
        private readonly ILogger<PipelineController> _logger;

        public PipelineController(Services.BlockingCollectionPipelineAwaitable<string, bool> pipeline,
            ILogger<PipelineController> logger)
        {
            _pipeline = pipeline;
            _logger = logger;

            Task.Run(async () => _logger.LogInformation((await _pipeline.Execute("dacrom test")).ToString()));
        }

        // GET: api/<PipelineController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/Pipeline/dacrom test
        [HttpGet("{strInput}")]
        public async Task<bool> Get(string strInput)
        {
            return await _pipeline.Execute(strInput);
        }

        // POST api/<PipelineController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<PipelineController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<PipelineController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
