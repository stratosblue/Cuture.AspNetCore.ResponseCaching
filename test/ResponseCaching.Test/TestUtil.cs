using System.Linq;

using Cuture.AspNetCore.ResponseCaching.ResponseCaches;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ResponseCaching.Test;

public static class TestUtil
{
    public static void EntryEquals(ResponseCacheEntry entry1, ResponseCacheEntry entry2)
    {
        Assert.AreEqual(entry1.ContentType, entry2.ContentType);
        Assert.AreEqual(entry1.Body.Length, entry2.Body.Length);
        Assert.IsTrue(Enumerable.SequenceEqual(entry1.Body.ToArray(), entry2.Body.ToArray()));
    }
}
