using System;
using System.Collections.Generic;

namespace IKDFrontEnd.BookModels;

public partial class Product
{
    public int Id { get; set; }

    public short CategoryId { get; set; }

    public int? BrandId { get; set; }

    public string? ProductName { get; set; }

    public string? Model { get; set; }

    public string? Sku { get; set; }

    public decimal? Price { get; set; }

    public decimal Discount { get; set; }

    public string? Description { get; set; }

    public string? ProductCompany { get; set; }

    public string? ProductFeatures { get; set; }

    public string? ProductSpecification { get; set; }

    public int? Quantity { get; set; }

    public string? MetaTitle { get; set; }

    public string? MetaKeyWords { get; set; }

    public string? MetaDescription { get; set; }

    public string? ShortDescription { get; set; }

    public string? ProductTags { get; set; }

    public bool? IsExclusive { get; set; }

    public decimal? DiscountPercentage { get; set; }

    public decimal? Discountfixed { get; set; }

    public decimal? NetPrice { get; set; }

    public string? Url { get; set; }

    public string? Authorname { get; set; }

    public bool? IsStock { get; set; }

    public string? IsbnNumber { get; set; }

    public bool? Optionional { get; set; }

    public bool? IsDelete { get; set; }

    public string? Usprice { get; set; }

    public virtual ProductBrand? Brand { get; set; }

    public virtual ICollection<OrderDetail1> OrderDetail1s { get; set; } = new List<OrderDetail1>();

    public virtual ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();

    public virtual ICollection<ProductPhoto> ProductPhotos { get; set; } = new List<ProductPhoto>();

    public virtual ICollection<TblProductColor> TblProductColors { get; set; } = new List<TblProductColor>();
}
