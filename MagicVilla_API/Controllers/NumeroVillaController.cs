using AutoMapper;

using MagicVilla_API.Datos;
using MagicVilla_API.Modelos;
using MagicVilla_API.Modelos.Dto;
using MagicVilla_API.Repositorio.IRepositorio;

using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

using System.Net;

namespace MagicVilla_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class NumeroVillaController : ControllerBase
    {
        private readonly ILogger<NumeroVillaController>  _logger;
        private readonly IVillaRepositorio _villaRepo;
        private readonly INumeroVillaRepositorio _numeroRepo;
        private readonly IMapper _mapper;
        protected ApiResponse _response;

        public NumeroVillaController(ILogger<NumeroVillaController> logger, 
                                     INumeroVillaRepositorio numeroRepo, 
                                     IVillaRepositorio villaRepo, IMapper mapper)
        {
            _logger = logger;
            _numeroRepo = numeroRepo;
            _villaRepo = villaRepo;
            _mapper = mapper;
            _response = new ApiResponse();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ApiResponse>>> GetNumeroVillas()
        {
            try
            {
                _logger.LogInformation("Obtener los numeros de villas");
                IEnumerable<NumeroVilla> numeroVillaList = await _numeroRepo.ObtenerTodos();

                _response.Resultado = _mapper.Map<IEnumerable<NumeroVillaDto>>(numeroVillaList);
                _response.statusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsExistoso = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return BadRequest(_response);
        }

        [HttpGet("id:int", Name="GetNumeroVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse>> GetNumeroVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    _logger.LogError("Error al traer el numero de villa con id: " + id);
                    _response.IsExistoso = false;
                    _response.statusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                //var villa = VillaStore.villlaList.FirstOrDefault(v => v.Id == id);
                var numeroVilla = await _numeroRepo.Obtener(x => x.VillaNo == id);

                if (numeroVilla == null)
                {
                    _response.IsExistoso = false;
                    _response.statusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages = new List<string>() {"Id no exite"};
                    return BadRequest(_response);
                }

                _response.Resultado = _mapper.Map<VillaDto>(numeroVilla);
                _response.statusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsExistoso = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return BadRequest(_response);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> CrearNumeroVilla([FromBody] NumeroVillaCreateDto numeroVillaDto)
        {
            try
            {
                if (numeroVillaDto == null)
                    return BadRequest();

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (await _numeroRepo.Obtener(v => v.VillaNo == numeroVillaDto.VillaNo) != null)
                {
                    ModelState.AddModelError("NumeroVilla", "El numero de villa ya existe");
                    return BadRequest(ModelState);
                }

                if (await _villaRepo.Obtener(v => v.Id == numeroVillaDto.VillaId) == null)
                {
                    ModelState.AddModelError("IdVillaPadre", "El numero de id villa padre no existe");
                    return BadRequest(ModelState);
                }

                NumeroVilla modelo = _mapper.Map<NumeroVilla>(numeroVillaDto);

                modelo.FechaCreacion = DateTime.Now;
                modelo.FechaActualizacion = DateTime.Now;

                await _numeroRepo.Crear(modelo);
                _response.Resultado = modelo;
                _response.statusCode = HttpStatusCode.Created;

                return CreatedAtRoute("GetNumeroVilla", new { id = modelo.VillaNo }, _response);
            }
            catch (Exception ex)
            {
                _response.IsExistoso = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return BadRequest(_response);
        }

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteNumeroVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.IsExistoso = false;
                    _response.statusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var numeroVilla = await _numeroRepo.Obtener(v => v.VillaNo == id);

                if (numeroVilla == null)
                {
                    _response.IsExistoso = false;
                    _response.statusCode = HttpStatusCode.NotFound;
                    return BadRequest(_response);
                }

                await _numeroRepo.Remover(numeroVilla);

                _response.IsExistoso = true;
                _response.statusCode = HttpStatusCode.NoContent;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsExistoso = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return BadRequest(_response);
        }


        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateNumeroVilla(int id, [FromBody] NumeroVillaUpdateDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _response.IsExistoso = false;
                    _response.statusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                if (updateDto == null || updateDto.VillaNo != id)
                {
                    _response.IsExistoso = false;
                    _response.statusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                if (await _villaRepo.Obtener(v => v.Id == updateDto.VillaId) == null)
                {
                    ModelState.AddModelError("IdVillaPadre", "El numero de id villa padre no existe");
                    return BadRequest(ModelState);
                }

                NumeroVilla modelo = _mapper.Map<NumeroVilla>(updateDto);

                await _numeroRepo.Actualizar(modelo);

                _response.IsExistoso = true;
                _response.statusCode = HttpStatusCode.NoContent;

                return Ok(_response);

            }
            catch (Exception ex)
            {
                _response.IsExistoso = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return BadRequest(_response);
        }

    }
}
