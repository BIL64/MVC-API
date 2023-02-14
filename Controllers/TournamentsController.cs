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
using AutoMapper;
using Lms.Core.Dto;
using System.Linq.Expressions;
using Microsoft.AspNetCore.JsonPatch;
using System.Xml.XPath;

namespace Lms.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TournamentsController : ControllerBase
    {
        private readonly IUoW uow; // Viktigt att det är interfacet här!
        private readonly IMapper mapper;

        public TournamentsController(IUoW uow, IMapper mapper)
        {
            this.uow = uow;
            this.mapper = mapper;
        }

        // GET: api/Tournament
        [HttpGet]
        public async Task<IEnumerable<TournamentDto>> GetTournament()
        {
          if (uow.TournamentRepository == null)
          {
              throw new NotImplementedException();
          }
            var expression = await uow.TournamentRepository.GetAll(); // ToList()
            var dto = mapper.Map<IEnumerable<TournamentDto>>(expression);

            return dto;
        }
        // GET = Skriver ut alla poster.
        // Postman:
        // Applicera URL från Swagger.
        // Välj GET
        // SEND.

        // GET: api/Tournaments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TournamentDto>> GetTournament(int id)
        {
          if (uow.TournamentRepository == null)
          {
              return NotFound();
          }
            var tournamentDto = await uow.TournamentRepository.Get(id); // FindAsync()
            var dto = mapper.Map<TournamentDto>(tournamentDto);

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

        // PUT: api/Tournaments/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTournament(int id, TournamentDto tournamentDto)
        {
            if (id != tournamentDto.Id)
            {
                return BadRequest();
            }

            //uow.Entry(tournamentDto).State = EntityState.Modified;
            var tourmapped = mapper.Map<Tournament>(tournamentDto); // Konverteringsmappning.
            uow.TournamentRepository.Update(tourmapped);

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
        // PUT = För att genomföra en redigering.
        // Postman:
        // Applicera URL från Swagger.
        // Kopiera en post med ALLA data ink. id och klistra in den i Bodyn.
        // Exempel:
        // {
        //  "id": 1,
        //  "title": "Rubrik",
        //  "startDate": "2023-01-01 12:30",
        //  "endDate": "2023-02-01 12:30"
        // }
        // Ändra på ett eller flera ställen.
        // Lägg även till ett id i urlen.
        // Välj PUT
        // SEND.

        // PATCH (extra)
        //[HttpPut]
        [HttpPatch("{tournamentId}")]
        public async Task<IActionResult> PatchTournament(int tournamentId, JsonPatchDocument<TournamentDto> patchDoc)
        {
            var tournament = await uow.TournamentRepository.Get(tournamentId); // FindAsync()

            if (tournament == null)
            {
                return NotFound();
            }

            var tournamentDto = mapper.Map<TournamentDto>(tournament); // Konverteringsmappning.
            patchDoc.ApplyTo(tournamentDto, ModelState);

            if (!ModelState.IsValid) return BadRequest(ModelState);
            mapper.Map(tournamentDto, tournament);
            
            try
            {
                await uow.CompleteAsync(); // SaveChangesAsync()
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await TournamentExists(tournamentId))
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

        // POST: api/Tournaments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Tournament>> PostTournament(TournamentDto tournamentDto)
        {
          if (uow.TournamentRepository == null)
          {
              return Problem("Entity set 'LmsApiContext.Tournament'  is null.");
          }
            var tourmapped = mapper.Map<Tournament>(tournamentDto); // Konverteringsmappning.
            uow.TournamentRepository.Add(tourmapped); // Add()
            await uow.CompleteAsync(); // SaveChangesAsync()

            return CreatedAtAction("GetTournament", new { id = tournamentDto.Id }, tournamentDto);
        }
        // POST = Skapar en ny post.
        // Postman:
        // Applicera URL från Swagger.
        // Exempelvis kan man först kopiera en post för att sedan klistra in den i Bodyn.
        // Ange ej några id'n !!!
        // Exempel:
        // {
        //  "title": "En rubrik",
        //  "startDate": "2023-01-01 12:30",
        //  "endDate": "2023-02-01 12:30"
        // }
        // Välj POST
        // SEND.

        // DELETE: api/Tournaments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTournament(int id)
        {
            if (uow.TournamentRepository == null)
            {
                return NotFound();
            }
            var tournamentDto = await uow.TournamentRepository.Get(id);  // FindAsync()
            if (tournamentDto == null)
            {
                return NotFound();
            }

            uow.TournamentRepository.Remove(tournamentDto); // Remove()
            await uow.CompleteAsync(); // SaveChangesAsync()

            return NoContent();
        }
        // DELETE = Raderar en post.
        // Postman:
        // Applicera URL från Swagger.
        // Ange rätt id i urlen för den post som ska raderas.
        // Välj DELETE
        // SEND.

        private async Task<bool> TournamentExists(int id)
        {
            return await uow.TournamentRepository.AnyAsync(id); // Any()
        }
    }
}
