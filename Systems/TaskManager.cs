using UnityEngine;
using System.Collections.Generic;

namespace cvbhhnClassLibrary1.Systems
{
    public enum TaskType
    {
        Kill,
        ConditionalKill,
        Pickup,
        Transport,
        Trigger,
        ReadAll
    }

    public class TaskData
    {
        public int id;
        public string title;
        public string description;
        public TaskType type;
        public int current;
        public int target;
        public bool completed;
        public Dictionary<string, object> conditions;

        public TaskData(int id, string title, TaskType type, int target)
        {
            this.id = id;
            this.title = title;
            this.type = type;
            this.target = target;
            this.current = 0;
            this.completed = false;
            this.conditions = new Dictionary<string, object>();
        }
    }

    public class TaskManager
    {
        private List<TaskData> tasks = new List<TaskData>();
        private int nextId = 1;

        public void Initialize()
        {
            Debug.Log("[TaskManager] Initialized");
        }

        public TaskData CreateTask(string title, TaskType type, int target, string description = "")
        {
            var task = new TaskData(nextId++, title, type, target);
            task.description = description;
            tasks.Add(task);
            Debug.Log($"[TaskManager] Created task: {title} ({type})");
            return task;
        }

        public void UpdateTaskProgress(int taskId, int amount)
        {
            var task = GetTask(taskId);
            if (task != null && !task.completed)
            {
                task.current += amount;
                if (task.current >= task.target)
                {
                    task.current = task.target;
                    task.completed = true;
                    Debug.Log($"[TaskManager] Task completed: {task.title}");
                }
                else
                {
                    Debug.Log($"[TaskManager] Task progress: {task.title} ({task.current}/{task.target})");
                }
            }
        }

        public void CompleteTask(int taskId)
        {
            var task = GetTask(taskId);
            if (task != null)
            {
                task.current = task.target;
                task.completed = true;
                Debug.Log($"[TaskManager] Task manually completed: {task.title}");
            }
        }

        public void DeleteTask(int taskId)
        {
            var task = GetTask(taskId);
            if (task != null)
            {
                tasks.Remove(task);
                Debug.Log($"[TaskManager] Task deleted: {task.title}");
            }
        }

        public TaskData GetTask(int taskId)
        {
            return tasks.Find(t => t.id == taskId);
        }

        public List<TaskData> GetAllTasks()
        {
            return new List<TaskData>(tasks);
        }

        public List<TaskData> GetActiveTasks()
        {
            return tasks.FindAll(t => !t.completed);
        }

        public List<TaskData> GetCompletedTasks()
        {
            return tasks.FindAll(t => t.completed);
        }

        public void SetTaskCondition(int taskId, string key, object value)
        {
            var task = GetTask(taskId);
            if (task != null)
            {
                task.conditions[key] = value;
                Debug.Log($"[TaskManager] Set condition for task {task.title}: {key} = {value}");
            }
        }

        public object GetTaskCondition(int taskId, string key)
        {
            var task = GetTask(taskId);
            if (task != null && task.conditions.ContainsKey(key))
            {
                return task.conditions[key];
            }
            return null;
        }

        public void CheckKillTask(string enemyType, int killCount = 1)
        {
            foreach (var task in GetActiveTasks())
            {
                if (task.type == TaskType.Kill)
                {
                    UpdateTaskProgress(task.id, killCount);
                }
                else if (task.type == TaskType.ConditionalKill)
                {
                    var targetEnemy = GetTaskCondition(task.id, "enemyType") as string;
                    if (targetEnemy == enemyType)
                    {
                        UpdateTaskProgress(task.id, killCount);
                    }
                }
            }
        }

        public void CheckPickupTask(string itemId)
        {
            foreach (var task in GetActiveTasks())
            {
                if (task.type == TaskType.Pickup)
                {
                    var targetItem = GetTaskCondition(task.id, "itemId") as string;
                    if (string.IsNullOrEmpty(targetItem) || targetItem == itemId)
                    {
                        UpdateTaskProgress(task.id, 1);
                    }
                }
            }
        }

        public void CheckTransportTask(string itemId, string location)
        {
            foreach (var task in GetActiveTasks())
            {
                if (task.type == TaskType.Transport)
                {
                    var targetItem = GetTaskCondition(task.id, "itemId") as string;
                    var targetLocation = GetTaskCondition(task.id, "location") as string;
                    
                    if ((string.IsNullOrEmpty(targetItem) || targetItem == itemId) &&
                        (string.IsNullOrEmpty(targetLocation) || targetLocation == location))
                    {
                        UpdateTaskProgress(task.id, 1);
                    }
                }
            }
        }

        public void CheckTriggerTask(string triggerId)
        {
            foreach (var task in GetActiveTasks())
            {
                if (task.type == TaskType.Trigger)
                {
                    var targetTrigger = GetTaskCondition(task.id, "triggerId") as string;
                    if (string.IsNullOrEmpty(targetTrigger) || targetTrigger == triggerId)
                    {
                        UpdateTaskProgress(task.id, 1);
                    }
                }
            }
        }

        public void Clear()
        {
            tasks.Clear();
            nextId = 1;
            Debug.Log("[TaskManager] Cleared all tasks");
        }
    }
}
