using System;
using System.Security.Cryptography;
using System.Text;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace csharpProject
{

    public class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
    public class User
    {
        private int _nextUserID = 1;
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public DateTime CreatedOn { get; set; }


        public User(string username, string password, string email)
        {
            UserId = _nextUserID++;
            Username = username;
            Password = password;
            Email = email;
            CreatedOn = DateTime.Now;
        }

    }

    
    public enum Status
    {
        Pending,
        Completed
    }

    public class Task
    {
        private int _nextId = 1;
        public int TaskId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public User Asignee { get; set; }
        public Status Status { get; set; }
        public DateTime DeadLine { get; set; }


        public Task(string title, string description, User asignee, DateTime deadline)
        {
            TaskId = _nextId++;
            Title = title;
            Description = description;
            Asignee = asignee;
            Status = Status.Pending;
            DeadLine = deadline;
        }

    }

    public class TaskManager
    {
        public List<Task> Tasks { get; set; }
        public List<User> Users { get; set; }

        public TaskManager(List<Task> tasks, List<User> users)
        {
            Users = users;
            Tasks = tasks;
        }

        

        public  async System.Threading.Tasks.Task AddUserAsync(User user)
        {
            string connectionString = "Server=localhost;Database=master;Trusted_Connection=True;";
;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Users (Username, PasswordHash, Email, CreatedDate) Values (@Username, @PasswordHash, @Email, @CreatedOn)";

                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@Username", user.Username);
                command.Parameters.AddWithValue("@PasswordHash", user.Password);
                command.Parameters.AddWithValue("@Email", user.Email);
                command.Parameters.AddWithValue("@CreatedOn", user.CreatedOn);

                try
                {
                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
                catch (SqlException ex)
                {
                    Console.WriteLine($"Database error: {ex.Message}");
                }
                finally
                {
                    connection.Close();
                }
            }
        }
        public void AddTask(Task task)
        {
            if(task != null)
            {
                Tasks.Add(task);
                Console.WriteLine("Task Added Successfuly.");
            }
            else
            {
                Console.WriteLine("Task Can Not Be Added.");
            }
        }
        public void DeleteTask(int taskId)
        {
            Task taskToDelete = Tasks.Find(t => t.TaskId == taskId);

            if(taskToDelete != null)
            {
                Tasks.Remove(taskToDelete);
                Console.WriteLine("Task Removed Successfuly.");
            }
            else
            {
                Console.WriteLine("There is no task with that ID.");
            }
        }
        public void GetTaskByID(int taskId)
        {
            Task taskToFind = Tasks.Find(t => t.TaskId == taskId);

            if(taskToFind != null)
            {
                Console.WriteLine($"TaskID: {taskToFind.TaskId}, Title: {taskToFind.Title}, Description: {taskToFind.Description}, DeadLine: {taskToFind.DeadLine.Day} days.");
            }
            else
            {
                Console.WriteLine("Can not find task with that ID.");
            }
        }
        public List<Task> GetAllTasks() => Tasks.ToList();

        public List<Task> GetTasksByAsignee(int userId) => Tasks.FindAll(t => t.Asignee.UserId == userId).ToList();
        public List<Task> GetTasksByStatus(Status status) => Tasks.FindAll(t => t.Status == status).ToList();

        public void CompleteTask(int taskId)
        {
            Task taskToComplete = Tasks.Find(t => t.TaskId == taskId);

            if(taskToComplete != null)
            {
                taskToComplete.Status = Status.Completed;
                Console.WriteLine("Task Completed Successfuly.");
            }
            else
            {
                Console.WriteLine("Can not find task with that ID.");
            }
        }
        public void AssignTask(int taskId, int userId)
        {
            Task taskToFind = Tasks.Find(t => t.TaskId == taskId);
            User userToFind = Users.Find(u => u.UserId == userId);


            if(taskToFind != null && userToFind != null)
            {
                taskToFind.Asignee = userToFind;
                Console.WriteLine("Task assigned successfuly.");
            }
            else
            {
                Console.WriteLine("TaskId or UserId is incorrect.");
            }
        }

        public List<Task> GetOverdueTasks(DateTime datetime) => Tasks.FindAll(t => t.DeadLine < datetime).ToList();

        public User Login(string username, string password)
        {
            string passwordHash = PasswordHelper.HashPassword(password);
            User user = Users.FirstOrDefault(u => u.Username == username && u.Password == passwordHash);

            if (user != null)
            {
                Console.WriteLine("Login successful.");
                return user;
            }
            else
            {
                Console.WriteLine("Invalid username or password.");
                return null;
            }
        }

        public async System.Threading.Tasks.Task RegisterUserAsync(string username, string password, string email)
        {
            if (Users.Any(u => u.Username == username))
            {
                Console.WriteLine("Username already taken.");
                return;
            }

            User newUser = new User(username, PasswordHelper.HashPassword(password), email);
            Users.Add(newUser);

            await AddUserAsync(newUser);
            Console.WriteLine("User registered successfully.");
        }





    }
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            
            User user1 = new User("john_doe1", PasswordHelper.HashPassword("password123"), "john_doe@example.com");
            User user2 = new User("jane_doe2", PasswordHelper.HashPassword("password456"), "jane_doe@example.com");
            
           
            List<User> users = new List<User> { user1, user2 };

            
            Task task1 = new Task("Design website", "Design the landing page for the new project", user1, DateTime.Now.AddDays(7));
            Task task2 = new Task("Fix bugs", "Fix the reported bugs from the QA team", user2, DateTime.Now.AddDays(3));

            
            List<Task> tasks = new List<Task> { task1, task2 };

   
            TaskManager taskManager = new TaskManager(tasks, users);
            //await taskManager.AddUserAsync(user1);
            //await taskManager.AddUserAsync(user2);

            Console.WriteLine("All Tasks:");
            foreach (var task in taskManager.GetAllTasks())
            {
                Console.WriteLine($"{task.TaskId}: {task.Title} - Assigned to: {task.Asignee.Username}");
            }

            
            Task newTask = new Task("Test new feature", "Test the new feature added to the project", user1, DateTime.Now.AddDays(5));
            taskManager.AddTask(newTask);

            
            Console.WriteLine("\nTasks for John Doe:");
            foreach (var task in taskManager.GetTasksByAsignee(user1.UserId))
            {
                Console.WriteLine($"{task.TaskId}: {task.Title} - Status: {task.Status}");
            }

           
            taskManager.CompleteTask(task1.TaskId);

            
            Console.WriteLine("\nCompleted Tasks:");
            foreach (var task in taskManager.GetTasksByStatus(Status.Completed))
            {
                Console.WriteLine($"{task.TaskId}: {task.Title} - Status: {task.Status}");
            }



            Console.WriteLine("Do you want to log in or register? (login/register)");
            string option = Console.ReadLine();

            if (option == "register")
            {
                Console.WriteLine("Enter username:");
                string username = Console.ReadLine();

                Console.WriteLine("Enter password:");
                string password = Console.ReadLine();

                Console.WriteLine("Enter email:");
                string email = Console.ReadLine();

                await taskManager.RegisterUserAsync(username, password, email);
            }else if(option == "login")
            {
                taskManager.AssignTask(task2.TaskId, user1.UserId);
                Console.WriteLine($"\nTask {task2.TaskId} reassigned to: {task2.Asignee.Username}");

                Console.WriteLine("Enter username:");
                string username = Console.ReadLine();

                Console.WriteLine("Enter password:");
                string password = Console.ReadLine();

                User loggedInUser = taskManager.Login(username, password);

                if (loggedInUser == null)
                {
                    Console.WriteLine("Exiting the program.");
                    return;
                }
            }



            

        }
    }
}
