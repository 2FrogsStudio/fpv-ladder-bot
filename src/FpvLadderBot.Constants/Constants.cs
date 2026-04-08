namespace FpvLadderBot;

public static class Constants {
    public const string FpvLadderUrl = "fpvladder.ru";
    public const string FpvLadderIndexYamlBase = "https://raw.githubusercontent.com/eterverda/fpvladder/main/data/pilot/";
    public static readonly DateTimeOffset ApplicationStartDate = DateTimeOffset.UtcNow;
    public static readonly TimeZoneInfo RussianTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Moscow");
}
