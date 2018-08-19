using System;
using Microsoft.AspNetCore.Mvc;
using Clockwork.API.Models;
using System.Web.Http.Cors;
using System.Collections.Generic;
using static System.TimeZoneInfo;

namespace Clockwork.API.Controllers
{
    [EnableCors(origins: "http://localhost:50047", headers: "*", methods: "*")]
    public class CurrentTimeController : Controller
    {
        [Route("api/[controller]")]
        // GET api/currenttime
        [HttpGet]
        public IActionResult Get()
        {
            var utcTime = DateTime.UtcNow;
            var serverTime = DateTime.Now;
            var ip = this.HttpContext.Connection.RemoteIpAddress.ToString();

            var returnVal = new CurrentTimeQuery
            {
                UTCTime = utcTime,
                ClientIp = ip,
                Time = serverTime
            };

            return Ok(returnVal);
        }

        [Route("api/[controller]/get/{timezone}")]
        // GET api/currenttime/get/timezone
        [HttpGet]
        public IActionResult GetConvertedTime(string timeZone)
        {
            var utcTime = DateTime.UtcNow;
            var serverTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, timeZone);
            var ip = this.HttpContext.Connection.RemoteIpAddress.ToString();



            var returnVal = new CurrentTimeQuery
            {
                UTCTime = utcTime,
                ClientIp = ip,
                Time = serverTime,
                TimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZone).DisplayName
            };

            using (var db = new ClockworkContext())
            {
                db.CurrentTimeQueries.Add(returnVal);
                var count = db.SaveChanges();
                Console.WriteLine("{0} records saved to database", count);

                Console.WriteLine();
                foreach (var CurrentTimeQuery in db.CurrentTimeQueries)
                {
                    Console.WriteLine(" - {0}", CurrentTimeQuery.UTCTime);
                }
            }

            return Ok(returnVal);
        }

        [Route("api/[controller]/getall")]
        // GET api/currenttime/getall
        [HttpGet]
        public IActionResult GetAll()
        {
            var returnVal = new List<CurrentTimeQuery>();

            using (var db = new ClockworkContext())
            {
                foreach (var CurrentTimeQuery in db.CurrentTimeQueries)
                {
                    returnVal.Add(CurrentTimeQuery);
                }
            }

            return Ok(returnVal);
        }
    }
}
