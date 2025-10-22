using ConfigServiceAPI.Commons;
using ConfigServiceAPI.DTOs;
using ConfigServiceAPI.Repository;
using ConfigServiceAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace ConfigServiceAPI.Controllers
{
    [ApiController]
    [Route("api/ServiceConfigAPI")]
    public class ServiceConfigAPIController : ControllerBase
    {
        private readonly EnviromentRepository _enviromentRepository;
        private readonly VariablesRepository _variablesRepository;

        public ServiceConfigAPIController (EnviromentRepository enviromentRepository, VariablesRepository variablesRepository)
        {
            _enviromentRepository = enviromentRepository;
            _variablesRepository = variablesRepository;
        }
        
        //Health Check: Debe responder simplemente pong
        [HttpGet("status")]
        public async Task<IActionResult> Status()
        {
            return Ok("Pong");
        }

        //Crear un nuevo entorno.
        [HttpPost("enviroments")]
        public async Task<IActionResult> CreateEnviroment([FromBody] CreateEnviromentDTO enviromentDto)
        {
            var slugName = SlugNameGenerator.GenerateSlug(enviromentDto.name);

            if (await _enviromentRepository.ExistsEnviromentNameAsync(slugName))
            {
               return Conflict($"Ya existe un entorno con el nombre '{slugName}'.");
            }

            var enviroment = new Enviroments
            {
                Id = Guid.NewGuid(),
                name = slugName, 
                description = enviromentDto.description, 
                createdAt = DateTimeOffset.UtcNow, 
                updatedAt = DateTimeOffset.UtcNow
            };
            await _enviromentRepository.CreateEnviromentAsync(enviroment);

            var responseDto = new EnviromentDTO
            {
                name = enviroment.name,
                description = enviroment.description,
                createdAt = enviroment.createdAt,
                updatedAt = enviroment.updatedAt
            };

            return Created($"/enviroments/{slugName}", responseDto);
        }

        //Listado de todas los entornos (debe incluir parámetros de paginación).
        [HttpGet("enviroments")]
        public async Task<IActionResult> GetEnviroments([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize > 100 ? 100 : pageSize;

            var (items, totalCount) = await _enviromentRepository.GetAllEnviromentsAsync(page, pageSize);

            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            string baseUrl = $"{Request.Path}?page=";
            string next = page < totalPages ? $"{baseUrl}{page + 1}" : null;
            string previous = page > 1 ? $"{baseUrl}{page - 1}" : null;

            var results = items.Select(e => new
            {
                name = e.name,
                description = e.description,
                created_at = e.createdAt,
                updated_at = e.updatedAt
            });
            var response = new
            {
                count = totalCount,
                next,
                previous,
                results
            };

            return Ok(response);
        }

        //Obtener los detalles de un entorno.
        [HttpGet("enviroments/{env_name}")]
        public async Task<IActionResult> GetEnviroment([FromRoute] string env_name)
        {
            var slugName = SlugNameGenerator.GenerateSlug(env_name);

            var enviroment = await _enviromentRepository.GetEnviromentAsync(slugName);


            if (enviroment == null)
            {
                return NotFound($"No se encontró un entorno con el nombre '{env_name}'.");
            }


            var responseDto = new EnviromentDTO
            {
                name = enviroment.name,
                description = enviroment.description,
                createdAt = enviroment.createdAt,
                updatedAt = enviroment.updatedAt
            };

            return Ok(responseDto);
        }

        //Actualizar un entorno existente.
        [HttpPut("enviroments/{env_name}")]
        public async Task<IActionResult> UpdateEnviroment([FromRoute] string env_name, [FromBody] UpdateEnviromentDTO enviromentDto)
        {

            var slugNameEnviromentUpdated = SlugNameGenerator.GenerateSlug(env_name);


            var enviromentUpdated = await _enviromentRepository.GetEnviromentAsync(slugNameEnviromentUpdated);

            if (enviromentUpdated == null)
            {
                return NotFound($"No se encontró un entorno con el nombre '{env_name}'.");
            }

            var slugName = SlugNameGenerator.GenerateSlug(enviromentDto.name);
            Console.WriteLine(slugName);
            var enviroment = new Enviroments
            {
                name = slugName,
                description = enviromentDto.description,
                createdAt = enviromentUpdated.createdAt,
                updatedAt = DateTimeOffset.UtcNow
            };

            await _enviromentRepository.UpdateEnviromentAsync(slugNameEnviromentUpdated, enviroment);

            var responseDto = new EnviromentDTO
            {
                name = enviroment.name,
                description = enviroment.description,
                createdAt = enviroment.createdAt,
                updatedAt = enviroment.updatedAt
            };

            return Ok(responseDto);
        }

        //Actualizar parcialmente un entorno.
        [HttpPatch("enviroments/{env_name}")]
        public async Task<IActionResult> PatchEnviroment([FromRoute] string env_name, [FromBody] PatchEnviromentDTO dto)
        {
            var enviroment = await _enviromentRepository.GetEnviromentAsync(SlugNameGenerator.GenerateSlug(env_name));
            if (enviroment == null)
                return NotFound($"No se encontró un entorno con el nombre '{env_name}'.");

            bool modified = false;

            if (!string.IsNullOrWhiteSpace(dto.description) && dto.description != enviroment.description)
            {
                enviroment.description = dto.description;
                modified = true;
            }

            if (!string.IsNullOrWhiteSpace(dto.name) && dto.name != enviroment.name)
            {
                var newSlug = SlugNameGenerator.GenerateSlug(dto.name);
                if (await _enviromentRepository.ExistsEnviromentNameAsync(newSlug))
                    return Conflict($"Ya existe un entorno con el nombre '{newSlug}'.");

                enviroment.name = newSlug;
                modified = true;
            }

            if (!modified)
                return BadRequest("No se proporcionaron cambios válidos.");

            enviroment.updatedAt = DateTimeOffset.UtcNow;
            await _enviromentRepository.SaveChangesAsync();

            return NoContent();
        }

        //Eliminar una entorno.
        [HttpDelete("enviroments/{env_name}")]
        public async Task<IActionResult> DeleteEnviroment([FromRoute] string env_name)
        {
            var slugName = SlugNameGenerator.GenerateSlug(env_name);

            var enviroment = await _enviromentRepository.GetEnviromentAsync(slugName);


            if (enviroment == null)
            {
                return NotFound($"No se encontró un entorno con el nombre '{env_name}'.");
            }

            await _enviromentRepository.DeleteEnviromentAsync(slugName);

            return NoContent();
        }

        //Crear una nueva variable para un entorno.
        [HttpPost("enviroments/{env_name}/variables")]
        public async Task<IActionResult> CreateVariables([FromRoute] string env_name, [FromBody] CreateVariablesDTO variablesDTO)
        {
            var slugName = SlugNameGenerator.GenerateSlug(env_name);

            var enviroment = await _enviromentRepository.GetEnviromentAsync(slugName);


            if (enviroment == null)
            {
                return NotFound($"No se encontró un entorno con el nombre '{env_name}'.");
            }

            var idEnviroment = await _enviromentRepository.GetEnviromentIdByNameAsync(slugName);

            var slugNameVariable = SlugNameGenerator.GenerateSlug(variablesDTO.name);

            if (await _variablesRepository.ExistsVariableNameAsync(slugNameVariable))
            {
                return Conflict($"Ya existe una variable con el nombre '{slugNameVariable}' en el entorno '{env_name}'.");
            }


            var variable = new Variables
            {
                Id = Guid.NewGuid(),
                name = slugNameVariable,
                value = variablesDTO.value,
                description = variablesDTO.description,
                createdAt = DateTimeOffset.UtcNow,
                updatedAt = DateTimeOffset.UtcNow,
                isSensitive = variablesDTO.is_sensitive,
                EnviromentId = idEnviroment
                
            };
            await _variablesRepository.CreateVariablesAsync(variable);

            var responseDto = new VariableDTO
            {
                name = variable.name,
                description = variable.description,
                value = variable.value,
                createdAt = variable.createdAt,
                updatedAt = variable.updatedAt,
                isSensitive = variable.isSensitive
            };

            return Created($"/enviroments/{env_name}/variables/{variablesDTO.name}", responseDto);
        }

        //Listado de todas las variables de un entorno (debe incluir parámetros de paginación).
        [HttpGet("enviroments/{env_name}/variables")]
        public async Task<IActionResult> GetVariablesByEnviroment([FromRoute] string env_name, [FromQuery] int page = 1,
           [FromQuery] int pageSize = 10)
        {
            var slug = SlugNameGenerator.GenerateSlug(env_name);

            var env = await _enviromentRepository.GetEnviromentAsync(slug);
            if (env == null)
                return NotFound($"No se encontró el entorno '{env_name}'.");

            var (variables, totalItems) = await _variablesRepository.GetVariablesByEnviromentAsync(env.Id, page, pageSize);

            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var next = page < totalPages
                ? $"/enviroments/{env_name}/variables?page={page + 1}&pageSize={pageSize}"
                : null;

            var previous = page > 1
                ? $"/enviroments/{env_name}/variables?page={page - 1}&pageSize={pageSize}"
                : null;

            var results = variables.Select(v => new
            {
                name = v.name,
                value = v.value,
                description = v.description,
                is_sensitive = v.isSensitive,
                created_at = v.createdAt,
                updated_at = v.updatedAt
            });

            var response = new
            {
                enviroment = new
                {
                    name = env.name,
                    description = env.description,
                    created_at = env.createdAt,
                    updated_at = env.updatedAt
                },
                count = totalItems,
                next,
                previous,
                results
            };

            return Ok(response);
        }

        //Obtener los detalles de una variable de un entorno.
        [HttpGet("enviroments/{env_name}/variables/{var_name}")]
        public async Task<IActionResult> GetVariable([FromRoute] string env_name, [FromRoute] string var_name)
        {
            var slugName = SlugNameGenerator.GenerateSlug(env_name);

            var enviromentId = await _enviromentRepository.GetEnviromentIdByNameAsync(slugName);


            if (enviromentId == null)
            {
                return NotFound($"No se encontró un entorno con el nombre '{env_name}'.");
            }

            var slugNameVariable = SlugNameGenerator.GenerateSlug(var_name);

            var variable = await _variablesRepository.GetVariableAsync(slugNameVariable, enviromentId);

            if (variable == null)
            {
                    return NotFound($"No se encontró una variable con el nombre '{var_name}' en el entorno '{env_name}'.");
            }

            var responseDto = new VariableDTO()
            {
                name = variable.name,
                description = variable.description,
                value = variable.value,
                createdAt = variable.createdAt,
                updatedAt = variable.updatedAt,
                isSensitive = variable.isSensitive
            };

            return Ok(responseDto);
        }

        //Actualizar una variable existente de un entorno.
        [HttpPut("enviroments/{env_name}/variables/{var_name}")]
        public async Task<IActionResult> UpdateVariable([FromRoute] string env_name, [FromRoute] string var_name, [FromBody] CreateVariablesDTO variableDto)
        {

            var slugNameEnviroment = SlugNameGenerator.GenerateSlug(env_name);


            var enviromentId = await _enviromentRepository.GetEnviromentIdByNameAsync(slugNameEnviroment);

            if (enviromentId == null)
            {
                return NotFound($"No se encontró un entorno con el nombre '{env_name}'.");
            }

            var slugNameVariableUpdated = SlugNameGenerator.GenerateSlug(var_name);

            var variable = await _variablesRepository.GetVariableAsync(slugNameVariableUpdated, enviromentId);

            if (variable == null)
            {
                return NotFound($"No se encontró una variable con el nombre '{var_name}' en el entorno '{env_name}'.");
            }

            var newSlugNameVariable = SlugNameGenerator.GenerateSlug(variableDto.name);

            var variableUpdated = new Variables
            {
                name = newSlugNameVariable,
                description = variableDto.description,
                value = variableDto.value,
                createdAt = variable.createdAt,
                updatedAt = DateTimeOffset.UtcNow,
                isSensitive = variableDto.is_sensitive
            };

            await _variablesRepository.UpdateVariableAsync(slugNameEnviroment, slugNameVariableUpdated, variableUpdated);

            var responseDto = new VariableDTO
            {
                name = variableUpdated.name,
                description = variableUpdated.description,
                value = variableUpdated.value,
                createdAt = variableUpdated.createdAt,
                updatedAt = variableUpdated.updatedAt,
                isSensitive = variableUpdated.isSensitive
            };

            return Ok(responseDto);
        }

        //Actualizar parcialmente una variable existente de un entorno.
        [HttpPatch("enviroments/{env_name}/variables/{var_name}")]
        public async Task<IActionResult> PatchVariable([FromRoute] string env_name, [FromRoute] string var_name, [FromBody] PatchVariableDTO variableDto)
        {
            var enviromentId = await _enviromentRepository.GetEnviromentIdByNameAsync(SlugNameGenerator.GenerateSlug(env_name));

            if (enviromentId == null)
                return NotFound($"No se encontró un entorno con el nombre '{env_name}'.");

            var slugNameVariableUpdated = SlugNameGenerator.GenerateSlug(var_name);

            var variable = await _variablesRepository.GetVariableAsync(slugNameVariableUpdated, enviromentId);

            if (variable == null)
            {
                return NotFound($"No se encontró una variable con el nombre '{var_name}' en el entorno '{env_name}'.");
            }

            bool modified = false;

            if (!string.IsNullOrWhiteSpace(variableDto.description) && variableDto.description != variable.description)
            {
                variable.description = variableDto.description;
                modified = true;
            }

            if (!string.IsNullOrWhiteSpace(variableDto.value) && variableDto.value != variable.value)
            {
                variable.value = variableDto.value;
                modified = true;
            }

            if (!string.IsNullOrWhiteSpace(variableDto.name) && variableDto.name != variable.name)
            {
                var slugNameVariable = SlugNameGenerator.GenerateSlug(variableDto.name);
                if (await _variablesRepository.ExistsVariableNameAsync(slugNameVariable))
                    return Conflict($"Ya existe una variable con el nombre '{slugNameVariable}' en el entorno '{env_name}'.");

                variable.name = slugNameVariable;
                modified = true;
            }


            if (variableDto.isSensitive != variable.isSensitive)
            {
                variable.isSensitive = variableDto.isSensitive;
                modified = true;
            }

            if (!modified)
                return BadRequest("No se proporcionaron cambios válidos.");

            variable.updatedAt = DateTimeOffset.UtcNow;
            await _variablesRepository.SaveChangesAsync();

            return NoContent();
        }

        //Eliminar una variable existente de un entorno.
        [HttpDelete("enviroments/{env_name}/variables/{var_name}")]
        public async Task<IActionResult> DeleteVariable([FromRoute] string env_name, [FromRoute] string var_name)
        {
            var slugNameEnviroment = SlugNameGenerator.GenerateSlug(env_name);

            var idEnviroment = await _enviromentRepository.GetEnviromentIdByNameAsync(slugNameEnviroment);


            if (idEnviroment == null)
            {
                return NotFound($"No se encontró un entorno con el nombre '{env_name}'.");
            }

            var slugNameVariable = SlugNameGenerator.GenerateSlug(var_name);

            var variable = await _variablesRepository.GetVariableAsync(slugNameVariable, idEnviroment);

            if (variable == null)
            {
                return NotFound($"No se encontró una variable con el nombre '{var_name}' en el entorno '{env_name}'.");
            }

            await _variablesRepository.DeleteVariableAsync(slugNameEnviroment,slugNameVariable);

            return NoContent();
        }

        //Consumo Masivo: Devuelve el JSON de configuración para un entorno específico 
        [HttpGet("enviroments/{env_name}.json")]
        public async Task<IActionResult> GetEnviromentVariablenJson([FromRoute] string env_name)
        {
            var slugNameEnviroment = SlugNameGenerator.GenerateSlug(env_name);

            var env = await _enviromentRepository.GetEnviromentAsync(slugNameEnviroment);

            if (env == null)
                return NotFound($"No se encontró el entorno '{env_name}'.");

            var json = await _variablesRepository.GenerateMassConsuptiomJson(env.Id);

            return Ok(json);
        }


    }
}
