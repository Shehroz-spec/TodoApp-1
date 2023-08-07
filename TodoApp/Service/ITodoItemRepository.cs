using System.Collections.Generic;
using TodoApp.Models;

namespace TodoApp.Service
{
    public interface ITodoItemRepository
    {
        IEnumerable<TodoItem> GetAllItems();
       // IEnumerable<TodoItem> GetAllItemsFromSession();
        TodoItem GetItemById(int id);
        void AddItem(TodoItem item);
        void UpdateItem(int Id,TodoItem updatedItem);
        void DeleteCompletedTasks();
        bool IsDuplicateName(string name, int id);
        bool HasDeleteCompletedTasks();
        
    }

}
