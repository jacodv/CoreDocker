using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Security.Claims;
using GraphQL;
using GraphQL.Resolvers;
using GraphQL.Server.Transports.Subscriptions.Abstractions;
using GraphQL.Subscription;
using GraphQL.Types;


namespace CoreDocker.Api.GraphQl
{
    public class Message
    {
        
        public string Sub { get; set; }

        public string Content { get; set; }

        public DateTime SentAt { get; set; }
        public MessageFrom From { get; set; }
    }

    public class MessageType : ObjectGraphType<Message>
    {
        public MessageType()
        {
            Field(o => o.Content);
            Field(o => o.SentAt,type: typeof(OriginalDateGraphType));
            Field(o => o.Sub);
            Field(o => o.From, false, typeof(MessageFromType)).Resolve(ResolveFrom);
        }

        private MessageFrom ResolveFrom(ResolveFieldContext<Message> context)
        {
            var message = context.Source;
            return message.From;
        }
    }

    public class MessageFromType : ObjectGraphType<MessageFrom>
    {
        public MessageFromType()
        {
            Field(o => o.Id);
            Field(o => o.DisplayName);
        }
    }

    public class DefaultSubscription : ObjectGraphType<object>
    {
        private readonly IChat _chat;

        public DefaultSubscription(IChat chat)
        {
            _chat = chat;
            AddField(new EventStreamFieldType
            {
                Name = "messageAdded",
                Type = typeof(MessageType),
                Resolver = new FuncFieldResolver<Message>(ResolveMessage),
                Subscriber = new EventStreamResolver<Message>(Subscribe)
            });

            AddField(new EventStreamFieldType
            {
                Name = "messageAddedByUser",
                Arguments = new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "id" }
                ),
                Type = typeof(MessageType),
                Resolver = new FuncFieldResolver<Message>(ResolveMessage),
                Subscriber = new EventStreamResolver<Message>(SubscribeById)
            });
        }

        private Message ResolveMessage(ResolveFieldContext context)
        {
            var message = context.Source as Message;

            return message;
        }

        private IObservable<Message> Subscribe(ResolveEventStreamContext context)
        {
            var messageContext = context.UserContext.As<MessageHandlingContext>();
            var user = messageContext.Get<ClaimsPrincipal>("user");

            var sub = "Anonymous";
            if (user != null)
                sub = user.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

            return _chat.Messages(sub);
        }

        private IObservable<Message> SubscribeById(ResolveEventStreamContext context)
        {
            var messageContext = context.UserContext.As<MessageHandlingContext>();
            var user = messageContext.Get<ClaimsPrincipal>("user");

            var sub = "Anonymous";
            if (user != null)
                sub = user.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

            var messages = _chat.Messages(sub);

            var id = context.GetArgument<string>("id");
            return messages.Where(message => message.From.Id == id);
        }
    }

    public class ReceivedMessage
    {
        public string FromId { get; set; }

        public string Content { get; set; }

        public DateTime SentAt { get; set; }
    }

    public interface IChat
    {
        ConcurrentStack<Message> AllMessages { get; }

        Message AddMessage(Message message);

        IObservable<Message> Messages(string user);

        Message AddMessage(ReceivedMessage message);
    }

    public class Chat : IChat
    {
        private readonly ISubject<Message> _messageStream = new ReplaySubject<Message>(1);


        public Chat()
        {
            AllMessages = new ConcurrentStack<Message>();
            Users = new ConcurrentDictionary<string, string>
            {
                ["1"] = "developer",
                ["2"] = "tester"
            };
        }

        public ConcurrentDictionary<string, string> Users { get; set; }

        public ConcurrentStack<Message> AllMessages { get; }

        public Message AddMessage(ReceivedMessage message)
        {
            if (!Users.TryGetValue(message.FromId, out var displayName))
            {
                displayName = "(unknown)";
            }

            return AddMessage(new Message
            {
                Content = message.Content,
                SentAt = message.SentAt,
                From = new MessageFrom
                {
                    DisplayName = displayName,
                    Id = message.FromId
                }
            });
        }

        public Message AddMessage(Message message)
        {
            AllMessages.Push(message);
            _messageStream.OnNext(message);
            return message;
        }

        public IObservable<Message> Messages(string user)
        {
            return _messageStream
                .Select(message =>
                {
                    message.Sub = user;
                    return message;
                })
                .AsObservable();
        }

        public void AddError(Exception exception)
        {
            _messageStream.OnError(exception);
        }
    }

    public class MessageFrom
    {
        public string DisplayName { get; set; }
        public string Id { get; set; }
    }
}