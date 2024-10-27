using System;
using System.Collections.Generic;
using System.Linq;                
using System.Text;                
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Data.SqlClient;


namespace Project_Managment_System_PMS_
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
    public enum UserRole
    {
        Admin,
        ProjectManager,
        TeamMember
    }
    public class User
    {
        private int _Id = 1;
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public UserRole Role { get; set; }
        public DateTime CreatedOn { get; set; }


        public User(string username, string password, string email, UserRole role)
        {
            UserId = _Id++;
            Username = username;
            Password = PasswordHelper.HashPassword(password);
            Email = email;
            Role = role;
            CreatedOn = DateTime.Now;
        }

        
    }

    public enum ProjectStatus
    {
        Pending,
        Active,
        Completed,
        OnHold
    }
    public class Project
    {
        private int _Id = 1;

        public int ProjectId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ProjectStatus Status { get; set; }
        public decimal Budget { get; set; }
        public List<Task> Tasks { get; set; }
        public List<User> TeamMembers { get; set; }


        public Project(string name, string description, DateTime endDate, ProjectStatus status, decimal budget, List<Task> tasks, List<User> teamMembers)
        {
            ProjectId = _Id++;
            Name = name;
            Description = description;
            StartDate = DateTime.Now;
            EndDate = endDate;
            Status = status;
            Budget = budget;
            Tasks = tasks;
            TeamMembers = teamMembers;
        }
        public void AddTask(Task task)
        {
            if (task == null)
            {
                Console.WriteLine("Task Cannot Be null");
                return;
            }

            Tasks.Add(task);
            Console.WriteLine("Task Added Successfuly.");
        }

        public void RemoveTask(int taskId)
        {
            Task taskToRemove = Tasks.Find(t => t.TaskId == taskId);
            if(taskToRemove != null)
            {
                Tasks.Remove(taskToRemove);
                Console.WriteLine("Task Removed Successfuly.");
            }
            else
            {
                Console.WriteLine("Task Cannot be found.");
            }
        }

        public void AddTeamMember(User member)
        {
            if (member != null)
            {
                TeamMembers.Add(member);
                Console.WriteLine("Team member added successfuly.");
            }
            else
            {
                Console.WriteLine("Invalid member.");
            }
        }
        public void RemoveTeamMember(int userId)
        {
            User memberToRemove = TeamMembers.Find(m => m.UserId == userId);
            if (memberToRemove != null)
            {
                TeamMembers.Remove(memberToRemove);
                Console.WriteLine("Member Removed Successfuly.");
            }
            else
            {
                Console.WriteLine("Member Cannot be found.");
            }
        }
    }

    public enum TaskStatus
    {
        ToDo,
        InProgress,
        Completed,
        Blocked
    }

    public class Task
    {
        private int _Id = 1;
        public int TaskId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime DueDate { get; set; }
        public User Asignee { get; set; }
        public List<TimeLog> TimeLogs { get; set; }
        public TaskStatus Status { get; set; }


        public Task(string title, string description, DateTime dueDate, User asignee, TaskStatus status, List<TimeLog> timeLogs)
        {
            TaskId = _Id++;
            Title = title;
            Description = description;
            CreatedOn = DateTime.Now;
            DueDate = dueDate;
            Asignee = asignee;
            TimeLogs = timeLogs;
            Status = status;
        }

        public void AssignUser(User user)
        {
            this.Asignee = user;
            Console.WriteLine("User Assigned Successfuly.");
        }

        public void UpdateStatus(TaskStatus status)
        {
            this.Status = status;
        }

        public void LogTime(TimeLog log)
        {
            TimeLogs.Add(log);
        }
        public TimeSpan GetTotalTimeSpent()
        {
            return DateTime.Now - this.CreatedOn;
        }
        
    }

    public class TimeLog
    {
        private int _Id = 1;
        public int Id { get; set; }
        public User User { get; set; }
        public DateTime LogDate { get; set; }
        public TimeSpan Duration { get; set; }

        public TimeLog(User user, DateTime logDate, TimeSpan duration)
        {
            Id = _Id;
            User = user;
            LogDate = logDate;
            Duration = duration;
        }


        public override string ToString()
        {
            return $"TimeLog ID: {Id}, User: {User.Username}, Date: {LogDate}, Duration: {Duration.TotalHours} hours";
        }

    }

    
    public class Report
    {
        private int _Id = 1;
        public int ReportId { get; set; }
        public Project Project { get; set; }
        public User GeneratedBy { get; set; }
        public DateTime GeneratedOn { get; set; }
        public string ReportType { get; set; }

        public Report(Project project, User generatedBy, string reportType)
        {
            ReportId = _Id++;
            Project = project;
            GeneratedBy = generatedBy;
            GeneratedOn = DateTime.Now;
            ReportType = reportType;
        }

        public string GenerateProjectReport()
        {
            StringBuilder report = new StringBuilder();
            report.AppendLine($"Project Report for: {Project.Name}");
            report.AppendLine($"Description: {Project.Description}");
            report.AppendLine($"Start Date: {Project.StartDate}");
            report.AppendLine($"End Date: {Project.EndDate}");
            report.AppendLine($"Status: {Project.Status}");
            report.AppendLine($"Budget: {Project.Budget}");
            report.AppendLine($"Generated By: {GeneratedBy.Username} on {GeneratedOn}");

        
            return report.ToString();
        }

        public string GenerateUserActivityReport(User user)
        {
            StringBuilder report = new StringBuilder();
            report.AppendLine($"User Activity Report for: {user.Username}");
            report.AppendLine($"Email: {user.Email}");
            report.AppendLine($"Role: {user.Role}");
            report.AppendLine($"Generated By: {GeneratedBy.Username} on {GeneratedOn}");

            return report.ToString();
        }

        public string GenerateTaskReport(Task task)
        {
            StringBuilder report = new StringBuilder();
            report.AppendLine($"Task Report for: {task.Title}");
            report.AppendLine($"Description: {task.Description}");
            report.AppendLine($"Assigned To: {task.Asignee?.Username ?? "Unassigned"}");
            report.AppendLine($"Status: {task.Status}");
            report.AppendLine($"Created On: {task.CreatedOn}");
            report.AppendLine($"Due Date: {task.DueDate}");
            report.AppendLine($"Total Time Spent: {task.GetTotalTimeSpent()} hours");


            report.AppendLine("Time Logs:");
            foreach (var log in task.TimeLogs)
            {
                report.AppendLine(log.ToString());
            }

            return report.ToString();
        }

    }

    public enum NotificationType
    {
        TaskDeadline,
        NewAssignment,
        ProjectUpdate
    }
    public class Notification
    {
        private int _Id = 1;
        public int NotificationId { get; set; }
        public string Message { get; set; }
        public DateTime DateSent { get; set; }
        public User Recipient { get; set; }
        public NotificationType Type { get; set; }

        public Notification(string message, User recipient, NotificationType type)
        {
            NotificationId = _Id++;
            Message = message;
            DateSent = DateTime.Now;
            Recipient = recipient;
            Type = type;
        }

        public override string ToString()
        {
            return $"Notification ID: {NotificationId}, Type: {Type}, Message: {Message}, Date: {DateSent}";
        }


    }

    public class UserService
    {
        private readonly string _connectionString;
        public List<User> Users { get; set; }

        public UserService(string connectionString)
        {
            _connectionString = connectionString;
            Users = new List<User>();
        }

        public async System.Threading.Tasks.Task AddUserAsync(User user)
        {
            Users.Add(user);
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO Users (Username, PasswordHash, Email, Role, CreatedOn) VALUES (@Username, @PasswordHash, @Email, @Role, @CreatedOn)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", user.Username);
                    command.Parameters.AddWithValue("@PasswordHash", user.Password); // Ensure this is hashed
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@CreatedOn", user.CreatedOn);
                    command.Parameters.AddWithValue("@Role", user.Role);

                    try
                    {
                        await connection.OpenAsync();
                        await command.ExecuteNonQueryAsync();
                    }
                    catch (SqlException ex)
                    {
                        Console.WriteLine($"Database error: {ex.Message}");
                    }
                }
            }
        }

        public async System.Threading.Tasks.Task<bool> RegisterUserAsync(string username, string password, string email, UserRole role)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(email))
            {
                Console.WriteLine("Username, password, and email are required.");
                return false;
            }

            if (Users.Any(u => u.Username == username))
            {
                Console.WriteLine("Username already taken.");
                return false;
            }

            User newUser = new User(username, PasswordHelper.HashPassword(password), email, role);
            Users.Add(newUser);

            try
            {
                await AddUserAsync(newUser);
                Console.WriteLine("User registered successfully.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error registering user: {ex.Message}");
                return false;
            }
        }

        public bool Login(string username, string password)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string query = "SELECT COUNT(1) FROM Users WHERE Username = @Username AND PasswordHash = @PasswordHash";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        command.Parameters.AddWithValue("@PasswordHash", PasswordHelper.HashPassword(password));

                        int result = Convert.ToInt32(command.ExecuteScalar());
                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during login: {ex.Message}");
                return false;
            }
        }

        public void Logout(string username)
        {
            Console.WriteLine($"{username} logged out successfully.");
        }

        public void AssignTask(Task task, User user)
        {
            if (task == null)
            {
                Console.WriteLine("Task cannot be null");
                return;
            }
            if (user == null)
            {
                Console.WriteLine("User cannot be null");
                return;
            }

            task.Asignee = user;
            Console.WriteLine($"Task '{task.Title}' has been assigned to {user.Username}.");
        }

        public void TrackTime(Task task, TimeSpan duration, User user)
        {
            if (duration.TotalMinutes <= 0)
            {
                Console.WriteLine("Duration must be greater than zero.");
                return;
            }

            TimeLog newTimeLog = new TimeLog(user, DateTime.Now, duration);
            task.TimeLogs.Add(newTimeLog);
            Console.WriteLine($"Logged {duration} for task '{task.Title}' by {user.Username}.");
        }
    }



    public class NotificationService
    {
        private List<Notification> Notifications;

        public NotificationService()
        {
            Notifications = new List<Notification>();
        }

        public void SendNotification(User user, string message, NotificationType type)
        {
            if (user == null)
            {
                Console.WriteLine("User cannot be null.");
                return;
            }

            Notification notification = new Notification(message, user, type);
            Notifications.Add(notification);
            Console.WriteLine($"Notification sent to {user.Username}: {message}");
        }

        public List<Notification> GetNotificationsForUser(User user)
        {
            if (user == null)
            {
                Console.WriteLine("User cannot be null.");
                return new List<Notification>();
            }

            return Notifications.Where(n => n.Recipient.UserId == user.UserId).ToList();
        }
    }

    public class DatabaseManager
    {
        private readonly string _connectionString;

        public DatabaseManager(string server, string databaseName)
        {
            _connectionString = $"Server={server};Database={databaseName};Trusted_Connection=True;";
        }

        public void CreateDatabase(string dbName)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString)) 
            {
                try
                {
                    connection.Open();
                    string query = $"CREATE DATABASE {dbName};";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                        Console.WriteLine($"Database '{dbName}' created successfully.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating database: {ex.Message}");
                }
            }
        }

        public SqlConnection ConnectToDatabase()
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            try
            {
                connection.Open();
                Console.WriteLine("Connection established successfully.");
                return connection;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting to database: {ex.Message}");
                return null;
            }
        }

        public void ExecuteNonQuery(string query)
        {
            using (SqlConnection connection = ConnectToDatabase())
            {
                if (connection == null) return;

                try
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                        Console.WriteLine("Query executed successfully.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error executing query: {ex.Message}");
                }
            }
        }

        public void ExecuteQuery(string query)
        {
            using (SqlConnection connection = ConnectToDatabase())
            {
                if (connection == null) return;

                try
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Console.WriteLine(reader[0].ToString());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error executing query: {ex.Message}");
                }
            }
        }

        public void CloseConnection(SqlConnection connection)
        {
            if (connection != null && connection.State == System.Data.ConnectionState.Open)
            {
                connection.Close();
                Console.WriteLine("Connection closed.");
            }
        }
    }


    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            string connectionString = "Server=localhost;Database=ProjectManagementSystem;Trusted_Connection=True;";
            UserService userService = new UserService(connectionString);
            NotificationService notificationService = new NotificationService();

            await userService.RegisterUserAsync("john_doe1", "password123", "john@example.com", UserRole.TeamMember);

            bool loginSuccess = userService.Login("john_doe", PasswordHelper.HashPassword("password123"));
            Console.WriteLine(loginSuccess ? "Login successful!" : "Login failed!");

            List<Task> tasks = new List<Task>();
            Project project = new Project("Project A", "Description for Project A", DateTime.Now.AddMonths(1), ProjectStatus.Active, 5000, tasks, new List<User>());

           
            Task task = new Task("Task 2", "Description for Task 2", DateTime.Now.AddDays(7), null, TaskStatus.ToDo, new List<TimeLog>());
            userService.AssignTask(task, userService.Users[1]); 
            project.AddTask(task);

            userService.TrackTime(task, TimeSpan.FromHours(2), userService.Users[0]);

            notificationService.SendNotification(userService.Users[0], "You have a new task assigned.", NotificationType.NewAssignment);

            var notifications = notificationService.GetNotificationsForUser(userService.Users[0]);
            foreach (var notification in notifications)
            {
                Console.WriteLine(notification.ToString());
            }


            Report report = new Report(project, userService.Users[0], "Weekly Report");
            Console.WriteLine(report.GenerateProjectReport());

            Console.WriteLine("Login/Register");
            string answer = Console.ReadLine();

            if(answer.Equals("Login", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Username:");
                string username = Console.ReadLine();
                Console.WriteLine("Password:");
                string password = Console.ReadLine();

                userService.Login(username, PasswordHelper.HashPassword(password));
            }else if(answer.Equals("Register", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Username:");
                string username = Console.ReadLine();
                Console.WriteLine("Password:");
                string password = Console.ReadLine();
                Console.WriteLine("Email:");
                string email = Console.ReadLine();
                Console.WriteLine("Role: 1 - Admin, 2 - Project Manager, 3 - TeamMember");
                string input = Console.ReadLine();
                UserRole role = UserRole.TeamMember;

                if (int.TryParse(input, out int roleNumber))
                {
                    
                    switch (roleNumber)
                    {
                        case 1:
                            role = UserRole.Admin;
                            break;
                        case 2:
                            role = UserRole.ProjectManager;
                            break;
                        case 3:
                            role = UserRole.TeamMember;
                            break;
                        default:
                            Console.WriteLine("Invalid role selected.");
                            return; 
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                    return; 
                }

                await userService.RegisterUserAsync(username, password, email, role);
            }
            else
            {
                Console.WriteLine("Invalid input.");
            }
            

            

        }
    }
}
