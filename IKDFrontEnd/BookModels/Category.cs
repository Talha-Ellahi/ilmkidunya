using System;
using System.Collections.Generic;

namespace IKDFrontEnd.BookModels;

public partial class Category
{
    public short Id { get; set; }

    public string? CategoryName { get; set; }

    public short? SortOrder { get; set; }

    public short? ParentCategoryId { get; set; }

    public string? CategoryImage { get; set; }

    public string? MetaTitle { get; set; }

    public string? MetaKeyWords { get; set; }

    public string? MetaDescription { get; set; }

    public string? CatIcon { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<Category> InverseParentCategory { get; set; } = new List<Category>();

    public virtual Category? ParentCategory { get; set; }

    public virtual ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();

    public virtual ICollection<Service> Services { get; set; } = new List<Service>();
}
