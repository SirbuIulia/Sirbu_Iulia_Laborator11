using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sirbu_Iulia_Lab2.Data;
using Sirbu_Iulia_Lab2.Models;
using Sirbu_Iulia_Lab2.Models.LibraryViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sirbu_Iulia_Lab2.Controllers
{
    [Authorize(Policy = "OnlySales")]
    public class PublishersController : Controller
    {
        private readonly LibraryContext _context;

        public PublishersController(LibraryContext context)
        {
            _context = context;
        }

        // GET: Publishers
        public async Task<IActionResult> Index(int? id, int? bookID)
        {
            var viewModel = new PublisherIndexData
            {
                Publishers = await _context.Publisher
                    .Include(p => p.PublishedBooks)
                    .ThenInclude(pb => pb.Book)
                    .ThenInclude(b => b.Orders)
                    .ThenInclude(o => o.Customer)
                    .AsNoTracking()
                    .OrderBy(p => p.PublisherName)
                    .ToListAsync()
            };

            if (id != null)
            {
                ViewData["PublisherID"] = id.Value;
                var publisher = viewModel.Publishers.Single(p => p.ID == id.Value);
                viewModel.Books = publisher.PublishedBooks.Select(pb => pb.Book);
            }

            if (bookID != null)
            {
                ViewData["BookID"] = bookID.Value;
                viewModel.Orders = viewModel.Books.Single(b => b.ID == bookID.Value).Orders;
            }

            return View(viewModel);
        }

        // GET: Publishers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Publishers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PublisherName,Adress")] Publisher publisher)
        {
            if (!ModelState.IsValid)
            {
                return View(publisher);
            }

            try
            {
                _context.Add(publisher);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "Unable to save the Publisher. Try again.");
            }

            return View(publisher);
        }

        // GET: Publishers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var publisher = await _context.Publisher
                .Include(p => p.PublishedBooks)
                .ThenInclude(pb => pb.Book)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);

            if (publisher == null) return NotFound();

            PopulatePublishedBookData(publisher);
            return View(publisher);
        }

        // POST: Publishers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string[] selectedBooks)
        {
            var publisherToUpdate = await _context.Publisher
                .Include(p => p.PublishedBooks)
                .ThenInclude(pb => pb.Book)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (publisherToUpdate == null) return NotFound();

            if (await TryUpdateModelAsync<Publisher>(publisherToUpdate, "", p => p.PublisherName, p => p.Adress))
            {
                UpdatePublishedBooks(selectedBooks, publisherToUpdate);

                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again.");
                }
            }

            PopulatePublishedBookData(publisherToUpdate);
            return View(publisherToUpdate);
        }

        // GET: Publishers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var publisher = await _context.Publisher
                .Include(p => p.PublishedBooks)
                .ThenInclude(pb => pb.Book)
                .ThenInclude(b => b.Orders)
                .ThenInclude(o => o.Customer)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);

            if (publisher == null)
            {
                return NotFound();
            }

            return View(publisher);
        }

        // GET: Publishers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Publisher == null)
            {
                return NotFound();
            }

            var publisher = await _context.Publisher
                .Include(p => p.PublishedBooks)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);

            if (publisher == null)
            {
                return NotFound();
            }

            return View(publisher);
        }

        // POST: Publishers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var publisher = await _context.Publisher
                    .Include(p => p.PublishedBooks)
                    .FirstOrDefaultAsync(m => m.ID == id);

                if (publisher == null)
                {
                    return NotFound();
                }

                if (publisher.PublishedBooks != null && publisher.PublishedBooks.Any())
                {
                    _context.PublishedBook.RemoveRange(publisher.PublishedBooks);
                }

                _context.Publisher.Remove(publisher);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to delete Publisher. It may have related data that prevents deletion.");
            }

            return RedirectToAction(nameof(Index));
        }

        private void PopulatePublishedBookData(Publisher publisher)
        {
            var allBooks = _context.Book;
            var publisherBooks = new HashSet<int>(publisher.PublishedBooks.Select(pb => pb.BookID));
            var viewModel = new List<PublishedBookData>();

            foreach (var book in allBooks)
            {
                viewModel.Add(new PublishedBookData
                {
                    BookID = book.ID,
                    Title = book.Title,
                    IsPublished = publisherBooks.Contains(book.ID)
                });
            }

            ViewData["Books"] = viewModel;
        }

        private void UpdatePublishedBooks(string[] selectedBooks, Publisher publisherToUpdate)
        {
            var selectedBooksHS = new HashSet<string>(selectedBooks);
            var publishedBooks = new HashSet<int>(publisherToUpdate.PublishedBooks.Select(pb => pb.BookID));

            foreach (var book in _context.Book)
            {
                if (selectedBooksHS.Contains(book.ID.ToString()))
                {
                    if (!publishedBooks.Contains(book.ID))
                    {
                        publisherToUpdate.PublishedBooks.Add(new PublishedBook
                        {
                            PublisherID = publisherToUpdate.ID,
                            BookID = book.ID
                        });
                    }
                }
                else
                {
                    if (publishedBooks.Contains(book.ID))
                    {
                        var bookToRemove = publisherToUpdate.PublishedBooks.Single(pb => pb.BookID == book.ID);
                        _context.PublishedBook.Remove(bookToRemove);
                    }
                }
            }
        }

        private bool PublisherExists(int id)
        {
            return _context.Publisher.Any(e => e.ID == id);
        }
    }
}