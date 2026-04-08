using System.Text;

namespace FpvLadderBot;

internal static class PilotLinkFormatter {
    internal static string FormatPilotLink(string pilotId) {
        string[] split = pilotId.Split(':');
        string linkId = split[0].Replace('_', '/');
        string linkClass = split[1];
        
        string link = new StringBuilder("https://").Append((string?)Constants.FpvLadderUrl)
            .Append("/pilot/")
            .Append(linkId)
            .Append(".html")
            .Append('#')
            .Append(linkClass)
            .ToString();

        return link.ToEscapedMarkdownV2();
    }
}
