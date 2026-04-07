using MassTransit;

namespace FpvLadderBot;

public interface IMediatorConsumer<in T> : IConsumer<T> where T : class;
