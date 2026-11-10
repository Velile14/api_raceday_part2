using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using RaceDay.Web.Services;
using RaceDay.Web.ViewModels;

namespace RaceDay.Web.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ApiService _apiService;

        public CategoriesController(ApiService apiService)
        {
            _apiService = apiService;
        }

        // Organiser: view all categories.
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Login", "Auth");

            try
            {
                var categories = await _apiService
                    .GetAsync<List<CategoryViewModel>>("api/Categories");

                return View(categories ?? new List<CategoryViewModel>());
            }
            catch (HttpRequestException ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(new List<CategoryViewModel>());
            }
        }

        // Organiser: display the create category form.
        public async Task<IActionResult> Create()
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Login", "Auth");

            var model = new CategoryFormViewModel();

            try
            {
                model.Events = await _apiService
                    .GetAsync<List<EventViewModel>>("api/Events")
                    ?? new List<EventViewModel>();
            }
            catch (HttpRequestException ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }

            return View(model);
        }

        // Organiser: submit a new category.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryFormViewModel model)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Login", "Auth");

            try
            {
                var request = new
                {
                    model.Category.EventID,
                    model.Category.CategoryName,
                    model.Category.MinimumAge,
                    model.Category.MaximumAge,
                    model.Category.Distance
                };

                await _apiService.PostAsync<object, CategoryViewModel>(
                    "api/Categories", request);

                TempData["SuccessMessage"] = "Category created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (HttpRequestException ex)
            {
                ViewBag.ErrorMessage = ex.Message;

                model.Events = await _apiService
                    .GetAsync<List<EventViewModel>>("api/Events")
                    ?? new List<EventViewModel>();

                return View(model);
            }
        }

        // Organiser: display the edit category form.
        public async Task<IActionResult> Edit(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Login", "Auth");

            try
            {
                var category = await _apiService
                    .GetAsync<CategoryViewModel>($"api/Categories/{id}");

                if (category == null)
                    return NotFound();

                var model = new CategoryFormViewModel
                {
                    Category = category,
                    Events = await _apiService
                        .GetAsync<List<EventViewModel>>("api/Events")
                        ?? new List<EventViewModel>()
                };

                return View(model);
            }
            catch (HttpRequestException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // Organiser: submit category changes.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryFormViewModel model)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Login", "Auth");

            try
            {
                await _apiService.PutAsync<object, JsonElement>(
                    $"api/Categories/{model.Category.CategoryID}",
                    new
                    {
                        model.Category.CategoryID,
                        model.Category.EventID,
                        model.Category.CategoryName,
                        model.Category.MinimumAge,
                        model.Category.MaximumAge,
                        model.Category.Distance
                    });

                TempData["SuccessMessage"] = "Category updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (HttpRequestException ex)
            {
                ViewBag.ErrorMessage = ex.Message;

                model.Events = await _apiService
                    .GetAsync<List<EventViewModel>>("api/Events")
                    ?? new List<EventViewModel>();

                return View(model);
            }
        }

        // Organiser: delete a category.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Login", "Auth");

            try
            {
                await _apiService.DeleteAsync($"api/Categories/{id}");
                TempData["SuccessMessage"] = "Category deleted successfully.";
            }
            catch (HttpRequestException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
