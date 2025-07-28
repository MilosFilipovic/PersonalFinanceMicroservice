

using Domain.Entities;

namespace Domain.Models
{
    public class Category
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string? ParentCode { get; set; }



        public Category() { }


        public Category(string code, string name, string? parentCode = null)
        {
            Code = code;
            Name = name;
            ParentCode = parentCode;
        }

        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    }
}
