using System.ComponentModel.Design;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using StackExchange.Redis;
using Watermelon.website.Models;
using Watermelon.website.Redis;
using StreamReader = System.IO.StreamReader;

namespace Watermelon.website;

public static class WebHelper
{
    private static readonly DatabaseContext DbContext = new();

    public static async Task ShowPriceAuto(HttpListenerContext httpListenerContext)
    {
        var dbContext = new DatabaseContext();
        var sessionId = httpListenerContext.Request.Cookies["exp"];
        Console.WriteLine(sessionId.Value);
        var saleTime = Math.Abs(DateTime.Now.Minute - int.Parse(sessionId.Value));
        var salePrice = await dbContext.ShowSale(httpListenerContext.Request.QueryString["page_id"]);
        var price = await dbContext.ShowPrice(httpListenerContext.Request.QueryString["page_id"]);

        if (sessionId != null)
        {
            if (saleTime >= 20)
            {
                httpListenerContext.Response.StatusCode = 200;
                await httpListenerContext.Response.OutputStream.WriteAsync(
                    Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(salePrice)));
            }
            else
            {
                httpListenerContext.Response.StatusCode = 200;
                await httpListenerContext.Response.OutputStream.WriteAsync(
                    Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(price)));
            }
        }
        else
        {
            httpListenerContext.Response.StatusCode = 200;
            await httpListenerContext.Response.OutputStream.WriteAsync(
                Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(price)));
        }
    }

    public static async Task CreateBuyRequest(HttpListenerContext httpListenerContext)
    {
        var dbContext = new DatabaseContext();
        using var sr = new StreamReader(httpListenerContext.Request.InputStream);
        var saving = JsonConvert.DeserializeObject<BuyModel>(await sr.ReadToEndAsync());
        if (saving != null )
        {
            var request = new BuyModel
            {
                Id = new Guid(),
                WatermelonName = saving.WatermelonName,
                Address = saving.Address,
                CountOfWatermelon = saving.CountOfWatermelon,
                Email = saving.Email,
                Sale = saving.Sale
            };
            if (saving.Sale == "True")
            {
                var result = await dbContext.AddBuyRequestWhitSale(request);
                if (result != null)
                {
                    httpListenerContext.Response.StatusCode = 200;
                    httpListenerContext.Response.ContentType = "text/plain; charset=utf-8";
                    await httpListenerContext.Response.OutputStream.WriteAsync(
                        Encoding.UTF8.GetBytes(" Ожидайте курьера, он будет примерно через 7-9 часов, кстаати"));
                }
            }
            if(saving.Sale =="False")
            {
                var result = await dbContext.AddBuyRequestNoSale(request);
                if (result != null)
                {
                    httpListenerContext.Response.StatusCode = 200;
                    httpListenerContext.Response.ContentType = "text/plain; charset=utf-8";
                    await httpListenerContext.Response.OutputStream.WriteAsync(
                        Encoding.UTF8.GetBytes(" Ожидайте курьера, он будет примерно через 7-9 часов"));
                }
            }
        }
    }

    public static async Task ShowPrice(HttpListenerContext httpListenerContext)
    {
        var dbContext = new DatabaseContext();
        using var sr = new StreamReader(httpListenerContext.Request.InputStream);
        var price = await dbContext.ShowPrice(httpListenerContext.Request.QueryString["watermelonName"]);
        httpListenerContext.Response.StatusCode = 200;
        await httpListenerContext.Response.OutputStream.WriteAsync(
            Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(price)));
    }

    public static async Task ShowSale(HttpListenerContext httpListenerContext)
    {
        var dbContext = new DatabaseContext();
        using var sr = new StreamReader(httpListenerContext.Request.InputStream);
        var sale = await dbContext.ShowSale(httpListenerContext.Request.QueryString["watermelonName"]);
        httpListenerContext.Response.StatusCode = 200;
        await httpListenerContext.Response.OutputStream.WriteAsync(
            Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(sale)));
    }

    public static async Task Login(HttpListenerContext context)
    {
        using var sr = new StreamReader(context.Request.InputStream);
        var loginModel = JsonConvert.DeserializeObject<LoginModel>(await sr.ReadToEndAsync());
        var user = await DbContext.GetUser(loginModel);
        if (user != null)
        {
            string sessionId = BCrypt.Net.BCrypt.HashPassword(user.Id + "ching");
            await RedisStorage.RedisCache.SetAddAsync(new RedisKey(sessionId), new RedisValue(user.Id.ToString()));
            context.Response.Cookies.Add(new Cookie
            {
                Name = "session-id",
                Value = sessionId,
                Expires = DateTime.UtcNow.AddMinutes(30d)
            });

            context.Response.StatusCode = 200;
            context.Response.OutputStream.Write(
                Encoding.UTF8.GetBytes("Успешно!"));
        }
        else
        {
            context.Response.StatusCode = 400;
            context.Response.OutputStream.Write(Encoding.UTF8.GetBytes("Неверный пароль или логин"));
        }
    }

    public static async Task SaveResume(HttpListenerContext httpListenerContext)
    {
        var dbContext = new DatabaseContext();
        using var sr = new StreamReader(httpListenerContext.Request.InputStream);
        var saving = JsonConvert.DeserializeObject<Resume>(await sr.ReadToEndAsync());
        if (saving != null)
        {
            if (await dbContext.GetResumeByEmail(saving.Email) == null)
            {
                var resume = new Resume
                {
                    Id = new Guid(),
                    Phone = saving.Phone,
                    Email = saving.Email,
                    Education = saving.Education,
                    Experience = saving.Experience,
                    City = saving.City
                };
                var result = await dbContext.AddResume(resume);
                if (result != null)
                {
                    httpListenerContext.Response.StatusCode = 200;
                    httpListenerContext.Response.ContentType = "text/plain; charset=utf-8";
                    await httpListenerContext.Response.OutputStream.WriteAsync(
                        Encoding.UTF8.GetBytes(" Заявка будет рассмотрена "));
                }
            }
            else
            {
                httpListenerContext.Response.StatusCode = 400;
                httpListenerContext.Response.OutputStream.Write(
                    Encoding.UTF8.GetBytes(
                        "Email уже используется"));
            }
        }
    }

    public static async Task DeleteResume(HttpListenerContext httpListenerContext)
    {
        var dbContext = new DatabaseContext();
        using var sr = new StreamReader(httpListenerContext.Request.InputStream);
        var result = await dbContext.GetResumeByEmail(httpListenerContext.Request.QueryString["email"]);
        if (result != null)
        {
            await dbContext.DeleteResume(httpListenerContext.Request.QueryString["email"]);
            httpListenerContext.Response.StatusCode = 200;
            await httpListenerContext.Response.OutputStream.WriteAsync(
                Encoding.UTF8.GetBytes("Заявка на этот Email удалена"));
        }
        else
        {
            httpListenerContext.Response.StatusCode = 200;
            await httpListenerContext.Response.OutputStream.WriteAsync(
                Encoding.UTF8.GetBytes("Заявки на такой Email нет"));
        }
    }


    public static async Task ShowComment(HttpListenerContext httpListenerContext)
    {
        var dbContext = new DatabaseContext();
        using var sr = new StreamReader(httpListenerContext.Request.InputStream);
        var comment = await dbContext.GetCommentByPage(httpListenerContext.Request.QueryString["watermelonName"]);
        httpListenerContext.Response.StatusCode = 200;
        await httpListenerContext.Response.OutputStream.WriteAsync(
            Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(comment)));
    }

    public static async Task SaveComment(HttpListenerContext httpListenerContext)
    {
        var dbContext = new DatabaseContext();
        using var sr = new StreamReader(httpListenerContext.Request.InputStream);
        var saving = JsonConvert.DeserializeObject<Comments>(await sr.ReadToEndAsync());
        var sessionId = httpListenerContext.Request.Cookies["session-id"];
        if (saving != null)
        {
            if (sessionId != null && await RedisStorage.RedisCache.KeyExistsAsync(sessionId.Value))
            {
                if (await dbContext.GetUserByEmail(saving.Email) != null)
                {
                    var comments = new Comments
                    {
                        Id = new Guid(),
                        Email = saving.Email,
                        WatermelonName = saving.WatermelonName,
                        Comment = saving.Comment
                    };
                    var result = await dbContext.AddComment(comments);
                    if (result != null)
                    {
                        httpListenerContext.Response.StatusCode = 200;
                        httpListenerContext.Response.ContentType = "text/plain; charset=utf-8";
                        await httpListenerContext.Response.OutputStream.WriteAsync(
                            Encoding.UTF8.GetBytes(" Комментарий оставлен \n"));
                    }
                }
                else
                {
                    httpListenerContext.Response.StatusCode = 400;
                    httpListenerContext.Response.OutputStream.Write(
                        Encoding.UTF8.GetBytes(
                            "Проверьте ввод Email, либо вы не зарегестрированы"));
                }
            }
            else
            {
                httpListenerContext.Response.StatusCode = 400;
                httpListenerContext.Response.OutputStream.Write(
                    Encoding.UTF8.GetBytes(
                        "Вы не вошли в аккаунт"));
            }
        }
    }


    public static async Task SaveUser(HttpListenerContext httpListenerContext)
    {
        var dbContext = new DatabaseContext();
        using var sr = new StreamReader(httpListenerContext.Request.InputStream);
        var registration = JsonConvert.DeserializeObject<RegistModel>(await sr.ReadToEndAsync());
        if (registration != null)
        {
            if (await dbContext.GetUserByEmail(registration.Email) != null)
            {
                httpListenerContext.Response.StatusCode = 400;
                httpListenerContext.Response.OutputStream.Write(
                    Encoding.UTF8.GetBytes("Такой пользователь уже существует"));
                return;
            }

            if (ValidationHelper.ValidateModel(registration) != null)
            {
                httpListenerContext.Response.StatusCode = 400;
                foreach (var errors in ValidationHelper.ValidateModel(registration))
                    httpListenerContext.Response.OutputStream.Write(Encoding.UTF8.GetBytes($"{errors}"));
                return;
            }

            var user = new User
            {
                Id = new Guid(),
                Email = registration.Email,
                Mobile = registration.Mobile,
                Password = registration.Password
            };
            var result = await dbContext.AddUser(user);
            if (result != null)
            {
                httpListenerContext.Response.StatusCode = 200;
                httpListenerContext.Response.ContentType = "text/plain; charset=utf-8";
                await httpListenerContext.Response.OutputStream.WriteAsync(
                    Encoding.UTF8.GetBytes(user.Id.ToString()));
            }
        }
        else
        {
            httpListenerContext.Response.StatusCode = 500;
            httpListenerContext.Response.ContentType = "text/plain; charset=utf-8";
            httpListenerContext.Response.OutputStream.Write(
                Encoding.UTF8.GetBytes("Передача данных на сервер не удалась!"));
        }
    }


    public static async Task ShowSignUp(HttpListenerContext context)
    {
        await ShowFile(@"www\regist.html", context);
    }

    public static async Task ShowSingIn(HttpListenerContext context)
    {
        await ShowFile(@"www\login.html", context);
    }

    public static async Task ShowIndex(HttpListenerContext context)
    {
        await ShowFile(@"www\watermelon.html", context);
    }

    public static async Task ShowStatic(HttpListenerContext context)
    {
        var path = context.Request.Url?.LocalPath
            .Split("/")
            .Skip(1)
            .ToArray();
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "WWW");

        if (path != null)
        {
            for (var i = 0; i < path.Length - 1; i++)
            {
                basePath = Path.Combine(basePath, $@"{path[i]}\");
            }
        }

        basePath = Path.Combine(basePath, path?[^1] ?? throw new InvalidOperationException());

        if (File.Exists(basePath))
        {
            await ShowFile(basePath, context);
        }
        else
        {
            await Show404(context);
        }
    }

    private static async Task Show404(HttpListenerContext context)
    {
        context.Response.ContentType = "text/plain; charset=utf-8";
        context.Response.StatusCode = 404;
        await context.Response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes("Нужная вам страница не найдена!"));
    }

    private static async Task ShowFile(string path, HttpListenerContext context)
    {
        var file = await File.ReadAllBytesAsync(path);
        context.Response.StatusCode = 200;
        context.Response.ContentType = Path.GetExtension(path) switch
        {
            ".js" => "application/javascript",
            ".css" => "text/css",
            ".html" => "text/html",
            _ => "text/plain"
        };
        await context.Response.OutputStream.WriteAsync(file);
    }

    public static async Task ShowAPI(HttpListenerContext context)
    {
        var httpClient = new HttpClient();
        context.Response.ContentType = "application/json";
        var content = await httpClient.GetByteArrayAsync("https://www.cbr-xml-daily.ru/daily_json.js");
        await context.Response.OutputStream.WriteAsync(content);
    }
}