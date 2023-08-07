using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Diagnostics;
using TodoApp.Models;
using TodoApp.Service;

namespace TodoApp.Controllers
{
    public class TodoItemsController : Controller
    {
        private readonly ITodoItemRepository _repository;

        public TodoItemsController(ITodoItemRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IActionResult Index()
        {

            return View(_repository.GetAllItems());
        }

        public IActionResult Create()
        {
            return View("Create");
        }
        [HttpGet]
        public IActionResult Edit()
        {
            return View("Edit");
        }

        [HttpGet("{id}")]
        public IActionResult Edit(int id)
        {
            var item = _repository.GetItemById(id);
            if (item == null)
                return NotFound();

            return View(item);

        }

        [HttpPost]
        public IActionResult Create(TodoItem item)
        {
            if (ModelState.IsValid)
            {
                if (_repository.IsDuplicateName(item.Name, item.Id))
                {
                    ModelState.AddModelError("Name", "Task with this name already exists.");
                    return View(item);
                }

                _repository.AddItem(item);
                TempData["Message"] = "Task added successfully!";
                return RedirectToAction("Index");
            }
            return View(item);

        }

        [HttpPost("Update")]
        public IActionResult Update(int id,TodoItem updatedItem)
        {

            if (ModelState.IsValid)
            {
                var existingItem = _repository.GetItemById(id);
                if (existingItem != null)
                {
                    if (_repository.IsDuplicateName(updatedItem.Name, updatedItem.Id))
                    {
                        TempData["Message"] = "Task with this name already exists.";

                        return RedirectToAction("Index");
                    }
                    _repository.UpdateItem(id,updatedItem);

                    TempData["Message"] = "Task updated successfully!";
                    return RedirectToAction("Index");
                }
            }

            return BadRequest(ModelState);

        }

        [HttpDelete("completed")]
        public IActionResult DeleteCompletedTasks()
        {
            if (_repository.HasDeleteCompletedTasks())
            {
                _repository.DeleteCompletedTasks();
                return Ok("Task has been deleted successfully!");
            }
            else
            {
                return NotFound();
            }

        }
    }
}
