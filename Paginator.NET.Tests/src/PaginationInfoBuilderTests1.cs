using DotNetXtensions;
using DotNetXtensions.Test;

using Xunit;

namespace Paginator.NET.Tests
{
	public class Builder_4PagesPlusExtra : XUnitTestBase
	{
		static readonly int[] _arr = { 10, 11, 12, 20, 21, 22, 30, 31, 32, 40, 41 };

		[Theory]
		[InlineData(1, 0, 3, "10,11,12")]
		[InlineData(2, 3, 3, "20,21,22")]
		[InlineData(3, 6, 3, "30,31,32")]

		// tests requests at last page >
		[InlineData(4, 9, 2, "40,41")]
		[InlineData(5, 9, 2, "40,41", 4)]
		[InlineData(30, 9, 2, "40,41", 4)]
		public void Test1(int page, int itemsIdx, int itemsOnCurrPg, string itemsStr, int finalPageNum = -1)
		{
			const int _itemsPerPage = 3;

			if(finalPageNum < 1)
				finalPageNum = page;

			int itemsIdx2 = (finalPageNum - 1) * _itemsPerPage;
			True(itemsIdx == itemsIdx2);

			True(_arr.Length == 11);

			PaginationInfo pg = GetPgInfo(_arr.Length, itemsPerPage: _itemsPerPage, currentPage: page);

			True(pg.TotalItemCount == _arr.Length);
			True(pg.CurrentPage == finalPageNum);
			True(pg.Index == itemsIdx);
			True(pg.Count == itemsOnCurrPg);

			int totalPages = 4;
			True(pg.PagesCount == totalPages);
			True(pg.TotalPageCount == totalPages);
			True(pg.PageNumbers.Length == totalPages);

			int[] slice = _arr[pg.Index..(pg.Index + pg.Count)];
			string result = slice.JoinToString(",");
			True(itemsStr == result);
		}

		PaginationInfo GetPgInfo(int totalItemCount, int itemsPerPage, int currentPage)
		{
			var pgBldr = PaginationBuilder.GetPagingInfo(
				totalItemCount: totalItemCount,
				itemsPerPage: itemsPerPage,
				currentPage: currentPage,
				maxDisplayPages: 12,
				fixPageOutOfRange: true);

			PaginationInfo pageInfo = pgBldr.ToPaginationInfo();
			return pageInfo;
		}
	}
}
