using System.Net;
using Watermelon.website;


var httpListener = new HttpListener();
httpListener.Prefixes.Add("http://localhost:4044/");
httpListener.Start();
while (httpListener.IsListening)
{
    var context = await httpListener.GetContextAsync();
    var request = context.Request;
    var response = context.Response;
    _ = Task.Run(async () =>
    {
        switch (request.Url?.LocalPath)
        {
            case "/home":
                await WebHelper.ShowIndex(context);
                break;
            case "/singin":
                await WebHelper.ShowSingIn(context);
                break;
            case "/singup":
                await WebHelper.ShowSignUp(context);
                break;
            case "/login":
                await WebHelper.Login(context);
                break;
            case "/registration":
                await WebHelper.SaveUser(context);
                break;
            case "/addcomment":
                await WebHelper.SaveComment(context);
                break;
            case "/showcomments":
                await WebHelper.ShowComment(context);
                break;
            case "/showPrice":
                await WebHelper.ShowPrice(context);
                break;
            case "/showSale":
                await WebHelper.ShowSale(context);
                break;
            case "/addResume":
                await WebHelper.SaveResume(context);
                break;
            case "/deleteResume":
                await WebHelper.DeleteResume(context);
                break;
            case "/buyRequest":
                await WebHelper.CreateBuyRequest(context);
                break;
            default:
                await WebHelper.ShowStatic(context);
                break;
        }

        response.OutputStream.Close();
        response.Close();
    });
}

httpListener.Stop();
httpListener.Close();