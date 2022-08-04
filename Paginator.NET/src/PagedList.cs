using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Paginator.NET;

public class PagedList<T> : IList<T> where T : class
{
	public PaginationBuilder PagingInfo { get; set; }

	public IList<T> Items { get; set; }

	public PagedList() { }




	#region --- GetPagedList ---

	public static PagedList<T> GetPagedList(
		int totalItemsCount,
		Func<int, int, IList<T>> getRange,
		int page,
		int itemsPerPage,
		int? maxDisplayPages = null,
		bool fixPageOutOfRange = true)
	{
		var list = _getPagedListBase(totalItemsCount, page, itemsPerPage, maxDisplayPages, fixPageOutOfRange);
		if(list == null || totalItemsCount == 0)
			return list;

		list.Items = getRange(list.PagingInfo.Index, list.PagingInfo.Count);
		return list;
	}

	public static async Task<PagedList<T>> GetPagedListAsync(
		int totalItemsCount,
		Func<int, int, Task<IList<T>>> getRange,
		int page,
		int itemsPerPage,
		int? maxDisplayPages = null,
		bool fixPageOutOfRange = true)
	{
		var list = _getPagedListBase(totalItemsCount, page, itemsPerPage, maxDisplayPages, fixPageOutOfRange);
		if(list == null || totalItemsCount == 0)
			return list;

		list.Items = await getRange(list.PagingInfo.Index, list.PagingInfo.Count);
		return list;
	}

	public static async Task<PagedList<T>> GetPagedListAsync(
		int totalItemsCount,
		Func<int, int, Task<T[]>> getRange,
		int page,
		int itemsPerPage,
		int? maxDisplayPages = null,
		bool fixPageOutOfRange = true)
	{
		var list = _getPagedListBase(totalItemsCount, page, itemsPerPage, maxDisplayPages, fixPageOutOfRange);
		if(list == null || totalItemsCount == 0)
			return list;

		list.Items = await getRange(list.PagingInfo.Index, list.PagingInfo.Count);
		return list;
	}

	static PagedList<T> _getPagedListBase(
		int totalItemsCount,
		int page,
		int itemsPerPage,
		int? maxDisplayPages,
		bool fixPageOutOfRange)
	{
		var list = new PagedList<T> {
			PagingInfo = PaginationBuilder.GetPagingInfo(
				totalItemsCount, itemsPerPage, page, maxDisplayPages, fixPageOutOfRange)
		};

		if(list.PagingInfo == null)
			return null;

		if(totalItemsCount == 0)
			list.Items = new T[0];

		return list;
	}

	#endregion




	// IList

	public int IndexOf(T item)
		=> Items.IndexOf(item);

	public T this[int index] {
		get => Items[index];
		set => Items[index] = value;
	}

	public bool Contains(T item)
		=> Items.Contains(item);

	public void CopyTo(T[] array, int arrayIndex)
		=> Items.CopyTo(array, arrayIndex);

	public int Count => Items.Count;

	public bool IsReadOnly => true;

	public IEnumerator<T> GetEnumerator()
		=> Items.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator()
		=> ((IEnumerable)(Items)).GetEnumerator();




	#region --- *Not Implemented* IList Members ---

	void ICollection<T>.Add(T item) => throw new NotImplementedException();

	void ICollection<T>.Clear() => throw new NotImplementedException();

	void IList<T>.Insert(int index, T item) => throw new NotImplementedException();

	bool ICollection<T>.Remove(T item) => throw new NotImplementedException();

	void IList<T>.RemoveAt(int index) => throw new NotImplementedException();

	#endregion

}
