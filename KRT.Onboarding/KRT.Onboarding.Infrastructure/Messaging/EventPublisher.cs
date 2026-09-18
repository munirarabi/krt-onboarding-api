using KRT.Onboarding.Application.Interfaces.Messaging;

namespace KRT.Onboarding.Infrastructure.Messaging
{
    // Implementação simplificada do publicador de eventos
    // mas não realiza integração com um broker real (como RabbitMQ, AWS SQS ou algum outro serviço de mensageria..).
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