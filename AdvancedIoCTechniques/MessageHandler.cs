using System;

namespace AdvancedIoCTechniques
{
    public class ChatServer
    {
        private readonly IMessageParser _messageParser;
        private readonly IMessageHandlerFactory _messageHandlerFactory;
        private readonly DumbSocket _socket;

        public ChatServer(IMessageParser messageParser, IMessageHandlerFactory messageHandlerFactory)
        {
            _messageParser = messageParser;
            _messageHandlerFactory = messageHandlerFactory;

            _socket = new DumbSocket();
            _socket.Start();
            _socket.IncomingMessage += OnSocketIncomingMessage;
        }

        private void OnSocketIncomingMessage(string message)
        {
            // the message parser is singleton, since there's only one definition possible
            // for each hardcoded message
            var msg = _messageParser.Parse(message);

            // depending on what type of message
            foreach (var handler in _messageHandlerFactory.GetHandlersFor(msg))
            {
                handler.Process(msg);
            }
        }
    }

    public interface IMessageHandlerFactory
    {
        IMessageHandler[] GetHandlersFor(IMessage msg);
    }

    public interface IMessageHandler
    {
        void Process(IMessage msg);
    }

    public class User
    {
    }

    public interface ISessionManager
    {
    }

    public interface IMessageParser
    {
        IMessage Parse(string message);
    }

    public interface IMessage
    {
    }

    public class DumbSocket
    {
        public event Action<string> IncomingMessage;

        public void Start()
        {
        }
    }
}