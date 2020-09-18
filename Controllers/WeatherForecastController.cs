using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microservices.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Microservices.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    // ten url se dc lay cua ten public class bo di phan Controller phia sau
    // vid du class WeatherForecastController => url: api/weatherforecast
    public class WeatherForecastController : ControllerBase
    {
        //khoi tao class co so du lieu de cac class ngang hang no co the truy cap 
        private readonly DataContext _context;
        public WeatherForecastController(DataContext context)
        {
            _context = context;

        }
        // private static readonly string[] Summaries = new[]
        // {
        //     "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        // };

        // private readonly ILogger<WeatherForecastController> _logger;

        // public WeatherForecastController(ILogger<WeatherForecastController> logger)
        // {
        //     _logger = logger;
        // }

        [HttpGet]
        //get  
        public async  Task<IActionResult> GetValues()
        //http response to client
        //async giu thread luon chay va khong block cac thread khac 
        {
           var values = await _context.Values.ToListAsync();
           return Ok(values);
        }
    }
}
