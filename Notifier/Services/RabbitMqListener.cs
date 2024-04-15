using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Extensions.Options;

using AutorizationMcsContract;
using PostgreSQLModelsLibrary;

using System.Reflection.Metadata;
using System.Text.Json;

using Notifier.Models;

namespace Notifier.Services
{
    public class RabbitMqListener : BackgroundService
    {
        private IConnection _connection;
        private IModel _channel_admin;
        private IModel _channel_homework;
        private readonly IRabbitMqOptions _options;
        private readonly IEmailService _emailService;

        public RabbitMqListener(IServiceProvider _serviceProvider, IEmailService emailService)
        {
            using var scope = _serviceProvider.CreateScope();
            var options = scope.ServiceProvider.GetRequiredService<IOptions<RabbitMqOptions>>();

            _options = options.Value;

            _emailService = emailService;

            ConnectionFactory factory;

            if (options.Value.Host == "localhost")
            {
                factory = new ConnectionFactory() { HostName = _options.Host }; // for local docker container
            }
            else
            {
                factory = new ConnectionFactory() { Uri = new Uri(_options.Host) };
            }


            _connection = factory.CreateConnection();

            _channel_admin = _connection.CreateModel();
            _channel_homework = _connection.CreateModel();
            //_channel.QueueDeclare(queue: _options.QueueNameFrom, durable: false, exclusive: false, autoDelete: false, arguments: null);

            /*--------------------------------*/
            _channel_admin.ExchangeDeclare(exchange: _options.Admin_direct, type: ExchangeType.Direct);
            _channel_homework.ExchangeDeclare(exchange: _options.Homework_direct, type: ExchangeType.Direct);
            /*--------------------------------*/
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            /*--------------------------------*/

            // declare a server-named queue
            var queueName_Admin = _channel_admin.QueueDeclare().QueueName;

            foreach (var severity in _options.Admin_RoutingKeys)
            {
                _channel_admin.QueueBind(queue: queueName_Admin,
                      exchange: _options.Admin_direct,
                      routingKey: severity); // severity <<<<<--------
            }
                

            Console.WriteLine(" [*] Waiting for messages.");

            var consumer_admin = new EventingBasicConsumer(_channel_admin);
            consumer_admin.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var routingKey = ea.RoutingKey;
                Console.WriteLine($" [x] Received '{routingKey}':'{message}'");

                var adminMessage = JsonSerializer.Deserialize<Progress>(message);

                string courseTitle = GetCourseTitleAsync(adminMessage.CourseId).Result;
                string lessonTitle = GetLessonTitleAsync(adminMessage.CourseId, adminMessage.LessonId).Result;

                if (routingKey == "testdone")
                {                   

                    SendAdminMessageAsync(adminMessage, "Test is Done", "Поздравляем, Тест по курсу " + 
                        "<<" + courseTitle + ">>," +
                        " урока " + "<<" + lessonTitle + ">>" +
                        " успешно пройден ! "); 

                } else if (routingKey == "homeworkdone")
                {
                    // Send Email report
                    SendAdminMessageAsync(adminMessage, "Homework Accept", "Поздравляем, ДЗ по курсу " + 
                        "<<" + courseTitle + ">>," +
                        " урока " + "<<" + lessonTitle + ">>" +
                        " принято преподавателем ! ");
                }
                
            };
            _channel_admin.BasicConsume(queue: queueName_Admin,
                                 autoAck: true,
                                 consumer: consumer_admin);

            /*--------------------------------*/

            // declare a server-named queue
            var queueName_Homework = _channel_homework.QueueDeclare().QueueName;

            foreach (var severity in _options.Homework_RoutingKeys)
            {
                _channel_homework.QueueBind(queue: queueName_Homework,
                      exchange: _options.Homework_direct,
                      routingKey: severity); // severity <<<<<--------
            }
                
            Console.WriteLine(" [*] Waiting for messages.");

            var consumer_homework = new EventingBasicConsumer(_channel_homework);
            consumer_homework.Received += (model, ea) =>
            {
                int sendUserId = 0;

                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var routingKey = ea.RoutingKey;
                
                Console.WriteLine($" [x] Received '{routingKey}':'{message}'");

                if (routingKey == "submit")
                {                    
                    var homeworkMessage = JsonSerializer.Deserialize<SubmitHomework>(message);
                    
                    SendSubmitToTeacherAsync(homeworkMessage);
                }
                else if (routingKey == "studentmessage")
                {
                    var homeworkMessage = JsonSerializer.Deserialize<StudentMessageHomework>(message);
                    
                    SendMessageToTeacherAsync(homeworkMessage);
                    
                }
                else if (routingKey == "teachermessage")
                {
                    var homeworkMessage = JsonSerializer.Deserialize<TeacherMessageHomework>(message);
                    
                    SendMessageToStudentAsync(homeworkMessage);
                }        
            };

            _channel_homework.BasicConsume(queue: queueName_Homework,
                                 autoAck: true,
                                 consumer: consumer_homework);

            /*--------------------------------*/

            return Task.CompletedTask;
        }

        async Task<string> GetCourseTitleAsync(string courseId)
        {
            string url = "https://localhost:7078/api/Course/" + courseId; //

            CourseDTO title = null;

            using (var client = new HttpClient())
            {
                // Take Questions array from request 
                var task = await client.GetFromJsonAsync(url, typeof(CourseDTO));
                title = (CourseDTO)task;
            }

            return title.Title;
        }

        async Task<string> GetLessonTitleAsync(string courseId, string lessonId)
        {
            string url = "https://localhost:7078/api/Lesson/" + courseId + "/" + lessonId; //

            LessonDTO title = null;

            using (var client = new HttpClient())
            {
                // Take Questions array from request 
                var task = await client.GetFromJsonAsync(url, typeof(LessonDTO));
                title = (LessonDTO)task;
            }

            return title.Title;
        }

        async Task SendAdminMessageAsync(Progress adminMessage, string subject, string text)
        {
            // Get Email from AuthorizatorMcs
            string email = await GetUserAsync(adminMessage.UserId);
            // Send Email
            await _emailService.SendEmailAsync(email, subject,
                "Date: " + DateTime.Now.ToString() + ";  \n" +

                "Message: " + text);
        }

        async Task SendMessageToStudentAsync(TeacherMessageHomework teacherMessageHomework)
        {
            // Get Email from AuthorizatorMcs
            string email = await GetUserAsync(teacherMessageHomework.ToStudentId);
            // Send Email
            await _emailService.SendEmailAsync(email, "Homework Message from Teacher",
                "Date: " + teacherMessageHomework.CreateDate.ToString() + ";  \n" +
                "HomeworkId = " + teacherMessageHomework.HomeworkId.ToString() + ";  \n" +
                "Message: " + teacherMessageHomework.Text);
        }

        async Task SendSubmitToTeacherAsync(SubmitHomework submitHomework)
        {
            List<long> teachersId = new List<long>();

            // Get Teacher Id from Admin
            teachersId = await GetTeachersIdAsync(submitHomework.CourseId);
            // GetTeacher Email from AuthorizatorMcs
            string email = await GetUserAsync(teachersId.FirstOrDefault<long>());
            //Send Email to teacher
            await _emailService.SendEmailAsync(email, "Student Submit Homework", 
                "Date: " + DateTime.Now.ToString() + ";  \n" +
                "HomeworkId = " + submitHomework.HomeworkId.ToString() + ";  \n" +
                "Message: " + submitHomework.Text);
        }

        async Task SendMessageToTeacherAsync(StudentMessageHomework studentMessageHomework)
        {
            List<long> teachersId = new List<long>();

            // Get Teacher Id from Admin
            teachersId = await GetTeachersIdAsync(studentMessageHomework.CourseId);
            // GetTeacher Email from AuthorizatorMcs
            string email = await GetUserAsync(teachersId.FirstOrDefault<long>());
            //Send Email
            await _emailService.SendEmailAsync(email, "Homework Message from Student", 
                "Date: " + studentMessageHomework.CreateDate.ToString() + ";  \n" +
                "HomeworkId = " + studentMessageHomework.HomeworkId.ToString() + ";  \n" +
                "Message: " + studentMessageHomework.Text);
        }

        async Task<string> GetUserAsync(long userId)
        {
            string url = "https://localhost:7029/api/User/" + userId.ToString(); //

            UserAutorizationModel user = null;

            using (var client = new HttpClient())
            {
                // Take Questions array from request 
                var task = await client.GetFromJsonAsync(url, typeof(UserAutorizationModel));
                user = (UserAutorizationModel)task;
            }

            return user.Email;
        }

        async Task<List<long>> GetTeachersIdAsync(string courseId)
        {
            string url = "https://localhost:7239/api/Access/teachers/" + courseId; // Request to Admin 

            List<long> teachersId = new List<long>();

            using (var client = new HttpClient())
            {
                // Take Questions array from request 
                var task = await client.GetFromJsonAsync(url, typeof(List<long>));
                teachersId = (List<long>)task;
            }

            //teachersId.Add(2); // Kostil !!!

            return teachersId;
        }

        public override void Dispose()
        {
            _channel_admin.Close();
            _channel_homework.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}
