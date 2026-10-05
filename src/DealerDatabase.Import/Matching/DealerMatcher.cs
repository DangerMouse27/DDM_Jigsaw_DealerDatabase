
using DealerDatabase.Import.Models;
using DealerDatabase.Import.Normalization;

namespace DealerDatabase.Import.Matching;

/// <summary>
/// Groups source records that refer to the same real-world dealership.
/// Hard identifier matches always merge; softer signals must meet 'DedupeConfidenceFloor' />.
/// </summary>
public static class DealerMatcher
{
    /// <summary>
    /// Minimum confidence score (0–100) required for two records to be treated as the same dealer
    /// when matching on soft signals such as postcode and name similarity.
    /// </summary>
    public const int DedupeConfidenceFloor = 70;

    public static IReadOnlyList<IReadOnlyList<SourceRecord>> Match(IReadOnlyList<SourceRecord> records)
    {
        var uf = new UnionFind(records.Count);

        // Certainty merges — shared regulatory / registry identifiers.
        UnionByKey(records, uf, r => r.CompanyNumber);
        UnionByKey(records, uf, r => r.VatNumber);
        UnionByKey(records, uf, r => r.FcaFrn);
        UnionByKey(records, uf, r => r.MarketcheckDealerId);
        UnionByKey(records, uf, r => r.SafMemberId);
        UnionByKey(records, uf, r => r.IcoRegistrationNumber);

        // Soft merges — only when the pairwise score meets DedupeConfidenceFloor.
        UnionByScoredSignals(records, uf);

        return records
            .Select((record, index) => (record, root: uf.Find(index)))
            .GroupBy(x => x.root)
            .Select(g => (IReadOnlyList<SourceRecord>)g.Select(x => x.record).ToList())
            .ToList();
    }

    /// <summary>
    /// Scores how likely two normalised source records describe the same dealership (0–100).
    /// </summary>
    public static int ScorePair(SourceRecord left, SourceRecord right)
    {
        if (SameKey(left.CompanyNumber, right.CompanyNumber)
            || SameKey(left.VatNumber, right.VatNumber)
            || SameKey(left.FcaFrn, right.FcaFrn)
            || SameKey(left.MarketcheckDealerId, right.MarketcheckDealerId)
            || SameKey(left.SafMemberId, right.SafMemberId)
            || SameKey(left.IcoRegistrationNumber, right.IcoRegistrationNumber))
        {
            return 100;
        }

        var score = 0;

        if (SameKey(left.WebsiteDomain, right.WebsiteDomain))
        {
            score = Math.Max(score, 88);
        }

        var leftPostcode = left.Postcode ?? left.RegisteredPostcode;
        var rightPostcode = right.Postcode ?? right.RegisteredPostcode;
        if (SameKey(leftPostcode, rightPostcode) && leftPostcode is not null)
        {
            if (string.Equals(left.NameKey, right.NameKey, StringComparison.Ordinal))
            {
                score = Math.Max(score, 92);
            }
            else if (Normalizer.NamesLikelyMatch(left.NameKey, right.NameKey))
            {
                score = Math.Max(score, 76);
            }
            else
            {
                score = Math.Max(score, 35);
            }
        }
        else if (Normalizer.NamesLikelyMatch(left.NameKey, right.NameKey)
                 && SameKey(left.Phone, right.Phone)
                 && left.Phone is not null)
        {
            score = Math.Max(score, 72);
        }

        return score;
    }

    private static void UnionByKey(
        IReadOnlyList<SourceRecord> records,
        UnionFind uf,
        Func<SourceRecord, string?> keySelector)
    {
        var seen = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        for (var i = 0; i < records.Count; i++)
        {
            var key = keySelector(records[i]);
            if (key is null)
            {
                continue;
            }

            if (seen.TryGetValue(key, out var existing))
            {
                uf.Union(existing, i);
            }
            else
            {
                seen[key] = i;
            }
        }
    }

    private static void UnionByScoredSignals(IReadOnlyList<SourceRecord> records, UnionFind uf)
    {
        // Domain matches are strong enough on their own when above the floor.
        UnionByKeyWhenScoreMet(records, uf, r => r.WebsiteDomain);

        var byPostcode = new Dictionary<string, List<int>>(StringComparer.OrdinalIgnoreCase);
        for (var i = 0; i < records.Count; i++)
        {
            var postcode = records[i].Postcode ?? records[i].RegisteredPostcode;
            if (postcode is null)
            {
                continue;
            }

            if (!byPostcode.TryGetValue(postcode, out var list))
            {
                list = [];
                byPostcode[postcode] = list;
            }

            list.Add(i);
        }

        foreach (var indexes in byPostcode.Values)
        {
            for (var a = 0; a < indexes.Count; a++)
            {
                for (var b = a + 1; b < indexes.Count; b++)
                {
                    var left = records[indexes[a]];
                    var right = records[indexes[b]];
                    if (ScorePair(left, right) >= DedupeConfidenceFloor)
                    {
                        uf.Union(indexes[a], indexes[b]);
                    }
                }
            }
        }

        // Phone + name soft matches across postcodes.
        var byPhone = new Dictionary<string, List<int>>(StringComparer.OrdinalIgnoreCase);
        for (var i = 0; i < records.Count; i++)
        {
            var phone = records[i].Phone;
            if (phone is null)
            {
                continue;
            }

            if (!byPhone.TryGetValue(phone, out var list))
            {
                list = [];
                byPhone[phone] = list;
            }

            list.Add(i);
        }

        foreach (var indexes in byPhone.Values)
        {
            for (var a = 0; a < indexes.Count; a++)
            {
                for (var b = a + 1; b < indexes.Count; b++)
                {
                    if (ScorePair(records[indexes[a]], records[indexes[b]]) >= DedupeConfidenceFloor)
                    {
                        uf.Union(indexes[a], indexes[b]);
                    }
                }
            }
        }
    }

    private static void UnionByKeyWhenScoreMet(
        IReadOnlyList<SourceRecord> records,
        UnionFind uf,
        Func<SourceRecord, string?> keySelector)
    {
        var seen = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        for (var i = 0; i < records.Count; i++)
        {
            var key = keySelector(records[i]);
            if (key is null)
            {
                continue;
            }

            if (seen.TryGetValue(key, out var existing))
            {
                if (ScorePair(records[existing], records[i]) >= DedupeConfidenceFloor)
                {
                    uf.Union(existing, i);
                }
            }
            else
            {
                seen[key] = i;
            }
        }
    }

    private static bool SameKey(string? left, string? right)
        => left is not null
           && right is not null
           && string.Equals(left, right, StringComparison.OrdinalIgnoreCase);

    private sealed class UnionFind(int size)
    {
        private readonly int[] _parent = Enumerable.Range(0, size).ToArray();
        private readonly int[] _rank = new int[size];

        public int Find(int x)
        {
            while (_parent[x] != x)
            {
                _parent[x] = _parent[_parent[x]];
                x = _parent[x];
            }

            return x;
        }

        public void Union(int a, int b)
        {
            var rootA = Find(a);
            var rootB = Find(b);
            if (rootA == rootB)
            {
                return;
            }

            if (_rank[rootA] < _rank[rootB])
            {
                _parent[rootA] = rootB;
            }
            else if (_rank[rootA] > _rank[rootB])
            {
                _parent[rootB] = rootA;
            }
            else
            {
                _parent[rootB] = rootA;
                _rank[rootA]++;
            }
        }
    }
}
