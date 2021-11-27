using DeviceManager.Core.Proto;
using Shared;

namespace DeviceManager.Core.Extensions
{
    public static class PaginationMataDataExtension
    {
        public static PaginationMetaData FromPagedList<T>(this PaginationMetaData self, PagedList<T> list)
        {
            self.TotalCount = list.TotalCount;
            self.PageSize = list.PageSize;
            self.CurrentPage = list.CurrentPage;
            self.TotalPages = list.TotalPages;
            self.HasNext = list.HasNext;
            self.HasPrevious = list.HasPrevious;
            return self;
        }
    }
}