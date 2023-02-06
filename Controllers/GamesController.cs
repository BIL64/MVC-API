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
        private readonly LmsApiContext uow;

        public GamesController(LmsApiContext uow)
        {
            this.uow = uow;
        }

        // GET: api/Games
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Game>>> GetGame()
        {
          if (uow.Game == null)
          {
              return NotFound();
          }
            return await uow.Game.ToListAsync();
        }

        // GET: api/Games/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Game>> GetGame(int id)
        {
          if (uow.Game == null)
          {
              return NotFound();
          }
            var game = await uow.Game.FindAsync(id);

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

            uow.Entry(game).State = EntityState.Modified;

            try
            {
                await uow.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GameExists(id))
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
          if (uow.Game == null)
          {
              return Problem("Entity set 'LmsApiContext.Game'  is null.");
          }
            uow.Game.Add(game);
            await uow.SaveChangesAsync();

            return CreatedAtAction("GetGame", new { id = game.Id }, game);
        }

        // DELETE: api/Games/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGame(int id)
        {
            if (uow.Game == null)
            {
                return NotFound();
            }
            var game = await uow.Game.FindAsync(id);
            if (game == null)
            {
                return NotFound();
            }

            uow.Game.Remove(game);
            await uow.SaveChangesAsync();

            return NoContent();
        }

        private bool GameExists(int id)
        {
            return (uow.Game?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
