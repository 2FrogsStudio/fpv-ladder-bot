namespace FpvLadderBot.Bot.Tests;

public class UriTryCreateTests {
    [Theory]
    [InlineData("https://fpvladder.ru/pilots/?id=52a31ad")]
    [InlineData("/pilots/?id=52a31ad")]
    public void TestUriTryCreate(string uriString) {
        var baseUri = new Uri($"https://{Constants.FpvLadderUrl}");
        if (!Uri.TryCreate(uriString, UriKind.Absolute, out Uri? uri)
            || string.IsNullOrWhiteSpace(uri.Host)) {
            uri = new Uri(baseUri, uriString);
        }

        uri.IsAbsoluteUri.ShouldBeTrue();
        uri.AbsolutePath.ShouldBe("/pilots/");
        uri.PathAndQuery.ShouldBe("/pilots/?id=52a31ad");
    }
}
