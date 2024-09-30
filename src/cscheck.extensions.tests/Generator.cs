using CsCheck;
using FluentAssertions;
using System.Linq;
using Xunit;

namespace cscheck.extensions.tests;


public class GeneratorTests
{
    [Fact]
    public void NonEmpty_ImmutableArray_is_not_empty()
    {
        var generator = Gen.Byte
                           .ImmutableArrayOf()
                           .NonEmpty();

        generator.Sample(array => array.Should().NotBeEmpty());
    }

    [Fact]
    public void NonEmpty_FrozenSet_is_not_empty()
    {
        var generator = Gen.Byte
                           .FrozenSetOf()
                           .NonEmpty();

        generator.Sample(set => set.Should().NotBeEmpty());
    }

    [Fact]
    public void SubImmutableArrayOf_returns_subset()
    {
        var generator = from source in Gen.String.List
                        from subset in Generator.SubImmutableArrayOf(source)
                        select (source, subset);

        generator.Sample((source, subset) => subset.Should().BeSubsetOf(source));
    }

    [Fact]
    public void SubFrozenSetOf_returns_subset()
    {
        var generator = from source in Gen.String.List
                        from subset in Generator.SubFrozenSetOf(source)
                        select (source, subset);

        generator.Sample((source, subset) => subset.Should().BeSubsetOf(source));
    }

    [Fact]
    public void OptionOf_returns_somes_or_nones()
    {
        var generator = Gen.Byte
                           .OptionOf()
                           .Array[1000];

        generator.Sample(options =>
        {
            var somesPercentage = options.Count(option => option.IsSome) / (double)options.Length;
            somesPercentage.Should().BeApproximately(0.8, 0.1);
        });
    }
}
