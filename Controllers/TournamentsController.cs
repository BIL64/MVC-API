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
using Bogus.DataSets;

namespace Lms.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TournamentsController : ControllerBase
    {
        private readonly IUoW uow; // Viktigt att det är interfacet här!

        public TournamentsController(IUoW uow)
        {
            this.uow = uow;
        }

        // GET: api/Tournaments
        [HttpGet]
        public async Task<IEnumerable<Tournament>> GetTournament()
        {
          if (uow.TournamentRepository == null)
          {
              throw new NotImplementedException();
          }
            return await uow.TournamentRepository.GetAll(); // ToList()
        }

        // GET: api/Tournaments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Tournament>> GetTournament(int id)
        {
          if (uow.TournamentRepository == null)
          {
              return NotFound();
          }
            var tournament = await uow.TournamentRepository.Get(id); // FindAsync()

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

            //uow.Entry(tournament).State = EntityState.Modified;
            uow.TournamentRepository.Update(tournament);

            try
            {
                await uow.CompleteAsync(); // SaveChangesAsync()
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await TournamentExists(id))
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
          if (uow.TournamentRepository == null)
          {
              return Problem("Entity set 'LmsApiContext.Tournament'  is null.");
          }
            uow.TournamentRepository.Add(tournament); // Add()
            await uow.CompleteAsync(); // SaveChangesAsync()

            return CreatedAtAction("GetTournament", new { id = tournament.Id }, tournament);
        }

        // DELETE: api/Tournaments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTournament(int id)
        {
            if (uow.TournamentRepository == null)
            {
                return NotFound();
            }
            var tournament = await uow.TournamentRepository.Get(id);  // FindAsync()
            if (tournament == null)
            {
                return NotFound();
            }

            uow.TournamentRepository.Remove(tournament); // Remove()
            await uow.CompleteAsync(); // SaveChangesAsync()

            return NoContent();
        }

        private async Task<bool> TournamentExists(int id)
        {
            return await uow.TournamentRepository.AnyAsync(id); // Any()
        }
    }
}
