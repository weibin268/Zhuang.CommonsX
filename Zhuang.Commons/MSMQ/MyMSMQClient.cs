using System;
using System.Messaging;

namespace Zhuang.Commons.MSMQ
{

    #region MSMQManager
    public class MyMSMQClient : MyMSMQClient<string>
    {
        public MyMSMQClient(string queueName)
            : base(queueName)
        { }

        public MyMSMQClient(string queueName, string remoteIp)
            : base(queueName, remoteIp)
        { }
    } 
    #endregion

    public class MyMSMQClient<T>:IDisposable
    {
        #region Fields
        private MessageQueue _mq;
        private MessageQueueTransaction _mqt;

        private string _localPriaveQueue = @".\private$\{0}";
        private string _remotePrivateQueue = @"FormatName:Direct=TCP:{0}\private$\{1}";
        private string _fullQueueName = string.Empty;


        #endregion

        #region Properties
        public MessageQueue MQ
        {
            get { return _mq; }
        }

        public bool Transactional
        {
            get { return _mq.Transactional; }
        }

        public string FullQueueName
        {
            get { return _fullQueueName; }
            set { _fullQueueName = value; }
        }

        #endregion

        #region Constructors
        public MyMSMQClient(string queueName)
            : this(queueName, false)
        {

        }

        public MyMSMQClient(string queueName, bool transactional)
        {
            if (transactional)
            {
                _mqt = new MessageQueueTransaction();
            }
            
            _fullQueueName = string.Format(_localPriaveQueue, queueName);
            if (!MessageQueue.Exists(_fullQueueName))
            {
                _mq = MessageQueue.Create(_fullQueueName, transactional);
            }
            else
            {
                _mq = new MessageQueue(_fullQueueName);
            }
            Init();
        }

        /// <summary>
        /// 远程访问是注意：
        /// 远程接收需求以下设置
        /// 就是在消息队列的安全中加入一个ANONYMOUS LOGON用户设定权限这样就可以接受了 
        /// 但注意的是必须是非事务的队列
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="remoteIp"></param>
        public MyMSMQClient(string queueName, string remoteIp)
        {
            string fullQueueName = string.Format(_remotePrivateQueue, remoteIp, queueName);
            _mq = new MessageQueue(fullQueueName);
            Init();
        }
        
        #endregion

        #region Methods
        private void Init()
        {
            _mq.Formatter = new XmlMessageFormatter(new Type[] { typeof(T) });
        }
        
        #endregion

        #region Set
        public void SetFormatter(IMessageFormatter formatter)
        {
            _mq.Formatter = formatter;
        }

        public void SetTransaction(MessageQueueTransaction mqt)
        {
            _mqt = mqt;
        }
        
        #endregion

        #region Transaction
        public void BeginTransaction()
        {
            if (_mq.Transactional)
                _mqt.Begin();
        }

        public void CommitTransaction()
        {
            if (_mq.Transactional)
                _mqt.Commit();
        }

        public void AbortTransaction()
        {
            if (_mq.Transactional)
                _mqt.Abort();
        } 
        #endregion

        #region Send
        public void Send(object obj)
        {
            Send(obj, null);
        }

        public void Send(object obj, string label)
        {
            Send(obj, label, MessagePriority.Normal);
        }

        public void Send(object obj, string label, MessagePriority priority)
        {
            Message msg = new Message();
            msg.Body = obj;

            if (!string.IsNullOrEmpty(label))
            {
                msg.Label = label;
            }

            msg.Priority = priority;

            Send(msg);
        }

        public void Send(Message msg)
        {
            if (_mqt != null)
            {
                _mq.Send(msg, _mqt);
            }
            else
            {
                _mq.Send(msg);
            }
        }
        #endregion

        #region Receive
        public Message Receive()
        {
            if (_mqt != null)
            {
                return _mq.Receive(_mqt);
            }
            else
            {
                return _mq.Receive();
            }
        }

        public T ReceiveBody()
        {
            return (T)Receive().Body;
        }
        #endregion

        public void Delete()
        {
            MessageQueue.Delete(_fullQueueName);
        }

        public void Dispose()
        {
            if (_mqt != null)
                _mqt.Dispose();

            if (_mq != null)
                _mq.Dispose();
        }
    }
}
