using KRT.Onboarding.Application.Interfaces.Messaging;

namespace KRT.Onboarding.Infrastructure.Messaging
{
    public class EventPublisher : IEventPublisher
    {
        public Task PublishAsync<T>(T message, CancellationToken cancellationToken)
        {
            /*
             Aqui seria onde ocorreria a conexão com o serviço de mensageria e a publicação da mensagem seria feita
             */

            // Simula a publicação de sucesso do evento sem enviar para um broker oficial.
            return Task.CompletedTask;
        }
    }
}