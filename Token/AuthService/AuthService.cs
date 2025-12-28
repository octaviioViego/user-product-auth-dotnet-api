using System.Security.Claims;
using apitienda.DTOs;
using System.IdentityModel.Tokens.Jwt;
using apitienda.Models;

public class AuthService : IAuthService
{
    private readonly IJwtService _jwtService;
    private readonly IUsuarioService _usuarioService;

    public AuthService(IJwtService jwtService, IUsuarioService usuarioService)
    {
        _jwtService = jwtService;
        _usuarioService = usuarioService;
    }

    // =========================
    // LOGIN
    // =========================
    public async Task<TokenResponse> LoginAsync(string email , string password)
    {
        // Usamos el código legado de User
        var result = await _usuarioService.ValidateUserAsync(email, password);

        /*
        * Verificamos en el if que el result sea un objeto de tipo OkObjectResult y lo guardamos en la
        * variable ok después verificamos que el ok.Value sea de tipo Usuario y lo
        * guardamos en una variable de tipo user.
        */ 
        
        if (result is not Usuario user)
            throw new BusinessException(401, MessageService.Instance.GetMessage("Credenciales inválidas"));
             

        var accessToken = _jwtService.GenerateAccessToken(user.id, user.email);
        var refreshToken = _jwtService.GenerateRefreshToken(user.id);
        return BuildResponse(accessToken, refreshToken);
        
    }

    // =========================
    // REFRESH TOKEN
    // =========================
    public async Task<TokenResponse> RefreshAsync(string refreshToken)
    {
        var principal = _jwtService.ValidateToken(refreshToken);

        if (principal.FindFirst("type")?.Value != "refresh")
            throw new BusinessException(404, MessageService.Instance.GetMessage("No es refresh token"));

        var jti = principal.FindFirst(JwtRegisteredClaimNames.Jti)!.Value;
        var userId = Guid.Parse(principal.FindFirst(JwtRegisteredClaimNames.Sub)!.Value);
        var exp = principal.FindFirst(JwtRegisteredClaimNames.Exp)!.Value;

        //  Revocar refresh token viejo
        await _jwtService.RevokeTokenAsync(
            jti,
            userId,
            DateTimeOffset.FromUnixTimeSeconds(long.Parse(exp)),
            "refresh"
        );

        // Extraer el email del token que nos enviaron para refrescar
        var email = principal.FindFirst(ClaimTypes.Email)?.Value 
                    ?? principal.FindFirst(JwtRegisteredClaimNames.Email)?.Value 
                    ?? ""; // O búscalo en la BD con el userId
        
        Console.WriteLine($"El email si lo saca: {email}");
        //  Emitir nuevos tokens
        var newAccess = _jwtService.GenerateAccessToken(userId, email);
        var newRefresh = _jwtService.GenerateRefreshToken(userId);
        return BuildResponse(newAccess, newRefresh);
    }

    private TokenResponse BuildResponse(string accessToken, string refreshToken)
    {
        return new TokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiresInMinutes = _jwtService.AccessTokenExpiryMinutes,
            RefreshTokenExpiresInDays = _jwtService.RefreshTokenExpiryDays
        };
    }

}
