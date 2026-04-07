namespace FpvLadderBot.Jobs;

public class UpdatePilotsJobConsumerDefinition : ConsumerDefinition<UpdatePilotsJobConsumer> {
    public UpdatePilotsJobConsumerDefinition() {
        ConcurrentMessageLimit = 1;
    }
}
