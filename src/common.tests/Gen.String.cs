using CsCheck;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace common.tests;

public class StringGenerator_Any_Tests
{
    [Test]
    public async ValueTask Is_never_null()
    {
        var gen = StringGenerator.Any;

        await gen.SampleAsync(async str =>
        {
            await Assert.That(str).IsNotNull();
        });
    }
}

public class StringGenerator_Whitespace_Tests
{
    [Test]
    public async ValueTask Contains_only_whitespace_characters()
    {
        var gen = StringGenerator.Whitespace;

        await gen.SampleAsync(async str =>
        {
            await Assert.That(str.AsEnumerable())
                        .All(char.IsWhiteSpace);
        });
    }

    [Test]
    public async ValueTask Can_generate_empty_values()
    {
        var gen = StringGenerator.Whitespace;

        await Assert.That(gen).CanGenerate(str => str is []);
    }
}

public class StringGenerator_NullOrWhitespace_Tests
{
    [Test]
    public async ValueTask Is_null_or_whitespace()
    {
        var gen = StringGenerator.NullOrWhitespace;

        await gen.SampleAsync(async str =>
        {
            await Assert.That(str).IsNullOrWhiteSpace();
        });
    }

    [Test]
    public async ValueTask Can_generate_null_values()
    {
        var gen = StringGenerator.NullOrWhitespace;

        await Assert.That(gen).CanGenerate(str => str is null);
    }

    [Test]
    public async ValueTask Can_generate_empty_values()
    {
        var gen = StringGenerator.NullOrWhitespace;

        await Assert.That(gen).CanGenerate(str => str is (not null) and []);
    }

    [Test]
    public async ValueTask Can_generate_whitespace_values()
    {
        var gen = StringGenerator.NullOrWhitespace;

        await Assert.That(gen).CanGenerate(str => str is (not null) and (not []) && str.All(char.IsWhiteSpace));
    }
}

public class StringGenerator_NonNullOrWhitespace_Tests
{
    [Test]
    public async ValueTask Is_not_null_or_whitespace()
    {
        var gen = StringGenerator.NonNullOrWhitespace;

        await gen.SampleAsync(async str =>
        {
            await Assert.That(str).IsNotNullOrWhiteSpace();
        });
    }
}

public class StringGenerator_Guid_Tests
{
    [Test]
    public async ValueTask Is_a_valid_guid()
    {
        var gen = StringGenerator.Guid;

        await gen.SampleAsync(async str =>
        {
            await Assert.That(Guid.TryParse(str, out _)).IsTrue();
        });
    }

    [Test]
    public async ValueTask Can_generate_an_empty_guid()
    {
        var gen = StringGenerator.Guid;

        await Assert.That(gen).CanGenerate(str => Guid.TryParse(str, out var guid) && guid == Guid.Empty);
    }
}

public class StringGenerator_NonEmptyGuid_Tests
{
    [Test]
    public async ValueTask Is_a_valid_guid()
    {
        var gen = StringGenerator.NonEmptyGuid;

        await gen.SampleAsync(async str =>
        {
            await Assert.That(Guid.TryParse(str, out _)).IsTrue();
        });
    }

    [Test]
    public async ValueTask Is_not_an_empty_guid()
    {
        var gen = StringGenerator.NonEmptyGuid;

        await gen.SampleAsync(async str =>
        {
            await Assert.That(Guid.Parse(str))
                        .IsNotEqualTo(Guid.Empty);
        });
    }
}

public class StringGenerator_Comparer_Tests
{
    [Test]
    public async ValueTask Comparer_is_reflexive()
    {
        var gen =
            from comparer in StringGenerator.Comparer
            from str in StringGenerator.Any
            select (comparer, str);

        await gen.SampleAsync(async tuple =>
        {
            var (comparer, str) = tuple;

            await Assert.That(comparer.Equals(str, str))
                        .IsTrue();
        });
    }

    [Test]
    public async ValueTask Comparer_is_symmetric()
    {
        var gen =
            from sources in
                from sources in StringGenerator.Any.ArrayOf()
                where sources.Length > 0
                from randomCases in Generator.Traverse(sources, StringGenerator.RandomizeCapitalization)
                select sources.AddRange(randomCases)
            from str1 in Gen.OneOfConst([.. sources])
            from str2 in Gen.OneOfConst([.. sources])
            from comparer in StringGenerator.Comparer
            select (comparer, str1, str2);

        await gen.SampleAsync(async tuple =>
        {
            var (comparer, str1, str2) = tuple;

            await Assert.That(comparer.Equals(str1, str2))
                        .IsEqualTo(comparer.Equals(str2, str1));
        });
    }

    [Test]
    public async ValueTask Comparer_is_transitive()
    {
        var gen =
            from comparer in StringGenerator.Comparer
            from sources in
                from sources in StringGenerator.Any.ArrayOf()
                where sources.Length > 0
                from randomCases in Generator.Traverse(sources, StringGenerator.RandomizeCapitalization)
                select sources.AddRange(randomCases)
            from str1 in Gen.OneOfConst([.. sources])
            from str2 in Gen.OneOfConst([.. sources])
            from str3 in Gen.OneOfConst([.. sources])
            where comparer.Equals(str1, str2) && comparer.Equals(str2, str3)
            select (comparer, str1, str3);

        await gen.SampleAsync(async tuple =>
        {
            var (comparer, str1, str3) = tuple;

            await Assert.That(comparer.Equals(str1, str3))
                        .IsTrue();
        });
    }

    [Test]
    public async ValueTask Equal_values_have_the_same_hash_code()
    {
        var gen =
            from sources in
                from sources in StringGenerator.Any.ArrayOf()
                where sources.Length > 0
                from randomCases in Generator.Traverse(sources, StringGenerator.RandomizeCapitalization)
                select sources.AddRange(randomCases)
            from str1 in Gen.OneOfConst([.. sources])
            from str2 in Gen.OneOfConst([.. sources])
            from comparer in StringGenerator.Comparer
            where comparer.Equals(str1, str2)
            select (comparer, str1, str2);

        await gen.SampleAsync(async tuple =>
        {
            var (comparer, str1, str2) = tuple;

            await Assert.That(comparer.GetHashCode(str1))
                        .IsEqualTo(comparer.GetHashCode(str2));
        });
    }
}

public class StringGenerator_AlphaNumeric_Tests
{
    [Test]
    public async ValueTask Is_not_empty()
    {
        var gen = StringGenerator.AlphaNumeric;

        await gen.SampleAsync(async str =>
        {
            await Assert.That(string.IsNullOrEmpty(str))
                        .IsFalse();
        });
    }

    [Test]
    public async ValueTask Contains_only_alpha_numeric_characters()
    {
        var gen = StringGenerator.AlphaNumeric;

        await gen.SampleAsync(async str =>
        {
            await Assert.That(str.AsEnumerable())
                        .All(char.IsLetterOrDigit);
        });
    }

    [Test]
    public async ValueTask Can_generate_letters()
    {
        var gen = StringGenerator.AlphaNumeric;

        await Assert.That(gen).CanGenerate(str => str.Any(char.IsLetter));
    }

    [Test]
    public async ValueTask Can_generate_digits()
    {
        var gen = StringGenerator.AlphaNumeric;

        await Assert.That(gen).CanGenerate(str => str.Any(char.IsDigit));
    }
}

public class StringGenerator_Alphabetic_Tests
{
    [Test]
    public async ValueTask Is_not_empty()
    {
        var gen = StringGenerator.Alphabetic;

        await gen.SampleAsync(async str =>
        {
            await Assert.That(string.IsNullOrEmpty(str))
                        .IsFalse();
        });
    }

    [Test]
    public async ValueTask Contains_only_alphabetic_characters()
    {
        var gen = StringGenerator.Alphabetic;

        await gen.SampleAsync(async str =>
        {
            await Assert.That(str.AsEnumerable())
                        .All(char.IsLetter);
        });
    }

    [Test]
    public async ValueTask Can_generate_uppercase_letters()
    {
        var gen = StringGenerator.Alphabetic;

        await Assert.That(gen).CanGenerate(str => str.Any(char.IsUpper));
    }

    [Test]
    public async ValueTask Can_generate_lowercase_letters()
    {
        var gen = StringGenerator.Alphabetic;

        await Assert.That(gen).CanGenerate(str => str.Any(char.IsLower));
    }
}

public class StringGenerator_RandomizeCapitalization_Tests
{
    [Test]
    public async ValueTask Normalized_output_matches_input()
    {
        var gen = from input in StringGenerator.Any
                  from output in StringGenerator.RandomizeCapitalization(input)
                  select (input, output);

        await gen.SampleAsync(async tuple =>
        {
            var (input, output) = tuple;

            await Assert.That(input.ToUpperInvariant())
                        .IsEqualTo(output.ToUpperInvariant());
        });
    }
}