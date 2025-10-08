using INESPRE.Core.Models.Users;
using INESPRE.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace INESPRE.Core.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _svc;
        private readonly IAuthService _auth;

        public UsersController(IUsersService svc, IAuthService auth)
        {
            _svc = svc;
            _auth = auth;
        }

        // POST: api/users  -> crear usuario
        [HttpPost]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromBody] RegisterRequest req)
        {
            var id = await _auth.RegisterAsync(req); // Registro del usuario

            var created = await _svc.GetByIdAsync(id); // Intentamos obtener el usuario creado

            // Si no se encuentra, devolvemos solo el ID
            return CreatedAtAction(nameof(GetById), new { id }, (object?)created ?? new { userId = id });
        }

        // GET: api/users
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _svc.GetAllAsync();
            return Ok(users); // Devuelve la lista de usuarios
        }

        // GET: api/users/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _svc.GetByIdAsync(id);
            return user is not null ? Ok(user) : NotFound(); // Retorna 404 si no se encuentra el usuario
        }

        // PUT: api/users/{id} -> actualizar
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromBody] UserUpdateRequest req)
        {
            // Validación para asegurar que el id de la URL coincida con el de los datos enviados en el cuerpo
            if (id != req.UserId)
            {
                return BadRequest("El ID en la URL no coincide con el ID de los datos del cuerpo de la solicitud.");
            }

            // Mapeamos los datos del usuario
            var user = new User
            {
                UserId = req.UserId, // Usamos el UserId del cuerpo de la solicitud
                Username = req.UserName,
                Email = req.Email,
                FullName = req.FullName,
                Phone = req.Phone,
                RoleId = req.RoleId,
                IsActive = req.IsActive
            };

            // Llamamos al servicio para actualizar el usuario en la base de datos
            await _svc.UpdateAsync(user);
            return NoContent(); // Respuesta exitosa sin contenido adicional
        }

        // DELETE lógico: deshabilitar el usuario (borrado lógico)
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Disable(int id)
        {
            await _svc.DisableAsync(id); // Llamamos al servicio para realizar el borrado lógico
            return NoContent(); // Respuesta exitosa sin contenido adicional
        }
    }
}
