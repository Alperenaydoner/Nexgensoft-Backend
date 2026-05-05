namespace CoreService.Common;

/// <summary>
/// Sayfalı liste istekleri için ortak parametreler (<c>page</c> / <c>pageSize</c> sınırları ve <c>Skip</c>).
/// </summary>
public readonly struct PageRequest : IEquatable<PageRequest>
{
    /// <summary>Geçersiz veya küçük değerlerde kullanılan varsayılan sayfa boyutu.</summary>
    public const int DefaultPageSize = 20;

    /// <summary>Üst sınır (DoS ve aşırı bellek yükünü sınırlar).</summary>
    public const int MaxPageSize = 100;

    /// <summary>Minimum sayfa numarası (1 tabanlı).</summary>
    public const int MinPageNumber = 1;

    public PageRequest(int pageNumber, int pageSize)
    {
        PageNumber = pageNumber < MinPageNumber ? MinPageNumber : pageNumber;
        var s = pageSize < 1 ? DefaultPageSize : pageSize;
        PageSize = s > MaxPageSize ? MaxPageSize : s;
    }

    /// <summary>1 tabanlı sayfa numarası.</summary>
    public int PageNumber { get; }

    /// <summary>Sayfa başına kayıt sayısı (1–<see cref="MaxPageSize"/>).</summary>
    public int PageSize { get; }

    /// <summary>EF <c>Skip</c> için ofset.</summary>
    public int Skip => (PageNumber - 1) * PageSize;

    public bool Equals(PageRequest other) => PageNumber == other.PageNumber && PageSize == other.PageSize;

    public override bool Equals(object? obj) => obj is PageRequest other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(PageNumber, PageSize);
}
