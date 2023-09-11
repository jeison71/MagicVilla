using AutoMapper;

using MagicVilla_API.Datos;
using MagicVilla_API.Modelos;
using MagicVilla_API.Modelos.Dto;

using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class VillaController : ControllerBase
    {
        private readonly ILogger<VillaController>   _logger;
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        public VillaController(ILogger<VillaController> logger, ApplicationDbContext db, IMapper mapper)
        {
            _logger = logger;
            _db = db;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<VillaDto>>> GetVillas()
        {
            _logger.LogInformation("Obtener las villas");
            IEnumerable<Villa> villaList = await _db.Villas.ToListAsync();

            return Ok(_mapper.Map<IEnumerable<VillaDto>>(villaList));
        }

        [HttpGet("id:int", Name="GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VillaDto>> GetVilla(int id)
        {
            if (id == 0)
            {
                _logger.LogError("Error al traer la villa con id: " + id);
                return BadRequest();
            }

            //var villa = VillaStore.villlaList.FirstOrDefault(v => v.Id == id);
            var villa = await _db.Villas.FirstOrDefaultAsync(x => x.Id == id);

            if (villa == null)
                return NotFound();

            return Ok(_mapper.Map<VillaDto>(villa));

        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<VillaDto>> CrearVilla([FromBody] VillaDto villaDto)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _db.Villas.FirstOrDefaultAsync(v => v.Nombre.ToLower() == villaDto.Nombre.ToLower()) != null)
            {
                ModelState.AddModelError("NombreExiste", "El nombre de villa ya existe");
                return BadRequest(ModelState);
            }

            if (villaDto == null)
                return BadRequest();

            if (villaDto.Id > 0)
                return StatusCode(StatusCodes.Status500InternalServerError);

            Villa modelo = _mapper.Map<Villa>(villaDto);

            //Villa modelo = new()
            //{
            //    Nombre = villaDto.Nombre,
            //    Amenidad = villaDto.Amenidad,
            //    Detalle = villaDto.Detalle,
            //    ImagenUrl = villaDto.ImagenUrl,
            //    Ocupantes = villaDto.Ocupantes,
            //    MetrosCuadrados = villaDto.MetrosCuadrados,
            //    Tarifa = villaDto.Tarifa,
            //    FechaCreacion = DateTime.Now,
            //    FechaActualizacion = DateTime.Now,
            //};
            await _db.Villas.AddAsync(modelo);
            await _db.SaveChangesAsync();

            return CreatedAtRoute("GetVilla", new {id = modelo.Id}, modelo);
        }

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteVilla(int id)
        {
            if(id== 0) return BadRequest();

            var villa = await _db.Villas.FirstOrDefaultAsync(v => v.Id == id);
            if (villa == null) return NotFound();

            _db.Villas.Remove(villa);
            await _db.SaveChangesAsync();

            return NoContent();
        }


        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<VillaDto>> UpdateVilla(int id, [FromBody] VillaDto villaDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (villaDto == null || villaDto.Id != id)
                return BadRequest();


            var villa = await _db.Villas.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);

            if (villa == null)
                return NotFound();

            Villa modelo = _mapper.Map<Villa>(villaDto);

            //Villa model = new()
            //{
            //    Id = villaDto.Id,
            //    Nombre = villaDto.Nombre,
            //    Amenidad = villaDto.Amenidad,
            //    Detalle = villaDto.Detalle,
            //    ImagenUrl = villaDto.ImagenUrl,
            //    Ocupantes = villaDto.Ocupantes,
            //    MetrosCuadrados = villaDto.MetrosCuadrados,
            //    Tarifa = villaDto.Tarifa,
            //    FechaActualizacion = DateTime.Now,
            //};

            _db.Villas.Update(modelo);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<VillaDto>> UpdatePartialVilla(int id, JsonPatchDocument<VillaDto> patchDto)
        {

            if (patchDto == null || id == 0)
                return BadRequest();

            var villa = await _db.Villas.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

            if (villa == null) return BadRequest();

            VillaDto villaDto = _mapper.Map<VillaDto>(villa);

            //VillaDto villaDto = new()
            //{
            //    Id = villa.Id,
            //    Nombre = villa.Nombre,
            //    Amenidad = villa.Amenidad,
            //    Detalle = villa.Detalle,
            //    ImagenUrl = villa.ImagenUrl,
            //    Ocupantes = villa.Ocupantes,
            //    MetrosCuadrados = villa.MetrosCuadrados,
            //    Tarifa = villa.Tarifa,
            //};

            patchDto!.ApplyTo(villaDto, ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Villa modelo = _mapper.Map<Villa>(villaDto);

            //Villa modelo = new()
            //{
            //    Id = villaDto.Id,
            //    Nombre = villaDto.Nombre,
            //    Amenidad = villaDto.Amenidad,
            //    Detalle = villaDto.Detalle,
            //    ImagenUrl = villaDto.ImagenUrl,
            //    Ocupantes = villaDto.Ocupantes,
            //    MetrosCuadrados = villaDto.MetrosCuadrados,
            //    Tarifa = villaDto.Tarifa,
            //    FechaActualizacion = DateTime.Now,
            //};

            _db.Villas.Update(modelo);
            await _db.SaveChangesAsync();
            return NoContent();
        }

    }
}
