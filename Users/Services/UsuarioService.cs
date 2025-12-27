using apitienda.Models;
using Microsoft.AspNetCore.Mvc;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioDAO _iUsuarioDAO; 
    private readonly IUsuarioMapper _usuarioMapper;
    private readonly PasswordResetEmail _emailService; // Inyectar EmailService


    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="UsuarioService"/>.
    /// </summary>
    /// <param name="usuarioDAO">El DAO para la interacción con la base de datos.</param>
    /// <param name="usuarioMapper">El mapper para convertir entre entidades y DTOs.</param>
    public UsuarioService(IUsuarioDAO iUsuarioDAO, IUsuarioMapper usuarioMapper, PasswordResetEmail emailService)
    {
        _iUsuarioDAO = iUsuarioDAO;
        _usuarioMapper = usuarioMapper;
        _emailService = emailService; // Asignar en el constructor
    }

    /// <summary>
    /// Obtiene todos los usuarios de manera asíncrona.
    /// Obtiene una lista de usarios donde tengamos que pasarle parametros 
    /// </summary>
    /// <returns>Una lista de objetos <see cref="UsuarioDTO"/>.</returns>
    public async  Task<ResponseGetUsers<List<UsuarioDTO>>> GetAllAsync2(int? safePage, int? safeLimit, string? sort, string? safeOrder, bool? safeActive, bool? safeisDeleted, bool? safeIsSuperuser, bool? safeEmailVerified)
    {
        if(!safePage.HasValue && !safeLimit.HasValue && string.IsNullOrEmpty(sort) && string.IsNullOrEmpty(safeOrder) && !safeActive.HasValue && !safeisDeleted.HasValue && !safeIsSuperuser.HasValue && !safeEmailVerified.HasValue)
        {
            safeisDeleted = false;
        }
        
        SentenciaUsuarios Crearsentencia = new SentenciaUsuarios(safePage, safeLimit, sort, safeOrder, safeActive, safeisDeleted, safeIsSuperuser, safeEmailVerified);
        

        var sentencia = Crearsentencia.CrearSenentiaSQLUser();
        var usuarios = await _iUsuarioDAO.GetUserAsync(sentencia.Sentencia, sentencia.Parametros);
        
        if(usuarios == null || !usuarios.Any())
        {
             throw new BusinessException(404, MessageService.Instance.GetMessage("getAllUser404"));
        }
        
        var numResultado = usuarios.Count();
        var resultado = usuarios.Select(u => _usuarioMapper.ToDTO(u)).ToList();
        
        /// <summary>
        /// Retornamos la lista de usuarios con las configuraciones de la petición.
        /// </summary>
        
        return new ResponseGetUsers<List<UsuarioDTO>> (numResultado, //Total de usuarios
                                                       sentencia.valores[7], //Valor correspondiente a la paginación
                                                       sentencia.valores[6], //Valor correspondiente al límite
                                                       sentencia.valores[4], //Valor correspondiente al sort
                                                       sentencia.valores[5], //Valor correspondiente al safeOrder
                                                       sentencia.valores[0], //Valor correspondiente a la actividad
                                                       sentencia.valores[1], //Valor correspondiente a la eliminación
                                                       sentencia.valores[2], //Valor correspondiente al superusuario
                                                       sentencia.valores[3], //Valor correspondiente a la verificación del correo
                                                       resultado             //Lista de usuarios
                                                      );
    }

    /// <summary>
    /// Acviva un usuario que este desactivado.
    /// </summary>
    /// <param name="id">El identificador único del usuario.</param>
    /// <returns>Un objeto <see cref="UsuarioDTO"/> o <c>null</c> si no se encuentra.</returns>
    
    public async Task<UsuarioDTOResponce> ActiveteUserAsync(Guid id)
    {
        
        var usuario = await _iUsuarioDAO.GetByIdAsync(id);
        
        if(usuario == null)
        {
            throw new BusinessException(404, MessageService.Instance.GetMessage("ActivateUserUser404"));
        }

        if(usuario.is_active)
        {
             throw new BusinessException(400, MessageService.Instance.GetMessage("ActivateUserUser400"));
        }
    
        usuario.is_active = true;
        
        var usuarioActivado = _usuarioMapper.ToDTO(usuario);
        
        await _iUsuarioDAO.UpdateAsync(usuario);
        
        var resultado = new UsuarioDTOResponce(id,usuarioActivado);
        
        return resultado;
    }

    /// <summary>
    /// Obtiene un usuario por su ID de manera asíncrona.
    /// </summary>
    /// <param name="id">El identificador único del usuario.</param>
    /// <returns>Un objeto <see cref="UsuarioDTO"/> o <c>null</c> si no se encuentra.</returns>
    public async Task<UsuarioDTO> GetByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new BusinessException(400, "Id inválido");
        }
            
        var usuario = await _iUsuarioDAO.GetByIdAsync(id); 
        
        if(usuario == null)
        {
            throw new BusinessException(404, MessageService.Instance.GetMessage("GetByIdAsyncUser404"));
        }
        
        var resultado = _usuarioMapper.ToDTO(usuario);
        return resultado;
    }

    /// <summary>
    /// Agrega un nuevo usuario de manera asíncrona.
    /// </summary>
    /// <param name="usuarioDTO">Los datos del usuario en formato DTO.</param>
    /// <returns>Una tarea que representa la operación de inserción.</returns>
    public async Task<UsuarioDTOResponceExtends> AddAsync(UsuarioCreateDTO usuarioCreateDTO)
    {
        
        SentenciaUsuarios Crearsentencia = new SentenciaUsuarios(usuarioCreateDTO.email, usuarioCreateDTO.username);
        var sentencia = Crearsentencia.CrearSentenciaSQLValidarEmailUsername();
        var existeUsuario = await _iUsuarioDAO.GetUserAsync(sentencia.Sentencia, sentencia.Parametros);
        

        if(!(existeUsuario == null || !existeUsuario.Any()))
        {
             throw new BusinessException(409, MessageService.Instance.GetMessage("AddAsyncUser409"));
        }
        
        var usuario = _usuarioMapper.CreateUser(usuarioCreateDTO); //_createUserMapper.ToEntity(usuarioCreateDTO); 
        var usuarioDTO = _usuarioMapper.ToDTO(usuario);
        
        await _iUsuarioDAO.AddAsync(usuario); 
        
        var resultado = new UsuarioDTOResponceExtends(usuario.id,usuarioDTO); 
        return resultado;

    }

    /// <summary>
    /// Actualiza un usuario existente de manera asíncrona.
    /// </summary>
    /// <param name="usuarioDTO">Los datos del usuario en formato DTO.</param>
    /// <returns>Una tarea que representa la operación de actualización.</returns>
    public async Task<UsuarioDTOResponceExtends> UpdateAsync(Guid id, UsuarioUpdate usuarioUpdate)
    {
        var usuario = await _iUsuarioDAO.GetByIdAsync(id); // Convierte el DTO en una entidad.

        if(usuario == null)
        {
            throw new BusinessException(404, MessageService.Instance.GetMessage("UpdateAsyncUser404"));
        }
        
        if(usuario.is_deleted)
        {
            throw new BusinessException(400, MessageService.Instance.GetMessage("UpdateAsyncUser400Deleted"));
        }
        
        if(usuario.is_active == false)
        {
            throw new BusinessException(400, MessageService.Instance.GetMessage("UpdateAsyncUser400Active"));
        }

        // Verifica si el correo electrónico ya está en uso por otro usuario
        SentenciaUsuarios Crearsentencia = new SentenciaUsuarios(usuarioUpdate.email, usuarioUpdate.username);
        var sentencia = Crearsentencia.CrearSentenciaSQLValidarEmailUsername();
        var existeUsuario = await _iUsuarioDAO.GetUserAsync(sentencia.Sentencia, sentencia.Parametros);
       
        if(!(existeUsuario == null || !existeUsuario.Any()))
        {
            throw new BusinessException(409, MessageService.Instance.GetMessage("AddAsyncUser409"));
        }

        usuario = _usuarioMapper.toUpdate(usuario,usuarioUpdate);

        _iUsuarioDAO.Detach(usuario);
        await _iUsuarioDAO.UpdateAsync(usuario);

        // Convierte la entidad actualizada a DTO
        var usuarioDTOResponce = _usuarioMapper.ToDTO(usuario); // Convierte la entidad actualizada a DTO
        var resultado = new UsuarioDTOResponceExtends(usuario.id,usuarioDTOResponce);
        return resultado;
    }

    /// <summary>
    /// Elimina un usuario por su ID de manera asíncrona.
    /// </summary>
    /// <param name="id">El identificador único del usuario a eliminar.</param>
    /// <returns>Una tarea que representa la operación de eliminación.</returns>
    public async Task<String> DeleteAsync(Guid id)
    {
        var existeusuario = await _iUsuarioDAO.GetByIdAsync(id);
        if(existeusuario == null)
        {
            throw new BusinessException(404, MessageService.Instance.GetMessage("controllerUser404"));
        }
        
        if(existeusuario.is_deleted)
        {
            throw new BusinessException(400, MessageService.Instance.GetMessage("DeleteAsyncUser400"));
        }
        
        await _iUsuarioDAO.DeleteAsync(id); 
        return "DeleteAsyncUser200";
    }

    /// <summary>
    /// Actualiza parcialmente un usuario existente de manera asíncrona.
    /// </summary>
    /// <param name="id">El identificador único del usuario a actualizar.</param>
    /// <param name="usuarioDTO">Los datos del usuario en formato DTO.</param>
    /// <returns>Una tarea que representa la operación de actualización parcial.</returns>
    public async Task<UsuarioDTOResponceExtends> UpdatePartialAsync(Guid id, UsuarioPatchDTO usuarioDTO)
    {
        
        var existingUser = await _iUsuarioDAO.GetByIdAsync(id);
        
        if (existingUser == null)
        {
            throw new BusinessException(404, MessageService.Instance.GetMessage("controllerUser404"));
        }

        if(existingUser.is_deleted)
        {
            throw new BusinessException(400, MessageService.Instance.GetMessage("UpdatePartialAsyncUser400"));
        }
        
        //Actualiza parcialmente el usuario
        existingUser.first_name = usuarioDTO.first_name ?? existingUser.first_name;
        existingUser.last_name = usuarioDTO.last_name ?? existingUser.last_name;
        existingUser.email = usuarioDTO.email ?? existingUser.email;

        existingUser.modified_at = DateTimeOffset.UtcNow;
        await _iUsuarioDAO.UpdateAsync(existingUser); // Guarda los cambios en la base de datos
        
        var resultado = _usuarioMapper.ToDTO(existingUser); // Convierte la entidad actualizada a DTO
        var resultadoDTO = new UsuarioDTOResponceExtends(id, resultado); // Crea un nuevo DTO de respuesta
        return resultadoDTO;
    }

    /// <summary>
    /// Restaura un usuario eliminado lógicamente.
    /// </summary>
    /// <param name="id">El identificador único del usuario.</param>
    /// <returns>Un objeto <see cref="UsuarioDTO"/> del usuario restaurado o <c>null</c> si no se encuentra.</returns>
    /// <exception cref="InvalidOperationException">Si el usuario no estaba eliminado.</exception>
    public async Task<UsuarioDTO> RestoreUserAsync(Guid id)
    {
        var restaurarUsuario = await _iUsuarioDAO.GetByIdAsync(id);
        
        if (restaurarUsuario == null)
        {
            throw new BusinessException(404, MessageService.Instance.GetMessage("RestoreUserAsyncUser404"));
        }
        
        if (!restaurarUsuario.is_deleted)
        {
            throw new BusinessException(400, MessageService.Instance.GetMessage("UpdatePartialAsyncUser400"));
        }
        
        restaurarUsuario.is_deleted = false;
        restaurarUsuario.deleted_at = null; 
        restaurarUsuario.modified_at = DateTimeOffset.UtcNow; 
        await _iUsuarioDAO.UpdateAsync(restaurarUsuario);
  
        var resultado = _usuarioMapper.ToDTO(restaurarUsuario);
        return resultado;
    }

    /// <summary>
    /// Verifica el correo electrónico de un usuario.
    /// </summary>
    /// <param name="id">El identificador único del usuario.</param>
    /// <returns>El DTO del usuario actualizado, o null si el usuario no existe.</returns>
    /// <exception cref="InvalidOperationException">Se lanza si el correo ya estaba verificado.</exception>
    public async Task<UsuarioDTOVerifiedEmail> VerifyEmailAsync(Guid id)
    {
        var usuario = await _iUsuarioDAO.GetByIdAsync(id); // Busca el usuario en la base de datos

        if (usuario == null)
        {
            throw new BusinessException(404, MessageService.Instance.GetMessage("controllerUser404"));
        }
        
        if (usuario.email_verified)
        {
            throw new BusinessException(400, MessageService.Instance.GetMessage("UpdatePartialAsyncUser400"));
        }

        usuario.email_verified = true;
        usuario.email_verified_at = DateTime.UtcNow; 

        await _iUsuarioDAO.UpdateAsync(usuario); 
        
        var resultado = new UsuarioDTOVerifiedEmail(id,usuario);
        return resultado;
    }

    /// <summary>
    /// Desactiva un usuario estableciendo is_active en false.
    /// </summary>
    /// <param name="id">El identificador único del usuario.</param>
    /// <returns>El DTO del usuario desactivado, o null si no se encuentra.</returns>
    /// <exception cref="InvalidOperationException">Si el usuario ya está desactivado.</exception>
    public async Task<UsuarioDTOResponce> DeactivateUserAsync(Guid id)
    {
        var usuario = await _iUsuarioDAO.GetByIdAsync(id);

        if (usuario == null)
        {
            throw new BusinessException(404, MessageService.Instance.GetMessage("controllerUser404"));
        }


        if (!usuario.is_active)
        {
            throw new BusinessException(400, MessageService.Instance.GetMessage("UpdatePartialAsyncUser400"));
        }

        usuario.is_active = false;
        await _iUsuarioDAO.UpdateAsync(usuario);

        var usuarioActivado = _usuarioMapper.ToDTO(usuario); 

        var resultado = new UsuarioDTOResponce(id,usuarioActivado);
        return resultado;
    }
    
    /// <summary>
    /// Cambia la contraseña de un usuario autenticado.
    /// </summary>
    /// <param name="id">Identificador único del usuario (UUID).</param>
    /// <param name="model">DTO con la contraseña actual, nueva contraseña y confirmación.</param>
    /// <returns>Retorna true si la contraseña fue cambiada exitosamente.</returns>
    /// <exception cref="KeyNotFoundException">Se lanza si el usuario no existe.</exception>
    /// <exception cref="InvalidOperationException">Se lanza si la contraseña actual es incorrecta, 
    /// la nueva contraseña no cumple los requisitos o las contraseñas no coinciden.</exception>
    public async Task<String> ChangePasswordAsync(Guid id, ChangePasswordDTO model)
    {
        var usuario = await _iUsuarioDAO.GetByIdAsync(id);
        
        if (usuario == null)
        {   
            throw new BusinessException(404, MessageService.Instance.GetMessage("controllerUser404"));
        }

        // Si la contraseña almacenada NO tiene formato BCrypt, la hasheamos primero
        if (!usuario.password.StartsWith("$2a$") && !usuario.password.StartsWith("$2b$") && !usuario.password.StartsWith("$2y$"))
        {
            usuario.password = BCrypt.Net.BCrypt.HashPassword(usuario.password, 12);
            await _iUsuarioDAO.UpdateAsync(usuario);
        }
        
        // Verificar que la contraseña actual no sea igual a la nueva
        if (model.CurrentPassword==model.NewPassword)
        {
            throw new BusinessException(400, MessageService.Instance.GetMessage("ChangePasswordAsyncUser400Current"));
        }

        // Verificar que la contraseña actual sea correcta
        if (!BCrypt.Net.BCrypt.Verify(model.CurrentPassword, usuario.password))
        {
            throw new BusinessException(400, MessageService.Instance.GetMessage("ChangePasswordAsyncUser400Current"));
        }

        // Validar que la nueva contraseña cumple con seguridad
        if (model.NewPassword.Length < 8 || !model.NewPassword.Any(char.IsDigit) || !model.NewPassword.Any(char.IsLetter))
        {
            throw new BusinessException(400, MessageService.Instance.GetMessage("ChangePasswordAsyncUser400Size"));
        }

        // Validar que la confirmación coincida con la nueva contraseña
        if (model.NewPassword != model.ConfirmPassword)
        {
            throw new BusinessException(400, MessageService.Instance.GetMessage("ChangePasswordAsyncUser400Confirmation"));
        }

        // Hashear la nueva contraseña antes de guardarla
        usuario.password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword, 12);
        await _iUsuarioDAO.UpdateAsync(usuario);
        return "ChangePasswordAsyncUser200";
    }

    /// <summary>
    /// Valida las credenciales de un usuario a partir del correo electrónico y la contraseña proporcionados.
    /// </summary>
    /// <param name="email">Correo electrónico del usuario.</param>
    /// <param name="password">Contraseña en texto plano ingresada por el usuario.</param>
    /// <returns>
    /// Una tarea que representa el resultado de la validación. Retorna un <see cref="UnauthorizedObjectResult"/> si las credenciales son inválidas o el usuario está inactivo, 
    /// o un <see cref="OkObjectResult"/> si las credenciales son válidas.
    /// </returns>

    public async Task<Usuario> ValidateUserAsync(string email, string password)
    {
        var user = await _iUsuarioDAO.GetByEmailAsync(email);

        if (user == null)
        {
            throw new BusinessException(401, MessageService.Instance.GetMessage("Credenciales inválidas"));
        }

        if (!user.is_active)
        {
            throw new BusinessException(401, MessageService.Instance.GetMessage("Usuario inactivo"));
        }

        // Si la contraseña no está hasheada, la hasheamos y guardamos
        if (!user.password.StartsWith("$2a$") && !user.password.StartsWith("$2b$") && !user.password.StartsWith("$2y$"))
        {
            user.password = BCrypt.Net.BCrypt.HashPassword(user.password, 12);
            await _iUsuarioDAO.UpdateAsync(user);
        }

        if (!BCrypt.Net.BCrypt.Verify(password, user.password))
        {
            throw new BusinessException(401, MessageService.Instance.GetMessage("Credenciales inválidas"));
        }

        return user;
    }
}
