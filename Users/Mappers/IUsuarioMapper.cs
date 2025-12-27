using apitienda.Models;

public interface IUsuarioMapper
{
    public UsuarioDTO ToDTO(Usuario usuario);
    public Usuario ToEntity(UsuarioDTO usuarioDTO);
    public Usuario toUpdate(Usuario usuario, UsuarioUpdate usuarioUpdate);
    public Usuario CreateUser(UsuarioCreateDTO userCreateDTO);
}