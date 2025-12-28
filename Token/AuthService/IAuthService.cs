using apitienda.DTOs;

public interface IAuthService
{
    Task<TokenResponse> LoginAsync(string email, string password);
    Task<TokenResponse> RefreshAsync(string refreshToken);
}

