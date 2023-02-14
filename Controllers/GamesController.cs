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
using AutoMapper;
using Lms.Core.DTOs;
using Lms.Core.Dto;
using Microsoft.AspNetCore.JsonPatch;

namespace Lms.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly IUoW uow; // Viktigt att det är interfacet här!
        private readonly IMapper mapper;

        public GamesController(IUoW uow, IMapper mapper)
        {
            this.uow = uow;
            this.mapper = mapper;
        }

        // GET: api/Games
        [HttpGet]
        public async Task<IEnumerable<GameDto>> GetGame()
        {
          if (uow.GameRepository == null)
          {
              throw new NotImplementedException();
          }
            var game = await uow.GameRepository.GetAll(); // ToList()
            var dto = mapper.Map<IEnumerable<GameDto>>(game);

            return dto;
        }
        // GET = Skriver ut alla poster.
        // Postman:
        // Applicera URL från Swagger.
        // Välj GET
        // SEND.

        // GET: api/Games/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GameDto>> GetGame(int id)
        {
          if (uow.GameRepository == null)
          {
              return NotFound();
          }
            var game = await uow.GameRepository.Get(id); // FindAsync()
            var dto = mapper.Map<GameDto>(game);

            if (dto == null)
            {
                return NotFound();
            }

            return dto;
        }
        // GET + id = Skriver ut en specifik post.
        // Postman:
        // Applicera URL från Swagger.
        // Lägg till ett /id.
        // Välj GET
        // SEND.

        // PUT: api/Games/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGame(int id, Game game) // Enklare uppbyggd äm för Tournaments.
        {
            if (id != game.Id)
            {
                return BadRequest();
            }

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
        // PUT = För att genomföra en redigering.
        // Postman:
        // Applicera URL från Swagger.
        // Kopiera en post med ALLA data ink. id och klistra in den i Bodyn.
        // Exempel:
        // {
        //  "id": 11,
        //  "title": "Ny rubrik",
        //  "timeInMinutes": 111,
        //  "TournamentId": 1
        // }
        // Ändra på ett eller flera ställen.
        // Lägg även till ett id i urlen.
        // Välj PUT
        // SEND.

        // PATCH (extra)
        [HttpPatch("{gameId}")]
        public async Task<IActionResult> PatchGame(int gameId, JsonPatchDocument<GameDto> patchDoc)
        {
            var game = await uow.GameRepository.Get(gameId); // FindAsync()

            if (game == null)
            {
                return NotFound();
            }

            var gameDto = mapper.Map<GameDto>(game); // Konverteringsmappning.
            patchDoc.ApplyTo(gameDto, ModelState);

            if (!ModelState.IsValid) return BadRequest(ModelState);
            mapper.Map(gameDto, game);

            try
            {
                await uow.CompleteAsync(); // SaveChangesAsync()
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await GameExists(gameId))
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
        // PATCH = För att endast göra en ändring på ett specifikt ställe.
        // Postman:
        // Applicera URL från Swagger.
        // Skriv ett eller flera patchkommandon...
        // Exempel för att ändra på en rad:
        //[
        // {
        //  "op": "remove", <-- typ av operation
        //  "path": "title", <-- det som ska patchas
        //  "value": "Gammal rubrik" <-- värdet
        // },
        // {
        //  "op": "add",
        //  "path": "title",
        //  "value": "Ny rubrik"
        // }
        //]
        // Lägg även till ett id i urlen.
        // Välj PATCH
        // SEND.

        // POST: api/Games
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Game>> PostGame(CreateGameDto gameDto) // CreateGameDto.
        {
          if (uow.GameRepository == null)
          {
              return Problem("Entity set 'LmsApiContext.Game'  is null.");
          }

          //Tournament exists? DB

          //ProblemDetailsFactory eller Sätta Modelstate.AddModelError DB

            var gamemapped = mapper.Map<Game>(gameDto); // Konverteringsmappar.
            uow.GameRepository.Add(gamemapped); // Add()
            await uow.CompleteAsync(); // SaveChangesAsync()

            return CreatedAtAction("GetGame", new { id = gamemapped.Id }, gameDto);
        }
        // POST = Skapar en ny post. CreateGameDto som saknar ett id måste användas här!
        // Postman:
        // Applicera URL från Swagger.
        // Exempelvis kan man först kopiera en post för att sedan klistra in den i Bodyn.
        // Ange foregin id (TournamentId).
        // Exempel:
        // {
        //  "title": "Ny rubrik",
        //  "timeInMinutes": 100,
        //  "TournamentId" : 1
        // }
        // Välj POST
        // SEND.

        // DELETE: api/Games/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGame(int id)
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

            uow.GameRepository.Remove(game); // Remove()
            await uow.CompleteAsync(); // SaveChangesAsync()

            return NoContent();
        }
        // DELETE = Raderar en post.
        // Postman:
        // Applicera URL från Swagger.
        // Ange rätt id i urlen för den post som ska raderas.
        // Välj DELETE
        // SEND.

        private async Task<bool> GameExists(int id)
        {
            return await uow.GameRepository.AnyAsync(id); // Any()
        }
    }
}
