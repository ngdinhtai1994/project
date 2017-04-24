using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BaiTap.Models;
using System.IO;

namespace BaiTap.Controllers
{
    public class CategoryController : Controller
    {
        private OnlineStoreEntities db = new OnlineStoreEntities();

        // GET: Categories
        public ActionResult Index()
        {
            var Categories = db.Categories.Select(m => new CategoryViewModel
            {
                CategoryId = m.CategoryId,
                CategoryName = m.CategoryName,
                Description = m.Description,
                ParentId = m.ParentId,
                SortOrder = m.SortOrder,
                Status = m.Status,
                Thumbnail = m.Thumbnail,
                TotalItems = m.TotalItems
            });
            return View(Categories);
        }

        // GET: Categories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            CategoryViewModel viewModel = new CategoryViewModel
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                Description = category.Description,
                ParentId = category.ParentId,
                SortOrder = category.SortOrder,
                Status = category.Status,
                Thumbnail = category.Thumbnail,
                TotalItems = category.TotalItems
            };
            return View(viewModel);
        }

        // GET: Categories/Create
        public ActionResult Create()
        {
            ViewBag.ParentId = new SelectList(db.Categories.ToList().OrderBy(m => m.CategoryName), "CategoryId", "CategoryName");
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CategoryViewModel viewModel, HttpPostedFileBase file)
        {
            ViewBag.ParentId = new SelectList(db.Categories.ToList().OrderBy(m => m.CategoryName), "CategoryId", "CategoryName", viewModel.CategoryId);
            if (ModelState.IsValid)
            {
                //create category to get id
                Category category = new Category();
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
                        //save category

                        category.CategoryName = viewModel.CategoryName;
                        category.CreatedAt = DateTime.Now;
                        category.Description = viewModel.Description;
                        category.ParentId = viewModel.ParentId;
                        category.Thumbnail = viewModel.Thumbnail;
                        category.SortOrder = viewModel.SortOrder;
                        category.Status = viewModel.Status;
                        category.TotalItems = viewModel.TotalItems;
                        db.Categories.Add(category);
                        db.SaveChanges();
                    }
                }
                return RedirectToAction("Index");
            }
            return View(viewModel);
        }

        // GET: Categories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            CategoryViewModel viewModel = new CategoryViewModel
            {
                CategoryName = category.CategoryName,
                CategoryId = category.CategoryId,
                Thumbnail = category.Thumbnail,
                Description = category.Description,
                ParentId = category.ParentId,
                SortOrder = category.SortOrder,
                TotalItems = category.TotalItems,
                Status = category.Status
            };
            var lstCategory = db.Categories.ToList();
            ViewBag.ParentId = new SelectList(lstCategory, "CategoryId", "CategoryName", viewModel.ParentId);
            return View(viewModel);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CategoryViewModel viewModel, HttpPostedFileBase file)
        {
            var categories = db.Categories.ToList().OrderBy(m => m.CategoryName);
            ViewBag.ParentId = new SelectList(categories, "CategoryId", "CategoryName", viewModel.ParentId);

            var category = db.Categories.Find(viewModel.CategoryId);
            if (ModelState.IsValid)
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
                            viewModel.Thumbnail = category.Thumbnail;
                            return View(viewModel);
                        }
                        //create file name                       
                        var fileName = Path.GetFileName(file.FileName);
                        // check size
                        if (file.ContentLength > 2097152)
                        {
                            ViewBag.Notice = "Invalid file size. Please choice again.";
                            viewModel.Thumbnail = category.Thumbnail;
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
                        category.Thumbnail = filePath + fileName;
                    }
                }

                //create new category                
                category.CategoryName = viewModel.CategoryName;
                category.CreatedAt = DateTime.Now;
                category.Description = viewModel.Description;
                category.ParentId = viewModel.ParentId;
                category.SortOrder = viewModel.SortOrder;
                category.Status = viewModel.Status;
                category.TotalItems = viewModel.TotalItems;
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        // GET: Categories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            CategoryViewModel viewModel = new CategoryViewModel
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                Description = category.Description,
                ParentId = category.ParentId,
                SortOrder = category.SortOrder,
                Status = category.Status,
                Thumbnail = category.Thumbnail,
                TotalItems = category.TotalItems
            };
            return View(viewModel);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Category category = db.Categories.Find(id);
                db.Categories.Remove(category);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                ViewBag.Notice = "The DELETE statement conflicted with the REFERENCE";
                Category category = db.Categories.Find(id);
                if (category == null)
                {
                    return HttpNotFound();
                }
                CategoryViewModel viewModel = new CategoryViewModel
                {
                    CategoryId = category.CategoryId,
                    CategoryName = category.CategoryName,
                    Description = category.Description,
                    ParentId = category.ParentId,
                    SortOrder = category.SortOrder,
                    Status = category.Status,
                    Thumbnail = category.Thumbnail,
                    TotalItems = category.TotalItems
                };
                return View(viewModel);
            }

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
