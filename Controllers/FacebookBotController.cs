public ActionResult Receive()
{
    var query = Request.QueryString;

    _logWriter.WriteLine(Request.RawUrl);

    if (query["hub.mode"] == "subscribe" &&
        query["hub.verify_token"] == "myToken874523")
    {
        //string type = Request.QueryString["type"];
        var retVal = query["hub.challenge"];
        return Json(int.Parse(retVal), JsonRequestBehavior.AllowGet);
    }
    else
    {
        return HttpNotFound();
    }
}

[ActionName("Receive")]
[AcceptVerbs(HttpVerbs.Post)]
public ActionResult ReceivePost(BotRequest data)
{
    Task.Factory.StartNew(() =>
    {
        foreach (var entry in data.entry)
        {
            foreach (var message in entry.messaging)
            {
                if (string.IsNullOrWhiteSpace(message?.message?.text))
                    continue;

                var msg = "You said: " + message.message.text;
                var json = $@" {{recipient: {{  id: {message.sender.id}}},message: {{text: ""{msg}"" }}}}";
                PostRaw("https://graph.facebook.com/v2.6/me/messages?access_token=YOUR_TOKEN", json);
            }
        }
    });

    return new HttpStatusCodeResult(HttpStatusCode.OK);
}

private string PostRaw(string url, string data)
{
    var request = (HttpWebRequest)WebRequest.Create(url);
    request.ContentType = "application/json";
    request.Method = "POST";
    using (var requestWriter = new StreamWriter(request.GetRequestStream()))
    {
        requestWriter.Write(data);
    }

    var response = (HttpWebResponse)request.GetResponse();
    if (response == null)
        throw new InvalidOperationException("GetResponse returns null");

    using (var sr = new StreamReader(response.GetResponseStream()))
    {
        return sr.ReadToEnd();
    }
}