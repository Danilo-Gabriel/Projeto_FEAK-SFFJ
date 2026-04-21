using DomainService.DTOs;
using DomainService.DTOs.Request;
using DomainService.Entities;
using DomainService.Interfaces;
using DomainService.Interfaces.Services;
using Microsoft.AspNetCore.Identity;

namespace DomainService.services;

public class UsuarioDomainService : BaseDomainService<Usuario>, IUsuarioDomainService
{
    private readonly IUsuarioRepository _usuarioRepository;
    
    private readonly PasswordHasher<object> _passwordHasher = new PasswordHasher<object>();
    
    public UsuarioDomainService(IUsuarioRepository usuarioRepository) : base(usuarioRepository)
    {
        /* todo além de enviamos inicializado a usuarioRepository para o BaseDomainService para fazer uso da variavel _repository da base
         e fazer uso daquele contexto (Tentity) da mesma forma que é estruturado os dbcontext do Repository, os services também aproveita da mesma logica
         Caso queira utilzar metodos especificos da IUsuarioDomainService é apenas descomentar a injeção da classe.
        */
        _usuarioRepository = usuarioRepository;
    }
    
    // public async Task<Usuario> UpdateAsync(Usuario entity)
    // {
    //     return await _repository.UpdateAsync(entity);
    // }

    public async Task<ServiceResponse<UsuarioDTO>> AtualizarUsuario(UsuarioAtualizacaoRequest dados)
    {
        var serviceResponse = new ServiceResponse<UsuarioDTO>();

        try
        {
            if (dados == null)
            {
                serviceResponse.Mensagem = "Dados invalidos.";
                serviceResponse.Success = false;
                return serviceResponse;
            }
            
            var usuario = await _usuarioRepository.GetByIdAsync(dados.Id);

            if (usuario == null)
            {
                serviceResponse.Mensagem = "Usuário não encontrado.";
                serviceResponse.Success = false;
                return serviceResponse;
            }
            
            var usuarioMesmoLogin = await _usuarioRepository.GetByNomeLoginAsync(dados.NomeLogin);
            if (usuarioMesmoLogin != null && usuarioMesmoLogin.Id != dados.Id)
            {
                serviceResponse.Mensagem = "Já existe um usuário com o nome de login informado.";
                serviceResponse.Success = false;
                return serviceResponse;
            }

            usuario.NomeCompleto = dados.NomeCompleto;
            usuario.NomeLogin = dados.NomeLogin;

            serviceResponse.Dados = (await _usuarioRepository.AtualizarUsuario(usuario)).toDTO();
            serviceResponse.Success = true;
            serviceResponse.Mensagem = "Usuário atualizado com sucesso.";
        }
        catch (Exception ex)
        {
            serviceResponse.Mensagem = $"Erro ao atualizar usuário: {ex.Message}";
            serviceResponse.Success = false;
        }

        return serviceResponse;
    }

    public async Task<ServiceResponse<UsuarioDTO>> ObterUsuarioPorId(Guid id)
    {
        ServiceResponse<UsuarioDTO> serviceResponse = new ServiceResponse<UsuarioDTO>();

        try
        {
            serviceResponse.Dados = (await _usuarioRepository.GetByIdAsync(id)).toDTO();

        }
        catch (Exception ex)
        {
            serviceResponse.Mensagem = ex.Message;
            serviceResponse.Success = false;
        }

        return serviceResponse;
    }
    
    public async Task<ServiceResponse<UsuarioDTO>> ObterUsuarioNomeLogin(string NomeLogin)
    {
        ServiceResponse<UsuarioDTO> serviceResponse = new ServiceResponse<UsuarioDTO>();

        try
        {
            serviceResponse.Dados = (await _usuarioRepository.GetByNomeLoginAsync(NomeLogin)).toDTO();

        }
        catch (Exception ex)
        {
            serviceResponse.Mensagem = ex.Message;
            serviceResponse.Success = false;
        }

        return serviceResponse;
    }

    public async Task<ServiceResponse<List<UsuarioDTO>>> ObterUsuarios()
    {
        ServiceResponse<List<UsuarioDTO>> serviceResponse = new ServiceResponse<List<UsuarioDTO>>();

        try
        {
            var usuarios = await _usuarioRepository.ObterUsuarios();
            serviceResponse.Dados = usuarios.Where(x => x.DhExclusao == null).Select(x => x.toDTO()).OrderBy(x => x.NomeCompleto).ToList();

        }
        catch (Exception ex)
        {
            serviceResponse.Mensagem = ex.Message;
            serviceResponse.Success = false;
        }

        return serviceResponse;
    }

    public async Task<ServiceResponse<UsuarioDTO>> InativarUsuarioPorId(Guid id)
    {
        ServiceResponse<UsuarioDTO> serviceResponse = new ServiceResponse<UsuarioDTO>();

        try
        {
            serviceResponse.Dados = (await _usuarioRepository.inativarUsuario(id)).toDTO();

        }
        catch (Exception ex)
        {
            serviceResponse.Mensagem = ex.Message;
            serviceResponse.Success = false;
        }

        return serviceResponse;

    }
    public async Task<ServiceResponse<UsuarioDTO>> CadastrarUsuario(UsuarioRequest dados)
    {
        ServiceResponse<UsuarioDTO> serviceResponse = new ServiceResponse<UsuarioDTO>();
        try
        {
            var usuarioMesmoLogin = await _usuarioRepository.GetByNomeLoginAsync(dados.NomeLogin);
            if (usuarioMesmoLogin != null)
            {
                serviceResponse.Mensagem = "Já existe um usuário com o nome de login informado.";
                serviceResponse.Success = false;
                return serviceResponse;
            }

            dados.Senha = _passwordHasher.HashPassword(null, dados.Senha);
            serviceResponse.Dados = (await _usuarioRepository.CadastrarUsuario(dados.ToEntity())).toDTO();
             
        }
        catch (Exception ex)
        {
            serviceResponse.Mensagem = ex.Message;
            serviceResponse.Success = false;
        }

        return serviceResponse;
    }
}