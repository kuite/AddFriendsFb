public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
{
    if(activity.Type === ActivityTypes.Message){

        ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));

            // calculate something for us to return
            int length = (activityText ?? string.Empty).Length;

            // return our reply to user
            Activity reply = activity.CreateReply($"You sent {activity.Text} which was {length} characters");
            await connector.Conversation.ReplyToActivityAsync(reply);
    }

    else
    {
        HandleSystemMessage(activity);
    }

    var response = Request.CreateResponse(HttpStatusCode.OK);
    return response;
    
}