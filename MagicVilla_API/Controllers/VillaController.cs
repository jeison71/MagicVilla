﻿using AutoMapper;

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

    public class VillaController : ControllerBase
    {
        private readonly ILogger<VillaController>   _logger;
        private readonly IVillaRepositorio _villaRepo;
        private readonly IMapper _mapper;
        protected ApiResponse _response;

        public VillaController(ILogger<VillaController> logger, IVillaRepositorio villaRepo, IMapper mapper)
        {
            _logger = logger;
            _villaRepo = villaRepo;
            _mapper = mapper;
            _response = new ApiResponse();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ApiResponse>>> GetVillas()
        {
            try
            {
                _logger.LogInformation("Obtener las villas");
                IEnumerable<Villa> villaList = await _villaRepo.ObtenerTodos();

                _response.Resultado = _mapper.Map<IEnumerable<VillaDto>>(villaList);
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

        [HttpGet("id:int", Name="GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse>> GetVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    _logger.LogError("Error al traer la villa con id: " + id);
                    _response.IsExistoso = false;
                    _response.statusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                //var villa = VillaStore.villlaList.FirstOrDefault(v => v.Id == id);
                var villa = await _villaRepo.Obtener(x => x.Id == id);

                if (villa == null)
                {
                    _response.IsExistoso = false;
                    _response.statusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages = new List<string>() {"Id no exite"};
                    return BadRequest(_response);
                }

                _response.Resultado = _mapper.Map<VillaDto>(villa);
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
        public async Task<ActionResult<ApiResponse>> CrearVilla([FromBody] VillaDto villaDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (await _villaRepo.Obtener(v => v.Nombre.ToLower() == villaDto.Nombre.ToLower()) != null)
                {
                    ModelState.AddModelError("NombreExiste", "El nombre de villa ya existe");
                    return BadRequest(ModelState);
                }

                if (villaDto == null)
                    return BadRequest();

                if (villaDto.Id > 0)
                    return StatusCode(StatusCodes.Status500InternalServerError);

                Villa modelo = _mapper.Map<Villa>(villaDto);

                modelo.FechaCreacion = DateTime.Now;
                modelo.FechaActualizacion = DateTime.Now;

                await _villaRepo.Crear(modelo);
                _response.Resultado = modelo;
                _response.statusCode = HttpStatusCode.Created;

                return CreatedAtRoute("GetVilla", new { id = modelo.Id }, _response);
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
        public async Task<IActionResult> DeleteVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.IsExistoso = false;
                    _response.statusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var villa = await _villaRepo.Obtener(v => v.Id == id);

                if (villa == null)
                {
                    _response.IsExistoso = false;
                    _response.statusCode = HttpStatusCode.NotFound;
                    return BadRequest(_response);
                }

                await _villaRepo.Remover(villa);

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
        public async Task<IActionResult> UpdateVilla(int id, [FromBody] VillaDto villaDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _response.IsExistoso = false;
                    _response.statusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                if (villaDto == null || villaDto.Id != id)
                {
                    _response.IsExistoso = false;
                    _response.statusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                Villa modelo = _mapper.Map<Villa>(villaDto);

                await _villaRepo.Actualizar(modelo);

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

        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<VillaDto>> UpdatePartialVilla(int id, JsonPatchDocument<VillaDto> patchDto)
        {

            try
            {
                if (patchDto == null || id == 0)
                {
                    _response.IsExistoso = false;
                    _response.statusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var villa = await _villaRepo.Obtener(x => x.Id == id, tracked: false);

                if (villa == null)
                {
                    _response.IsExistoso = false;
                    _response.statusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                VillaDto villaDto = _mapper.Map<VillaDto>(villa);

                patchDto!.ApplyTo(villaDto, ModelState);

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                Villa modelo = _mapper.Map<Villa>(villaDto);

                await _villaRepo.Actualizar(modelo);

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
