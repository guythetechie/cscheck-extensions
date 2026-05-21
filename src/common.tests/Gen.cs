using CsCheck;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using TUnit.Assertions.Enums;

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

public class Generator_HashSetOf_Tests
{
    [Test]
    public async ValueTask Returns_values_generated_by_the_source()
    {
        var gen =
            from sources in
                from sources in StringGenerator.Any.Array
                where sources.Length > 0
                select sources
            from fixture in
                from fixture in Fixture.Generate()
                let fixtureGen = Gen.OneOfConst([.. sources])
                select fixture with { Gen = fixtureGen }
            from set in fixture.Run()
            select (sources, set);

        await gen.SampleAsync(async tuple =>
        {
            // Arrange
            var (sources, set) = tuple;

            // Assert
            await Assert.That(set.Except(sources))
                        .IsEmpty();
        });
    }

    [Test]
    public async ValueTask Returns_a_set_with_count_in_the_default_range()
    {
        var gen =
            from fixture in Fixture.Generate()
            from set in fixture.Run()
            select set;

        await gen.SampleAsync(async set =>
        {
            // Assert
            await Assert.That(set.Count).IsBetween(0, 10);
        });
    }

    [Test]
    public async ValueTask Returns_a_set_with_the_passed_comparer()
    {
        var gen =
            from fixture in Fixture.Generate()
            from set in fixture.Run()
            let inputComparer = fixture.Comparer
            let setComparer = set.KeyComparer
            from testArray in
                from testString in StringGenerator.Alphabetic
                from randomizedCases in StringGenerator.RandomizeCapitalization(testString).Array
                select randomizedCases.ToImmutableArray()
            select (inputComparer, setComparer, testArray);

        await gen.SampleAsync(async tuple =>
        {
            // Arrange
            var (inputComparer, setComparer, testArray) = tuple;

            // Assert
            await Assert.That(testArray.ToImmutableHashSet(inputComparer))
                        .IsEquivalentTo(testArray.ToImmutableHashSet(setComparer));
        });
    }

    private sealed record Fixture
    {
        public required Gen<string> Gen { get; init; }
        public IEqualityComparer<string>? Comparer { get; init; }

        public Gen<ImmutableHashSet<string>> Run() =>
            Gen.HashSetOf(Comparer);

        public static Gen<Fixture> Generate() =>
            from comparer in StringGenerator.Comparer
            select new Fixture
            {
                Gen = StringGenerator.Any,
                Comparer = comparer
            };
    }
}

public class Generator_HashSetOf_With_Length_Tests
{
    [Test]
    public async ValueTask Returns_values_generated_by_the_source()
    {
        var gen =
            from sources in
                from sources in StringGenerator.Any.Array
                where sources.Length > 0
                select sources
            from fixture in
                from fixture in Fixture.Generate()
                let fixtureGen = Gen.OneOfConst([.. sources])
                let sourcesSetCount = sources.ToImmutableHashSet(fixture.Comparer).Count
                from length in Gen.Int[0, sourcesSetCount / 10]
                select fixture with
                {
                    Gen = fixtureGen,
                    Length = length
                }
            from set in fixture.Run()
            select (sources, set);

        await gen.SampleAsync(async tuple =>
        {
            // Arrange
            var (sources, set) = tuple;

            // Assert
            await Assert.That(set.Except(sources))
                        .IsEmpty();
        });
    }

    [Test]
    public async ValueTask Returns_a_set_with_the_specified_length()
    {
        var gen =
            from fixture in Fixture.Generate()
            from set in fixture.Run()
            let length = fixture.Length
            select (length, set);

        await gen.SampleAsync(async tuple =>
        {
            // Arrange
            var (length, set) = tuple;

            // Assert
            await Assert.That(set.Count).IsEqualTo(length);
        });
    }

    [Test]
    public async ValueTask Returns_a_set_with_the_passed_comparer()
    {
        var gen =
            from fixture in Fixture.Generate()
            from set in fixture.Run()
            let inputComparer = fixture.Comparer
            let setComparer = set.KeyComparer
            from testArray in
                from testString in StringGenerator.Alphabetic
                from randomizedCases in StringGenerator.RandomizeCapitalization(testString).Array
                select randomizedCases.ToImmutableArray()
            select (inputComparer, setComparer, testArray);

        await gen.SampleAsync(async tuple =>
        {
            // Arrange
            var (inputComparer, setComparer, testArray) = tuple;

            // Assert
            await Assert.That(testArray.ToImmutableHashSet(inputComparer))
                        .IsEquivalentTo(testArray.ToImmutableHashSet(setComparer));
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

    private sealed record Fixture
    {
        public required Gen<string> Gen { get; init; }
        public required int Length { get; init; }
        public IEqualityComparer<string>? Comparer { get; init; }

        public Gen<ImmutableHashSet<string>> Run() =>
            Gen.HashSetOf(Length, Comparer);

        public static Gen<Fixture> Generate() =>
            from comparer in StringGenerator.Comparer
            from length in CsCheck.Gen.Int[0, 10]
            select new Fixture
            {
                Gen = StringGenerator.Any,
                Comparer = comparer,
                Length = length
            };
    }
}

public class Generator_HashSetOf_With_MinimumLength_And_MaximumLength_Tests
{
    [Test]
    public async ValueTask Returns_values_generated_by_the_source()
    {
        var gen =
            from sources in
                from sources in StringGenerator.Any.Array
                where sources.Length > 0
                select sources
            from fixture in
                from fixture in Fixture.Generate()
                let fixtureGen = Gen.OneOfConst([.. sources])
                let sourcesSetCount = sources.ToImmutableHashSet(fixture.Comparer).Count
                from minimumLength in Gen.Int[0, sourcesSetCount / 10]
                from maximumLength in Gen.Int[minimumLength, sourcesSetCount]
                select fixture with
                {
                    Gen = fixtureGen,
                    MinimumLength = minimumLength,
                    MaximumLength = maximumLength
                }
            from set in fixture.Run()
            select (sources, set);

        await gen.SampleAsync(async tuple =>
        {
            // Arrange
            var (sources, set) = tuple;

            // Assert
            await Assert.That(set.Except(sources))
                        .IsEmpty();
        });
    }

    [Test]
    public async ValueTask Returns_a_set_with_count_in_the_specified_range()
    {
        var gen =
            from fixture in Fixture.Generate()
            from set in fixture.Run()
            let minimumLength = fixture.MinimumLength
            let maximumLength = fixture.MaximumLength
            select (minimumLength, maximumLength, set);

        await gen.SampleAsync(async tuple =>
        {
            // Arrange
            var (minimumLength, maximumLength, set) = tuple;

            // Assert
            await Assert.That(set.Count).IsBetween(minimumLength, maximumLength);
        });
    }

    [Test]
    public async ValueTask Returns_a_set_with_the_passed_comparer()
    {
        var gen =
            from fixture in Fixture.Generate()
            from set in fixture.Run()
            let inputComparer = fixture.Comparer
            let setComparer = set.KeyComparer
            from testArray in
                from testString in StringGenerator.Alphabetic
                from randomizedCases in StringGenerator.RandomizeCapitalization(testString).Array
                select randomizedCases.ToImmutableArray()
            select (inputComparer, setComparer, testArray);

        await gen.SampleAsync(async tuple =>
        {
            // Arrange
            var (inputComparer, setComparer, testArray) = tuple;

            // Assert
            await Assert.That(testArray.ToImmutableHashSet(inputComparer))
                        .IsEquivalentTo(testArray.ToImmutableHashSet(setComparer));
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
        public required Gen<string> Gen { get; init; }
        public required int MinimumLength { get; init; }
        public required int MaximumLength { get; init; }
        public IEqualityComparer<string>? Comparer { get; init; }

        public Gen<ImmutableHashSet<string>> Run() =>
            Gen.HashSetOf(MinimumLength, MaximumLength, Comparer);

        public static Gen<Fixture> Generate() =>
            from comparer in StringGenerator.Comparer
            from minimumLength in CsCheck.Gen.Int[0, 10]
            from maximumLength in CsCheck.Gen.Int[minimumLength, 10]
            select new Fixture
            {
                Gen = StringGenerator.Any,
                Comparer = comparer,
                MinimumLength = minimumLength,
                MaximumLength = maximumLength
            };
    }
}

public class Generator_ArrayOf_Tests
{
    [Test]
    public async ValueTask Returns_values_generated_by_the_source()
    {
        var gen =
            from sources in
                from sources in StringGenerator.Any.Array
                where sources.Length > 0
                select sources
            from fixture in
                from fixture in Fixture.Generate()
                let fixtureGen = Gen.OneOfConst([.. sources])
                select fixture with { Gen = fixtureGen }
            from array in fixture.Run()
            select (sources, array);

        await gen.SampleAsync(async tuple =>
        {
            // Arrange
            var (sources, array) = tuple;

            // Assert
            await Assert.That(array.Except(sources))
                        .IsEmpty();
        });
    }

    [Test]
    public async ValueTask Returns_an_array_with_count_in_the_default_range()
    {
        var gen =
            from fixture in Fixture.Generate()
            from array in fixture.Run()
            select array;

        await gen.SampleAsync(async array =>
        {
            // Assert
            await Assert.That(array.Length).IsBetween(0, 10);
        });
    }

    private sealed record Fixture
    {
        public required Gen<string> Gen { get; init; }

        public Gen<ImmutableArray<string>> Run() =>
            Gen.ArrayOf();

        public static Gen<Fixture> Generate() =>
            CsCheck.Gen.Const(new Fixture
            {
                Gen = StringGenerator.Any
            });
    }
}

public class Generator_ArrayOf_With_Length_Tests
{
    [Test]
    public async ValueTask Returns_values_generated_by_the_source()
    {
        var gen =
            from sources in
                from sources in StringGenerator.Any.Array
                where sources.Length > 0
                select sources
            from fixture in
                from fixture in Fixture.Generate()
                let fixtureGen = Gen.OneOfConst([.. sources])
                select fixture with { Gen = fixtureGen }
            from array in fixture.Run()
            select (sources, array);

        await gen.SampleAsync(async tuple =>
        {
            // Arrange
            var (sources, array) = tuple;

            // Assert
            await Assert.That(array.Except(sources))
                        .IsEmpty();
        });
    }

    [Test]
    public async ValueTask Returns_an_array_with_the_specified_length()
    {
        var gen =
            from fixture in Fixture.Generate()
            from array in fixture.Run()
            let length = fixture.Length
            select (length, array);

        await gen.SampleAsync(async tuple =>
        {
            // Arrange
            var (length, array) = tuple;

            // Assert
            await Assert.That(array.Length).IsEqualTo(length);
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

    private sealed record Fixture
    {
        public required Gen<string> Gen { get; init; }
        public required int Length { get; init; }

        public Gen<ImmutableArray<string>> Run() =>
            Gen.ArrayOf(Length);

        public static Gen<Fixture> Generate() =>
            from length in CsCheck.Gen.Int[0, 10]
            select new Fixture
            {
                Gen = StringGenerator.Any,
                Length = length
            };
    }
}

public class Generator_ArrayOf_With_MinimumLength_And_MaximumLength_Tests
{
    [Test]
    public async ValueTask Returns_values_generated_by_the_source()
    {
        var gen =
            from sources in
                from sources in StringGenerator.Any.Array
                where sources.Length > 0
                select sources
            from fixture in
                from fixture in Fixture.Generate()
                let fixtureGen = Gen.OneOfConst([.. sources])
                select fixture with { Gen = fixtureGen }
            from array in fixture.Run()
            select (sources, array);

        await gen.SampleAsync(async tuple =>
        {
            // Arrange
            var (sources, array) = tuple;

            // Assert
            await Assert.That(array.Except(sources))
                        .IsEmpty();
        });
    }

    [Test]
    public async ValueTask Returns_an_array_with_count_in_the_specified_range()
    {
        var gen =
            from fixture in Fixture.Generate()
            from array in fixture.Run()
            let minimumLength = fixture.MinimumLength
            let maximumLength = fixture.MaximumLength
            select (minimumLength, maximumLength, array);

        await gen.SampleAsync(async tuple =>
        {
            // Arrange
            var (minimumLength, maximumLength, array) = tuple;

            // Assert
            await Assert.That(array.Length).IsBetween(minimumLength, maximumLength);
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
        public required Gen<string> Gen { get; init; }
        public required int MinimumLength { get; init; }
        public required int MaximumLength { get; init; }

        public Gen<ImmutableArray<string>> Run() =>
            Gen.ArrayOf(MinimumLength, MaximumLength);

        public static Gen<Fixture> Generate() =>
            from minimumLength in CsCheck.Gen.Int[0, 10]
            from maximumLength in CsCheck.Gen.Int[minimumLength, 10]
            select new Fixture
            {
                Gen = StringGenerator.Any,
                MinimumLength = minimumLength,
                MaximumLength = maximumLength
            };
    }
}

public class Generator_Traverse_Tests
{
    [Test]
    public async ValueTask Satisfies_identity_law()
    {
        var gen =
            from fixture in
                from fixture in Fixture.Generate()
                let mapper = new Func<object, Gen<object>>(x => Gen.Const(x))
                select fixture with { Mapper = mapper }
            from result in fixture.Run()
            let source = fixture.Source
            select (source, result);

        await gen.SampleAsync(async tuple =>
        {
            // Arrange
            var (source, result) = tuple;

            // Assert
            await Assert.That(result).IsEquivalentTo(source, CollectionOrdering.Matching);
        });
    }

    [Test]
    public async ValueTask Satisfies_naturality_law()
    {
        var gen =
            from g in MapperGenerator.ObjectToObject
            from fixture in Fixture.Generate()
            from result1 in
                from result in fixture.Run()
                select result.Select(g)
            from result2 in
                from updatedFixture in
                    Gen.Const(fixture with
                    {
                        Mapper = x => fixture.Mapper(x).Select(g)
                    })
                from result in updatedFixture.Run()
                select result
            select (result1, result2);

        await gen.SampleAsync(async tuple =>
        {
            // Arrange
            var (result1, result2) = tuple;

            // Assert
            await Assert.That(result1).IsEquivalentTo(result2, CollectionOrdering.Matching);
        });
    }

    [Test]
    public async ValueTask Does_not_reuse_accumulator_between_samples()
    {
        var source = new[] { 1, 2 };
        var traversed = Generator.Traverse(source, Gen.Const);

        var gen =
            from first in traversed
            from second in traversed
            select (first, second);

        await gen.SampleAsync(async tuple =>
        {
            // Arrange
            var (first, second) = tuple;

            // Assert
            await Assert.That(first).IsEquivalentTo(source, CollectionOrdering.Matching);
            await Assert.That(second).IsEquivalentTo(source, CollectionOrdering.Matching);
        });
    }

    private sealed record Fixture
    {
        public required IEnumerable<object> Source { get; init; }
        public required Func<object, Gen<object>> Mapper { get; init; }

        public Gen<ImmutableArray<object>> Run() =>
            Generator.Traverse(Source, Mapper);

        public static Gen<Fixture> Generate() =>
            from source in Generator.Object.Array
            from mapper in
                from mapper in MapperGenerator.ObjectToObject
                select new Func<object, Gen<object>>(x => Gen.Const(mapper(x)))
            select new Fixture
            {
                Source = source,
                Mapper = mapper
            };
    }
}