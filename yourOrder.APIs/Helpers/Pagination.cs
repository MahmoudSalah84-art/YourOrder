using yourOrder.APIs.DTOs.ProductDto;

namespace yourOrder.APIs.Helpers
{
    public class Pagination<T>
    {
        private int totalItems;
        private IEnumerable<ProductToReturnDto> data;

        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int Count { get; set; }
        public IEnumerable<T> Data { get; set; }


        public Pagination(int pageIndex, int pageSize, int Count, IEnumerable<T> Data)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            this.Count = Count;
            this.Data = Data;
        }

    }
        
}


