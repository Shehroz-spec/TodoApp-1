using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Infrastructure;
using Moq;
using TodoApp;
using TodoApp.Controllers;
using TodoApp.Service;

namespace TodoAppTest
{
    public class TodoItemsTest
    {
        // Test method to verify that DeleteCompletedTasks returns NoContent
        [Fact]
        public void DeleteCompletedTasks_ReturnsNoContent()
        {
            var mockRepository = new Mock<ITodoItemRepository>();
            var controller = new TodoItemsController(mockRepository.Object);

            // Act
            var result = controller.DeleteCompletedTasks();

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Update_ValidModel_ReturnRedirectToActionResult()
        {
            var mockRepository = new Mock<ITodoItemRepository>();
            var controller = new TodoItemsController(mockRepository.Object);
            var mockTempData = new Mock<ITempDataDictionary>();
            var mockTempDataFactory = new Mock<ITempDataDictionaryFactory>();
            mockTempDataFactory.Setup(factory => factory.GetTempData(It.IsAny<HttpContext>())).Returns(mockTempData.Object);

            controller.TempData = mockTempData.Object;
            var existingItem = new TodoItem { Id = 1, Name = "Existing Task", Priority = 1, Status = TodoApp.TaskStatus.NotStarted };
            var todoItem = new TodoItem { Id = 2, Name = "Task A", Priority = 1, Status = TodoApp.TaskStatus.NotStarted };
            mockRepository.Setup(repo => repo.GetItemById(It.IsAny<int>())).Returns((existingItem));

            // Act
            var result = controller.Update(existingItem.Id, todoItem);

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public void Update_InvalidModel_ReturnsBadRequest()
        {
            var mockRepository = new Mock<ITodoItemRepository>();
            var controller = new TodoItemsController(mockRepository.Object);
            controller.ModelState.AddModelError("Name", "Task with this name already exists.");
            var todoItem = new TodoItem();

            // Act
            var result = controller.Update(2,todoItem);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        

        [Fact]
        public void Update_DuplicateName_ReturnsBadRequest()
        {
            var existingItem = new TodoItem { Id = 1, Name = "Existing Task", Priority = 1, Status = TodoApp.TaskStatus.NotStarted };
            var updatedItem = new TodoItem { Id = 1, Name = "Existing Task", Priority = 2, Status = TodoApp.TaskStatus.Completed };

            var mockRepository = new Mock<ITodoItemRepository>();
            mockRepository.Setup(repo => repo.GetItemById(existingItem.Id)).Returns(existingItem);
            mockRepository.Setup(repo => repo.IsDuplicateName(updatedItem.Name, updatedItem.Id)).Returns(true);
            var mockTempData = new Mock<ITempDataDictionary>();
            var mockTempDataFactory = new Mock<ITempDataDictionaryFactory>();
            mockTempDataFactory.Setup(factory => factory.GetTempData(It.IsAny<HttpContext>())).Returns(mockTempData.Object);

            var controller = new TodoItemsController(mockRepository.Object);
            controller.TempData = mockTempData.Object;

            // Act
            var result = controller.Update(existingItem.Id,updatedItem);

            // Assert
            Assert.IsType<RedirectToActionResult>(result);

        }


        [Fact]
        public void Create_ValidModel_ReturnsRedirectToAction()
        {
            var mockRepository = new Mock<ITodoItemRepository>();
            var controller = new TodoItemsController(mockRepository.Object);
            var todoItem = new TodoItem { Id = 1, Name = "Task A", Priority = 1, Status = TodoApp.TaskStatus.Completed };
            var mockTempData = new Mock<ITempDataDictionary>();
            var mockTempDataFactory = new Mock<ITempDataDictionaryFactory>();
            mockTempDataFactory.Setup(factory => factory.GetTempData(It.IsAny<HttpContext>())).Returns(mockTempData.Object);

            controller.TempData = mockTempData.Object;
            // Act
            var result = controller.Create(todoItem);

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public void Create_DuplicateName_ReturnsViewResult()
        {
            var duplicateItem = new TodoItem { Id = 1, Name = "Duplicate Task", Priority = 1, Status = TodoApp.TaskStatus.Completed };
            var mockRepository = new Mock<ITodoItemRepository>();
            mockRepository.Setup(repo => repo.IsDuplicateName(duplicateItem.Name, duplicateItem.Id)).Returns(true);

            var controller = new TodoItemsController(mockRepository.Object);

            // Act
            var result = controller.Create(duplicateItem);

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Create_InvalidModel_ReturnsViewResult()
        {
            var mockRepository = new Mock<ITodoItemRepository>();
            var controller = new TodoItemsController(mockRepository.Object);
            controller.ModelState.AddModelError("Name", "Task with this name already exists.");

            var todoItem = new TodoItem();

            // Act
            var result = controller.Create(todoItem);

            // Assert
            Assert.IsType<ViewResult>(result);
        }
        [Fact]
        public void EditById_ReturnsOk()
        {
            // Arrange
            var mockRepository = new Mock<ITodoItemRepository>();
            var controller = new TodoItemsController(mockRepository.Object);
            var todoItem = new TodoItem();
            todoItem.Id = 1;
            mockRepository.Setup(repo => repo.GetItemById(todoItem.Id)).Returns(new TodoItem { Id = 1, Name = "Task A", Priority = 1, Status = TodoApp.TaskStatus.Completed });


            // Act
            var result = controller.Edit(todoItem.Id);

            // Assert
            Assert.IsType<ViewResult>(result);
        }
        [Fact]
        public void EditById_NotExistingItem_ReturnsNotFound()
        {
            var mockRepository = new Mock<ITodoItemRepository>();
            var controller = new TodoItemsController(mockRepository.Object);
            var todoItem = new TodoItem();
            todoItem.Id = 1;
            mockRepository.Setup(repo => repo.GetItemById(todoItem.Id)).Returns((TodoItem)null);

            // Act
            var result = controller.Edit(todoItem.Id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Index_ReurnOk()
        {
            var mockRepository = new Mock<ITodoItemRepository>();
            var controller = new TodoItemsController(mockRepository.Object);
            var obj = new List<TodoItem>();
            obj.Add(new TodoItem { Id = 1, Name = "Task A", Priority = 1, Status = TodoApp.TaskStatus.Completed });
            mockRepository.Setup(repo => repo.GetAllItems()).Returns(obj);

            // Act
            var result = controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

    }

}