using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Hosting;

namespace WebApplication9.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
    }
    
    [HttpGet("GetWeatherForecastJSONData",Name = "GetWeatherForecastJSONData")]
    public IActionResult GetData()
    {
        List<WeatherForecast> wList = new List<WeatherForecast>();
        int[] numbers = new[] { 1, 4, 6, 8 };
        foreach (var i in numbers)
        {
            var w = new WeatherForecast()
            {
                Date = DateTime.Now.AddDays(i),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            };
            wList.Add(w);
        }


        return Ok(wList);

    }

    [HttpPost("Login")]
    public IActionResult Login([FromForm] string userName, [FromForm] string password)
    {
        if (string.IsNullOrEmpty(userName) && string.IsNullOrEmpty(password))
        {
            return BadRequest("User ID or Password is empty");
        }

        bool isValidUser = (userName == "jadda" && password == "test123");
        
            return isValidUser? Ok("Succesfull"): Unauthorized("Invalid Creds");
    }

    [HttpGet("searchCity")]
    [ProducesResponseType<City>(StatusCodes.Status200OK )]
    [ProducesResponseType(400, Type = typeof(BadRequest))]
    public IActionResult searchCity([FromQuery] int? cityCode, [FromQuery] int page = 1)
    {

        if (cityCode is null)
        {
            return BadRequest("City name is Empty ");
        } 
        else if (cityCode == 1)
        {
            return Ok(new City { CityCode = (int)cityCode, CityName = "Hyderabad" });
        }
        
        return Ok(new City{ CityCode = (int)cityCode, CityName = "Other City"});
    }

    [HttpGet("searchUser")]
    [ProducesResponseType(200, Type= typeof(WeatherForecast) )]
    [ProducesResponseType(400, Type = typeof(BadRequest))]
    public ActionResult<WeatherForecast> searchUserDetails([FromQuery] int? temprature, [FromQuery] int page = 1)
    {

        return temprature is null? throw new Exception("Temprature is Empty ") :  new OkObjectResult(new WeatherForecast() { Date = DateTime.Now, Summary = "Hot", TemperatureC = (int)temprature });
    }
    [HttpPost("Create")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public IActionResult Create([FromHeader(Name = "X-Customer-ID")] string customerID,[FromBody] WeatherForecast? weatherForecast)
    {
        if (customerID == string.Empty)
            return BadRequest("Header is missing");
        
        if (weatherForecast.TemperatureC == null)
            _logger.LogError("this is bad request");
         return BadRequest("weatherForecast temprature data is missing");

        return CreatedAtAction("searchUserDetails", new { temprature = weatherForecast.TemperatureC, page = 1}, weatherForecast);
    }
    
}