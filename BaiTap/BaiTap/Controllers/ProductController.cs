using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BaiTap.Models;
using System.IO;

namespace BaiTap.Controllers
{
    public class ProductController : Controller
    {
        OnlineStoreEntities db = new OnlineStoreEntities();
        // GET: Product
        public ActionResult Index()
        {
            var lstProduct = db.Products.Select(m => new ProductViewModel
            {
                Id = m.Id,
                Category = m.Category,
                Description = m.Description,
                Discount = m.Discount,
                Price = m.Price,
                ProductName = m.ProductName,
                ShortDescription = m.ShortDescription,
                SortOrder = m.SortOrder,
                Status = m.Status,
                Thumbnail = m.Thumbnail,
            }).ToList();
            return View(lstProduct);
        }

        //get create page
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(db.Categories.ToList(), "CategoryId", "CategoryName");
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(ProductViewModel viewModel, IEnumerable<HttpPostedFileBase> files)
        {
            ViewBag.CategoryId = new SelectList(db.Categories.ToList(), "CategoryId", "CategoryName", viewModel.CategoryId);
            if (ModelState.IsValid)
            {
                //biến dùng để lưu sản phẩm 1 lần duy nhất
                bool saveProduct = true;
                //create product to get id
                Product product = new Product();
                foreach (var file in files)
                {
                    if (file != null)
                    {
                        if (file.ContentLength > 0)
                        {
                            string extension = Path.GetExtension(file.FileName).ToLower();
                            // check type file
                            if (extension != ".png" && extension != ".gif" && extension != ".jpg")
                            {
                                ViewBag.Notice = "Invalid file format. Please choice again.";
                                return View(viewModel);
                            }
                            //create file name                       
                            var fileName = Path.GetFileName(file.FileName);
                            // check size
                            if (file.ContentLength > 2097152)
                            {
                                ViewBag.Notice = "Invalid file size. Please choice again.";
                                return View(viewModel);
                            }
                            string filePath = "/Uploads/" + Guid.NewGuid().ToString() + "/";

                            if (!Directory.Exists(Server.MapPath(filePath)))
                            {
                                Directory.CreateDirectory(Server.MapPath(filePath));
                            }
                            //create path of file 
                            var path = Path.Combine(Server.MapPath(filePath), fileName);
                            file.SaveAs(path);
                            viewModel.Thumbnail = filePath + fileName;
                            //save product
                            if (saveProduct)
                            {

                                product.ProductName = viewModel.ProductName;
                                product.Price = viewModel.Price;
                                product.Discount = viewModel.Discount;
                                product.Thumbnail = viewModel.Thumbnail;
                                product.ShortDescription = viewModel.ShortDescription;
                                product.Description = viewModel.Description;
                                product.CategoryId = viewModel.CategoryId;
                                product.SortOrder = viewModel.SortOrder;
                                product.Status = viewModel.Status;
                                product.CreatedAt = DateTime.Now;
                                db.Products.Add(product);
                                db.SaveChanges();
                                saveProduct = false;
                            }

                            //create images product
                            var imgProduct = new ImageProduct
                            {
                                ProductId = product.Id,
                                ImagePath = viewModel.Thumbnail,
                                CreatedAt = DateTime.Now
                            };
                            db.ImageProducts.Add(imgProduct);
                            db.SaveChanges();
                        }
                    }

                }

                return RedirectToAction("Index");
            }
            return View(viewModel);
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            var product = db.Products.Find(id);
            if (product != null)
            {
                ProductViewModel viewModel = new ProductViewModel
                {
                    Category = product.Category,
                    Discount = product.Discount,
                    Id = product.Id,
                    Price = product.Price,
                    ProductName = product.ProductName,
                    ShortDescription = product.ShortDescription,
                    Thumbnail = product.Thumbnail,
                    Description = product.Description,
                    SortOrder = product.SortOrder
                };
                return View(viewModel);
            }
            return RedirectToAction("Index");
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult ConfirmDelete(int id)
        {
            var product = db.Products.Find(id);
            try
            {
                if (product != null)
                {
                    db.ImageProducts.RemoveRange(db.ImageProducts.Where(m => m.ProductId == id));
                    db.Products.Remove(product);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Notice = "Error deleting product";
                return View(product);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var imagesProduct = db.ImageProducts.Where(m => m.ProductId == id).OrderBy(m => m.Id).ToList();
            var product = db.Products.Find(id);
            ProductViewModel viewModel = new ProductViewModel
            {
                Category = product.Category,
                CategoryId = product.CategoryId,
                Discount = product.Discount,
                Id = product.Id,
                Price = product.Price,
                ProductName = product.ProductName,
                ShortDescription = product.ShortDescription,
                Thumbnail = product.Thumbnail,
                SortOrder = product.SortOrder,
                Description = product.Description,
                Images = imagesProduct
            };
            ViewBag.CategoryId = new SelectList(db.Categories.ToList(), "CategoryId", "CategoryName");
            return View(viewModel);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(ProductViewModel viewModel, IEnumerable<HttpPostedFileBase> files)
        {
            ViewBag.CategoryId = new SelectList(db.Categories.ToList(), "CategoryId", "CategoryName", viewModel.CategoryId);
            if (ModelState.IsValid)
            {
                var product = db.Products.Find(viewModel.Id);
                //set value to return view if failed
                var tempThumbnail = product.Thumbnail;
                //count for image
                int count = 0, countImage;
                //get ImageProduct 
                var lstImageProduct = db.ImageProducts.Where(m => m.ProductId == viewModel.Id).OrderBy(m => m.Id).ToList();
                //count lstImageProduct
                int countImagesProduct = lstImageProduct.Count();

                foreach (var file in files)
                {
                    //check file null
                    if (file != null)
                    {
                        if (file.ContentLength > 0)
                        {
                            //get extension cover to lower
                            string extension = Path.GetExtension(file.FileName).ToLower();
                            // check type file
                            if (extension != ".png" && extension != ".gif" && extension != ".jpg")
                            {
                                ViewBag.Notice = "Invalid file format. Please choice again.";
                                viewModel.Thumbnail = tempThumbnail;
                                viewModel.Images = lstImageProduct;
                                return View(viewModel);
                            }
                            //create file name                       
                            var fileName = Path.GetFileName(file.FileName);
                            // check size
                            if (file.ContentLength > 2097152)
                            {
                                ViewBag.Notice = "Invalid file size. Please choice again.";
                                viewModel.Thumbnail = tempThumbnail;
                                viewModel.Images = lstImageProduct;
                                return View(viewModel);
                            }
                            string filePath = "/Uploads/" + Guid.NewGuid().ToString() + "/";
                            //check directory exists
                            if (!Directory.Exists(Server.MapPath(filePath)))
                            {
                                Directory.CreateDirectory(Server.MapPath(filePath));
                            }
                            //create path of file 
                            var path = Path.Combine(Server.MapPath(filePath), fileName);
                            file.SaveAs(path);
                            viewModel.Thumbnail = filePath + fileName;
                            countImage = 0;
                            //difference between old images and new upload images
                            if (countImagesProduct > count)
                            {
                                foreach (var image in lstImageProduct)
                                {
                                    //lấy ảnh đúng thứ tự ảnh-file upload
                                    if (countImage < count)
                                    {
                                        countImage++;
                                        continue;
                                    }

                                    //if is thumbnail, save at thumbnail and replace image path with new thumbnail
                                    if (count == 0 && image.ImagePath == product.Thumbnail)
                                    {
                                        product.Thumbnail = viewModel.Thumbnail;
                                        image.ImagePath = viewModel.Thumbnail;
                                        //db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                                        db.Entry(image).State = System.Data.Entity.EntityState.Modified;
                                        break;
                                    }
                                    else if (count != 0)
                                    {
                                        //update image exists, replace
                                        image.ImagePath = viewModel.Thumbnail;
                                        db.Entry(image).State = System.Data.Entity.EntityState.Modified;
                                        break;
                                    }
                                    countImage++;
                                }
                            }
                            else
                            {
                                //create new  images product
                                var imgProduct = new ImageProduct
                                {
                                    ProductId = product.Id,
                                    ImagePath = viewModel.Thumbnail,
                                    CreatedAt = DateTime.Now
                                };
                                db.ImageProducts.Add(imgProduct);
                            }
                        }
                    } 
                    count++;
                }
                //create new product                
                product.CategoryId = viewModel.CategoryId;
                product.Description = viewModel.Description;
                product.Discount = viewModel.Discount;
                product.ModifiedAt = DateTime.Now;
                product.Price = viewModel.Price;
                product.ProductName = viewModel.ProductName;
                product.ShortDescription = viewModel.ShortDescription;
                product.SortOrder = viewModel.SortOrder;
                db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(viewModel);
        }

        //get details product
        [HttpGet]
        public ActionResult Details(int id)
        {
            var product = db.Products.Find(id);
            if (product != null)
            {
                ProductViewModel viewModel = new ProductViewModel
                {
                    Category = product.Category,
                    Discount = product.Discount,
                    Id = product.Id,
                    Price = product.Price,
                    ProductName = product.ProductName,
                    ShortDescription = product.ShortDescription,
                    Thumbnail = product.Thumbnail,
                    Description = product.Description,
                    SortOrder = product.SortOrder
                };
                return View(viewModel);
            }
            return RedirectToAction("Index");
        }
    }
}