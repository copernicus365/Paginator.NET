using System;

namespace Paginator.NET
{
	public class PaginationBuilder
	{
		#region FIELDS

		/// <summary>
		/// Maximum number of pages to display.
		/// It's best for this to be an ODD number, that way spans on each side will be equal.
		/// </summary>
		public static int DefaultMaxPagesCount = 13; // including 1rst and last 


		public readonly int CurrentPage;
		public readonly int Index; // CurrentPageItemsIndex
		public readonly int Count; // ItemsOnCurrentPage
		public readonly int ItemsPerPage;
		public readonly int TotalItemCount;
		public readonly int TotalPageCount;

		/// <summary>
		/// Maximum number of pages to display.
		/// It's best for this to be an ODD number, 
		/// that way spans on each side will be equal.
		/// </summary>
		public readonly int MaxPagesCount = DefaultMaxPagesCount; // including 1rst and last 
		public readonly bool ShowFirstLastPages = true;


		/// <summary>
		/// The array of pages for display, the number of which is limited by 
		/// <see cref="MaxPagesCount"/>.
		/// </summary>
		public int[] Pages => _pages;
		int[] _pages = Array.Empty<int>(); // helps for count==0 ?

		/// <summary>
		/// The count of the final display <see cref="Pages"/>. This is different from <see cref="TotalPageCount"/>.
		/// </summary>
		public int PagesCount => _pages?.Length ?? 0;

		#endregion

		#region CONSTRUCTOR and INIT

		/// <summary>
		/// Constructor, which recieves the readonly numeric values that will dictate this page info.
		/// </summary>
		/// <param name="totalItemCount">Total number of items in the source collection.</param>
		/// <param name="itemsPerPage">Number of items from source to display per page.</param>
		/// <param name="currentPage">Current *page* (this will often have been dictated by e.g. a user clicking a given page to navigate to).</param>
		/// <param name="maxDisplayPages">The number of pages you want to generate (links or what not) for. 
		/// This sets the maximum length of the <see cref="Pages"/> array of numbers. The final number can be lower
		/// when not enough items from input values made up that many pages.</param>
		public PaginationBuilder(
			int totalItemCount,
			int itemsPerPage,
			int currentPage,
			int? maxDisplayPages = null,
			bool? showFirstLastPages = null,
			bool fixPageOutOfRange = true)
		{
			TotalPageCount = GetTotalPageCountAndValidateParams(totalItemCount, itemsPerPage, fixPageOutOfRange, ref currentPage);

			TotalItemCount = totalItemCount;
			ItemsPerPage = Math.Min(itemsPerPage, TotalItemCount);
			CurrentPage = currentPage;
			MaxPagesCount = Math.Max(maxDisplayPages ?? DefaultMaxPagesCount, 3);

			if(showFirstLastPages != null)
				ShowFirstLastPages = showFirstLastPages.Value;

			//if (MaxDisplayPages < 3) -- just set a max above and be done with it...
			//	throw new ArgumentOutOfRangeException(nameof(maxDisplayPages), "Max pages to display cannot be less than 3");
			if(TotalPageCount < 0)
				throw new ArgumentOutOfRangeException();
			else if(TotalPageCount > 0) {
				Index = ItemsPerPage * (CurrentPage - 1);
				SetPageNumbers();
			}

			Count = TotalPageCount < 2 || CurrentPage < TotalPageCount
					? ItemsPerPage
					: ItemsPerPage - ((TotalPageCount * ItemsPerPage) - TotalItemCount);
		}

		static int GetTotalPageCountAndValidateParams(int totalItemCount, int itemsPerPage, bool fixPageOutOfRange, ref int currentPage)
		{
			if(totalItemCount == 0)
				return 0;

			if(currentPage < 1 && fixPageOutOfRange)
				currentPage = 1;

			if(totalItemCount < 1 || itemsPerPage < 1 || currentPage < 1)
				return -1;

			int totalPageCount = (int)Math.Ceiling((decimal)totalItemCount / itemsPerPage);

			if(currentPage > totalPageCount) {
				if(!fixPageOutOfRange || totalPageCount < 1)
					return -1;
				currentPage = totalPageCount;
			}

			return totalPageCount;
		}

		public static PaginationBuilder GetPagingInfo(
			int totalItemCount,
			int itemsPerPage,
			int currentPage,
			int? maxDisplayPages = null,
			bool fixPageOutOfRange = true)
		{
			// oops! maxDisplayPages not being used!
			if(GetTotalPageCountAndValidateParams(totalItemCount, itemsPerPage, fixPageOutOfRange, ref currentPage) < 0)
				return null;

			return new PaginationBuilder(totalItemCount, itemsPerPage, currentPage, maxDisplayPages);
		}

		#endregion

		public bool IsLastPage
			=> TotalPageCount < 1 || CurrentPage >= TotalPageCount;

		public bool IsPageInRange(int page)
			=> page > 0 && page <= TotalPageCount;

		public bool HasGapAfterStart
			=> PagesCount > 2 && _pages[1] > 2; // let's just read page 2, bypass if first page blah blah

		public bool HasGapBeforeEnd
			=> PagesCount > 2 && _pages[^2] < (TotalPageCount - 1);


		public PaginationInfo ToPaginationInfo()
		{
			return new PaginationInfo() {
				CurrentPage = CurrentPage,
				Index = Index,
				HasGapAfterStart = HasGapAfterStart,
				HasGapBeforeEnd = HasGapBeforeEnd,
				IsLastPage = IsLastPage,
				Count = Count,
				ItemsPerPage = ItemsPerPage,
				PageNumbers = _pages ?? Array.Empty<int>(),
				PagesCount = PagesCount,
				ShowFirstLastPages = ShowFirstLastPages,
				TotalItemCount = TotalItemCount,
				TotalPageCount = TotalPageCount
			};
		}

		// --- CORE FUNCTIONS ---

		void SetPageNumbers()
		{
			if(TotalPageCount == 0)
				return;

			// AFTER this point, some pages ARE going to be Cut...!
			int cnt = Math.Min(MaxPagesCount, TotalPageCount);
			_pages = new int[cnt];

			if(TotalPageCount <= MaxPagesCount) {
				for(int i = 0; i < cnt; i++)
					_pages[i] = i + 1;
				return;
			}

			int sides = cnt / 2; // division of a odd never rounds up, even if > .5, so (5 / 2) = 2 

			int start = Math.Max(CurrentPage - sides, 1); // note: `start` is 1 based (it is in fact a page #), not index based
			if(start + cnt > TotalPageCount)
				start = TotalPageCount - cnt + 1;

			for(int i = 0; i < cnt; i++)
				_pages[i] = i + start;

			if(ShowFirstLastPages) {
				// REMEMBER: After this point, 
				_pages[0] = 1;
				_pages[cnt - 1] = TotalPageCount;

				//cnt = MaxDisplayPages - 2;
			}




			//bool _isBeginningRun = CurrentPage < middleCnt; //5;
			//bool _isEndRun = CurrentPage > TotalPageCount - 4;

			//if (_isBeginningRun)
			//	for (int i = 1; i <= cnt; i++)
			//		_displayPages[i] = i + 1;
			//else if (_isEndRun)
			//	for (int i = cnt, start = TotalPageCount - 1; i > 0; i--, start--)
			//		_displayPages[i] = start;
			//else { // _isMidRun
			//	int _start = CurrentPage - middleCnt;
			//	for (int i = 1, start = _start; i <= cnt; i++, start++) //CurrentPage - 3
			//		_displayPages[i] = start;
			//}
		}
	}
}
