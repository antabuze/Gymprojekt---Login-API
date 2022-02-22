using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Grupparbete___API_Login;
using NLog;

namespace Grupparbete___API_Login.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserCheckinsController : ControllerBase
    {
        Logger log = LogManager.GetCurrentClassLogger();
        private readonly AdminContext _context;

        public UserCheckinsController(AdminContext context)
        {
            _context = context;
        }

        // GET: api/UserCheckins
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserCheckin>>> GetUserCheckins()
        {
            return await _context.UserCheckins.ToListAsync();
        }

        // GET: api/UserCheckins
        [HttpGet("{id}")]
        public async Task<ActionResult<UserCheckin>> GetUserCheckin(int id)
        {
            var userCheckin = await _context.UserCheckins.FindAsync(id);

            if (userCheckin == null)
            {
                log.Error("Statuscode: NotFound: User checkin " + id + " could not be found.");
                return NotFound();
            }
            log.Info("User checkin: " + id + " was retrieved sucessfully.");
            return userCheckin;
        }

        [HttpGet]
        [Route("GetUserCheckinsInterval")]
        public async Task<ActionResult<IEnumerable<UserCheckin>>> GetUserCheckinsInterval(int userId, string startDate, string endDate)
        {
            if (_context.UserCheckins.Where(e => e.EmployeeId == userId) == null)
            {
                log.Error("Statuscode: BadRequest: " + userId + " does not exist");
            }
            var checkinInterval = _context.UserCheckins.Where(e => e.EmployeeId == userId && e.StartTime >= Convert.ToDateTime(startDate) && e.StartTime <= Convert.ToDateTime(endDate).AddDays(1));
            log.Info("Checkin interval was sucessfully retrieved.");
           return await checkinInterval.ToListAsync();
        }

        //Borttagen funktion. Logiken fungerar men är bättre lämpad för att hanteras på klientsidan.
        //[HttpGet]
        //[Route("GetUserWorkedHours")]
        //public async Task<ActionResult<IEnumerable<UserCheckin>>> GetUserWorkedHours(int employeeId, string startDate, string endDate)
        //{

        //    var checkinInterval = _context.UserCheckins.Where(e => e.EmployeeId == employeeId && e.StartTime >= Convert.ToDateTime(startDate) && e.StartTime <= Convert.ToDateTime(endDate).AddDays(1));
        //    List<UserCheckin> userCheckinList = new List<UserCheckin>();
        //    userCheckinList = checkinInterval.ToList();
        //    var totalWorkHours = 0;
        //    foreach (var item in userCheckinList)
        //    {
        //        var buffer = item.EndTime.Value - item.StartTime.Value;

        //        totalWorkHours += buffer.Hours;
        //    }
        //    return Ok(totalWorkHours);
        //}

        
        
        
        // PUT: api/UserCheckins/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserCheckin(int id, UserCheckin userCheckin)
        {
            if (id != userCheckin.Id)
            {
                log.Error("Statuscode: BadRequest: " + id + " did not match the checkins Id.");
                return BadRequest();
            }

            _context.Entry(userCheckin).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserCheckinExists(id))
                {
                    log.Error("Statuscode: NotFound: " + id + " does not exist.");
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            log.Info("User checkin was edited sucessfully.");
            return NoContent();
        }

        // POST: api/UserCheckins
        [HttpPost]
        public async Task<ActionResult<UserCheckin>> PostUserCheckin(UserCheckin userCheckin)
        {
            _context.UserCheckins.Add(userCheckin);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetUserCheckin", new { id = userCheckin.Id }, userCheckin);
        }

        // POST: Check-In
        [HttpPost]
        [Route("CheckIn")]
        public async Task<ActionResult<UserCheckin>> CheckIn([FromBody]int userId)
        {
            if (_context.UserCheckins.Where(e => e.EmployeeId == userId && e.EndTime == null).FirstOrDefault() == null) {
                UserCheckin checkin = new UserCheckin();
                checkin.EmployeeId = userId;
                checkin.StartTime = DateTime.Now;
                checkin.EndTime = null;
                _context.UserCheckins.Add(checkin);
                await _context.SaveChangesAsync();
                log.Info("User: " + userId + " checked in.");
                return CreatedAtAction("GetUserCheckin", new { id = checkin.Id }, checkin);
            }
            log.Error("Statuscode: BadRequest: " + "User: " + userId + " could not be checked in.");
            return BadRequest();       
        }

        // PUT: Check-out
        [HttpPut]
        [Route("CheckOut")]
        public async Task<ActionResult<UserCheckin>> CheckOut([FromBody]int userId)
        {
            var currentCheckin = _context.UserCheckins.Where(e => e.EmployeeId == userId && e.EndTime == null).FirstOrDefault<UserCheckin>();
            if (currentCheckin != null)
            {
                currentCheckin.EndTime = DateTime.Now;
                await _context.SaveChangesAsync();
                log.Info("User: " + userId + " checked out.");
                return Ok();
            }
            log.Error("Statuscode: BadRequest: " + "User: " + userId + " could not be checked out.");
            return BadRequest();
        }

        // DELETE: api/UserCheckins/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserCheckin(int id)
        {
            var userCheckin = await _context.UserCheckins.FindAsync(id);
            if (userCheckin == null)
            {
                log.Error("Statuscode: NotFound: " + id + " could not be found.");
                return NotFound();
            }

            _context.UserCheckins.Remove(userCheckin);
            await _context.SaveChangesAsync();
            log.Info("User checkin: " + id + " was deleted sucessfully.");
            return NoContent();
        }

        private bool UserCheckinExists(int id)
        {
            return _context.UserCheckins.Any(e => e.Id == id);
        }
    }
}
