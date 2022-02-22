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
    public class AdminsController : ControllerBase
    {
        Logger log = LogManager.GetCurrentClassLogger();
        private readonly AdminContext _context;

        public AdminsController(AdminContext context)
        {
            _context = context;
        }

        // GET: api/Admins
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Admin>>> GetAdmins()
        {
            return await _context.Admins.ToListAsync();
        }

        // GET: api/Admins/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Admin>> GetAdmin(int id)
        {
            var admin = await _context.Admins.FindAsync(id);

            if (admin == null)
            {
                log.Error("Statuscode: NotFound: " + id + " was not found.");
                return NotFound();
            }
            log.Info("Admin: " + id + " was retrieved sucessfully.");
            return admin;
        }

        // PUT: api/Admins/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAdmin(int id, Admin admin)
        {
            if (id != admin.Id)
            {
                log.Error("Statuscode: BadRequest: " + id + " did not match the admins Id.");
                return BadRequest();
            }

            _context.Entry(admin).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdminExists(id))
                {
                    log.Error("Statuscode: NotFound: " + id + " does not exist.");
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            log.Info("Admin was edited sucessfully.");
            return NoContent();
        }

        // POST: api/Admins        
        [HttpPost]
        [Route("PostAdmin")]
        public async Task<ActionResult<Admin>> PostAdmin(Admin admin)
        {
             _context.Admins.Add(admin);
             await _context.SaveChangesAsync();
             return CreatedAtAction("GetAdmin", new { id = admin.Id }, admin);
        }

        //POST: Login
        [HttpPost]
        [Route("VerifyAdminLogin")]
        public async Task<ActionResult<Admin>> VerifyAdminLogin(Admin admin)
        {
            if(_context.Admins.Where(e => e.Email == admin.Email && e.Password == admin.Password && e.RoleId.Contains(admin.RoleId)).FirstOrDefault() != null)
            {
                log.Info("Admin: " + admin.Email.ToString() + " has logged in.");
                return Ok(true);
            }
            log.Error("Admin: " + admin.Email.ToString() + " failed to log in.");
            return Ok(false);
        }

        // DELETE: api/Admins/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdmin(int id)
        {
            var admin = await _context.Admins.FindAsync(id);
            if (admin == null)
            {
                log.Error("Statuscode: NotFound: " + id + " could not be found.");
                return NotFound();
            }

            _context.Admins.Remove(admin);
            await _context.SaveChangesAsync();
            log.Info("Admin: " + id + " was deleted sucessfully.");
            return NoContent();
        }

        private bool AdminExists(int id)
        {
            return _context.Admins.Any(e => e.Id == id);
        }
    }
}
