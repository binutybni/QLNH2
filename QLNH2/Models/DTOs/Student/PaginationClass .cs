namespace QLNH2.Models.DTOs.Student
{
    public class PaginationClass
    {
        public int Page { get; set; }

        public int PageSize { get; set; }

        public string? SearchTerm { get; set; }
    }
}
