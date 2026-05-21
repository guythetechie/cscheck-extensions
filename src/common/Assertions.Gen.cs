using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CsCheck;
using TUnit.Assertions.Core;
using TUnit.Assertions.Sources;

namespace common;

public sealed class CanGenerateWithPredicateAssertion<T>(AssertionContext<Gen<T>> context, Func<T, bool> predicate, int attempts) : Assertion<Gen<T>>(context)
{
    private T? value;

    public ValueAssertion<T> Which
    {
        get
        {
            Context.ExpressionBuilder.Append(".Which");

            return new TAssertion(Context.Map(async _ =>
            {
                await AssertAsync();

                return value;
            }));
        }
    }

    protected override string GetExpectation()
    {
        return $"to find a value that satisfies the predicate";
    }

    protected override async Task<AssertionResult> CheckAsync(EvaluationMetadata<Gen<T>> metadata)
    {
        await ValueTask.CompletedTask;

        value = default;
        var found = false;

        if (metadata.Exception is not null)
        {
            return AssertionResult.Failed($"threw {metadata.Exception.GetType().Name} with message {metadata.Exception.Message}", metadata.Exception);
        }

        if (metadata.Value is not { } source)
        {
            return AssertionResult.Failed("was null");
        }

        source.Sample(t =>
        {
            if (found)
            {
                return;
            }

            if (predicate(t))
            {
                found = true;
                value = t;
            }
        }, iter: attempts, threads: 1);

        return found
                ? AssertionResult.Passed
                : AssertionResult.Failed($"found none within {attempts} attempts.");
    }

    private sealed class TAssertion(AssertionContext<T> context) : ValueAssertion<T>(context);
}

public static class GenAssertionExtensions
{
    extension<T>(IAssertionSource<Gen<T>> source)
    {
        public CanGenerateWithPredicateAssertion<T> CanGenerate(Func<T, bool> predicate, int attempts = 1000, [CallerArgumentExpression(nameof(predicate))] string? predicateExpression = null)
        {
#pragma warning disable CA1305 // Specify IFormatProvider
            source.Context.ExpressionBuilder.Append($".CanGenerate({predicateExpression})");
#pragma warning restore CA1305 // Specify IFormatProvider
            return new(source.Context, predicate, attempts);
        }
    }
}