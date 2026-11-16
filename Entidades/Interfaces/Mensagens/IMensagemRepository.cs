using Entidades.Mensagens;

namespace Entidades.Interfaces.Mensagens;

public interface IMensagemRepository
{
    int Adicionar(Mensagem mensagem);
    List<Mensagem> Listar();
}
