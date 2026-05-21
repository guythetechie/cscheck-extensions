using CsCheck;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace common.tests;

public class Generator_SubSetOf_Tests
{
    [Test]
    public async ValueTask Returns_a_subset_of_the_input_collection()
    {
        var gen =
            from fixture in Fixture.Generate()
            from subset in fixture.Run()
            let input = fixture.Collection
            let comparer = fixture.Comparer
            select (input, comparer, subset);

        await gen.SampleAsync(async tuple =>
        {
            // Arrange
            var (input, comparer, subset) = tuple;

            // Assert
            await Assert.That(subset.Except(input, comparer))
                        .IsEmpty();
        });
    }

    [Test]
    public async ValueTask Returns_a_subset_with_the_passed_comparer()
    {
        var gen =
            from fixture in Fixture.Generate()
            from subset in fixture.Run()
            from testArray in
                from testString in StringGenerator.Alphabetic
                from randomizedCases in StringGenerator.RandomizeCapitalization(testString).Array
                select randomizedCases.ToImmutableArray()
            let inputComparer = fixture.Comparer
            let subsetComparer = subset.KeyComparer
            select (inputComparer, subsetComparer, testArray);

        await gen.SampleAsync(async tuple =>
        {
            // Arrange
            var (inputComparer, subsetComparer, testArray) = tuple;

            // Assert
            await Assert.That(testArray.ToImmutableHashSet(inputComparer))
                        .IsEquivalentTo(testArray.ToImmutableHashSet(subsetComparer));
        });
    }

    private sealed record Fixture
    {
        public required ICollection<string> Collection { get; init; }
        public IEqualityComparer<string>? Comparer { get; init; }
        public Gen<ImmutableHashSet<string>> Run() => Generator.SubSetOf(Collection, Comparer);

        public static Gen<Fixture> Generate() =>
            from collection in Gen.String.Array
            from comparer in StringGenerator.Comparer
            select new Fixture
            {
                Collection = collection,
                Comparer = comparer
            };
    }
}

public class Generator_SubSetOf_With_Length_Tests
{
    [Test]
    public async ValueTask Returns_a_subset_of_the_input_collection()
    {
        var gen =
            from fixture in Fixture.Generate()
            from subset in fixture.Run()
            let input = fixture.Collection
            let comparer = fixture.Comparer
            select (input, comparer, subset);

        await gen.SampleAsync(async tuple =>
        {
            // Arrange
            var (input, comparer, subset) = tuple;

            // Assert
            await Assert.That(subset.Except(input, comparer))
                        .IsEmpty();
        });
    }

    [Test]
    public async ValueTask Returns_a_subset_with_the_specified_length()
    {
        var gen =
            from fixture in Fixture.Generate()
            from subset in fixture.Run()
            let length = fixture.Length
            select (length, subset);

        await gen.SampleAsync(async tuple =>
        {
            // Arrange
            var (length, subset) = tuple;

            // Assert
            await Assert.That(subset.Count).IsEqualTo(length);
        });
    }

    [Test]
    public async ValueTask Returns_a_subset_with_the_passed_comparer()
    {
        var gen =
            from fixture in Fixture.Generate()
            from subset in fixture.Run()
            from testArray in
                from testString in StringGenerator.Alphabetic
                from randomizedCases in StringGenerator.RandomizeCapitalization(testString).Array
                select randomizedCases.ToImmutableArray()
            let inputComparer = fixture.Comparer
            let subsetComparer = subset.KeyComparer
            select (inputComparer, subsetComparer, testArray);

        await gen.SampleAsync(async tuple =>
        {
            // Arrange
            var (inputComparer, subsetComparer, testArray) = tuple;

            // Assert
            await Assert.That(testArray.ToImmutableHashSet(inputComparer))
                        .IsEquivalentTo(testArray.ToImmutableHashSet(subsetComparer));
        });
    }

    [Test]
    public async ValueTask Throws_if_the_length_is_less_than_zero()
    {
        var gen =
            from fixture in Fixture.Generate()
            from length in Gen.Int[int.MinValue, -1]
            let updatedFixture = fixture with { Length = length }
            select updatedFixture;

        await gen.SampleAsync(async fixture =>
        {
            // Assert
            await Assert.That(fixture.Run).Throws<ArgumentOutOfRangeException>();
        });
    }

    [Test]
    public async ValueTask Throws_if_the_length_is_greater_than_the_set_count()
    {
        var gen =
            from fixture in Fixture.Generate()
            from length in Gen.Int[fixture.Collection.ToImmutableHashSet(fixture.Comparer).Count + 1, int.MaxValue]
            let updatedFixture = fixture with { Length = length }
            select updatedFixture;

        await gen.SampleAsync(async fixture =>
        {
            // Assert
            await Assert.That(fixture.Run).Throws<ArgumentOutOfRangeException>();
        });
    }

    private sealed record Fixture
    {
        public required ICollection<string> Collection { get; init; }
        public required int Length { get; init; }
        public IEqualityComparer<string>? Comparer { get; init; }

        public Gen<ImmutableHashSet<string>> Run() => Generator.SubSetOf(Collection, Length, Comparer);

        public static Gen<Fixture> Generate() =>
            from collection in Gen.String.Array
            from comparer in StringGenerator.Comparer
            from length in Gen.Int[0, collection.ToImmutableHashSet(comparer).Count]
            select new Fixture
            {
                Collection = collection,
                Comparer = comparer,
                Length = length
            };
    }
}

public class Generator_SubSetOf_With_MinimumLength_And_MaximumLength_Tests
{
    [Test]
    public async ValueTask Returns_a_subset_of_the_input_collection()
    {
        var gen =
            from fixture in Fixture.Generate()
            from subset in fixture.Run()
            let input = fixture.Collection
            let comparer = fixture.Comparer
            select (input, comparer, subset);

        await gen.SampleAsync(async tuple =>
        {
            // Arrange
            var (input, comparer, subset) = tuple;

            // Assert
            await Assert.That(subset.Except(input, comparer))
                        .IsEmpty();
        });
    }

    [Test]
    public async ValueTask Returns_a_subset_with_count_in_the_specified_range()
    {
        var gen =
            from fixture in Fixture.Generate()
            from subset in fixture.Run()
            let minimumLength = fixture.MinimumLength
            let maximumLength = fixture.MaximumLength
            select (minimumLength, maximumLength, subset);

        await gen.SampleAsync(async tuple =>
        {
            // Arrange
            var (minimumLength, maximumLength, subset) = tuple;

            // Assert
            await Assert.That(subset.Count).IsBetween(minimumLength, maximumLength);
        });
    }

    [Test]
    public async ValueTask Returns_a_subset_with_the_passed_comparer()
    {
        var gen =
            from fixture in Fixture.Generate()
            from subset in fixture.Run()
            from testArray in
                from testString in StringGenerator.Alphabetic
                from randomizedCases in StringGenerator.RandomizeCapitalization(testString).Array
                select randomizedCases.ToImmutableArray()
            let inputComparer = fixture.Comparer
            let subsetComparer = subset.KeyComparer
            select (inputComparer, subsetComparer, testArray);

        await gen.SampleAsync(async tuple =>
        {
            // Arrange
            var (inputComparer, subsetComparer, testArray) = tuple;

            // Assert
            await Assert.That(testArray.ToImmutableHashSet(inputComparer))
                        .IsEquivalentTo(testArray.ToImmutableHashSet(subsetComparer));
        });
    }

    [Test]
    public async ValueTask Throws_if_the_minimum_length_is_less_than_zero()
    {
        var gen =
            from fixture in Fixture.Generate()
            from minimumLength in Gen.Int[int.MinValue, -1]
            let updatedFixture = fixture with { MinimumLength = minimumLength }
            select updatedFixture;

        await gen.SampleAsync(async fixture =>
        {
            // Assert
            await Assert.That(fixture.Run).Throws<ArgumentOutOfRangeException>();
        });
    }

    [Test]
    public async ValueTask Throws_if_the_minimum_length_is_greater_than_the_set_count()
    {
        var gen =
            from fixture in Fixture.Generate()
            from minimumLength in Gen.Int[fixture.Collection.ToImmutableHashSet(fixture.Comparer).Count + 1, int.MaxValue]
            let updatedFixture = fixture with { MinimumLength = minimumLength }
            select updatedFixture;

        await gen.SampleAsync(async fixture =>
        {
            // Assert
            await Assert.That(fixture.Run).Throws<ArgumentOutOfRangeException>();
        });
    }

    [Test]
    public async ValueTask Throws_if_the_minimum_length_is_greater_than_the_maximum_length()
    {
        var gen =
            from fixture in Fixture.Generate()
            from minimumLength in Gen.Int[fixture.MaximumLength + 1, int.MaxValue]
            let updatedFixture = fixture with { MinimumLength = minimumLength }
            select updatedFixture;

        await gen.SampleAsync(async fixture =>
        {
            // Assert
            await Assert.That(fixture.Run).Throws<ArgumentOutOfRangeException>();
        });
    }

    private sealed record Fixture
    {
        public required ICollection<string> Collection { get; init; }
        public required int MinimumLength { get; init; }
        public required int MaximumLength { get; init; }
        public IEqualityComparer<string>? Comparer { get; init; }

        public Gen<ImmutableHashSet<string>> Run() => Generator.SubSetOf(Collection, MinimumLength, MaximumLength, Comparer);

        public static Gen<Fixture> Generate() =>
            from collection in Gen.String.Array
            from comparer in StringGenerator.Comparer
            let set = collection.ToImmutableHashSet(comparer)
            from minimumLength in Gen.Int[0, set.Count]
            from maximumLength in Gen.Int[minimumLength, int.MaxValue]
            select new Fixture
            {
                Collection = collection,
                Comparer = comparer,
                MinimumLength = minimumLength,
                MaximumLength = maximumLength
            };
    }
}