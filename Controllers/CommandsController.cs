using System.Collections.Generic;
using AutoMapper;
using Commander.Data;
using Commander.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using WebApi2.DTOS;

namespace Commander.Controllers
{
    [Route("api/command")]
    [ApiController]
    public class CommandController : ControllerBase
    {
        private readonly ICommanderRepo _repo;
        public IMapper _mapper { get; }

        public CommandController(ICommanderRepo repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        //GET api/command

        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDTOS>> GetAllCommands()
        {
            var commandItems = _repo.GetAppCommands();
            return Ok(_mapper.Map<IEnumerable<CommandReadDTOS>>(commandItems));
        }

        //GET api/command/{id}
        [HttpGet("{id}",Name = "GetCommandById")]
        public ActionResult<CommandReadDTOS> GetCommandById(int id)
        {

            var commandItem = _repo.GetCommandById(id);

            if(commandItem != null)
            {
                return Ok(_mapper.Map<CommandReadDTOS>(commandItem));

            }
            else
            {
                return NotFound();
            }
        }

        //Post api/com       
        [HttpPost]
        public ActionResult<CommandReadDTOS> CreateCommand(CommandCreateDTO commandCreateDTO)
        {
            var command = _mapper.Map<Command>(commandCreateDTO);
            _repo.CreateCommand(command);
            _repo.SaveChanges();
            var commandReadDTO = _mapper.Map<CommandReadDTOS>(command);
            return CreatedAtRoute(nameof(GetCommandById), new { Id = commandReadDTO.Id }, commandReadDTO);
            
            //return Ok(_mapper.Map<CommandReadDTOS>(commandReadDTO));
        }

        //PUT api/commands/{id}
        [HttpPut("{id}")]
        public ActionResult UpdateCommand(int id, CommandUpdateDTO commandUpdateDTO)
        {
            var commandFromRepo = _repo.GetCommandById(id);
            if(commandFromRepo == null)
            {
                return NotFound();
            }
            _mapper.Map(commandUpdateDTO, commandFromRepo);
            _repo.UpdateCommand(commandFromRepo);

            _repo.SaveChanges();

            return NoContent();
        }

        //Patch api/commands/{id}
        [HttpPatch("{id}")]
        public ActionResult PartialCommandUpdate(int id, JsonPatchDocument<CommandUpdateDTO> patchDoc)
        {
            var commandFromRepo = _repo.GetCommandById(id);
            if (commandFromRepo == null)
            {
                return NotFound();
            }

            var commandToPatch = _mapper.Map<CommandUpdateDTO>(commandFromRepo);
            patchDoc.ApplyTo(commandToPatch, ModelState);
            if (!TryValidateModel(commandToPatch))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(commandToPatch, commandFromRepo);

            _repo.UpdateCommand(commandFromRepo);
            _repo.SaveChanges();

            return NoContent();
        }

        //DELETE api/command/{id}
        [HttpDelete("{id}")]
        public ActionResult DeleteCommand(int id)
        {
            var commandFromRepo = _repo.GetCommandById(id);
            if (commandFromRepo == null)
            {
                return NotFound();
            }
            _repo.DeleteCommand(commandFromRepo);
            _repo.SaveChanges();

            return NoContent();
        }
    }
}
