using apitienda.DTOs;
using Microsoft.AspNetCore.Mvc;

public interface IAuthController
{
    Task<IActionResult> Login(LoginRequest dto);
    Task<IActionResult> Refresh(RefreshRequest dto);
}