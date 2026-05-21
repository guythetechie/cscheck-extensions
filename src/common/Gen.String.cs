using CsCheck;
using System;
using System.Collections.Generic;
using System.Linq;

namespace common;

public static class StringGenerator
{
    public static Gen<string?> NullOrWhitespace { get; } =
        from str in Gen.String.Null()
        where string.IsNullOrWhiteSpace(str)
        select str;

    public static Gen<string> NonNullOrWhitespace { get; } =
        from str in Gen.String
        where string.IsNullOrWhiteSpace(str) is false
        select str;

#pragma warning disable CA1720 // Identifier contains type name
    public static Gen<string> Guid { get; } =
#pragma warning restore CA1720 // Identifier contains type name
        from guid in Gen.Guid
        select guid.ToString();

    public static Gen<string> NonEmptyGuid { get; } =
        from guid in Gen.Guid
        where guid != System.Guid.Empty
        select guid.ToString();

    public static Gen<string> AlphaNumeric { get; } =
        from str in Gen.String.AlphaNumeric
        where string.IsNullOrEmpty(str) is false
        select str;

    public static Gen<IEqualityComparer<string>?> Comparer { get; } =
        Gen.OneOfConst<IEqualityComparer<string>?>(null,
                                                  StringComparer.Ordinal,
                                                  StringComparer.OrdinalIgnoreCase,
                                                  StringComparer.CurrentCulture,
                                                  StringComparer.CurrentCultureIgnoreCase,
                                                  StringComparer.InvariantCulture,
                                                  StringComparer.InvariantCultureIgnoreCase);

    public static Gen<string> Alphabetic { get; } =
        from chars in Gen.Char['a', 'z'].Array
        where chars.Length > 0
        select new string([.. chars]);

    public static Gen<string> RandomizeCapitalization(string input) =>
        from characters in
            Generator.Traverse(input,
                               character => Gen.OneOfConst(char.ToUpperInvariant(character),
                                                           char.ToLowerInvariant(character)))
        select new string([.. characters]);
}