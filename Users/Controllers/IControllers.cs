using Microsoft.AspNetCore.Mvc;

public interface IControllers
{
    Task<IActionResult> GetAllUsersAsync([FromQuery] GetUsersQuery query);
    Task<IActionResult> Get(Guid id);
    Task<IActionResult> Post([FromBody] UsuarioCreateDTO usuarioCreateDTO);
    Task<IActionResult> Put(Guid id, [FromBody] UsuarioUpdate usuarioUpdate);
    Task<IActionResult> Delete(Guid id);
    Task<IActionResult> Patch(Guid id, [FromBody] UsuarioPatchDTO usuarioDTO);
    Task<IActionResult> RestoreUserAsync(Guid id);
    Task<IActionResult> VerifyEmail(Guid id);
    Task<IActionResult> ActivateUser(Guid id);
    Task<IActionResult> DeactivateUser(Guid id);
    Task<IActionResult> ChangePassword(Guid id, [FromBody] ChangePasswordDTO model);
}