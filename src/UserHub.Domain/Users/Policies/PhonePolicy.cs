using System.Text.RegularExpressions;

namespace UserHub.Domain.Users.Policies;

public sealed partial class PhonePolicy
{
    [GeneratedRegex(@"^\+[1-9]\d{6,14}$")]
    private static partial Regex E164Regex();

    [GeneratedRegex(@"[\s\-()]")]
    private static partial Regex SeparatorRegex();

    public string? Normalize(string? input) =>
        string.IsNullOrWhiteSpace(input) ? null : SeparatorRegex().Replace(input, string.Empty);

    public bool IsValid(string? normalized) =>
        normalized is null || E164Regex().IsMatch(normalized);
}