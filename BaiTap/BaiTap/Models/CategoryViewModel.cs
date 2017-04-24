using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace BaiTap.Models
{
    public class CategoryViewModel
    {
        public int CategoryId { get; set; }

        [Display(Name = "Category Name")]
        [Required(ErrorMessage = "{0} is required")]
        public string CategoryName { get; set; }

        public string Description { get; set; }

        public string Thumbnail { get; set; }

        [Display(Name = "Parent Category")]
        public Nullable<int> ParentId { get; set; }

        [Display(Name = "Total Items")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "{0} only accepts numbers, not letters or symbols (ex: !@#$%^&* ) ")]
        public Nullable<int> TotalItems { get; set; }

        [Display(Name = "Sort Order")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "{0} only accepts numbers, not letters or symbols (ex: !@#$%^&* ) ")]
        public Nullable<int> SortOrder { get; set; }

        [RegularExpression(@"^[0-9]+$", ErrorMessage = "{0} only accepts numbers, not letters or symbols (ex: !@#$%^&* ) ")]
        public Nullable<int> Status { get; set; }
    }
}