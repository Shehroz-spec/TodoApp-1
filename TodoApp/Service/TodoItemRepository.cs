namespace TodoApp
{
    // TodoItemRepository.cs
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text.Json;
    using System.Xml.Linq;
    using TodoApp.Service;

    public class TodoItemRepository : ITodoItemRepository
    {
        private List<TodoItem> _todoItems;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<TodoItemRepository> _logger;


        public TodoItemRepository(IHttpContextAccessor httpContextAccessor, ILogger<TodoItemRepository> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _todoItems = new List<TodoItem>();

            if (_httpContextAccessor.HttpContext != null)
            {
                _httpContextAccessor.HttpContext.Session.TryGetValue("TodoList", out var todoItemsData);
                if (todoItemsData == null)
                {
                    _todoItems.Add(
                      new TodoItem()
                      {
                          Id = 1,
                          Name = "Task1",
                          Priority = 4,
                          Status = TaskStatus.NotStarted
                      });
                    _httpContextAccessor.HttpContext.Session.Set("TodoList", JsonSerializer.SerializeToUtf8Bytes(_todoItems));
                }
            }
        }

        public IEnumerable<TodoItem> GetAllItems()
        {

            if (_httpContextAccessor.HttpContext == null)
            {
                throw new Exception();
            }
            lock (_httpContextAccessor.HttpContext.Session)
            {
                try
                {
                    _httpContextAccessor.HttpContext.Session.TryGetValue("TodoList", out var todoItemsData);
                    var TodoItems = JsonSerializer.Deserialize<List<TodoItem>>(todoItemsData);
                    if (TodoItems != null && TodoItems.Count > 0)
                    {
                        return TodoItems;

                    }

                    return new List<TodoItem>();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error while retrieving data.");
                    throw;
                }
            }

        }

        public TodoItem GetItemById(int id)
        {
            if (_httpContextAccessor.HttpContext == null)
            {
                throw new Exception();
            }
            lock (_httpContextAccessor.HttpContext.Session)
            {
                try
                {

                    _httpContextAccessor.HttpContext.Session.TryGetValue("TodoList", out var todoItemsData);
                    {
                        // If the key exists, deserialize the data and retrieve the item
                        var TodoItems = JsonSerializer.Deserialize<List<TodoItem>>(todoItemsData);
                        if (TodoItems != null && TodoItems.Count > 0)
                        {
                            return TodoItems.FirstOrDefault(item => item.Id == id);

                        }
                    }
                    return new TodoItem();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error while retrieving data with ID {id}.");
                    throw;
                }
            }

        }

        public void AddItem(TodoItem item)
        {
            if (_httpContextAccessor.HttpContext == null)
            {
                throw new Exception();
            }
            lock (_httpContextAccessor.HttpContext.Session)
            {
                try
                {


                    item.Id = GetMaxId();

                    _httpContextAccessor.HttpContext.Session.TryGetValue("TodoList", out var todoItemsData);
                    var TodoItems = JsonSerializer.Deserialize<List<TodoItem>>(todoItemsData);
                    if (TodoItems != null && TodoItems.Count > 0)
                    {
                        TodoItems.Add(item);
                        _httpContextAccessor.HttpContext.Session.Set("TodoList", JsonSerializer.SerializeToUtf8Bytes(TodoItems));

                    }
                    else
                    {
                        TodoItems = new List<TodoItem>();
                        TodoItems.Add(item);
                        _httpContextAccessor.HttpContext.Session.Set("TodoList", JsonSerializer.SerializeToUtf8Bytes(TodoItems));

                    }

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while adding data .");
                    throw;
                }
            }
        }
        public void UpdateItem(int Id,TodoItem updatedItem)
        {
            if (_httpContextAccessor.HttpContext == null)
            {
                throw new Exception();
            }
            lock (_httpContextAccessor.HttpContext.Session)
            {
                try
                {


                    _httpContextAccessor.HttpContext.Session.TryGetValue("TodoList", out var todoItemsData);
                    var updatedTodoItems = JsonSerializer.Deserialize<List<TodoItem>>(todoItemsData);
                    if (updatedTodoItems != null && updatedTodoItems.Count > 0)
                    {
                        var existingItem = updatedTodoItems.FirstOrDefault(item => item.Id == Id);
                        if (existingItem != null)
                        {
                            updatedTodoItems.RemoveAll(x => x.Id == existingItem.Id);
                            existingItem.Name = updatedItem.Name;
                            existingItem.Priority = updatedItem.Priority;
                            existingItem.Status = updatedItem.Status;
                            updatedTodoItems.Add(existingItem);
                            _httpContextAccessor.HttpContext.Session.Set("TodoList", JsonSerializer.SerializeToUtf8Bytes(updatedTodoItems));
                        }
                    }

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while updating data.");
                    throw;
                }
            }
        }

        public void DeleteCompletedTasks()
        {
            if (_httpContextAccessor.HttpContext == null)
            {
                throw new Exception();
            }
            lock (_httpContextAccessor.HttpContext.Session)
            {
                try
                {

                    _httpContextAccessor.HttpContext.Session.TryGetValue("TodoList", out var todoItemsData);

                    // If the key exists, deserialize the data and retrieve the item 
                    var TodoItems = JsonSerializer.Deserialize<List<TodoItem>>(todoItemsData);
                    if (TodoItems != null && TodoItems.Count > 0)
                    {
                        TodoItems.RemoveAll(item => item.Status == TaskStatus.Completed);
                        _httpContextAccessor.HttpContext.Session.Set("TodoList", JsonSerializer.SerializeToUtf8Bytes(TodoItems));
                    }

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while deleting data.");
                    throw;
                }
            }

        }

        public bool IsDuplicateName(string name, int id)
        {
            if (_httpContextAccessor.HttpContext == null)
            {
                throw new Exception();
            }
            lock (_httpContextAccessor.HttpContext.Session)
            {
                try
                {
                    _httpContextAccessor.HttpContext.Session.TryGetValue("TodoList", out var todoItemsData);

                    var TodoItems = JsonSerializer.Deserialize<List<TodoItem>>(todoItemsData);

                    if (TodoItems != null && TodoItems.Count > 0)
                    {
                        return TodoItems.Any(item => item.Name.ToLower() == name.ToLower() && item.Id != id);
                    }

                    return false;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Something went wrong.");
                    throw;
                }
            }
        }
        public bool HasDeleteCompletedTasks()
        {
            if (_httpContextAccessor.HttpContext == null)
            {
                throw new Exception();
            }
            lock (_httpContextAccessor.HttpContext.Session)
            {
                try
                {

                    _httpContextAccessor.HttpContext.Session.TryGetValue("TodoList", out var todoItemsData);
                    {
                        var TodoItems = JsonSerializer.Deserialize<List<TodoItem>>(todoItemsData);

                        if (TodoItems != null && TodoItems.Count > 0)
                        {
                            return TodoItems.Any(item => item.Status == TaskStatus.Completed);
                        }
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Something went wrong.");
                    throw;
                }
            }

        }
        public int GetMaxId()
        {
            if (_httpContextAccessor.HttpContext == null)
            {
                throw new Exception();
            }
            lock (_httpContextAccessor.HttpContext.Session)
            {
                try
                {

                    int maxId = 1;
                    _httpContextAccessor.HttpContext.Session.TryGetValue("TodoList", out var todoItemsData);

                    var TodoItems = JsonSerializer.Deserialize<List<TodoItem>>(todoItemsData);
                    if (TodoItems != null && TodoItems.Count > 0)
                    {

                        maxId = TodoItems.OrderByDescending(x => x.Id).FirstOrDefault().Id + 1;
                    }

                    return maxId;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while getting data.");
                    throw;
                }
            }
        }

    }
}


