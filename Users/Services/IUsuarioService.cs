using apitienda.Models;
using Microsoft.AspNetCore.Mvc;

public interface IUsuarioService
{
    /// <summary>
    /// Obtiene todos los usuarios.
    /// </summary>
    /// <returns>Una acción que devuelve una lista de todos los usuarios.</returns>
    Task<ResponseGetUsers<List<UsuarioDTO>>> GetAllAsync2(int? safePage, int? safeLimit, string? sort, string? safeOrder, bool? safeActive, bool? safeisDeleted, bool? safeIsSuperuser, bool? safeEmailVerified);
    /// <summary>
    /// Activa un usuario según el ID proporcionado.
    /// </summary>
    /// <param name="id">El identificador único del usuario a activar.</param>
    /// <returns>Una acción que indica el resultado de la operación.</returns>
    Task<UsuarioDTOResponce> ActiveteUserAsync(Guid id);

    /// <summary>
    /// Obtiene un usuario específico según su ID.
    /// </summary>
    /// <param name="id">El identificador único del usuario a obtener.</param>
    /// <returns>Una acción que devuelve los datos del usuario solicitado.</returns>
    Task<UsuarioDTO> GetByIdAsync(Guid id);

    /// <summary>
    /// Agrega un nuevo usuario a la base de datos.
    /// </summary>
    /// <param name="usuarioDTO">El DTO que contiene los datos del usuario a agregar.</param>
    /// <returns>Una acción que indica el resultado de la operación.</returns>
    Task<UsuarioDTOResponceExtends> AddAsync(UsuarioCreateDTO usuarioCreateDTO);

    /// <summary>
    /// Actualiza los datos de un usuario existente.
    /// </summary>
    /// <param name="UsuarioUpdate">El DTO con los datos actualizados del usuario.</param>
    /// <returns>Una acción que indica el resultado de la operación.</returns>
    Task<UsuarioDTOResponceExtends> UpdateAsync(Guid id, UsuarioUpdate usuarioUpdate);

    /// <summary>
    /// Elimina un usuario de la base de datos según su ID.
    /// </summary>
    /// <param name="id">El identificador único del usuario a eliminar.</param>
    /// <returns>Una acción que indica el resultado de la operación.</returns>
    Task<String> DeleteAsync(Guid id);

    /// <summary>
    /// Actualiza parcialmente los datos de un usuario según su ID y el DTO proporcionado.
    /// </summary>
    /// <param name="id">El identificador único del usuario a actualizar.</param>
    /// <param name="usuarioDTO">El DTO que contiene los datos parciales para la actualización del usuario.</param>
    /// <returns>Una acción que indica el resultado de la operación.</returns>
    Task<UsuarioDTOResponceExtends> UpdatePartialAsync(Guid id, UsuarioPatchDTO usuarioDTO);

    /// <summary>
    /// Restaura un usuario que fue desactivado previamente.
    /// </summary>
    /// <param name="id">El identificador único del usuario a restaurar.</param>
    /// <returns>Una acción que indica el resultado de la operación.</returns>
    Task<UsuarioDTO> RestoreUserAsync(Guid id);

    /// <summary>
    /// Verifica el correo electrónico de un usuario según su ID.
    /// </summary>
    /// <param name="id">El identificador único del usuario cuya dirección de correo electrónico se verificará.</param>
    /// <returns>Una acción que indica si la verificación fue exitosa o no.</returns>
    Task<UsuarioDTOVerifiedEmail> VerifyEmailAsync(Guid id);

    /// <summary>
    /// Desactiva un usuario según su ID.
    /// </summary>
    /// <param name="id">El identificador único del usuario a desactivar.</param>
    /// <returns>Una acción que indica el resultado de la operación.</returns>
    Task<UsuarioDTOResponce> DeactivateUserAsync(Guid id);

    /// <summary>
    /// Cambia la contraseña de un usuario.
    /// </summary>
    /// <param name="id">El identificador único del usuario cuya contraseña se actualizará.</param>
    /// <param name="model">El modelo que contiene la nueva contraseña y la contraseña actual.</param>
    /// <returns>Una acción que indica el resultado de la operación.</returns>
    Task<String> ChangePasswordAsync(Guid id, ChangePasswordDTO model);

    /// <summary>
    /// Valida las credenciales del usuario (email y contraseña).
    /// </summary>
    /// <param name="email">Correo electrónico.</param>
    /// <param name="password">Contraseña sin hashear.</param>
    /// <returns>El usuario si es válido; null si no.</returns>
    Task<Usuario> ValidateUserAsync(string email, string password);

}
   