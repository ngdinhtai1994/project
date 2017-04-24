using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace BaiTap.Models
{
    public class ProductViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Name")]
        [Required(ErrorMessage = "{0} is required")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [RegularExpression(@"^\d*\.?\d*$", ErrorMessage = "{0} only accepts numbers, not letters or symbols (ex: !@#$%^&* ) ")]
        public decimal Price { get; set; }

        [RegularExpression(@"^\d*\.?\d*$", ErrorMessage = "{0} only accepts numbers, not letters or symbols (ex: !@#$%^&* ) ")]
        public Nullable<decimal> Discount { get; set; }

        public string Thumbnail { get; set; }

        [Display(Name = "Short Description")]
        [Required(ErrorMessage = "{0} is required")]
        public string ShortDescription { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        public string Description { get; set; }

        [Display(Name = "Category")]
        [Required(ErrorMessage = "{0} is required")]
        public Nullable<int> CategoryId { get; set; }

        [Display(Name = "Sort Order")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "{0} only accepts numbers, not letters or symbols (ex: !@#$%^&* ) ")]
        public Nullable<int> SortOrder { get; set; }

        [RegularExpression(@"^[0-9]+$", ErrorMessage = "{0} only accepts numbers, not letters or symbols (ex: !@#$%^&* ) ")]
        public Nullable<int> Status { get; set; }

        public virtual Category Category { get; set; }

        public Nullable<int> CreatedBy { get; set; }

        public Nullable<System.DateTime> CreatedAt { get; set; } 

        public virtual List<ImageProduct> Images
        {
            get;
            set;
        }
    }
}