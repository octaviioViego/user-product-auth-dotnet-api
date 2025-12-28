using apitienda.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("auth")]
[AllowAnonymous]
public class AuthController : ControllerBase, IAuthController
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest dto)
    {
        try{
            
            var tokens = await _authService.LoginAsync(dto.Email, dto.Password);
            return  Ok(new ApiResponse<TokenResponse>(200,MessageService.Instance.GetMessage("authUser200"),tokens));
        
        }catch (BusinessException ex){
             return StatusCode(ex.StatusCode,new ApiResponse<string>(ex.StatusCode, ex.Message));
            
        }catch(Exception ex){
                Console.WriteLine("Error 500: " + ex);
                return StatusCode(500,new ApiResponse<string>(500,MessageService.Instance.GetMessage("authUser500")));
        }
    }


    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshRequest dto)
    {
        try{
            
            var tokens = await _authService.RefreshAsync(dto.RefreshToken);
            return  Ok(new ApiResponse<TokenResponse>(200,MessageService.Instance.GetMessage("authUser200"),tokens));
        
        }catch (BusinessException ex){
             return StatusCode(ex.StatusCode,new ApiResponse<string>(ex.StatusCode, ex.Message));
            
        }catch(Exception ex){
                Console.WriteLine("Error 500: " + ex);
                return StatusCode(500,new ApiResponse<string>(500,MessageService.Instance.GetMessage("authUser500")));
        }
    }

}
