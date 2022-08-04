namespace Paginator.NET;

/// <summary>
/// Represents the data results output (ideally)
/// from <see cref="PaginationBuilder"/>. The purpose of
/// this type is to be ideally an immutable and data properties
/// only type communicating a pagination results, leaving the
/// complicated process of building those results to the separate
/// builder.
/// </summary>
public class PaginationInfo
{
	public int CurrentPage { get; set; }

	/// <summary>
	/// Starting index into original items for <see cref="CurrentPage"/>.
	/// It is often this and <see cref="Count"/> that one needs
	/// to know what items to get from your flat list of items.
	/// </summary>
	public int Index { get; set; }

	/// <summary>
	/// Count of items after <see cref="Index"/> that represent
	/// the items for this page.
	/// </summary>
	public int Count { get; set; }

	public int ItemsPerPage { get; set; }

	public int TotalItemCount { get; set; }

	public int TotalPageCount { get; set; }

	public int PagesCount { get; set; }

	public bool HasGapAfterStart { get; set; }

	public bool HasGapBeforeEnd { get; set; }

	public bool IsLastPage { get; set; }

	public int[] PageNumbers { get; set; }

	public bool ShowFirstLastPages { get; set; }

	public bool IsPageInRange(int page)
		=> page > 0 && page <= TotalPageCount;

}
