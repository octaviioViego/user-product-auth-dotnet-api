using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

// <summary>
/// Controlador para gestionar las operaciones relacionadas con los usuarios.
/// </summary>
[Route("api/[controller]")]
[Authorize]
[ApiController]
public class usersController : ControllerBase , IControllers 
{
    private readonly IUsuarioService _iUsuarioService;

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="UsuarioController"/>.
    /// </summary>
    public usersController(IUsuarioService iUsuarioService)
    {
        _iUsuarioService = iUsuarioService;
    }
    

    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsersAsync([FromQuery] GetUsersQuery query)
    {
        try
        {
            var usuarios = await _iUsuarioService.GetAllAsync2( query.Page,query.Limit, 
                                                                query.Sort,query.Order,
                                                                query.IsActive,query.IsDeleted, 
                                                                query.IsSuperuser,query.EmailVerified);
              

            return new OkObjectResult(new ApiResponse<ResponseGetUsers<List<UsuarioDTO>>>(200,MessageService.Instance.GetMessage("getAlluser200"),usuarios)); 
        }catch (BusinessException ex){
             return StatusCode(ex.StatusCode,new ApiResponse<string>(ex.StatusCode, ex.Message));
            
        }catch(Exception ex){
                Console.WriteLine("Error 500: " + ex);
                return StatusCode(500,new ApiResponse<string>(500,MessageService.Instance.GetMessage("controllerGetAll500")));
        }
    }

    /// <summary>
    /// Obtiene un usuario por su identificador único.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        try
        {
            var usuario = await _iUsuarioService.GetByIdAsync(id);
            return Ok(new ApiResponse<UsuarioDTO>(200,MessageService.Instance.GetMessage("GetByIdAsyncUser200"),usuario));
                
        }catch (BusinessException ex){
             return StatusCode(ex.StatusCode,new ApiResponse<string>(ex.StatusCode, ex.Message));
            
        }catch(Exception ex){
                Console.WriteLine("Error 500: " + ex);
                return StatusCode(500,new ApiResponse<string>(500,MessageService.Instance.GetMessage("controllerGetAll500")));
        }
        
    }

    /// <summary>
    /// Crea un nuevo usuario.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] UsuarioCreateDTO usuarioCreateDTO)
    {
        try
        {
            var usuario = await _iUsuarioService.AddAsync(usuarioCreateDTO);
            return new CreatedResult("", new ApiResponse<UsuarioDTOResponceExtends>(201, MessageService.Instance.GetMessage("AddAsyncUser201"),usuario));
        }catch (BusinessException ex){
             return StatusCode(ex.StatusCode,new ApiResponse<string>(ex.StatusCode, ex.Message));
            
        }catch(Exception ex){
                Console.WriteLine("Error 500: " + ex);
                return StatusCode(500,new ApiResponse<string>(500,MessageService.Instance.GetMessage("controllerGetAll500")));
        }
        
    }

    /// <summary>
    /// Actualiza un usuario existente.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id, [FromBody] UsuarioUpdate usuarioUpdate)
    {
        try
        {        
            var updatedUser = await _iUsuarioService.UpdateAsync(id,usuarioUpdate);
            return Ok(new ApiResponse<UsuarioDTOResponceExtends>(200,MessageService.Instance.GetMessage("UpdateAsyncUser200"),updatedUser));
        }catch (BusinessException ex){
             return StatusCode(ex.StatusCode,new ApiResponse<string>(ex.StatusCode, ex.Message));
            
        }catch(Exception ex){
                Console.WriteLine("Error 500: " + ex);
                return StatusCode(500,new ApiResponse<string>(500,MessageService.Instance.GetMessage("controllerGetAll500")));
        }
    }

    /// <summary>
    /// Elimina un usuario por su identificador único.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            
            var existingUser = await _iUsuarioService.DeleteAsync(id);
            return  Ok(new ApiResponse<string>(200,MessageService.Instance.GetMessage(existingUser)));
        
        }catch (BusinessException ex){
             return StatusCode(ex.StatusCode,new ApiResponse<string>(ex.StatusCode, ex.Message));
            
        }catch(Exception ex){
                Console.WriteLine("Error 500: " + ex);
                return StatusCode(500,new ApiResponse<string>(500,MessageService.Instance.GetMessage("controllerGetAll500")));
        }
        
    }

    /// <summary>
    /// Actualiza parcialmente un usuario existente.
    /// </summary>
    [HttpPatch("{id}")]
    public async Task<IActionResult> Patch(Guid id, [FromBody] UsuarioPatchDTO usuarioDTO)
    {
        try{
            
             var existingUser = await _iUsuarioService.UpdatePartialAsync(id,usuarioDTO);
             return Ok(new ApiResponse<UsuarioDTOResponceExtends>(200,MessageService.Instance.GetMessage("UpdatePartialAsyncUser200"),existingUser));
        }catch (BusinessException ex){
             return StatusCode(ex.StatusCode,new ApiResponse<string>(ex.StatusCode, ex.Message));
            
        }catch(Exception ex){
                Console.WriteLine("Error 500: " + ex);
                return StatusCode(500,new ApiResponse<string>(500,MessageService.Instance.GetMessage("controllerGetAll500")));
        }
    }

    /// <summary>
    /// Restaura un usuario eliminado.
    /// </summary>
    [HttpPatch("{id}/restore")]
    public async Task<IActionResult> RestoreUserAsync(Guid id)
    {
        try
        {
            var usuarioRestaurado = await _iUsuarioService.RestoreUserAsync(id);
            return Ok(new ApiResponse<UsuarioDTO>(200,MessageService.Instance.GetMessage("RestoreUserAsyncUser200"),usuarioRestaurado));

        }catch (BusinessException ex){
             return StatusCode(ex.StatusCode,new ApiResponse<string>(ex.StatusCode, ex.Message));
            
        }catch(Exception ex){
                Console.WriteLine("Error 500: " + ex);
                return StatusCode(500,new ApiResponse<string>(500,MessageService.Instance.GetMessage("controllerGetAll500")));
        }
    }

    /// <summary>
    /// Endpoint HTTP POST para verificar el correo electrónico de un usuario.
    /// </summary>
    [HttpPost("{id}/verify-email")]
    public async Task<IActionResult> VerifyEmail(Guid id)
    {
        try
        {
            var usuarioVerificado = await _iUsuarioService.VerifyEmailAsync(id); 
            return Ok(new ApiResponse<UsuarioDTOVerifiedEmail>(200,MessageService.Instance.GetMessage("VerifyEmailAsyncUser200"),usuarioVerificado));
        }catch (BusinessException ex){
             return StatusCode(ex.StatusCode,new ApiResponse<string>(ex.StatusCode, ex.Message));
            
        }catch(Exception ex){
                Console.WriteLine("Error 500: " + ex);
                return StatusCode(500,new ApiResponse<string>(500,MessageService.Instance.GetMessage("controllerGetAll500")));
        }
    }

    /// <summary>
    /// Endpoint HTTP PATCH para activar un usuario estableciendo el campo is_active en true.
    /// </summary>
    [HttpPatch("{id}/activate")]
    public async Task<IActionResult> ActivateUser(Guid id)
    {
        try{
            
            var activarUsuario = await _iUsuarioService.ActiveteUserAsync(id);
            return  Ok(new ApiResponse<UsuarioDTOResponce>(200,MessageService.Instance.GetMessage("ActivateUserUser200"),activarUsuario));
        
        }catch (BusinessException ex){
             return StatusCode(ex.StatusCode,new ApiResponse<string>(ex.StatusCode, ex.Message));
            
        }catch(Exception ex){
                Console.WriteLine("Error 500: " + ex);
                return StatusCode(500,new ApiResponse<string>(500,MessageService.Instance.GetMessage("controllerGetAll500")));
        }
    }
    
    /// <summary>
    /// Desactiva un usuario estableciendo is_active en false.
    /// </summary>
    [HttpPatch("{id}/deactivate")]
    public async Task<IActionResult> DeactivateUser(Guid id)
    {
        try
        {
            var usuario = await _iUsuarioService.DeactivateUserAsync(id);
            return Ok(new ApiResponse<UsuarioDTOResponce>(200,MessageService.Instance.GetMessage("DeactivateUserAsyncUser200"),usuario));
        }catch (BusinessException ex){
             return StatusCode(ex.StatusCode,new ApiResponse<string>(ex.StatusCode, ex.Message));
            
        }catch(Exception ex){
                Console.WriteLine("Error 500: " + ex);
                return StatusCode(500,new ApiResponse<string>(500,MessageService.Instance.GetMessage("controllerGetAll500")));
        }
    }

    /// <summary>
    /// Cambia la contraseña de un usuario.
    /// </summary>
    [HttpPost("{id}/change-password")]
    public async Task<IActionResult> ChangePassword(Guid id, [FromBody] ChangePasswordDTO model)
    {
        
        try{
            
            var result = await _iUsuarioService.ChangePasswordAsync(id, model);
            return Ok(new ApiResponse<string>(200,MessageService.Instance.GetMessage(result)));  
        }catch (BusinessException ex){
             return StatusCode(ex.StatusCode,new ApiResponse<string>(ex.StatusCode, ex.Message));
            
        }catch(Exception ex){
                Console.WriteLine("Error 500: " + ex);
                return StatusCode(500,new ApiResponse<string>(500,MessageService.Instance.GetMessage("controllerGetAll500")));
        }
        
    }

}
