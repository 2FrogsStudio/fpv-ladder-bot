using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FpvLadderBot;

public static class HostApplicationBuilderExtensions {
    public static HostApplicationBuilder AddFpvLadderClient(this HostApplicationBuilder builder) {
        builder.Services.AddHttpClient<IFpvLadderClient, FpvLadderClient>()
            .ConfigureHttpClient(client => {
                client.BaseAddress = new Uri(Constants.FpvLadderIndexYamlBase);
            });

        return builder;
    }
}
