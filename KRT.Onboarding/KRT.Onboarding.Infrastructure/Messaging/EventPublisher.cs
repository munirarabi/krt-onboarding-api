using KRT.Onboarding.Application.Interfaces.Messaging;
using KRT.Onboarding.Domain.Events;

namespace KRT.Onboarding.Infrastructure.Messaging
{
    public class EventPublisher : IEventPublisher
    {
        public Task PublishAsync<T>(T message, CancellationToken cancellationToken)
        {
            // Aqui seria onde ocorreria a conexão com o serviço de mensageria e a publicação da mensagem seria feita
            // Simula a publicação de sucesso do evento sem enviar para um broker oficial

            switch (message)
            {
                case AccountCreatedEvent:
                    // Publicar evento de criação
                    Console.WriteLine("Publicar evento de create para as áreas");
                    break;

                case AccountUpdatedEvent:
                    // Publicar evento de atualização
                    Console.WriteLine("Publicar evento de update para as áreas");
                    break;

                case AccountDeletedEvent:
                    // Publicar evento de exclusão
                    Console.WriteLine("Publicar evento de delete para as áreas");
                    break;

                default:
                    throw new InvalidOperationException(
                        $"Event type '{typeof(T).Name}' não é suportado");
            }

            return Task.CompletedTask;
        }
    }
}