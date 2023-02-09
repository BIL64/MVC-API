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
    public class GamesController : ControllerBase
    {
        private readonly IUoW uow; // Viktigt att det är interfacet här!

        public GamesController(IUoW uow)
        {
            this.uow = uow;
        }

        // GET: api/Games
        [HttpGet]
        public Task<IEnumerable<Game>> GetGame()
        {
          if (uow.GameRepository == null)
          {
              throw new NotImplementedException();
          }
            return uow.GameRepository.GetAll(); // ToList()
        }

        // GET: api/Games/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Game>> GetGame(int id)
        {
          if (uow.GameRepository == null)
          {
              return NotFound();
          }
            var game = await uow.GameRepository.Get(id); // FindAsync()

            if (game == null)
            {
                return NotFound();
            }

            return game;
        }

        // PUT: api/Games/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGame(int id, Game game)
        {
            if (id != game.Id)
            {
                return BadRequest();
            }

            //uow.Entry(game).State = EntityState.Modified;
            uow.GameRepository.Update(game);

            try
            {
                await uow.CompleteAsync(); // SaveChangesAsync()
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await GameExists(id))
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

        // POST: api/Games
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Game>> PostGame(Game game)
        {
          if (uow.GameRepository == null)
          {
              return Problem("Entity set 'LmsApiContext.Game'  is null.");
          }
            uow.GameRepository.Add(game); // Add()
            await uow.CompleteAsync(); // SaveChangesAsync()

            return CreatedAtAction("GetGame", new { id = game.Id }, game);
        }

        // DELETE: api/Games/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGame(int id)
        {
            if (uow.GameRepository == null)
            {
                return NotFound();
            }
            var game = await uow.GameRepository.Get(id);  // FindAsync()
            if (game == null)
            {
                return NotFound();
            }

            uow.GameRepository.Remove(game); // Remove()
            await uow.CompleteAsync(); // SaveChangesAsync()

            return NoContent();
        }

        private async Task<bool> GameExists(int id)
        {
            return await uow.GameRepository.AnyAsync(id); // Any()
        }
    }
}
