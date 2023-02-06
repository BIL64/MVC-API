using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lms.Data.Data;
using Lms.Core.Entities;
using Lms.Data.Data.Repositories;

namespace Lms.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TournamentsController : ControllerBase
    {
        private readonly LmsApiContext uow;

        public TournamentsController(LmsApiContext uow)
        {
            this.uow = uow;
        }

        // GET: api/Tournaments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tournament>>> GetTournament()
        {
          if (uow.Tournament == null)
          {
              return NotFound();
          }
            return await uow.Tournament.ToListAsync();
        }

        // GET: api/Tournaments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Tournament>> GetTournament(int id)
        {
          if (uow.Tournament == null)
          {
              return NotFound();
          }
            var tournament = await uow.Tournament.FindAsync(id);

            if (tournament == null)
            {
                return NotFound();
            }

            return tournament;
        }

        // PUT: api/Tournaments/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTournament(int id, Tournament tournament)
        {
            if (id != tournament.Id)
            {
                return BadRequest();
            }

            uow.Entry(tournament).State = EntityState.Modified;

            try
            {
                await uow.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TournamentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Tournaments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Tournament>> PostTournament(Tournament tournament)
        {
          if (uow.Tournament == null)
          {
              return Problem("Entity set 'LmsApiContext.Tournament'  is null.");
          }
            uow.Tournament.Add(tournament);
            await uow.SaveChangesAsync();

            return CreatedAtAction("GetTournament", new { id = tournament.Id }, tournament);
        }

        // DELETE: api/Tournaments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTournament(int id)
        {
            if (uow.Tournament == null)
            {
                return NotFound();
            }
            var tournament = await uow.Tournament.FindAsync(id);
            if (tournament == null)
            {
                return NotFound();
            }

            uow.Tournament.Remove(tournament);
            await uow.SaveChangesAsync();

            return NoContent();
        }

        private bool TournamentExists(int id)
        {
            return (uow.Tournament?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
