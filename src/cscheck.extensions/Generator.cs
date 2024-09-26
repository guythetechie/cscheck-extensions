using CsCheck;
using LanguageExt;
using System.Collections;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace cscheck.extensions;

public static class Generator
{
    public static Gen<ImmutableArray<T>> ImmutableArrayOf<T>(this Gen<T> gen) =>
        from array in gen.Array
        select array.ToImmutableArray();

    public static Gen<FrozenSet<T>> FrozenSetOf<T>(this Gen<T> gen, IEqualityComparer<T>? comparer = default) =>
        from array in gen.ImmutableArrayOf()
        select array.ToFrozenSet(comparer);

    public static Gen<ImmutableArray<T>> NonEmpty<T>(this Gen<ImmutableArray<T>> array) =>
        from items in array
        where items.Length > 0
        select items;

    public static Gen<FrozenSet<T>> NonEmpty<T>(this Gen<FrozenSet<T>> set) =>
        from items in set
        where items.Count > 0
        select items;

    public static Gen<ImmutableArray<T>> SubImmutableArrayOf<T>(ICollection<T> collection) =>
        collection.Count is 0
        ? Gen.Const(ImmutableArray<T>.Empty)
        : from items in Gen.Shuffle(collection.ToArray(), collection.Count)
          select items.ToImmutableArray();

    public static Gen<FrozenSet<T>> SubFrozenSetOf<T>(ICollection<T> collection, IEqualityComparer<T>? comparer = default)
    {
        var comparerToUse = (comparer, collection) switch
        {
            (null, FrozenSet<T> frozenSet) => frozenSet.Comparer,
            _ => comparer
        };

        return collection.Count is 0
                ? Gen.Const(FrozenSet<T>.Empty)
                : from items in Gen.Shuffle(collection.ToArray(), collection.Count)
                  select items.ToFrozenSet(comparerToUse);
    }

    public static Gen<Option<T>> OptionOf<T>(this Gen<T> gen) =>
        Gen.Frequency((1, Gen.Const(Option<T>.None)),
                      (4, gen.Select(Option<T>.Some)));

    public static Gen<ImmutableArray<T2>> TraverseToImmutableArray<T1, T2>(ICollection<T1> collection, Func<T1, Gen<T2>> f) =>
        collection.Select(f)
                  .SequenceToImmutableArray();

    /// <summary>
    /// Converts a list of generators into a generator of immutable arrays.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="gens"></param>
    /// <returns></returns>
    public static Gen<ImmutableArray<T>> SequenceToImmutableArray<T>(this IEnumerable<Gen<T>> gens) =>
        gens.Aggregate(Gen.Const(ImmutableArray<T>.Empty),
                       (acc, gen) => from items in acc
                                     from item in gen
                                     select items.Add(item));

    public static Gen<FrozenSet<T2>> TraverseToFrozenSet<T1, T2>(ICollection<T1> collection, Func<T1, Gen<T2>> f, IEqualityComparer<T2>? comparer = default) =>
        collection.Select(f)
                  .SequenceToFrozenSet(comparer);

    /// <summary>
    /// Converts a list of generators into a generator of frozen sets.
    /// </summary>
    public static Gen<FrozenSet<T>> SequenceToFrozenSet<T>(this IEnumerable<Gen<T>> gens, IEqualityComparer<T>? comparer = default) =>
        from items in gens.SequenceToImmutableArray()
        select items.ToFrozenSet(comparer);
}
