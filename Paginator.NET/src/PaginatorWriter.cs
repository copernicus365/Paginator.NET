using System;

namespace Paginator.NET
{
	public static class PaginatorWriterX
	{
		public static bool WritePages(this PaginationBuilder pageInfo, IPaginatorWriter writer)
		{
			var pi = pageInfo ?? throw new ArgumentNullException(nameof(pageInfo));
			var w = writer ?? throw new ArgumentNullException(nameof(writer));

			if(pi.TotalItemCount == 0 || pi.TotalPageCount <= 1)
				return false;

			int[] pages = pi.Pages;
			int currPg = pi.CurrentPage;
			int pagesCountMinusFirstLast = pi.PagesCount - (pi.HasGapAfterStart ? 1 : 0) - (pi.HasGapBeforeEnd ? 1 : 0);

			bool hasPrev = w.CanShowPrevious && currPg - 1 > 0;
			bool hasNext = w.CanShowNext && currPg + 1 <= pi.TotalPageCount;

			//  --- Write Prev Chapt ---
			if(w.Chapters && pi.HasGapAfterStart) {
				int prevChJumpPage = Math.Max(pi.CurrentPage - Math.Max(pagesCountMinusFirstLast, 3), 1);

				w.WritePrevNextPage(
					prevChJumpPage,
					isNext: false,
					isForChapter: true);
			}

			//  --- Write Prev ---
			if(w.AlwaysShowPrevNext || hasPrev) {
				w.WritePrevNextPage(
					currPg - 1,
					isNext: false,
					isDisabled: !hasPrev);
			}

			//  --- Write Main Links ---
			int lastIdx = pages.Length - 1;
			bool showGap = w.ShowGap;

			for(int i = 0; i < pages.Length; i++) {
				int page = pages[i];

				if(showGap && i == lastIdx && pi.HasGapBeforeEnd)
					w.WriteGap();

				w.WritePage(
					page,
					isCurrent: page == currPg,
					isDisabled: false);

				if(showGap && i == 0 && pi.HasGapAfterStart)
					w.WriteGap();
			}

			//  --- Write Next ---
			if(w.AlwaysShowPrevNext || hasNext) {
				w.WritePrevNextPage(
					currPg + 1,
					isNext: true,
					isDisabled: !hasNext);
			}

			//  --- Write Next Chapt ---
			if(w.Chapters && pi.HasGapBeforeEnd) {
				int nextChJumpPage = Math.Min(pi.CurrentPage + Math.Max(pagesCountMinusFirstLast, 3), pi.TotalPageCount);
				w.WritePrevNextPage(
					nextChJumpPage,
					isNext: true,
					isForChapter: true);
			}
			return true;
		}

	}
}
